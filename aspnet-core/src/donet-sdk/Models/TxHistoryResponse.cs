namespace MmnDotNetSdk.Models
{
    /// <summary>
    /// Response from GetTxHistory operation
    /// </summary>
    public class TxHistoryResponse
    {
        public List<TxInfo> Transactions { get; set; } = new List<TxInfo>();
        public uint Total { get; set; }
        public List<TxMetaResponse> Txs { get; set; } = new List<TxMetaResponse>();
    }
}
