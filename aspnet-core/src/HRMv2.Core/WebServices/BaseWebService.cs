using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using HRMv2.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices
{
    public abstract class BaseWebService
    {
        private readonly HttpClient httpClient;
        protected readonly ILogger<BaseWebService> Logger;
        private readonly IAbpSession _abpSession;
        private readonly TenantManager _tenantManager;

        public BaseWebService(HttpClient httpClient, IAbpSession abpSession, IIocResolver iocResovler)
        {
            this.httpClient = httpClient;
            Logger = IocManager.Instance.Resolve<ILogger<BaseWebService>>();
            this._abpSession = abpSession;
            _tenantManager = iocResovler.Resolve<TenantManager>();
            AddTenantNameToHeader();
        }
        public void SetAuthorizationToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        protected virtual async Task<T> GetAsync<T>(string url)
        {
            var logInfo = $"Get: BaseAddress [{httpClient.BaseAddress}], url: {url}";
            try
            {
                Logger.LogInformation(logInfo);
                var response = await httpClient.GetAsync(url);

                var responseContent = await response.Content.ReadAsStringAsync();
                Logger.LogInformation($"{logInfo} response: {responseContent}");
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                Logger.LogError($"{logInfo} error: {ex.Message}");
            }
            return default;

        }
        protected virtual async Task<T> PostAsync<T>(string url, object input)
        {
            var strInput = JsonConvert.SerializeObject(input);
            var logInfo = $"Post: BaseAddress [{httpClient.BaseAddress}], url: {url}, input: {strInput}";
            var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
            try
            {
                Logger.LogInformation(logInfo);
                var response = await httpClient.PostAsync(url, contentString);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Logger.LogInformation($"{logInfo} response: {responseContent}");
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"{logInfo} error: {ex.Message}");
            }
            return default;
        }

        protected virtual async Task<T> PostAllowErrorResponseAsync<T>(string url, object input)
        {
            var strInput = JsonConvert.SerializeObject(input);
            var logInfo = $"Post: BaseAddress [{httpClient.BaseAddress}], url: {url}, input: {strInput}";
            var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");
            try
            {
                Logger.LogInformation(logInfo);
                var response = await httpClient.PostAsync(url, contentString);
                var responseContent = await response.Content.ReadAsStringAsync();

                Logger.LogInformation($"{logInfo} response ({(int)response.StatusCode}): {responseContent}");

                var result = JsonConvert.DeserializeObject<T>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"{logInfo} error: {ex.Message}");
            }
            return default;
        }

        protected virtual async Task<T> PostFormUrlEncodedAsync<T>(string url, Dictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            try
            {
                var response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"{url} error: {ex.Message}");
            }
            return default;
        }

        public void Post(string url, object input)
        {
            string strInput = JsonConvert.SerializeObject(input);
            var logInfo = $"Post: BaseAddress [{httpClient.BaseAddress}], url: {url}, input: {strInput}";
            try
            {
                var contentString = new StringContent(strInput, Encoding.UTF8, "application/json");

                Logger.LogInformation(logInfo);

                httpClient.PostAsync(url, contentString);
            }
            catch (Exception e)
            {
                Logger.LogError($"{logInfo} Error: {e.Message}");
            }

        }
        protected string GetTenantName()
        {
            if (!_abpSession.TenantId.HasValue) return string.Empty;
            var tenant = _tenantManager.FindById(_abpSession.TenantId.Value);
            return tenant.TenancyName;
        }
        private void AddTenantNameToHeader()
        {
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Add("Abp-TenantName", GetTenantName());
        }


    }
}
