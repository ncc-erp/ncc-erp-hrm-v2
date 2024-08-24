using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Authorization.Users.Dto
{
    [AutoMap(typeof(User))]
    public class GetUserName
    {
        public string UserName { get; set; }
    }
}
