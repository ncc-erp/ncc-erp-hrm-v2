using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class GetBonusDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public bool IsActive { get; set; }
    }
    public class GetDetailBonusDto : GetBonusDto
    {
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string FullNameCreation { get; set; }
        public string FullNameModification { get; set; }

    }

    public class GetAllBonusDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeCount { get; set; }
        public double TotalMoney { get; set; }
    }
}
