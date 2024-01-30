using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Utils;
using NccCore.Anotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Charts.Dto
{
    [AutoMap(typeof(Chart))]
    public class ChartDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }

        public ChartType ChartType { get; set; }

        public ChartDataType ChartDataType { get; set; }

        public TimePeriodType TimePeriodType { get; set; }

        public string ChartDataTypeName => Enum.GetName(typeof(ChartDataType), ChartDataType);

        public string ChartTypeName => Enum.GetName(typeof(ChartType), ChartType);

        public string TimePeriodTypeName => Enum.GetName(typeof(TimePeriodType), TimePeriodType);
        [JsonIgnore]
        public string ShareToUserIds { get; set; }
        [JsonIgnore]
        public string ShareToRoleIds { get; set; }

        public List<long> ListShareToUserIds => CommonUtil.ConvertStringToList<long>(ShareToUserIds);
        public List<long> ListShareToRoleIds => CommonUtil.ConvertStringToList<long>(ShareToRoleIds);
        public List<KeyValueDto> ShareToUsers { get; set; } = new List<KeyValueDto>();
        public List<KeyValueDto> ShareToRoles { get; set; } = new List<KeyValueDto>();
        public bool IsActive { get; set; }
    }

    public class ChartSelectionDto
    {
        public List<KeyValueDto> ShareToUsers { get; set; }
        public List<KeyValueDto> ShareToRoles { get; set; }

    }

}
