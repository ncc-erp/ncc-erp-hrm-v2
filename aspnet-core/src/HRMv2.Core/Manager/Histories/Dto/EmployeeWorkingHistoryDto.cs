using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Histories.Dto
{
    public class EmployeeWorkingHistoryDto: EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public EmployeeStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime DateAt { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }

        public bool IsNotAllowToDelete { get; set; }
    }

    public class UpdateNoteWorkingHistoryDto
    {
        public long Id { get; set; }
        public string Note { get; set; }
    }

    public class UpdateDateWorkingHistoryDto
    {
        public long Id { get; set; }
        public DateTime DateAt { get; set; }
    }

    [AutoMapTo(typeof(EmployeeWorkingHistory))]
    public class CreateWorkingHistoryDto
    {
        public long EmployeeId { get; set; }
        public EmployeeStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime DateAt { get; set; }
    }
}
