using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.Categories.UserTypes.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.UserTypes
{
    [AbpAuthorize]
    public class UserTypeAppService: HRMv2AppServiceBase
    {
        private readonly UserTypeManager _userTypeManager;
        public UserTypeAppService(UserTypeManager userTypeManager)
        {
            _userTypeManager = userTypeManager;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Category_Usertype_View)]
        public List<UserTypeDto> GetAll()
        {
            return _userTypeManager.GetAll();
        }
    }
}
