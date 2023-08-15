using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetExceptEmployeeDto : BaseEmployeeDto
    {
        public DateBetweenDto Seniority { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime StartWorkingDate { get; set; }
        public string? UserTypeCode { get; set; }
    }
}
