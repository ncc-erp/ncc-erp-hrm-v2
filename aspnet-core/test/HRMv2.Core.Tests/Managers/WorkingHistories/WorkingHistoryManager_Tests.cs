using Abp.ObjectMapping;
using HRMv2.Manager.Debts.PaidsManagger;
using HRMv2.Manager.WorkingHistories;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HRMv2.Core.Tests.Managers.WorkingHistories
{
    public class WorkingHistoryManager_Tests : HRMv2CoreTestBase
    {

        private readonly WorkingHistoryManager _workingHistoryManager;
        private readonly IWorkScope _workScope;
        public WorkingHistoryManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();
            _workingHistoryManager = new WorkingHistoryManager(_workScope);
            _workingHistoryManager.ObjectMapper = LocalIocManager.Resolve<IObjectMapper>();
        }

        [Fact]
        public void GetLastEmployeeWorkingHistories_Test1()
        {
            WithUnitOfWork(() =>
            {
                var lastDate = new DateTime(2022, 10, 1);
                var startDate = new DateTime(2022, 9, 1);
                var results = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, lastDate);
                Assert.Equal(12, results.Count);
            });
        }

        [Fact]
        public void GetLastEmployeeWorkingHistories_Test2()
        {
            var expectedHistoryValue = new LastEmployeeWorkingHistoryDto
            {
                EmployeeId = 905,
                BranchId = 100,
                //Avatar = s.Key.Avatar,
                Sex = Constants.Enum.HRMEnum.Sex.Male,
                Email = "thong.nguyenba@ncc.asia",
                FullName = "Nguyễn Bá Thông",
                UserType = Constants.Enum.HRMEnum.UserType.Collaborators,
                LastStatus = Constants.Enum.HRMEnum.EmployeeStatus.Working,
                DateAt = new DateTime(2022, 11, 01),
                BranchInfo = new() { Name = "Vinh", Color = "#ff9800" },
                JobPositionInfo = new() { Name = "Finance", Color = "#a91e1e" },
                LevelInfo = new() { Name = "Fresher", Color = "#318CE7" }
            };

            WithUnitOfWork(() =>
            {
                var lastDate = new DateTime(2022, 11, 01);
                var startDate = new DateTime(2022, 10, 1);
                var result = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, lastDate).Last();
                Assert.Equal(expectedHistoryValue.BranchId, result.BranchId);
                Assert.Equal(expectedHistoryValue.FullName, result.FullName);
                //Assert.Equal(expectedHistoryValue.LastStatus, result.LastStatus);
                Assert.Equal(expectedHistoryValue.DateAt, result.DateAt);
                Assert.Equal(expectedHistoryValue.BranchInfo.Color, result.BranchInfo.Color);
                Assert.Equal(expectedHistoryValue.BranchInfo.Name, result.BranchInfo.Name);
                Assert.Equal(expectedHistoryValue.JobPositionInfo.Color, result.JobPositionInfo.Color);
                Assert.Equal(expectedHistoryValue.JobPositionInfo.Name, result.JobPositionInfo.Name);
                Assert.Equal(expectedHistoryValue.LevelInfo.Color, result.LevelInfo.Color);
                Assert.Equal(expectedHistoryValue.LevelInfo.Name, result.LevelInfo.Name);
                Assert.Equal(expectedHistoryValue.EmployeeId, result.EmployeeId);
                Assert.Equal(expectedHistoryValue.Email, result.Email);
            });
        }

        [Fact]
        public void GetLastEmployeeWorkingHistories_Test3()
        {
            var expectedHistoryValue = new LastEmployeeWorkingHistoryDto
            {
                EmployeeId = 901,
                BranchId = 95,
                //Avatar = s.Key.Avatar,
                Sex = Constants.Enum.HRMEnum.Sex.Female,
                Email = "thi.nguyenleanh@ncc.asia",
                FullName = "Nguyễn Lê Anh Thi",
                UserType = Constants.Enum.HRMEnum.UserType.Collaborators,
                LastStatus = Constants.Enum.HRMEnum.EmployeeStatus.Working,
                DateAt = new DateTime(2022, 05, 01),
                BranchInfo = new() { Name = "HN2", Color = "#17a2b8" },
                JobPositionInfo = new() { Name = "PM", Color = "#a62626" },
                LevelInfo = new() { Name = "Junior-", Color = "#ad9fa1" }
            };

            WithUnitOfWork(() =>
            {
                var lastDate = new DateTime(2022, 05, 01);
                var startDate = lastDate.AddMonths(-1);
                var results = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, lastDate);
                var result = results.Last();
                Assert.Equal(expectedHistoryValue.BranchId, result.BranchId);
                Assert.Equal(expectedHistoryValue.FullName, result.FullName);
                Assert.Equal(expectedHistoryValue.LastStatus, result.LastStatus);
                Assert.Equal(expectedHistoryValue.DateAt, result.DateAt);
                Assert.Equal(expectedHistoryValue.BranchInfo.Color, result.BranchInfo.Color);
                Assert.Equal(expectedHistoryValue.BranchInfo.Name, result.BranchInfo.Name);
                Assert.Equal(expectedHistoryValue.JobPositionInfo.Color, result.JobPositionInfo.Color);
                Assert.Equal(expectedHistoryValue.JobPositionInfo.Name, result.JobPositionInfo.Name);
                Assert.Equal(expectedHistoryValue.LevelInfo.Color, result.LevelInfo.Color);
                Assert.Equal(expectedHistoryValue.LevelInfo.Name, result.LevelInfo.Name);
                Assert.Equal(expectedHistoryValue.EmployeeId, result.EmployeeId);
                Assert.Equal(expectedHistoryValue.Email, result.Email);
                Assert.Equal(3, results.Count);

            });


        }

        [Fact]
        public void GetLastEmployeeWorkingHistories_Test4()
        {
            var lastExpectedHistoryValue = new LastEmployeeWorkingHistoryDto
            {
                EmployeeId = 900,
                BranchId = 94,
                //Avatar = s.Key.Avatar,
                Sex = Constants.Enum.HRMEnum.Sex.Female,
                FullName = "Nguyễn Thị Quỳnh Hoa",
                Email = "hoa.nguyenthiquynh@ncc.asia",
                UserType = Constants.Enum.HRMEnum.UserType.Collaborators,
                LastStatus = Constants.Enum.HRMEnum.EmployeeStatus.Working,
                DateAt = new DateTime(2022, 02, 01),
            };
            var firstExpectedHistoryValue = new LastEmployeeWorkingHistoryDto
            {
                EmployeeId = 897,
                BranchId = 95,
                //Avatar = s.Key.Avatar,
                Sex = Constants.Enum.HRMEnum.Sex.Female,
                FullName = "Phạm Khánh Vy",
                Email = "vy.phamkhanh@ncc.asia",
                UserType = Constants.Enum.HRMEnum.UserType.Collaborators,
                LastStatus = Constants.Enum.HRMEnum.EmployeeStatus.Working,
                DateAt = new DateTime(2022, 04, 01),
            };

            WithUnitOfWork(() =>
            {
                var lastDate = new DateTime(2022, 04, 01);
                var startDate = lastDate.AddMonths(-1);
                var results = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, lastDate);
                var firstResult = results.First();
                var lastResult = results.Last();
                Assert.Equal(firstExpectedHistoryValue.FullName, firstResult.FullName);
                Assert.Equal(firstExpectedHistoryValue.LastStatus, firstResult.LastStatus);
                Assert.Equal(firstExpectedHistoryValue.DateAt, firstResult.DateAt);
                Assert.Equal(firstExpectedHistoryValue.EmployeeId, firstResult.EmployeeId);
                Assert.Equal(firstExpectedHistoryValue.Email, firstResult.Email);

                Assert.Equal(lastExpectedHistoryValue.FullName, lastResult.FullName);
                Assert.Equal(lastExpectedHistoryValue.LastStatus, lastResult.LastStatus);
                Assert.Equal(lastExpectedHistoryValue.DateAt, lastResult.DateAt);
                Assert.Equal(lastExpectedHistoryValue.EmployeeId, lastResult.EmployeeId);
                Assert.Equal(lastExpectedHistoryValue.Email, lastResult.Email);

                Assert.Equal(2, results.Count);

            });


        }
    }
}
