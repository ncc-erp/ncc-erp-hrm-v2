using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class UpdateBranchPayslip
    {
        public long PayslipId { get; set; }
        public long BranchId { get; set; }  
    }
    public class UpdateLevelPayslip
    {
        public long PayslipId { get; set; }
        public long LevelId { get; set; }
    }
    public class UpdateUserTypePayslip
    {
        public long PayslipId { get; set; }
        public UserType  UserType{ get; set; }
    }
    public class UpdatePositionPayslip
    {
        public long PayslipId { get; set; }
        public long JobPositionId { get; set; }
    }
}
