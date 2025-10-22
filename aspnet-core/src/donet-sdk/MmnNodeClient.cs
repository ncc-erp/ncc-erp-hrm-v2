using Grpc.Net.Client;
using Mmn;
using MmnDotNetSdk.Models;
using MmnDotNetSdk.Utils;
using System.Numerics;

namespace MmnDotNetSdk
{
    public class MmnNodeClient
    {
        private readonly GrpcChannel _channel;
        private readonly HealthService.HealthServiceClient _healthClient;
        private readonly TxService.TxServiceClient _txClient;
        private readonly AccountService.AccountServiceClient _accClient;
        private bool _disposed = false;

        public MmnNodeClient(string endpoint)
        {
            _channel = GrpcChannel.ForAddress(endpoint);
            _healthClient = new HealthService.HealthServiceClient(_channel);
            _txClient = new TxService.TxServiceClient(_channel);
            _accClient = new AccountService.AccountServiceClient(_channel);
        }

        public async Task<Mmn.HealthCheckResponse> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _healthClient.CheckAsync(new Empty(), cancellationToken: cancellationToken);

                return response;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Health check failed: {ex.Message}", ex);
            }
        }

        public async Task<Models.AddTxResponse> AddTxAsync(SignedTx tx, CancellationToken cancellationToken = default)
        {
            try
            {
                var signedTxMsg = ProtoConverter.ToProtoSigTx(tx);

                var response = await _txClient.AddTxAsync(signedTxMsg, cancellationToken: cancellationToken);

                if (!response.Ok)
                {
                    throw new InvalidOperationException($"Add transaction failed: {response.Error}");
                }

                return new Models.AddTxResponse
                {
                    Ok = response.Ok,
                    TxHash = response.TxHash,
                    Error = response.Error
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Add transaction failed: {ex.Message}", ex);
            }
        }

        public async Task<Account> GetAccountAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _accClient.GetAccountAsync(new GetAccountRequest { Address = address }, cancellationToken: cancellationToken);

                return ProtoConverter.FromProtoAccount(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAccountAsync failed for address {address}: {ex}");
                return new Account
                {
                    Address = address,
                    Balance = BigInteger.Zero,
                    Nonce = 0
                };
            }
        }

        public async Task<TxHistoryResponse> GetTxHistoryAsync(string address, int limit, int offset, int filter, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _accClient.GetTxHistoryAsync(new GetTxHistoryRequest
                {
                    Address = address,
                    Limit = (uint)limit,
                    Offset = (uint)offset,
                    Filter = (uint)filter
                }, cancellationToken: cancellationToken);

                return ProtoConverter.FromProtoTxHistory(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Get transaction history failed: {ex.Message}", ex);
            }
        }

        public Task<TxService.TxServiceClient> SubscribeTransactionStatusAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.FromResult(_txClient);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Subscribe transaction status failed: {ex.Message}", ex);
            }
        }

        public async Task<Models.TxInfo> GetTxByHashAsync(string txHash, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _txClient.GetTxByHashAsync(new GetTxByHashRequest { TxHash = txHash }, cancellationToken: cancellationToken);

                if (!string.IsNullOrEmpty(response.Error))
                {
                    throw new InvalidOperationException($"Get transaction by hash failed: {response.Error}");
                }

                return new Models.TxInfo
                {
                    Sender = response.Tx.Sender,
                    Recipient = response.Tx.Recipient,
                    Amount = ProtoConverter.Uint256FromString(response.Tx.Amount),
                    Timestamp = (long)response.Tx.Timestamp,
                    TextData = response.Tx.TextData,
                    Nonce = response.Tx.Nonce,
                    Status = response.Tx.Status.ToString(),
                    ErrMsg = response.Tx.ErrMsg,
                    ExtraInfo = response.Tx.ExtraInfo
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Get transaction by hash failed: {ex.Message}", ex);
            }
        }

        public async Task<ulong> GetCurrentNonceAsync(string address, string tag, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _accClient.GetCurrentNonceAsync(new GetCurrentNonceRequest { Address = address, Tag = tag }, cancellationToken: cancellationToken);

                return response.Nonce;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Get current nonce failed: {ex.Message}", ex);
            }
        }

        public GrpcChannel Channel => _channel;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _channel?.Dispose();
                _disposed = true;
            }
        }
    }
}
