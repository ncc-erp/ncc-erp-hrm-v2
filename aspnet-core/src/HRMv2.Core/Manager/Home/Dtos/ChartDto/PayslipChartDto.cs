using Abp.Application.Services.Dto;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class PayslipChartDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public double Salary { get; set; }
        public Sex Gender { get; set; }
        public long BranchId { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public List<long> TeamIds { get; set; }
        public UserType UserType { get; set; }
        public DateTime CreationTime { get; set; }
    }

}
