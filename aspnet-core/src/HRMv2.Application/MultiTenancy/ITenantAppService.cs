using Abp.Application.Services;
using HRMv2.MultiTenancy.Dto;

namespace HRMv2.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

