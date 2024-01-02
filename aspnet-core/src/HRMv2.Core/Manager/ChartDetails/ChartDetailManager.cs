using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.Charts;
using HRMv2.Manager.Charts.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChartDetails
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
                .Select(x => new BaseInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            var levels = _levelManager
                .QueryAllLevel()
                .Select(x => new BaseInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            var jobPositions = _jobPositionManager
                .QueryAllJobPosition()
                .Select(x => new BaseInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            var teams = _teamManager
                .QueryAllTeam()
                .Select(x => new BaseInfoDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

            var selectionData = new ChartDetailSelectionDto
            {
                Branches = branches,
                JobPositions = jobPositions,
                Levels = levels,
                Teams = teams,
                PayslipDetailTypes = CommonUtil.GetEnumKeyValueList<PayslipDetailType>(),
                UserTypes = CommonUtil.GetEnumKeyValueList<UserType>(),
                WorkingStatuses = CommonUtil.GetEnumKeyValueList<EmployeeStatus>(),
                Sexes = CommonUtil.GetEnumKeyValueList<Sex>(),

            };

            return selectionData;
        }

        public IQueryable<ChartDetailDto> QueryAllChartDetail()
        {
            var query = WorkScope.GetAll<ChartDetail>().Select(c => new ChartDetailDto
            {
                Id = c.Id,
                IsActive = c.IsActive,
                Name = c.Name,
                BranchIds = c.BranchIds,
                ChartId = c.ChartId,
                Color = c.Color,
                JobPositionIds = c.JobPositionIds,
                LevelIds = c.LevelIds,
                PayslipDetailTypes = c.PayslipDetailTypes,
                TeamIds = c.TeamIds,
                UserTypes = c.UserTypes,
                WorkingStatuses = c.WorkingStatuses,
                Sexes = c.Sexes
            });

            return query;
        }


        public List<ChartDetailDto> GetAll()
        {
            return QueryAllChartDetail().ToList();
        }

        public async Task<ChartFullDto> GetAllDetailsByChartId(long chartId)
        {
            var chart = await _chartManager.Get(chartId);
            var chartFullDetail = ObjectMapper.Map<ChartFullDto>(chart);

            var query = QueryAllChartDetail();
            var chartDetails = query
                .Where(c => c.ChartId == chartId)
                .ToList();

            foreach (var chartDetail in chartDetails)
            {
                var chartDetailContainBaseInfo = new ChartDetailFullDto
                {
                    Id = chartDetail.Id,
                    ChartId = chartDetail.ChartId,
                    Color = chartDetail.Color,
                    Name = chartDetail.Name,
                    IsActive = chartDetail.IsActive,
                    Branches = chartDetail.BranchIds.Select(id => new BaseInfoDto
                    {
                        Id = id,
                        Name = _branchManager.QueryAllBranch().SingleOrDefault(b => b.Id == id)?.ShortName
                    }).ToList(),
                    JobPositions = chartDetail.JobPositionIds.Select(id => new BaseInfoDto
                    {
                        Id = id,
                        Name = _jobPositionManager.QueryAllJobPosition().SingleOrDefault(j => j.Id == id)?.ShortName
                    }).ToList(),
                    Levels = chartDetail.LevelIds.Select(id => new BaseInfoDto
                    {
                        Id = id,
                        Name = _levelManager.QueryAllLevel().SingleOrDefault(j => j.Id == id)?.ShortName
                    }).ToList(),
                    Teams = chartDetail.TeamIds.Select(id => new BaseInfoDto
                    {
                        Id = id,
                        Name = _teamManager.QueryAllTeam().SingleOrDefault(j => j.Id == id)?.Name
                    }).ToList(),
                    PayslipDetailTypes = CommonUtil.GetEnumKeyValueList(chartDetail.PayslipDetailTypes),
                    UserTypes= CommonUtil.GetEnumKeyValueList(chartDetail.UserTypes),
                    WorkingStatuses = CommonUtil.GetEnumKeyValueList(chartDetail.WorkingStatuses),
                    Sexes = CommonUtil.GetEnumKeyValueList(chartDetail.Sexes)
                };

                chartFullDetail.ChartDetails.Add(chartDetailContainBaseInfo);
            }

            return chartFullDetail;
        }

        public async Task<ChartDetailDto> Get(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
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

            chartDetail.Id = await WorkScope.InsertAndGetIdAsync<ChartDetail>(chartDetail);

            return chartDetail;
        }

        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)
        {

            var chartDetail = await WorkScope.GetAsync<ChartDetail>(updateChartDetailDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<ChartDetail>().Any(c => c.Name == updateChartDetailDto.Name && c.Id != updateChartDetailDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"ChartDetail name {updateChartDetailDto.Name} is already existed");
            }

            // update
            ObjectMapper.Map<UpdateChartDetailDto, ChartDetail>(updateChartDetailDto, chartDetail);

            await WorkScope.UpdateAsync(chartDetail);

            return chartDetail;
        }


        public async Task<ChartDetailDto> Active(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = true;

            await WorkScope.UpdateAsync<ChartDetail>(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<ChartDetailDto> DeActive(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = false;

            await WorkScope.UpdateAsync<ChartDetail>(chartDetail);

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
