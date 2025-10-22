using MmnDotNetSdk.Models;
using MmnDotNetSdk.Utils;
using System.Numerics;
using System.Text.Json;
using Xunit;

namespace MmnDotNetSdk.Tests
{
    public class MmnClientTests
    {
        private readonly MmnClient _client;

        public MmnClientTests()
        {
            var config = new Config
            {
                Endpoint = TestData.TestMMNEndpoint,
                ZkProveEndpoint = TestData.TestZkVerifyUrl
            };
            _client = new MmnClient(config);
        }

        private static (string publicKey, byte[] seed) GetFaucetAccount()
        {
            Console.WriteLine("getFaucetAccount");
            var faucetPrivateKeyHex = TestData.TestFaucetPrivateKeyHex;
            var faucetPrivateKeyDer = Convert.FromHexString(faucetPrivateKeyHex);
            Console.WriteLine("faucetPrivateKeyDer");

            var faucetSeed = faucetPrivateKeyDer.Skip(faucetPrivateKeyDer.Length - 32).ToArray();
            var (faucetPublicKey, _) = GenerateEd25519KeyPair(faucetSeed);
            var faucetPublicKeyBase58 = CryptoHelper.Base58Encode(faucetPublicKey);
            Console.WriteLine($"faucetPublicKeyBase58: {faucetPublicKeyBase58}");

            return (faucetPublicKeyBase58, faucetSeed);
        }

        private static (byte[] publicKey, byte[] privateKey) GenerateEd25519KeyPair(byte[] seed)
        {
            if (seed.Length != 32)
                throw new ArgumentException("Seed must be 32 bytes");

            var publicKey = new byte[32];
            var privateKey = new byte[64];

            Chaos.NaCl.Ed25519.KeyPairFromSeed(publicKey, privateKey, seed);

            return (publicKey, privateKey);
        }

        private async Task FundTestAccount(string toAddress)
        {
            try
            {
                var (faucetPublicKey, faucetSeed) = GetFaucetAccount();
                Console.WriteLine($"faucetPublicKey: {faucetPublicKey}");

                var faucetAccount = await _client.NodeClient.GetAccountAsync(faucetPublicKey);
                var nextNonce = faucetAccount.Nonce + 1;
                Console.WriteLine($"Faucet account nonce: {faucetAccount.Nonce}, using next nonce: {nextNonce}");
                var transferType = TxType.Faucet;
                var fromAddr = faucetPublicKey;
                var fromAccount = await _client.NodeClient.GetAccountAsync(fromAddr);

                var toAddr = toAddress;
                var amount = BigInteger.Parse(TestData.TestFaucetAmount);
                var nonce = fromAccount.Nonce + 1;
                var textData = "Integration test transfer";

                var extraInfo = new Dictionary<string, string>
                {
                    ["type"] = "unlock_item"
                };

                var unsigned = CryptoHelper.BuildTransferTx(
                    (int)transferType,
                    fromAddr,
                    toAddr,
                    amount,
                    nonce,
                    (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    textData,
                    extraInfo,
                    "",
                    ""
                );

                var signedRaw = CryptoHelper.SignTx(unsigned, CryptoHelper.Base58Decode(faucetPublicKey), faucetSeed);

                Assert.True(CryptoHelper.Verify(unsigned, signedRaw.Sig));

                var res = await _client.NodeClient.AddTxAsync(signedRaw);

                Console.WriteLine($"Transaction successful! Hash: {res.TxHash}");

                await Task.Delay(3000);

                var toAccount = await _client.NodeClient.GetAccountAsync(toAddress);
                Console.WriteLine($"Account {toAddress} balance: {toAccount.Balance} tokens, nonce: {toAccount.Nonce}");

                var actualTxInfo = await _client.NodeClient.GetTxByHashAsync(res.TxHash);
                Console.WriteLine($"Transaction info: {JsonSerializer.Serialize(actualTxInfo)}");

                var actualTxExtra = JsonSerializer.Deserialize<Dictionary<string, string>>(actualTxInfo.ExtraInfo);
                Assert.NotNull(actualTxExtra);
                Assert.Equal("unlock_item", actualTxExtra["type"]);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                Console.WriteLine($"Faucet send token test failed (endpoint may not be available): {ex.Message}");
            }
        }

        [Fact]
        public void TestCryptoHelper_GenerateAddress()
        {
            // Test with a known input
            var input = "3767478432163172990";
            var expectedAddress = "DqrAfFo3yDQJhKuUo948RG4XfygHJPEe4UhcXxHF8hS2";
            var address = CryptoHelper.GenerateAddress(input);
            Console.WriteLine($"Generated address for '{input}': {address}");

            // Verify the address is not null or empty
            Assert.NotNull(address);
            Assert.NotEmpty(address);
            Assert.Equal(address, expectedAddress);
        }

        [Fact]
        public async Task TestClient_CheckHealth()
        {
            try
            {
                var resp = await _client.NodeClient.CheckHealthAsync();

                Assert.NotNull(resp);

                if (resp.Status == Mmn.HealthCheckResponse.Types.ServingStatus.Unknown)
                {
                    Assert.Fail($"Warning: Node localhost:9001 returned UNKNOWN status");
                }

                if (resp.Status == Mmn.HealthCheckResponse.Types.ServingStatus.NotServing)
                {
                    Assert.Fail($"Warning: Node localhost:9001 is NOT_SERVING");
                }

                Console.WriteLine($"Health Check - Status: {resp.Status}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                Console.WriteLine($"Health check failed (endpoint may not be available): {ex.Message}");
            }
        }

        [Fact]
        public async Task TestClient_SendTokenZk()
        {
            try
            {
                var senderZkAccount = await ZkAccount.GenerateZkAccount(_client.ZkProveClient);
                await FundTestAccount(senderZkAccount.Address);
                var toAddress = TestData.TestToAddress;

                var senderPrivateKeyBytes = senderZkAccount.PrivateKey;
                var fromSeed = senderPrivateKeyBytes.Skip(senderPrivateKeyBytes.Length - 32).ToArray();

                var fromAccount = await _client.NodeClient.GetAccountAsync(senderZkAccount.Address);
                var nextNonce = fromAccount.Nonce + 1;
                Console.WriteLine($"From account nonce: {fromAccount.Nonce}, using next nonce: {nextNonce}");

                var amount = BigInteger.Parse(TestData.TestTransferAmount);
                var nonce = fromAccount.Nonce + 1;
                var textData = "Integration test transfer";

                var extraInfo = new Dictionary<string, string>
                {
                    ["type"] = "transfer"
                };

                var unsigned = CryptoHelper.BuildTransferTx(
                    (int)TxType.Transfer,
                    senderZkAccount.Address,
                    toAddress,
                    amount,
                    nonce,
                    (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    textData,
                    extraInfo,
                    senderZkAccount.ZkProof,
                    senderZkAccount.ZkPub);

                var fromPublicKeyBytes = CryptoHelper.Base58Decode(senderZkAccount.PublicKey);
                var signedRaw = CryptoHelper.SignTx(unsigned, fromPublicKeyBytes, fromSeed);

                Assert.True(CryptoHelper.Verify(unsigned, signedRaw.Sig));

                var res = await _client.NodeClient.AddTxAsync(signedRaw);

                Console.WriteLine($"Transaction successful! Hash: {res.TxHash}");

                await Task.Delay(3000);

                var updatedFromAccount = await _client.NodeClient.GetAccountAsync(senderZkAccount.Address);
                Console.WriteLine($"Account {senderZkAccount.Address} balance: {updatedFromAccount.Balance} tokens, nonce: {updatedFromAccount.Nonce}");

                var toAccount = await _client.NodeClient.GetAccountAsync(toAddress);
                Console.WriteLine($"Account {toAddress} balance: {toAccount.Balance} tokens, nonce: {toAccount.Nonce}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                Console.WriteLine($"Send token test failed (endpoint may not be available): {ex.Message}");
            }
        }

        [Fact]
        public async Task TestClient_SendTokenKeyPair()
        {
            try
            {
                var senderKeyPairAccount = ZkAccount.GenerateKeyPairAccount();
                await FundTestAccount(senderKeyPairAccount.PublicKey);
                var toAddress = TestData.TestToAddress;

                var senderPrivateKeyBytes = senderKeyPairAccount.PrivateKey;
                var fromSeed = senderPrivateKeyBytes.Skip(senderPrivateKeyBytes.Length - 32).ToArray();

                var fromAccount = await _client.NodeClient.GetAccountAsync(senderKeyPairAccount.PublicKey);
                var nextNonce = fromAccount.Nonce + 1;
                Console.WriteLine($"From account nonce: {fromAccount.Nonce}, using next nonce: {nextNonce}");

                var amount = BigInteger.Parse(TestData.TestTransferAmount);
                var nonce = fromAccount.Nonce + 1;
                var textData = "Integration test transfer";

                var extraInfo = new Dictionary<string, string>
                {
                    ["type"] = "transfer"
                };

                var unsigned = CryptoHelper.BuildTransferTx(
                    (int)TxType.Faucet,
                    senderKeyPairAccount.PublicKey,
                    toAddress,
                    amount,
                    nonce,
                    (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    textData,
                    extraInfo,
                    "",
                    "");

                var fromPublicKeyBytes = CryptoHelper.Base58Decode(senderKeyPairAccount.PublicKey);
                var signedRaw = CryptoHelper.SignTx(unsigned, fromPublicKeyBytes, fromSeed);

                Assert.True(CryptoHelper.Verify(unsigned, signedRaw.Sig));

                var res = await _client.NodeClient.AddTxAsync(signedRaw);

                Console.WriteLine($"Transaction successful! Hash: {res.TxHash}");

                await Task.Delay(3000);

                var updatedFromAccount = await _client.NodeClient.GetAccountAsync(senderKeyPairAccount.PublicKey);
                Console.WriteLine($"Account {senderKeyPairAccount.PublicKey} balance: {updatedFromAccount.Balance} tokens, nonce: {updatedFromAccount.Nonce}");

                var toAccount = await _client.NodeClient.GetAccountAsync(toAddress);
                Console.WriteLine($"Account {toAddress} balance: {toAccount.Balance} tokens, nonce: {toAccount.Nonce}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                Console.WriteLine($"Send token test failed (endpoint may not be available): {ex.Message}");
            }
        }
    }
}
