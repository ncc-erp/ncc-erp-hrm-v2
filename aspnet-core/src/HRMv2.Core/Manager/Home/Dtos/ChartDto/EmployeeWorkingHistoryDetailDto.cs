using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    /// <summary>
    /// Employee Detail trong dto chỉ là ở thời điểm hiện tại, chỉ có Status là ứng với đúng thời gian DateAt
    /// </summary>
    public class EmployeeWorkingHistoryDetailDto
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public EmployeeStatus Status { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public UserType UserType { get; set; }
        public long BranchId { get; set; }
        public List<long> TeamIds { get; set; }
        public Sex Gender { get; set; }
        public DateTime DateAt { get; set; }
    }
}
