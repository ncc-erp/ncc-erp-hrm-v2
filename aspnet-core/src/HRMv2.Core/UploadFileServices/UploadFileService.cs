using Abp.Application.Services;
using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.MultiTenancy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.UploadFileServices
{
    public class UploadFileService : ApplicationService, ITransientDependency
    {
        private readonly IUploadFileService _uploadFileService;
        private readonly TenantManager _tenantManager;
        private readonly IAbpSession _session;
        public UploadFileService(IUploadFileService uploadFileService, TenantManager tenantManager , IAbpSession session)
        {
            _uploadFileService = uploadFileService;
            _tenantManager = tenantManager;
            _session = session;
        }

        public async Task<string> UploadAvatar(IFormFile file)
        {
            var tenantName =  await GetTenantName();
            var filePath = await  _uploadFileService.UploadAvatar(file, tenantName);
            return filePath;
        }

        public async Task<string> UploadFile(IFormFile file , string prefix)
        {
            var tenantName = await GetTenantName();
            var filePath = await _uploadFileService.UploadFiles(file, tenantName, prefix);
            return filePath;
        }

        private async Task<string> GetTenantName()
        {
            if (_session.TenantId.HasValue)
            {
                var tenant = await _tenantManager.GetByIdAsync(_session.TenantId.Value);
                return tenant.TenancyName;
            }
            return "host";
        }
    }
}
