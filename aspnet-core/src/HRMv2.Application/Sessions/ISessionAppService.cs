using System.Threading.Tasks;
using Abp.Application.Services;
using HRMv2.Sessions.Dto;

namespace HRMv2.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
