using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System.ComponentModel.DataAnnotations;

namespace HRMv2.Manager.Categories.IssuedBys.Dto
{
    [AutoMapTo(typeof(IssuedBy))]
    public class IssuedByDto : EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }
    }
}
