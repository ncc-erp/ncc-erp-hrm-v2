using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Utils;
using NccCore.Anotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    [AutoMap(typeof(ChartDetail))]
    public class ChartDetailDto : EntityDto<long>
    {
        public long ChartId { get; set; }

        [ApplySearch]
        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; }
        [JsonIgnore]
        public string JobPositionIds { get; set; }
        [JsonIgnore]
        public string LevelIds { get; set; }
        [JsonIgnore]
        public string BranchIds { get; set; }
        [JsonIgnore]
        public string TeamIds { get; set; }
        [JsonIgnore]
        public string UserTypes { get; set; }
        [JsonIgnore]
        public string PayslipDetailTypes { get; set; }
        [JsonIgnore]
        public string Gender { get; set; }
        [JsonIgnore]
        public string WorkingStatuses { get; set; }

        public List<long> ListJobPositionId => CommonUtil.ConvertStringToList<long>(JobPositionIds);
        public List<long> ListLevelId => CommonUtil.ConvertStringToList<long>(LevelIds);
        public List<long> ListBranchId => CommonUtil.ConvertStringToList<long>(BranchIds);
        public List<long> ListTeamId => CommonUtil.ConvertStringToList<long>(TeamIds);
        public List<UserType> ListUserType => CommonUtil.ConvertStringToList<UserType>(UserTypes);
        public List<PayslipDetailType> ListPayslipDetailType => CommonUtil.ConvertStringToList<PayslipDetailType>(PayslipDetailTypes);
        public List<Sex> ListGender => CommonUtil.ConvertStringToList<Sex>(Gender);
        public List<EmployeeMonthlyStatus> ListWorkingStatus => CommonUtil.ConvertStringToList<EmployeeMonthlyStatus>(WorkingStatuses);

    }
}
