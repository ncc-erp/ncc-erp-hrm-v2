using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Punishments.Dto
{
    public class PunishmentDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public long? PunishmentTypeId { get; set; }
    }
    public class GetPunishmentDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeCount { get; set; }
        public double TotalMoney { get; set; }
        public bool IsActive { get; set; }

    }

    [AutoMapTo(typeof(Punishment))]
    public class GeneratePunishmentDto : EntityDto<long>
    {
        public List<long> PunishmentTypeIds { get; set; }
        public DateTime Date { get; set; }
    }

    [AutoMapTo(typeof(Punishment))]
    public class CreatePunishmentDto : EntityDto<long>
    {
        [StringLength(256)]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }

    [AutoMapTo(typeof(Punishment))]
    public class UpdatePunishmentDto : EntityDto<long>
    {
        [StringLength(256)]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public bool? isAbleUpdateNote { get; set; }
    }

    public class ResultGeneratePunishmentDto
    {
        public long PunishmentTypeId { get; set; }
        public string Message { get; set; }

    }


    public class GetEmployeePunishment
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public bool IsAndCondition { get; set; }
        public List<EmployeeStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> Usertypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }


    }
}
