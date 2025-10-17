using Abp.Dependency;
using Amazon.S3.Model.Internal.MarshallTransformations;
using HRMv2.Constants;
using HRMv2.Manager.MezonTokens.Dto;
using Mmn;
using MmnDotNetSdk;
using MmnDotNetSdk.Models;
using MmnDotNetSdk.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
namespace HRMv2.MMN
{
    public class MmnService : ISingletonDependency
    {
        private readonly MmnClient _client;

        public MmnService()
        {
            var config = new Config
            {
                Endpoint = "http://172.16.100.180:9001",
                ZkProveEndpoint = "https://dev-mmn.nccsoft.vn/zk-api"
            };
            _client = new MmnClient(config);
        }

        public async Task<Account> GetAmount(string address)
        {
            return await _client.NodeClient.GetAccountAsync(address);
        }

        private (byte[] publicKey, byte[] privateKey) GenerateEd25519KeyPair(byte[] seed)
        {
            if (seed.Length != 32)
                throw new ArgumentException("Seed must be 32 bytes");

            var publicKey = new byte[32];
            var privateKey = new byte[64];

            Chaos.NaCl.Ed25519.KeyPairFromSeed(publicKey, privateKey, seed);

            return (publicKey, privateKey);
        }

        public (string publicKey, byte[] privateKey) LoadKeyPair(string privateKeyHex)
        {
            var privateKeyDer = Convert.FromHexString(privateKeyHex);
            var privateSeed = privateKeyDer.Skip(privateKeyDer.Length - 32).ToArray();
            var (publicKey, _) = GenerateEd25519KeyPair(privateSeed);
            var publicKeyBase58 = CryptoHelper.Base58Encode(publicKey);

            return (publicKeyBase58, privateSeed);
        }

        // JWT token from Mezon after user login
        public async Task<(string zkProof, string zkPub, string address)> GetZkProof(string JWT, string userId, string publicKeyBase58)
        {
            // Generate address from user ID
            var address = CryptoHelper.GenerateAddress(userId);
            // Generate ZK proof
            var proofRes = await _client.ZkProveClient.GenerateZkProof(userId, address, publicKeyBase58, JWT);
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
            return (zkProof, zkPub, address);
        }

        public async Task<string> CheckHealthClient()
        {
            var resp = await _client.NodeClient.CheckHealthAsync();
            return resp.Status.ToString();
        }

        public async Task<MmnDotNetSdk.Models.AddTxResponse> TransferToken(TransferTokenMezonDongDto transferTokenDto)
        {
            try
            {
                var toAddress = CryptoHelper.GenerateAddress(transferTokenDto.toUserId.ToString());

                var currentNonce = await _client.NodeClient.GetCurrentNonceAsync(transferTokenDto.senderAddress, "pending");
                var nextNonce = currentNonce + 1;

                var amount = BigInteger.Parse(transferTokenDto.transferAmount.ToString());
                var amountToDecimal = ValidationHelper.AmountToDecimal(amount);

                var extraInfo = new Dictionary<string, string>
                {
                    ["type"] = "transfer",
                    ["UserSenderId"] = transferTokenDto.senderId.ToString(),
                    ["UserReceiverId"] = transferTokenDto.toUserId.ToString()
                };

                var unsigned = CryptoHelper.BuildTransferTx(
                    (int)TxType.Transfer,
                    transferTokenDto.senderAddress,
                    toAddress,
                    amountToDecimal,
                    nextNonce,
                    (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    transferTokenDto.note,
                    extraInfo,
                    transferTokenDto.zkProof,
                    transferTokenDto.zkPub);

                //Load publicKey
                var fromPublicKeyBytes = CryptoHelper.Base58Decode(transferTokenDto.publicKeyBase58);
                var signedRaw = CryptoHelper.SignTx(unsigned, fromPublicKeyBytes, transferTokenDto.privateKeySeed);

                var res = await _client.NodeClient.AddTxAsync(signedRaw);

                return res;
            }
            catch (Exception ex)
            {
                return new MmnDotNetSdk.Models.AddTxResponse
                {
                    Ok = false,
                    TxHash = string.Empty,
                    Error = ex.Message
                };
            }
        }
    }
}
