using HRMv2.Manager.Home;
using HRMv2.Manager.WorkingHistories;
using HRMv2.NccCore;
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
    }
}
