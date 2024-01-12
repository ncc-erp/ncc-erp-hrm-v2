using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
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



        public List<long> ListJobPositionIds => (string.IsNullOrWhiteSpace(JobPositionIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(JobPositionIds);

        public List<long> ListLevelIds => (string.IsNullOrWhiteSpace(LevelIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(LevelIds);

        public List<long> ListBranchIds => (string.IsNullOrWhiteSpace(BranchIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(BranchIds);

        public List<long> ListTeamIds => (string.IsNullOrWhiteSpace(TeamIds))
                                                ? new List<long>()
                                                : JsonConvert.DeserializeObject<List<long>>(TeamIds);

        public List<UserType> ListUserTypes => (string.IsNullOrWhiteSpace(UserTypes))
                                                ? new List<UserType>()
                                                : JsonConvert.DeserializeObject<List<UserType>>(UserTypes);

        public List<PayslipDetailType> ListPayslipDetailTypes => (string.IsNullOrWhiteSpace(PayslipDetailTypes))
                                                ? new List<PayslipDetailType>()
                                                : JsonConvert.DeserializeObject<List<PayslipDetailType>>(PayslipDetailTypes);

        public List<Sex> ListGender => (string.IsNullOrWhiteSpace(Gender))
                                                ? new List<Sex>()
                                                : JsonConvert.DeserializeObject<List<Sex>>(Gender);

        public List<EmployeeStatus> ListWorkingStatuses => (string.IsNullOrWhiteSpace(WorkingStatuses))
                                                ? new List<EmployeeStatus>()
                                                : JsonConvert.DeserializeObject<List<EmployeeStatus>>(WorkingStatuses);

    }
}
