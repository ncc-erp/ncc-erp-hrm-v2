using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Histories.Dto
{
    public class EmployeeBranchHistoryDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public string Note { get; set; }
        public DateTime DateAt { get; set; }

        public  BadgeInfoDto BranchInfo { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public bool IsNotAllowToDelete { get; set; }
    }

    public class UpdateNoteBranchHistoryDto
    {
        public long Id { get; set; }
        public string Note { get; set; }
    }

    [AutoMapTo(typeof(EmployeeBranchHistory))]
    public class CreateBranchHistoryDto
    {
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public string Note { get; set; }
        public DateTime DateAt { get; set; }
    }
}
