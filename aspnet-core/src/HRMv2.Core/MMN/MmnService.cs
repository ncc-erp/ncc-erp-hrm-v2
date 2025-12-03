using Abp.Dependency;
using Amazon.S3.Model.Internal.MarshallTransformations;
using HRMv2.Constants;
using HRMv2.Manager.MezonTokens.Dto;
using HRMv2.WebServices.Dto;
using Microsoft.Extensions.Logging;
using Mmn;
using MmnDotNetSdk;
using MmnDotNetSdk.Models;
using MmnDotNetSdk.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
namespace HRMv2.MMN
{
    public class MmnService : ISingletonDependency
    {
        private readonly MmnClient _client;
        protected readonly ILogger<MmnService> Logger;
        public MmnService()
        {
            var config = new Config
            {
                Endpoint = MmnConstant.NodeEndpoint,
                ZkProveEndpoint = MmnConstant.ZkProveEndpoint
            };
            _client = new MmnClient(config);
            Logger = IocManager.Instance.Resolve<ILogger<MmnService>>();
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

        public async Task<MmnDotNetSdk.Models.AddTxResponse> TransferToken(MmnTransferTokenDto mmnTransferTokenDto)
        {
            try
            {
                var senderId = MezonTokenConstant.BotId;
                var senderAddress = CryptoHelper.GenerateAddress(senderId);
                var botAccount = await this.GetAmount(senderAddress);
                var toAddress = CryptoHelper.GenerateAddress(mmnTransferTokenDto.receiver_id.ToString());

                var currentNonce = await _client.NodeClient.GetCurrentNonceAsync(senderAddress, "pending");
                var nextNonce = currentNonce + 1;

                var amount = BigInteger.Parse(mmnTransferTokenDto.amount.ToString());
                var amountToDecimal = ValidationHelper.AmountToDecimal(amount);

                var extraInfo = new Dictionary<string, string>
                {
                    ["type"] = MmnConstant.HRMTransferType,
                    ["UserSenderId"] = senderId.ToString(),
                    ["UserReceiverId"] = mmnTransferTokenDto.receiver_id.ToString()
                };
                // Lấy key pair từ config 
                var privateKeyHex = MezonTokenConstant.EphemeralMmnKeyPair;
                var (publicKeyBase58, privateKeySeed) = this.LoadKeyPair(privateKeyHex);

                // Lấy zk proof
                var (zkProof, zkPub, address) = await GetZkProof(mmnTransferTokenDto.JwtTokenBot, senderId, publicKeyBase58);

                var unsigned = CryptoHelper.BuildTransferTx(
                    (int)TxType.Transfer,
                    senderAddress,
                    toAddress,
                    amountToDecimal,
                    nextNonce,
                    (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    mmnTransferTokenDto.note,
                    extraInfo,
                    zkProof,
                    zkPub);

                //Load publicKey
                var fromPublicKeyBytes = CryptoHelper.Base58Decode(publicKeyBase58);
                var signedRaw = CryptoHelper.SignTx(unsigned, fromPublicKeyBytes, privateKeySeed);

                var res = await _client.NodeClient.AddTxAsync(signedRaw);

                return res;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return new MmnDotNetSdk.Models.AddTxResponse
                {
                    Ok = false,
                    TxHash = string.Empty,
                    Error = ex.Message
                };
            }
        }
        public async Task<GetResultConnectDto> CheckHealthMmnClient()
        {
            var client = new MmnClient(new MmnDotNetSdk.Models.Config
            {
                Endpoint = MmnConstant.NodeEndpoint,
                ZkProveEndpoint = MmnConstant.ZkProveEndpoint
            });
            try
            {
                var resp = await client.NodeClient.CheckHealthAsync();
                var respZk = await client.ZkProveClient.HealthCheckAsync();
            }
            catch
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to MMNService"
                };
            }
            return new GetResultConnectDto
            {
                IsConnected = true,
                Message = "Connected"
            };
        }
    }
}
