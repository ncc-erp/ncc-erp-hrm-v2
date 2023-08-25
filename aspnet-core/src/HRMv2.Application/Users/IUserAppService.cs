using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using HRMv2.Roles.Dto;
using HRMv2.Users.Dto;

namespace HRMv2.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<RoleDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);
    }
}
