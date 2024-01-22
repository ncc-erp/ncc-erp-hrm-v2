using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Histories.Dto;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos.ChartDto;
using HRMv2.Manager.WorkingHistories;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Home
{
    public class HomePageManager_tests : HRMv2CoreTestBase
    {
        private readonly HomePageManager _homePage;
        private readonly IWorkScope _workScope;

        public HomePageManager_tests()
        {
            var mockWorkingHistoryManager = new Mock<WorkingHistoryManager>(_workScope);
            _homePage = new HomePageManager(_workScope, mockWorkingHistoryManager.Object);
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

        [Theory]
        [InlineData(1, "2023-12-10", EmployeeMonthlyStatus.Pausing)]
        [InlineData(2, "2023-12-10", EmployeeMonthlyStatus.MaternityLeave)]
        [InlineData(3, "2023-12-10", EmployeeMonthlyStatus.BackToWork)]
        [InlineData(4, "2023-12-10", EmployeeMonthlyStatus.BackToWork)]
        [InlineData(5, "2023-12-10", EmployeeMonthlyStatus.Onboard)]
        [InlineData(6, "2023-12-10", EmployeeMonthlyStatus.Onboard)]
        [InlineData(7, "2023-12-10", EmployeeMonthlyStatus.OnOffInMonth)]
        [InlineData(8, "2023-12-10", EmployeeMonthlyStatus.OnOffInMonth)]
        [InlineData(9, "2023-12-10", EmployeeMonthlyStatus.Quit)]
        [InlineData(10, "2023-12-10", EmployeeMonthlyStatus.Quit)]
        [InlineData(11, "2023-12-10", EmployeeMonthlyStatus.Quit)]
        [InlineData(12, "2023-12-10", EmployeeMonthlyStatus.Working)]
        [InlineData(13, "2023-12-10", EmployeeMonthlyStatus.MaternityLeave)]
        public void GetMonthlyStatus_Test(int employeeId, DateTime month, EmployeeMonthlyStatus expectedStatus)
        {

            {
                // Arrange
                var workingHistories = GetAllEmployeeWorkingHistories()
                                    .GroupBy(s => s.EmployeeId)
                                    .ToDictionary(
                                        s => s.Key, 
                                        s => s.OrderByDescending(x => x.DateAt).ToList()
                                    );// Method to create list of WorkingHistoryDto
                var employee = new PayslipChartDto { EmployeeId = employeeId, DateAt = month };

                // Act
                _homePage.UpdateMonthlyStatus(employee, workingHistories[employeeId]);

                // Assert
                Assert.Equal(expectedStatus, employee.MonthlyStatus);
            }
        }

        private List<WorkingHistoryDto> GetAllEmployeeWorkingHistories()
        {
            return new List<WorkingHistoryDto>
            {
                //case 1: Work then Pause => Pause
                new WorkingHistoryDto { EmployeeId = 1, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 1, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 12, 1) },
                //case 2: Work then Maternity Leave => Maternity Leave

                new WorkingHistoryDto { EmployeeId = 2, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 2, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 12, 1) },
                //case 3: Maternity Leave then Working => BackToWork
                new WorkingHistoryDto { EmployeeId = 3, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 3, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 3, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 4: Pause then Working => BackToWork
                new WorkingHistoryDto { EmployeeId = 4, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 4, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 4, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 5: Quit then Working => Onboard
                new WorkingHistoryDto { EmployeeId = 5, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 5, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 5, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 6: Only Working => Onboard
                new WorkingHistoryDto { EmployeeId = 6, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 7: Onboard then Quit in Month => OnOffInMonth
                new WorkingHistoryDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
                new WorkingHistoryDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
                new WorkingHistoryDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new WorkingHistoryDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 8: Onboard then Quit then Onboard then Quit in Month => OnOffInMonth
                new WorkingHistoryDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
                new WorkingHistoryDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
                new WorkingHistoryDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new WorkingHistoryDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 9: Maternity Leave then BackToWork then Quit in Month => Quit
                new WorkingHistoryDto { EmployeeId = 9, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 9, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 9, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new WorkingHistoryDto { EmployeeId = 9, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 10: Pause then BackToWork then Quit in Month => Quit
                new WorkingHistoryDto { EmployeeId = 10, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 10, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 8, 1) },
                new WorkingHistoryDto { EmployeeId = 10, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new WorkingHistoryDto { EmployeeId = 10, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 11: Work then Quit (not in Month) => Quit
                new WorkingHistoryDto { EmployeeId = 11, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 11) },
                new WorkingHistoryDto { EmployeeId = 11, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 12) },
                //case 12: Working
                new WorkingHistoryDto { EmployeeId = 12, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
                //case 13: Maternity Leave
                new WorkingHistoryDto { EmployeeId = 13, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 6, 1) },
                new WorkingHistoryDto { EmployeeId = 13, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
             };
        }
    }
}
