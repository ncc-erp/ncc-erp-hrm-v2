using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos.ChartDto;
using HRMv2.Manager.WorkingHistories;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace HRMv2.Core.Tests.Managers.Home
{
    public class HomePageManager_tests : HRMv2CoreTestBase
    {
        private readonly HomePageManager _homePage;
        private readonly IWorkScope _workScope;

        public HomePageManager_tests()
        {
            var mockWorkingHistoryManager = new Mock<WorkingHistoryManager>(_workScope);
            var mockchartManager = new Mock<ChartManager>(_workScope);
            var mockchartDetailManager = new Mock<ChartDetailManager>(_workScope);
            _homePage = new HomePageManager(_workScope, mockWorkingHistoryManager.Object, mockchartManager.Object, mockchartDetailManager.Object);
            _homePage = Resolve<HomePageManager>();
        }

        [Fact]
        public async Task GetAllEmployeeWorkingHistoryByTimeSpan_Should_Get_For_2022()
        {
            var expectCount = 9;

            WithUnitOfWork(() =>
            {
                var result = _homePage.GetAllEmployeeWorkingHistoryByTimeSpan(new DateTime(2022, 01, 01), new DateTime(2022, 12, 31));
                Assert.Equal(expectCount, result.Count);
                result.ForEach(x =>
                {
                    if (x.BranchName == "Toàn công ty")
                    {
                        Assert.Equal(24, x.OnboardTotal);
                        Assert.Equal(1, x.QuitJobTotal);
                        Assert.Equal(3, x.PausingCount);
                        Assert.Equal(4, x.MaternityLeaveCount);
                    }
                    else if (x.BranchName == "ĐN" || x.BranchName == "Vinh")
                        Assert.Equal(2, x.OnboardTotal);
                    else if (x.BranchName == "HN1")
                        Assert.Equal(4, x.OnboardTotal);
                    else if (x.BranchName == "HN2")
                    {
                        Assert.Equal(12, x.OnboardTotal);
                        Assert.Equal(1, x.QuitJobTotal);
                        Assert.Equal(3, x.PausingCount);    
                        Assert.Equal(3, x.MaternityLeaveCount);    
                    }
                    else if (x.BranchName == "HN3" || x.BranchName == "QN" || x.BranchName == "SG1" || x.BranchName == "SG2")
                        Assert.Equal(1, x.OnboardTotal);
                });
            });
        }

        [Fact]
        public async Task GetAllEmployeeWorkingHistoryByTimeSpan_Should_Get_For_December_2022()
        {
            var expectCount = 9;

            WithUnitOfWork(() =>
            {
                var result = _homePage.GetAllEmployeeWorkingHistoryByTimeSpan(new DateTime(2022, 12, 01), new DateTime(2022, 12, 31));
                Assert.Equal(expectCount, result.Count);
                result.ForEach(x =>
                {
                    if (x.BranchName == "Toàn công ty")
                    {
                        Assert.Equal(4, x.OnboardTotal);
                        Assert.Equal(1, x.QuitJobTotal);
                        Assert.Equal(2, x.PausingCount);
                        Assert.Equal(1, x.MaternityLeaveCount);
                    }
                    else if (x.BranchName == "ĐN" || x.BranchName == "Vinh" || x.BranchName == "HN3" || x.BranchName == "QN" || x.BranchName == "SG1" || x.BranchName == "SG2")
                        Assert.Equal(0, x.OnboardTotal);
                    else if (x.BranchName == "HN1")
                        Assert.Equal(1, x.OnboardTotal);
                    else if (x.BranchName == "HN2")
                    {
                        Assert.Equal(3, x.OnboardTotal);
                        Assert.Equal(1, x.QuitJobTotal);
                        Assert.Equal(2, x.PausingCount);
                        Assert.Equal(1, x.MaternityLeaveCount);
                    }
                });
            });
        }

        [Fact]
        public async Task GetAllEmployeeWorkingHistoryByTimeSpan_Should_Get_None_For_2021()
        {
            var expectCount = 1;

            WithUnitOfWork(() =>
            {
                var result = _homePage.GetAllEmployeeWorkingHistoryByTimeSpan(new DateTime(2021, 01, 01), new DateTime(2021, 12, 31));
                Assert.Equal(expectCount, result.Count);
                result.ForEach(x =>
                {
                    if (x.BranchName == "Toàn công ty")
                    {
                        Assert.Equal(0, x.EmployeeTotal);
                    }
                });
            });
        }

        //[Fact]
        //public void GetMonthlyStatus_Test()
        //{
        //    var allEmloyeeWorkingHistories = new List<EmployeeWorkingHistoryDetailDto>
        //    {
        //        //case 1: Work then Pause => Pause
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 1, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 1, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 12, 1) },
        //        //case 2: Work then Maternity Leave => Maternity Leave
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 2, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 2, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 12, 1) },
        //        //case 3: Maternity Leave then Working => BackToWork
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 3, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 12, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 3, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
        //        //case 4: Pause then Working => BackToWork
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 4, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 12, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 4, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
        //        //case 5: Quit then Working => Onboard
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 5, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 5, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
        //        //case 6: Only Working => Onboard
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 6, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
        //        //case 7: Onboard then Quit in Month => OnOffInMonth
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
        //        //case 8: Onboard then Quit then Onboard then Quit in Month => OnOffInMonth
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
        //        //case 9: Maternity Leave then BackToWork then Quit in Month => Quit
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
        //        //case 10: Pause then BackToWork then Quit in Month => Quit
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 8, 1) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
        //        //case 11: Work then Quit (not in Month) => Quit
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 11, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 11) },
        //        new EmployeeWorkingHistoryDetailDto { EmployeeId = 11, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 12) },
        //     };
        //    var employees = new List<EmployeeDetailDto>
        //    { 
        //        new EmployeeDetailDto { EmployeeId = 1, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 2, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 3, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 4, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 5, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 6, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 7, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 8, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 9, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 10, Month = new DateTime(2023,12,10)},
        //        new EmployeeDetailDto { EmployeeId = 11, Month = new DateTime(2023,12,10)},
        //    };
        //    //Act
        //    foreach (var employee in employees)
        //    {
        //        employee.Status = _homePage.GetMontlyStatus(employee, allEmloyeeWorkingHistories);
        //    }
        //    Assert.Equal(EmployeeStatus.Pausing, employees[0].Status);
        //    Assert.Equal(EmployeeStatus.MaternityLeave, employees[1].Status);
        //    Assert.Equal(EmployeeStatus.BackToWork, employees[2].Status);
        //    Assert.Equal(EmployeeStatus.BackToWork, employees[3].Status);
        //    Assert.Equal(EmployeeStatus.Onboard, employees[4].Status);
        //    Assert.Equal(EmployeeStatus.Onboard, employees[5].Status);
        //    Assert.Equal(EmployeeStatus.OnOffInMonth, employees[6].Status);
        //    Assert.Equal(EmployeeStatus.OnOffInMonth, employees[7].Status);
        //    Assert.Equal(EmployeeStatus.Quit, employees[8].Status);
        //    Assert.Equal(EmployeeStatus.Quit, employees[9].Status);
        //    Assert.Equal(EmployeeStatus.Quit, employees[10].Status);
        //}
    }
}
