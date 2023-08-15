using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System.ComponentModel.DataAnnotations;

namespace HRMv2.Manager.Categories
{
    [AutoMapTo(typeof(Branch))]
    public class BranchDto:EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }

        [ApplySearch]
        [Required]
        public string ShortName { get; set; }
        [Required]
        [ApplySearch]
        public string Code { get; set; }
        public string Address { get; set; }
        public string Color { get; set; }
        public long? CEOId { get; set; }
        public long? HRId { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyTaxCode { get; set; }
        public string NameInContract { get; set; }
        public string CEOFullName { get; set; }
        public string HRFullName { get; set; }
    }
}
