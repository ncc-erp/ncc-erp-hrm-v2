
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Categories.Charts.DisplayChartDto;
using HRMv2.Manager.Categories.Charts.Dto;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Common.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Uitls;
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
                PayslipDetailTypes = CommonUtil.GetEnumKeyValueList<PayslipDetailType>()
                                    .Select(s => new KeyValueDto
                                    {
                                        Key = CommonUtil.FormatNameByAddingSpace(s.Key),
                                        Value = s.Value
                                    })
                                    .ToList(),
                UserTypes = CommonUtil.GetEnumKeyValueList<UserType>()
                                    .Select(s => new KeyValueDto
                                    {
                                        Key = CommonUtil.FormatNameByAddingSpace(s.Key),
                                        Value = s.Value
                                    })
                                    .ToList(),
                WorkingStatuses = CommonUtil.GetEnumKeyValueList<EmployeeMonthlyStatus>()
                                    .Select(s => new KeyValueDto
                                    {
                                        Key = CommonUtil.FormatNameByAddingSpace(s.Key),
                                        Value = s.Value
                                    })
                                    .ToList(),
                Gender = CommonUtil.GetEnumKeyValueList<Sex>(),

            };

            return selectionData;
        }

        public BadgeInfoChartDetail GetBadgeInfoChartDetail()
        {
            var branches = _branchManager
                .QueryAllBranch()
                .Select(x => new BadgeInfoDto(x.Id, x.Name, x.Color)).ToList();

            var levels = _levelManager
                .QueryAllLevel()
                .Select(x => new BadgeInfoDto(x.Id, x.Name, x.Color)).ToList();

            var jobPositions = _jobPositionManager
                .QueryAllJobPosition()
                .Select(x => new BadgeInfoDto(x.Id, x.Name, x.Color)).ToList();

            var teams = _teamManager
                .QueryAllTeam()
                .Select(x => new EmployeeTeamDto { TeamId = x.Id, TeamName = x.Name }).ToList();
            var result = new BadgeInfoChartDetail
            {
                BranchInfo = branches,
                JobPositionInfo = jobPositions,
                LevelInfo = levels,
                TeamInfos = teams
            };
            return result;
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
                Branches = chartDetail.ListBranchId.Select(id => new KeyValueDto
                {
                    Key = selectionData.Branches.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                JobPositions = chartDetail.ListJobPositionId.Select(id => new KeyValueDto
                {
                    Key = selectionData.JobPositions.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                Levels = chartDetail.ListLevelId.Select(id => new KeyValueDto
                {
                    Key = selectionData.Levels.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                Teams = chartDetail.ListTeamId.Select(id => new KeyValueDto
                {
                    Key = selectionData.Teams.SingleOrDefault(s => s.Value == id).Key,
                    Value = id
                }).ToList(),
                PayslipDetailTypes = CommonUtil.GetEnumKeyValueList(chartDetail.ListPayslipDetailType),
                UserTypes = CommonUtil.GetEnumKeyValueList(chartDetail.ListUserType),
                WorkingStatuses = CommonUtil.GetEnumKeyValueList(chartDetail.ListWorkingStatus),
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

        public async Task<List<EmployeeDataFromChartDetailDto>> GetDetailDataChart(
            long chartDetailId, 
            ChartDataType chartDataType,
            DateTime startDate, 
            DateTime endDate)
        {
            startDate = DateTimeUtils.FirstDayOfMonth(startDate);
            endDate = DateTimeUtils.LastDayOfMonth(endDate);
            var chartDetailInfo = await WorkScope.GetAll<ChartDetail>()
                                .Where(s => s.Id == chartDetailId)
                                .Select(s => new ChartDetailDto
                                {
                                    Id = s.Id,
                                    ChartId = s.ChartId,
                                    Name = s.Name,
                                    Color = s.Color,
                                    JobPositionIds = s.JobPositionIds,
                                    LevelIds = s.LevelIds,
                                    BranchIds = s.BranchIds,
                                    TeamIds = s.TeamIds,
                                    PayslipDetailTypes = s.PayslipDetailTypes,
                                    UserTypes = s.UserTypes,
                                    WorkingStatuses = s.WorkingStatuses,
                                    Gender = s.Gender,
                                }).FirstOrDefaultAsync();
            var listPayslipDataChart = new List<PayslipDataChartDto>();
            
            if (chartDataType == ChartDataType.Employee)
            {
                listPayslipDataChart = FilterDataEmployeeChart(chartDetailInfo, startDate, endDate);
            }
            else if (chartDataType == ChartDataType.Salary)
            {
                listPayslipDataChart = FilterDataPayslipChart(chartDetailInfo, startDate, endDate);
            }

            var allBadgeInfoChartDetail = GetBadgeInfoChartDetail();

            foreach (var payslip in listPayslipDataChart)
            {
                payslip.BranchInfo = allBadgeInfoChartDetail.BranchInfo.FirstOrDefault(x => x.Id == payslip.BranchId);
                payslip.JobPositionInfo = allBadgeInfoChartDetail.JobPositionInfo.FirstOrDefault(x => x.Id == payslip.JobPositionId);
                payslip.LevelInfo = allBadgeInfoChartDetail.LevelInfo.FirstOrDefault(x => x.Id == payslip.LevelId);
                payslip.TeamInfos = payslip.TeamIds
                    .Select(teamId => allBadgeInfoChartDetail.TeamInfos.FirstOrDefault(x => x.TeamId == teamId))
                    .ToList();
            }

            var result = listPayslipDataChart
                .GroupBy(p => p.EmployeeId)
                .Select(group => new EmployeeDataFromChartDetailDto
                {
                    EmployeeId = group.Key,
                    FullName = group.First().FullName,
                    Email = group.First().Email,
                    Avatar = group.First().Avatar,
                    Gender = group.First().Gender,
                    MonthlyEmployeeDetails = group.Select(g => new MonthlyEmployeeDetailDto
                    {
                        BranchInfo = g.BranchInfo,
                        JobPositionInfo = g.JobPositionInfo,
                        LevelInfo = g.LevelInfo,
                        TeamInfos = g.TeamInfos,
                        UserTypeInfo = g.UserTypeInfo,
                        MonthlyStatus = g.MonthlyStatus,
                        StatusMonth = g.StatusMonth,
                        Money = g.Money,
                        PayrollMonth = g.PayrollMonth
                    }).ToList()
                }).ToList();

            return result;
        }


        public List<PayslipDataChartDto> FilterDataEmployeeChart(
            ChartDetailDto detail,
            DateTime startDate,
            DateTime endDate)
        {
            var allDataForChartEmployee = _chartManager.GetDataForAllChartEmployee(startDate, endDate);

            var result = _chartManager.FilterDataEmployeeChartByChartDetail(allDataForChartEmployee, detail)
                        .Where(x => x.StatusMonth >= startDate && x.StatusMonth <= endDate)
                        .OrderBy(x => x.StatusMonth)
                        .ToList();

            return result;
        }


        public List<PayslipDataChartDto> FilterDataPayslipChart(
            ChartDetailDto detail,
            DateTime startDate,
            DateTime endDate)
        {
            var payslip = _chartManager.QueryAllPayslipDetail(startDate, endDate).ToList();

            var employeePayslips = _chartManager.FitlerDataPayslipChartByChartDetail(detail, payslip)
                .WhereIf(detail.ListPayslipDetailType.Any(), p => detail.ListPayslipDetailType.Any(payslipDetail => p.PayslipDetails.Any(s => s.Type == payslipDetail)))
                .Select(p => new PayslipDataChartDto
                {
                    FullName = p.FullName,
                    Id = p.Id,
                    Email = p.Email,
                    Avatar = p.Avatar,
                    Gender = p.Gender,
                    Salary = p.Salary,
                    BranchId = p.BranchId,
                    EmployeeId = p.EmployeeId,
                    JobPositionId = p.JobPositionId,
                    LevelId = p.LevelId,
                    TeamIds = p.TeamIds,
                    UserType = p.UserType,
                    PayrollMonth = p.PayrollMonth,
                    PayslipDetails = p.PayslipDetails
                                    .Select(pd => new PayslipDetailDataChartDto
                                    {
                                        Id = pd.Id,
                                        Money = pd.Money,
                                        Type = pd.Type,
                                    }).ToList(),
                    Money = detail.ListPayslipDetailType.Any()
                    ? p.PayslipDetails.Where(d => detail.ListPayslipDetailType.Contains(d.Type)).Sum(d => Math.Abs(d.Money))
                    : p.Salary
                })
                .Where(p => p.Money != 0)
                .ToList();

            return employeePayslips;
        }
    }
}
