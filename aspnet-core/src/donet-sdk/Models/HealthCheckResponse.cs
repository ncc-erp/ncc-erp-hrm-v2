namespace MmnDotNetSdk.Models
{
    /// <summary>
    /// Response from health check operation
    /// </summary>
    public class HealthCheckResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
