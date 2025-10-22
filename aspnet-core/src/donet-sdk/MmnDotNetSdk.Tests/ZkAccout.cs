using Microsoft.IdentityModel.Tokens;
using MmnDotNetSdk.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MmnDotNetSdk.Tests
{


    public class KeyPairAccount
    {
        public KeyPairAccount(string publicKey, byte[] privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
        public string PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
    }

    public class ZkAccount
    {
        public string PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public ulong Nonce { get; set; }
        public ulong Balance { get; set; }
        public string Address { get; set; }
        public string ZkProof { get; set; }
        public string ZkPub { get; set; }

        public ZkAccount(string publicKey, byte[] privateKey, ulong nonce, ulong balance, string address, string zkProof, string zkPub)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
            Nonce = nonce;
            Balance = balance;
            Address = address;
            ZkProof = zkProof;
            ZkPub = zkPub;
        }

        private static string GenerateJwt(long userId)
        {
            var exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();

            var claims = new[]
            {
                new Claim("tid", userId.ToString()),
                new Claim("uid", userId.ToString(), ClaimValueTypes.Integer64),
                new Claim("usn", userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, exp.ToString(), ClaimValueTypes.Integer64)
            };

            // Handle short key the same way as Go (pad to 32 bytes if needed)
            var keyBytes = Encoding.UTF8.GetBytes(TestData.JwtSecret);
            if (keyBytes.Length < 32)
            {
                var paddedKey = new byte[32];
                Array.Copy(keyBytes, paddedKey, keyBytes.Length);
                // Pad with zeros (most common approach)
                keyBytes = paddedKey;
            }
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public static async Task<ZkAccount> GenerateZkAccount(ZkProveClient zkClient)
        {
            // Generate Ed25519 key pair (equivalent to ed25519.GenerateKey)
            var (publicKey, privateKey) = CryptoHelper.GenerateEd25519KeyPair();

            // Generate random user ID (equivalent to rand.Intn(100000000))
            var random = new Random();
            var userId = (long)random.Next(100000000);

            // Generate address from user ID
            var address = CryptoHelper.GenerateAddress(userId.ToString());

            // Generate JWT token
            var jwt = GenerateJwt(userId);

            // Convert public key to Base58 (equivalent to base58.Encode(publicKey))
            var publicKeyHex = CryptoHelper.Base58Encode(publicKey);

            // Generate ZK proof
            var proofRes = await zkClient.GenerateZkProof(userId.ToString(), address, publicKeyHex, jwt);
            if (proofRes?.Error != null)
            {
                throw new Exception($"Failed to generate zk proof: {proofRes.Error}");
            }

            if (proofRes?.Data == null)
            {
                throw new Exception("ZK proof response data is null");
            }

            var zkPub = proofRes.Data.PublicInput ?? string.Empty;
            var zkProof = proofRes.Data.Proof ?? string.Empty;

            // Create and return ZkAccount
            return new ZkAccount(
                publicKey: publicKeyHex,
                privateKey: privateKey,
                nonce: 0,
                balance: 0,
                address: address,
                zkProof: zkProof,
                zkPub: zkPub
            );
        }

        public static KeyPairAccount GenerateKeyPairAccount()
        {
            var (publicKey, privateKey) = CryptoHelper.GenerateEd25519KeyPair();
            var publicKeyHex = CryptoHelper.Base58Encode(publicKey);
            return new KeyPairAccount(
                publicKey: publicKeyHex,
                privateKey: privateKey
            );
        }
    }
}