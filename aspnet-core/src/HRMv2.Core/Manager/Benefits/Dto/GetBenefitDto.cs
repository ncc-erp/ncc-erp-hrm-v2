using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Benefits.Dto
{
    public class GetBenefitDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public double Money { get; set; }
        public BenefitType Type { get; set; }
        public int UserCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsBelongToAllEmployee { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }

        public string BenefitTypeName
        {
            get
            {
                return Type switch
                {
                    BenefitType.CheDoChung => "Chế độ chung",
                    BenefitType.CheDoRieng => "Chế độ riêng",
                    BenefitType.CheDoRemote => "Chế độ remote",
                    _ => "",
                };
            }
        }
    }
}
