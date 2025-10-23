using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MmnDotNetSdk.Models;

namespace MmnDotNetSdk
{
    public class ProveResponseData
    {
        [JsonPropertyName("proof")]
        public string? Proof { get; set; }

        [JsonPropertyName("public_input")]
        public string? PublicInput { get; set; }
    }

    public class ProveResponse
    {
        [JsonPropertyName("data")]
        public ProveResponseData? Data { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    public class ZkProveClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        public ZkProveClient(string endpoint)
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _endpoint = endpoint;
        }

        public async Task<ProveResponse?> GenerateZkProof(string userId, string address, string ephemeralPk, string jwt)
        {
            var url = $"{_endpoint}/prove";

            // Create request body
            var requestBody = new Dictionary<string, string>
            {
                ["user_id"] = userId,
                ["address"] = address,
                ["ephemeral_pk"] = ephemeralPk,
                ["jwt"] = jwt
            };

            // Convert to JSON
            var jsonData = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Set headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");

            // Send request
            var response = await _httpClient.PostAsync(url, content);

            // Check response status
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Prove request failed: {(int)response.StatusCode} {response.ReasonPhrase} - {errorBody}");
            }

            // Parse response
            var responseBody = await response.Content.ReadAsStringAsync();

            var proveResp = JsonSerializer.Deserialize<ProveResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return proveResp;
        }
        
        public async Task<HealthCheckResponse> HealthCheckAsync()
        {
            var url = $"{_endpoint}/health/check";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Health check failed: {(int)response.StatusCode} {response.ReasonPhrase} - {errorBody}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var health = JsonSerializer.Deserialize<HealthCheckResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return health ?? new HealthCheckResponse();
        }
        
    }
}
