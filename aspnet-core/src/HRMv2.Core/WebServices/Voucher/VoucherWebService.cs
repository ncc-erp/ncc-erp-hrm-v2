using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.WebServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Voucher
{
    public class VoucherWebService : BaseWebService
    {
        public VoucherWebService(HttpClient httpClient, IAbpSession abpSession, IIocResolver iocResovler) : base(httpClient, abpSession, iocResovler)
        {
            
        }

        public void CreateUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post($"api/users/hrm-create-user", input);
        }
        public void UpdateUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post($"api/users/hrm-update-user", input);
        }

    }
}
