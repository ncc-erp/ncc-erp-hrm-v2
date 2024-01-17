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
        [InlineData(1, "2023-12-10", EmployeeStatus.Pausing)]
        [InlineData(2, "2023-12-10", EmployeeStatus.MaternityLeave)]
        [InlineData(3, "2023-12-10", EmployeeStatus.BackToWork)]
        [InlineData(4, "2023-12-10", EmployeeStatus.BackToWork)]
        [InlineData(5, "2023-12-10", EmployeeStatus.Onboard)]
        [InlineData(6, "2023-12-10", EmployeeStatus.Onboard)]
        [InlineData(7, "2023-12-10", EmployeeStatus.OnOffInMonth)]
        [InlineData(8, "2023-12-10", EmployeeStatus.OnOffInMonth)]
        [InlineData(9, "2023-12-10", EmployeeStatus.Quit)]
        [InlineData(10, "2023-12-10", EmployeeStatus.Quit)]
        [InlineData(11, "2023-12-10", EmployeeStatus.Quit)]
        public void GetMonthlyStatus_Test(int employeeId, DateTime month, EmployeeStatus expectedStatus)
        {

            {
                // Arrange
                var allEmployeeWorkingHistories = GetAllEmployeeWorkingHistories(); // Method to create list of EmployeeWorkingHistoryDetailDto
                var employee = new EmployeeDetailDto { EmployeeId = employeeId, Month = month };

                // Act
                var actualStatus = _homePage.GetMontlyStatus(employee, allEmployeeWorkingHistories);

                // Assert
                Assert.Equal(expectedStatus, actualStatus);
            }
        }

        private List<EmployeeWorkingHistoryDetailDto> GetAllEmployeeWorkingHistories()
        {
            return new List<EmployeeWorkingHistoryDetailDto>
            {
                //case 1: Work then Pause => Pause
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 1, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 1, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 12, 1) },
                //case 2: Work then Maternity Leave => Maternity Leave
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 2, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 2, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 12, 1) },
                //case 3: Maternity Leave then Working => BackToWork
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 3, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 3, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 4: Pause then Working => BackToWork
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 4, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 4, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 5: Quit then Working => Onboard
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 5, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 5, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 6: Only Working => Onboard
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 6, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 1) },
                //case 7: Onboard then Quit in Month => OnOffInMonth
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 7, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 8: Onboard then Quit then Onboard then Quit in Month => OnOffInMonth
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 8, 8) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 16) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 8, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 9: Maternity Leave then BackToWork then Quit in Month => Quit
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.MaternityLeave, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 9, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 10: Pause then BackToWork then Quit in Month => Quit
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Pausing, DateAt = new DateTime(2023, 8, 1) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Working, DateAt = new DateTime(2023, 12, 5) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 10, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 10) },
                //case 11: Work then Quit (not in Month) => Quit
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 11, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 8, 11) },
                new EmployeeWorkingHistoryDetailDto { EmployeeId = 11, Status = EmployeeStatus.Quit, DateAt = new DateTime(2023, 12, 12) },
             };
        }
    }
}
