using HRMv2.Manager.Categories.UserTypes.Dto;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.Constants;
using HRMv2.Utils;

namespace HRMv2.Manager.Categories.UserTypes
{
    public class UserTypeManager : BaseManager
    {
        public UserTypeManager(IWorkScope workScope) : base(workScope)
        {
        }

        public List<UserTypeDto> GetAll()
        {
            return CommonUtil.USERTYPE_COLOR;

        }

    }
}
