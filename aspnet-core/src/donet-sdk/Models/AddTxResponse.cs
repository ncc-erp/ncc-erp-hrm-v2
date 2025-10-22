namespace MmnDotNetSdk.Models
{
    /// <summary>
    /// Response from AddTx operation
    /// </summary>
    public class AddTxResponse
    {
        public bool Ok { get; set; }
        public string TxHash { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
