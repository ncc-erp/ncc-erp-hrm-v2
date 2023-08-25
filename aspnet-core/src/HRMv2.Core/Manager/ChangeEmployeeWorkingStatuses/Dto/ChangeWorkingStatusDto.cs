using HRMv2.Manager.Benefits.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto
{
    public class ToQuitDto : IIsConfirm
    {
        public long EmployeeId { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Note { get; set; }
        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }

        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class ToPauseDto : IIsConfirm 
    {
        public long EmployeeId { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime BackDate { get; set; }
        public string Note { get; set; }
        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class ToMaternityLeaveDto : IIsConfirm 
    {
        public long EmployeeId { get; set; }
        public long ToSalary { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime BackDate { get; set; }
        public string Note { get; set; }
        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class ToWorkingDto : IIsConfirm
    {
        public long EmployeeId { get; set; }
        public long ToLevelId { get; set; }
        public UserType ToUserType { get; set; }
        public long ToJobPositionId { get; set; }
        public bool HasContract { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public long RealSalary { get; set; }
        public long BasicSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime BackDate { get; set; }
        public string Note { get; set; }

        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public int? TenantId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class ExtendWorkingStatusDto
    {
        public long EmployeeId { get; set; }
        public DateTime BackDate { get; set; }
        public string Note { get; set; }
        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }
        public long? CurrentUserLoginId { get; set; }
    }

    public interface IIsConfirm
    {
        bool IsConfirmed { get; set; }
    }
}
