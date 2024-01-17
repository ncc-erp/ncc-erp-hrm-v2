using Abp.Application.Services.Dto;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Utils;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class PayslipDataChartDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public long PayrollId { get; set; }
        public string FullName { get; set; }
        public double Salary { get; set; }
        public Sex Gender { get; set; }
        public long BranchId { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public List<long> TeamIds { get; set; }
        public UserType UserType { get; set; }
        public DateTime ApplyMonth { get; set; }// Payroll ApplyMonth 
        public string MonthYear => DateTimeUtils.GetMonthYearLabelChart(ApplyMonth);
        
    }

}
