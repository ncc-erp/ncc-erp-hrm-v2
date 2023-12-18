using DocumentFormat.OpenXml.Wordprocessing;
using HRMv2.Constants.Enum;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.ChartDetails;
using HRMv2.Manager.Charts;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.WorkingHistories;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using NccCore.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home
{
    public class HomePageManager : BaseManager
    {
        protected readonly WorkingHistoryManager _workingHistoryManager;
        protected readonly ChartManager _chartManager;
        protected readonly ChartDetailManager _chartDetailManager;
        protected readonly JobPositionManager _jobPositionManager;
        protected readonly LevelManager _levelManager;
        protected readonly BranchManager _branchManager;
        protected readonly TeamManager _teamManager;

        public HomePageManager(
            IWorkScope workScope,
            WorkingHistoryManager workingHistoryManager,
            ChartManager chartManager,
            ChartDetailManager chartDetailManager,
            JobPositionManager jobPositionManager,
            LevelManager levelManager,
            BranchManager branchManager,
            TeamManager teamManager
            ) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
            _chartManager = chartManager;
            _chartDetailManager = chartDetailManager;
            _jobPositionManager = jobPositionManager;
            _levelManager = levelManager;
            _branchManager = branchManager;
            _teamManager = teamManager;
        }

        public List<HomepageEmployeeStatisticDto> GetAllEmployeeWorkingHistoryByTimeSpan(DateTime startDate, DateTime endDate)
        {
            List<LastEmployeeWorkingHistoryDto> empWorkingHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, endDate);

            var resultList = new List<HomepageEmployeeStatisticDto>();
            var wholeCompanyEmployees = GetEmployeeStatisticDto(empWorkingHistories, null, "Toàn công ty", startDate);
            resultList.Add(wholeCompanyEmployees);

            var branchList = empWorkingHistories.Select(s => new { s.BranchInfo.Name, s.BranchId }).Distinct().ToList();

            branchList.ForEach(branch =>
            {
                var statisticOfBranch = GetEmployeeStatisticDto(empWorkingHistories, branch.BranchId, branch.Name, startDate);

                resultList.Add(statisticOfBranch);
            });

            return resultList.OrderBy(i => i.BranchName != "Toàn công ty").ThenBy(x => x.BranchName).ToList();

        }

        public List<LastEmployeeWorkingHistoryDto> GetLastEmployeeWorkingHistories(DateTime startDate, DateTime endDate)
        {
            return _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, endDate);
        }

        private HomepageEmployeeStatisticDto GetEmployeeStatisticDto(List<LastEmployeeWorkingHistoryDto> empWorkingHistories, long? branchId, string branchName, DateTime startDate)
        {
            var histories = branchId.HasValue ? empWorkingHistories.Where(s => s.BranchId == branchId.Value) : empWorkingHistories;

            var qWorkingHistories = histories.Where(s => s.LastStatus == EmployeeStatus.Working);

            //var qOnboard = qWorkingHistories.Where(s => s.DateAt >= startDate.Date);
            //var qQuit = histories.Where(s => s.LastStatus == EmployeeStatus.Quit)
            //    .Where(s => s.DateAt >= startDate.Date);


            var item = new HomepageEmployeeStatisticDto()
            {
                OnboardEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Working && x.DateAt >= startDate.Date)).ToList(),
                
                QuitEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Quit && x.DateAt >= startDate.Date)).ToList(),

                PausingEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Pausing && x.DateAt >= startDate.Date)).ToList(),

                MatenityLeaveEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.MaternityLeave && x.DateAt >= startDate.Date)).ToList(),

                OnboardAndQuitEmployees = histories.Where(s => s.IsOnboardAndQuitInTimeSpan).ToList(),

                EmployeeTotal = qWorkingHistories.Count(),
                InternCount = qWorkingHistories.Where(s => s.UserType == UserType.Internship).Count(),
                StaffCount = qWorkingHistories.Where(s => s.UserType == UserType.Staff).Count(),
                CTVCount = qWorkingHistories.Where(s => s.UserType == UserType.Collaborators).Count(),
                TViecCount = qWorkingHistories.Where(s => s.UserType == UserType.ProbationaryStaff).Count(),
                VendorCount = qWorkingHistories.Where(s => s.UserType == UserType.Vendor).Count(),


                BranchName = branchName,
            };

            return item;
        }

        public List<dynamic> GetAllCharts(HomepageChartFilterDto filter)
        {
            List<LastEmployeeWorkingHistoryDto> empHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(filter.StartDate, filter.StartDate);

            var charts = _chartManager.GetAll();
            var chartDetails = _chartDetailManager.GetAll();

            var datas = charts
                .GroupJoin(
                chartDetails,
                c => c.Id,
                cd => cd.ChartId,
                (c, cd) => new
                {
                    c,
                    cd = cd.ToList()
                })
                .ToList();

            foreach ( var data in datas )
            {
                
                foreach ( var chartDetail in data.cd ) 
                { 
                    var job = _jobPositionManager.GetAll()
                        .Where(j => 
                        chartDetail.JobPositionIds.Any(jobId => jobId  == j.Id))
                        .ToList();

                    var level = _levelManager.GetAll()
                        .Where(l =>
                        chartDetail.LevelIds.Any(levelId => levelId == l.Id))
                        .ToList();

                    var branch = _branchManager.GetAll()
                        .Where(b =>
                        chartDetail.BranchIds.Any(branchId => branchId == b.Id))
                        .ToList();

                    var team = _teamManager.GetAll()
                        .Where(t =>
                        chartDetail.TeamIds.Any(teamId => teamId == t.Id))
                        .ToList();

                }
            }

            return null;
        }
        
    }
}