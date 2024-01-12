using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.UI;
using Amazon.S3.Model;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Categories.Charts.Dto;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.JobPositions.Dto;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Charts.ChartDetails
{
    public class ChartDetailManager : BaseManager
    {
        protected readonly ChartManager _chartManager;
        protected readonly BranchManager _branchManager;
        protected readonly LevelManager _levelManager;
        protected readonly JobPositionManager _jobPositionManager;
        protected readonly TeamManager _teamManager;


        public ChartDetailManager(IWorkScope workScope,
            ChartManager chartManager,
            BranchManager branchManager,
            LevelManager levelManager,
            JobPositionManager jobPositionManager,
            TeamManager teamManager
            ) : base(workScope)
        {
            _chartManager = chartManager;
            _branchManager = branchManager;
            _levelManager = levelManager;
            _jobPositionManager = jobPositionManager;
            _teamManager = teamManager;
        }

        public ChartDetailSelectionDto GetChartDetailSelectionData()
        {
            var branches = _branchManager
                .QueryAllBranch()
                .Select(x => new KeyValueDto(x.Name, x.Id)).ToList();

            var levels = _levelManager
                .QueryAllLevel()
                .Select(x => new KeyValueDto(x.Name, x.Id)).ToList();

            var jobPositions = _jobPositionManager
                .QueryAllJobPosition()
                .Select(x => new KeyValueDto(x.Name, x.Id)).ToList();

            var teams = _teamManager
                .QueryAllTeam()
                .Select(x => new KeyValueDto(x.Name, x.Id)).ToList();

            var selectionData = new ChartDetailSelectionDto
            {
                Branches = branches,
                JobPositions = jobPositions,
                Levels = levels,
                Teams = teams,
                PayslipDetailTypes = CommonUtil.GetEnumKeyValueList<PayslipDetailType>(),
                UserTypes = CommonUtil.GetEnumKeyValueList<UserType>(),
                WorkingStatuses = CommonUtil.GetEnumKeyValueList<EmployeeStatus>()
                                    .Where(x => x.Value != Convert.ToInt64(EmployeeStatus.Pausing))
                                    .ToList(),
                Gender = CommonUtil.GetEnumKeyValueList<Sex>(),

            };

            return selectionData;
        }

        public IQueryable<ChartDetailDto> QueryAllChartDetail()
        {
            var query = WorkScope.GetAll<ChartDetail>()
                .OrderByDescending(c => c.LastModificationTime)
                .Select(c => new ChartDetailDto
                {
                    Id = c.Id,
                    IsActive = c.IsActive,
                    Name = c.Name,
                    BranchIds = c.BranchIds,
                    ChartId = c.ChartId,
                    Color = c.Color,
                    JobPositionIds = c.JobPositionIds,
                    LevelIds = c.LevelIds,
                    TeamIds = c.TeamIds,
                    PayslipDetailTypes = c.PayslipDetailTypes,
                    UserTypes = c.UserTypes,
                    WorkingStatuses = c.WorkingStatuses,
                    Gender = c.Gender
                });

            return query;
        }


        public List<ChartDetailDto> GetAll()
        {
            return QueryAllChartDetail().ToList();
        }

        public async Task<ChartFullInfoDto> GetAllDetailsByChartId(long chartId)
        {
            var chart = await _chartManager.Get(chartId);
            var chartFullDetail = ObjectMapper.Map<ChartFullInfoDto>(chart);

            var query = QueryAllChartDetail();
            var chartDetails = query
                .Where(c => c.ChartId == chartId)
                .ToList();


            var selectionData = GetChartDetailSelectionData();

            foreach (var chartDetail in chartDetails)
            {
                var chartDetailContainBaseInfo = GetDataForChartDetailFullDto(chartDetail, selectionData);

                chartFullDetail.ChartDetails.Add(chartDetailContainBaseInfo);
            }

            return chartFullDetail;
        }

        public async Task<ChartDetailFullDto> Get(long id)
        {
            var chartDetail = QueryAllChartDetail().Single(x => x.Id == id);

            var selectionData = GetChartDetailSelectionData();

            var chartDetailContainBaseInfo = GetDataForChartDetailFullDto(chartDetail, selectionData);


            //var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailContainBaseInfo;
        }

        public ChartDetailFullDto GetDataForChartDetailFullDto(
            ChartDetailDto chartDetail,
            ChartDetailSelectionDto selectionData
            )
        {
            var chartDetailContainBaseInfo = new ChartDetailFullDto
            {
                Id = chartDetail.Id,
                ChartId = chartDetail.ChartId,
                Color = chartDetail.Color,
                Name = chartDetail.Name,
                IsActive = chartDetail.IsActive,
                Branches = chartDetail.ListBranchIds.Select(id => new KeyValueDto
                {
                    Key = selectionData.Branches.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                JobPositions = chartDetail.ListJobPositionIds.Select(id => new KeyValueDto
                {
                    Key = selectionData.JobPositions.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                Levels = chartDetail.ListLevelIds.Select(id => new KeyValueDto
                {
                    Key = selectionData.Levels.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                Teams = chartDetail.ListTeamIds.Select(id => new KeyValueDto
                {
                    Key = selectionData.Teams.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                PayslipDetailTypes = CommonUtil.GetEnumKeyValueList(chartDetail.ListPayslipDetailTypes),
                UserTypes = CommonUtil.GetEnumKeyValueList(chartDetail.ListUserTypes),
                WorkingStatuses = CommonUtil.GetEnumKeyValueList(chartDetail.ListWorkingStatuses),
                Gender = CommonUtil.GetEnumKeyValueList(chartDetail.ListGender)
            };


            //var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailContainBaseInfo;
        }

        public async Task<ChartDetail> Create(CreateChartDetailDto createChartDetailDto)
        {
            //var chart = await _chartManager.Get(createChartDetailDto.ChartId);

            // validate
            var isExistedNameInChart = WorkScope.GetAll<ChartDetail>().Where(cd => cd.ChartId == createChartDetailDto.ChartId).Any(cd => cd.Name == createChartDetailDto.Name);

            if (isExistedNameInChart)
            {
                throw new UserFriendlyException($"ChartDetail name {createChartDetailDto.Name} is already existed in ChartID: {createChartDetailDto.ChartId}");
            }

            var chartDetail = ObjectMapper.Map<ChartDetail>(createChartDetailDto);
            chartDetail.JobPositionIds = CommonUtil.ConvertListToJson(createChartDetailDto.JobPositionIds);
            chartDetail.LevelIds = CommonUtil.ConvertListToJson(createChartDetailDto.LevelIds);
            chartDetail.BranchIds = CommonUtil.ConvertListToJson(createChartDetailDto.BranchIds);
            chartDetail.TeamIds = CommonUtil.ConvertListToJson(createChartDetailDto.TeamIds);
            chartDetail.UserTypes = CommonUtil.ConvertListToJson(createChartDetailDto.UserTypes);
            chartDetail.PayslipDetailTypes = CommonUtil.ConvertListToJson(createChartDetailDto.PayslipDetailTypes);
            chartDetail.Gender = CommonUtil.ConvertListToJson(createChartDetailDto.Gender);
            chartDetail.WorkingStatuses = CommonUtil.ConvertListToJson(createChartDetailDto.WorkingStatuses);

            chartDetail.Id = await WorkScope.InsertAndGetIdAsync(chartDetail);

            return chartDetail;
        }

        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)
        {

            var chartDetail = await WorkScope.GetAsync<ChartDetail>(updateChartDetailDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<ChartDetail>().Any(c => 
                c.ChartId == chartDetail.ChartId &&
                c.Name == updateChartDetailDto.Name && 
                c.Id != updateChartDetailDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"ChartDetail name {updateChartDetailDto.Name} is already existed");
            }

            // update
            ObjectMapper.Map(updateChartDetailDto, chartDetail);
            chartDetail.JobPositionIds = CommonUtil.ConvertListToJson(updateChartDetailDto.JobPositionIds);
            chartDetail.LevelIds = CommonUtil.ConvertListToJson(updateChartDetailDto.LevelIds);
            chartDetail.BranchIds = CommonUtil.ConvertListToJson(updateChartDetailDto.BranchIds);
            chartDetail.TeamIds = CommonUtil.ConvertListToJson(updateChartDetailDto.TeamIds);
            chartDetail.UserTypes = CommonUtil.ConvertListToJson(updateChartDetailDto.UserTypes);
            chartDetail.PayslipDetailTypes = CommonUtil.ConvertListToJson(updateChartDetailDto.PayslipDetailTypes);
            chartDetail.Gender = CommonUtil.ConvertListToJson(updateChartDetailDto.Gender);
            chartDetail.WorkingStatuses = CommonUtil.ConvertListToJson(updateChartDetailDto.WorkingStatuses);

            await WorkScope.UpdateAsync(chartDetail);

            return chartDetail;
        }


        public async Task<ChartDetailDto> Active(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = true;

            await WorkScope.UpdateAsync(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<ChartDetailDto> DeActive(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = false;

            await WorkScope.UpdateAsync(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<ChartDetail>(id);

            return id;
        }
    }
}
