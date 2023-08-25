using Abp.Authorization;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs
{
    [AbpAuthorize]
    public class InitDataTestAppService : HRMv2AppServiceBase
    {
        public async Task<string> InitDataTestForCalculateSalary(int employeeCount)
        {
            var branches = new List<Branch>()
            {
                new Branch{Name = "HN1", Code = "hn1", Color = "#d73737"},
                new Branch{Name = "HN2", Code = "hn2", Color = "#d73737"},
                new Branch{Name = "Vinh", Code = "vinh", Color = "#d73737"},
                new Branch{Name = "DN", Code = "dn", Color = "#d73737"},
                new Branch{Name = "SG1", Code = "sg1", Color = "#d73737"},
                new Branch{Name = "SG2", Code = "sg2", Color = "#d73737"},
            };

            

            for(int i = 0; i< employeeCount; i++)
            {

            }
            return "ok";
        }
    }
}
