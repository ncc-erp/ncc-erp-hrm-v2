using HRMv2.Manager.Categories.UserTypes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class SalaryInfo
    {
        public UserType ToUserType {  get; set; }
        public double ToSalary { get; set; }
        public double ContractBasicSalary { get; set; }
        public double ContractRealSalary { get; set; }
    }
}
