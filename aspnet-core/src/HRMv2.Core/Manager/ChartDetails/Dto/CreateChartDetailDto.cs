﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChartDetails.Dto
{
    [AutoMap(typeof(ChartDetail))]
    public class CreateChartDetailDto 
    {
        public long ChartId { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public List<long> JobPositionIds { get; set; }

        public List<long> LevelIds { get; set; }

        public List<long> BranchIds { get; set; }

        public List<long> TeamIds { get; set; }

        public List<UserType> UserTypes { get; set; }

        public List<PayslipDetailType> PayslipDetailTypes { get; set; }

        public List<EmployeeStatus> WorkingStatuses { get; set; }
    }
}
