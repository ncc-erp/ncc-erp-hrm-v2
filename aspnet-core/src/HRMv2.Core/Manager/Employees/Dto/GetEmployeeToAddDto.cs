using HRMv2.Manager.Common.Dto;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeToAddDto : InputMultiFilterEmployeePagingDto
    {
        public List<long> AddedEmployeeIds { get; set; }
        public DateTime? BirthdayFromDate { get; set; }
        public DateTime? BirthdayToDate { get; set; }
    }

    
}
