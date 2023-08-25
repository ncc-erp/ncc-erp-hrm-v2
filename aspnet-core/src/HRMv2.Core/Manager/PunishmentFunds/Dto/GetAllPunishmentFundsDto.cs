using Abp.Application.Services.Dto;
using HRMv2.Authorization.Users;
using NccCore.Anotations;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.PunishmentFunds.Dto
{
    public class GetAllPunishmentFundsDto : EntityDto<long>
    {
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        [ApplySearch]
        public string Note { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreationUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUser { get; set; }
    }
    public class InputToGetAllPagingDto
    {
        public GridParam GridParam { get; set; }
        public FilterByComparision FilterByComparision { get; set; }
    }

    public class FilterByComparision
    {
        public Comparision OperatorComparison { get; set; }
        public string Value { get; set; }
    }
}
