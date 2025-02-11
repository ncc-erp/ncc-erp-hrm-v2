using Abp.Dependency;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using HRMv2.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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
