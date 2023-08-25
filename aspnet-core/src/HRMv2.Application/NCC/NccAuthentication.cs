using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.UI;
using HRMv2.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.NCC
{
    public class NccAuthentication: ActionFilterAttribute
    {
        private readonly IAbpSession _abpSession;
        public NccAuthentication()
        {
            _abpSession = NullAbpSession.Instance;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
          var secretCode = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetValue<string>($"App:SecurityCode");
            var header = context.HttpContext.Request.Headers;
            var securityCodeHeader = header["X-Secret-Key"];
            if (string.IsNullOrEmpty(securityCodeHeader))
            {
                securityCodeHeader = header["securityCode"];
            }
            if (secretCode != securityCodeHeader)
                throw new UserFriendlyException($"SecretCode does not match! HRMCode: {secretCode.Substring(secretCode.Length - 3)} != {securityCodeHeader}");

            var abpTenantName = header["Abp-TenantName"].ToString();
            if (string.IsNullOrEmpty(abpTenantName)) return;

            var _tenantManager = context.HttpContext.RequestServices.GetService(typeof(TenantManager)) as TenantManager;
            var tenant = _tenantManager.FindByTenancyName(abpTenantName);
            if (tenant == null) 
                throw new Exception($"Not Found Tenant.");

            _abpSession.Use(tenant.Id, null);
        }
    }
}
