using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.UploadFileServices
{
    public interface IUploadFileService : ITransientDependency
    {
        Task<string> UploadAvatar(IFormFile file, string tenantName);
        Task<string> UploadFiles(IFormFile file, string tenantName, string prefix);
    }
}
