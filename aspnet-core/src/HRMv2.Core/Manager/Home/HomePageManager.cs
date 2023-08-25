using HRMv2.Constants.Enum;
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
        public HomePageManager(IWorkScope workScope, WorkingHistoryManager workingHistoryManager) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
        }

        public List<HomepageEmployeeStatisticDto> GetAllEmployeeWorkingHistoryByTimeSpan(DateTime startDate, DateTime endDate)
        {
            List<LastEmployeeWorkingHistoryDto> empWorkingHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, endDate);

            var resultList = new List<HomepageEmployeeStatisticDto>();
            var cty = GetEmployeeStatisticDto(empWorkingHistories, null, "Toàn công ty", startDate);
            resultList.Add(cty);

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
        
    }
}