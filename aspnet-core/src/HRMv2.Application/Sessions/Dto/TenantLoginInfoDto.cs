using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.MultiTenancy;

namespace HRMv2.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
