using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
using NPOI.SS.Formula.Functions;

namespace HRMv2.Core.Tests.Managers.Employees
{
    public class EmployeeManagerTest : EmployeeManagerTestBase
    {
        private readonly EmployeeManager _employeeManager;
        private readonly int _numberOfEmployees = 24;
        private readonly int _theFirstEmployeeId = 882;
        private readonly int _numberOfEmployeeByBirthday = 2;
        private readonly int _numberOfEmployeeHasBirthdayInTheMonth = 5;
        private readonly DateTime _birthday1 = new DateTime(2000, 10, 1);
        private readonly DateTime _birthday2 = new DateTime(2000, 10, 1);
        private readonly string _theFirstEmployeeFullName = "Nguyễn Thanh Danh";
        private readonly string _theFirstEmployeeEmail = "danh.nguyenthanh@ncc.asia";
        private readonly string _theFirstEmployeeAvatar = "Avatar";
        private readonly string _theFirstEmployeeAddress = "Ninh Bình";
        private readonly string _theFirstEmployeeAccountNumber = "19007989789";
        private readonly string _theFirstEmployeePhone = "09119014801";
        private readonly string _theFirstEmployeeIdCard = "0632633198323";
        private readonly Sex _theFirstEmployeeSex = Sex.Male;
        private readonly UserType _theFirstEmployeeUserType = UserType.ProbationaryStaff;

        public EmployeeManagerTest()
        {
            _employeeManager = EmployeeManagerInstance();
        }

        // Check number of employees that get from function GetAll has similar with number of employees in temp db     
        [Fact]
        public void GetAll()
        {
            WithUnitOfWork(() =>
            {
                var results = _employeeManager.GetAll();

                Assert.NotNull(results);
                Assert.Equal(_numberOfEmployees, results.Count);

                results.ShouldContain(employee => employee.Id == _theFirstEmployeeId);
                results.ShouldContain(employee => employee.FullName == _theFirstEmployeeFullName);
                results.ShouldContain(employee => employee.Email == _theFirstEmployeeEmail);
                results.ShouldContain(employee => employee.Avatar == _theFirstEmployeeAvatar);
                results.ShouldContain(employee => employee.Sex == _theFirstEmployeeSex);
                results.ShouldContain(employee => employee.Phone == _theFirstEmployeePhone);
                results.ShouldContain(employee => employee.IdCard == _theFirstEmployeeIdCard);
                results.ShouldContain(employee => employee.Address == _theFirstEmployeeAddress);
                results.ShouldContain(employee => employee.BankAccountNumber == _theFirstEmployeeAccountNumber);
                results.ShouldContain(employee => employee.UserType == _theFirstEmployeeUserType);
            });
        }

        // Check number of employees that get from function GetAllEmployeeBasicInfo has similar with number of employees in temp db
        // Check basic info of employee
        [Fact]
        public void GetAllEmployeeBasicInfo()
        {
            WithUnitOfWork(() =>
            {
                var results = _employeeManager.GetAllEmployeeBasicInfo();

                Assert.Equal(_numberOfEmployees, results.Count);

                results.ShouldContain(employee => employee.Id == _theFirstEmployeeId);
                results.ShouldContain(employee => employee.FullName == _theFirstEmployeeFullName);
                results.ShouldContain(employee => employee.Email == _theFirstEmployeeEmail);
            });
        }

        [Fact]
        public void GetEmployeeBasicInfoForBreadcrumb()
        {
            WithUnitOfWork(() =>
            {
                var result = _employeeManager.GetEmployeeBasicInfoForBreadcrumb(_theFirstEmployeeId);

                Assert.Equal(_theFirstEmployeeId, result.Id);
                Assert.Equal(_theFirstEmployeeFullName, result.FullName);
                Assert.Equal(_theFirstEmployeeAvatar, result.Avatar);
            });
        }

        [Fact]
        public void GetEmployeesByBirthday()
        {
            WithUnitOfWork(() =>
            {
                var month = 10;
                var day = 1;
                var results = _employeeManager.GetEmployeesByBirthday(month, day);

                Assert.Equal(_numberOfEmployeeByBirthday, results.Count);
                results.ShouldContain(employee => employee.Birthday == _birthday1);
                results.ShouldContain(employee => employee.Birthday == _birthday2);
            });
        }

        [Fact]
        public void GetEmployeesBirthdayInMonth()
        {
            WithUnitOfWork(() =>
            {
                var month = 10;
                var results = _employeeManager.GetEmployeesBirthdayInMonth(month);

                Assert.Equal(_numberOfEmployeeHasBirthdayInTheMonth, results.Count);
                Assert.True(results.All(employee => employee.Birthday.Month == month));
            });
        }

        [Fact]
        public void GetEmployeeById()
        {
            WithUnitOfWork(() =>
            {
                var result = _employeeManager.Get(_theFirstEmployeeId);

                Assert.NotNull(result);
                Assert.Equal(_theFirstEmployeeId, result.EmployeeInfo.Id);
                Assert.Equal(_theFirstEmployeeFullName, result.EmployeeInfo.FullName);
                Assert.Equal(_theFirstEmployeeEmail, result.EmployeeInfo.Email);
                Assert.Equal(_theFirstEmployeeAddress, result.EmployeeInfo.Address);
                Assert.Equal(_theFirstEmployeeAvatar, result.EmployeeInfo.Avatar);
                Assert.Equal(_theFirstEmployeeSex, result.EmployeeInfo.Sex);
                Assert.Equal(_theFirstEmployeePhone, result.EmployeeInfo.Phone);
                Assert.Equal(_theFirstEmployeeIdCard, result.EmployeeInfo.IdCard);
                Assert.Equal(_theFirstEmployeeAccountNumber, result.EmployeeInfo.BankAccountNumber);
                Assert.Equal(_theFirstEmployeeUserType, result.EmployeeInfo.UserType);

                Assert.False(result.IsAllowEdit);
                Assert.True(result.IsAllowEditBranch);
                Assert.False(result.IsAllowEditWorkingStatus);
            });
        }

        [Fact]
        public void GetEmployeeExpect()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.GridParam = new GridParam
            {
                MaxResultCount = 10,
                SkipCount = 0,
            };
            var workScope = Resolve<IWorkScope>();

            // Case with gridParam
            WithUnitOfWork(async () =>
            {
                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count <= MaxResultCount in gridParam
                result.Items.Count.ShouldBeLessThanOrEqualTo(10);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByAdditonId()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.AddedEmployeeIds = new List<long> { 880, 881, 882 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => !_employeeAdditionDto.AddedEmployeeIds.Contains(employee.Id)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByStatus()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.StatusIds = new List<EmployeeStatus> { EmployeeStatus.Working, EmployeeStatus.Quit };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.StatusIds.Contains(employee.Status)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByBranch()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.BranchIds = new List<long> { 94, 95, 96 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.BranchIds.Contains(employee.BranchId)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByUserType()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.Usertypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.Usertypes.Contains(employee.UserType)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByLevel()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.LevelIds = new List<long> { 315, 316, 317 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.LevelIds.Contains(employee.LevelId)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByJobPosition()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.JobPositionIds = new List<long> { 47, 48, 49 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.JobPositionIds.Contains(employee.JobPositionId)).ToList();

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetEmployeeExpectFilterByBirthday()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.BirthdayToDate = new DateTime(1999, 10, 30);
            _employeeAdditionDto.BirthdayFromDate = new DateTime(1992, 10, 1);
            var fromDate = _employeeAdditionDto.BirthdayFromDate.Value;
            var toDate = _employeeAdditionDto.BirthdayToDate.Value;
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(x => x.Birthday.Value.Month > fromDate.Month
                                            || x.Birthday.Value.Month == fromDate.Month && x.Birthday.Value.Day >= fromDate.Day)
                                        .Where(x => x.Birthday.Value.Month < toDate.Month
                                            || x.Birthday.Value.Month == toDate.Month && x.Birthday.Value.Day <= toDate.Day);

                GridResult<GetEmployeeDto> result = await _employeeManager.GetEmployeeExcept(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count(), result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExport()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.GridParam.MaxResultCount = 10;

            WithUnitOfWork(() =>
            {
                var result = _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                Assert.NotNull(result);

                // Check result.Items.Count <= MaxResultCount in gridParam
                result.Result.Items.Count.ShouldBeLessThanOrEqualTo(10);
            });
        }
        [Fact]
        public void GetAllEmployeeForExportFilterByAdditonId()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.AddedEmployeeIds = new List<long> { 880, 881, 882 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => !_employeeAdditionDto.AddedEmployeeIds.Contains(employee.Id)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByStatus()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.StatusIds = new List<EmployeeStatus> { EmployeeStatus.Working, EmployeeStatus.Quit };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.StatusIds.Contains(employee.Status)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByBranch()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.BranchIds = new List<long> { 94, 95, 96 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.BranchIds.Contains(employee.BranchId)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByUserType()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.Usertypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.Usertypes.Contains(employee.UserType)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByLevel()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.LevelIds = new List<long> { 315, 316, 317 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.LevelIds.Contains(employee.LevelId)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByJobpositon()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.JobPositionIds = new List<long> { 47, 48, 49 };
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(employee => _employeeAdditionDto.JobPositionIds.Contains(employee.JobPositionId)).ToList();

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count, result.Items.Count);
            });
        }

        [Fact]
        public void GetAllEmployeeForExportFilterByBirthday()
        {
            var _employeeAdditionDto = EmployeeToAddDto();
            _employeeAdditionDto.BirthdayToDate = new DateTime(1999, 10, 30);
            _employeeAdditionDto.BirthdayFromDate = new DateTime(1992, 10, 1);
            var fromDate = _employeeAdditionDto.BirthdayFromDate.Value;
            var toDate = _employeeAdditionDto.BirthdayToDate.Value;
            var workScope = Resolve<IWorkScope>();

            WithUnitOfWork(async () =>
            {
                var expectedResult = workScope.GetAll<Employee>()
                                        .Where(x => x.Birthday.Value.Month > fromDate.Month
                                            || x.Birthday.Value.Month == fromDate.Month && x.Birthday.Value.Day >= fromDate.Day)
                                        .Where(x => x.Birthday.Value.Month < toDate.Month
                                            || x.Birthday.Value.Month == toDate.Month && x.Birthday.Value.Day <= toDate.Day);

                var result = await _employeeManager.GetAllEmployeeForExport(_employeeAdditionDto);

                // Check result is not null
                Assert.NotNull(result);

                // Check result.Items.Count = expectedResult.Count
                Assert.Equal(expectedResult.Count(), result.Items.Count);
            });
        }


        [Fact]
        public async Task CreateEmployee()
        {
            // Email is already
            var expectedMessage = "Email is Already Exist";
            var _employeeCreationDto = EmployeeCreationDto();
            _employeeCreationDto.Email = "danh.nguyenthanh@ncc.asia";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _employeeManager.CreateEmployee(_employeeCreationDto, null, true);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async void AddToBenefits()
        {
            var mockWorkScope = Resolve<IWorkScope>();
            var employeeId = 905;
            var startDate = new DateTime(2022, 1, 1);
            var listBenifitsBeforeAdd = new List<BenefitEmployee>();

            WithUnitOfWork(() =>
            {
                listBenifitsBeforeAdd = mockWorkScope.GetAll<BenefitEmployee>().ToList();
                _employeeManager.AddToBenefits(employeeId, startDate);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listBenifitsAfterAdd = mockWorkScope.GetAll<BenefitEmployee>();

                listBenifitsAfterAdd.Count().ShouldBeGreaterThan(listBenifitsBeforeAdd.Count);
                Assert.Equal(employeeId, listBenifitsAfterAdd.Last().EmployeeId);
            });
        }

        [Fact]
        public void Update()
        {
            var employeeUpdatingDto = EmployeeUpdatingDto();

            WithUnitOfWork(() =>
            {
                var result = _employeeManager.Update(employeeUpdatingDto);

                Assert.Equal(employeeUpdatingDto, result.Result);
            });
        }

        [Fact]
        public async void CanNotUpdateAlreadyEmail()
        {
            var expectedMessage = "Email is Already Exist";
            var employeeUpdatingDto = EmployeeUpdatingDto();
            employeeUpdatingDto.Email = "bao.tranngoc@ncc.asia";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _employeeManager.Update(employeeUpdatingDto);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async void InitWorkingHistory()
        {
            var workScope = Resolve<IWorkScope>();
            var _employeeAdditionDto = EmployeeCreationDto();
            _employeeAdditionDto.Id = 906;

            WithUnitOfWork(() =>
            {
                _employeeManager.InitWorkingHistory(_employeeAdditionDto);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listWorkingHistories = workScope.GetAll<EmployeeWorkingHistory>();

                Assert.Equal(_employeeAdditionDto.Id, listWorkingHistories.Last().EmployeeId);
            });
        }

        [Fact]
        public async void InitBranchHistory()
        {
            var workScope = Resolve<IWorkScope>();
            var _employeeAdditionDto = EmployeeCreationDto();
            _employeeAdditionDto.Id = 906;

            WithUnitOfWork(() =>
            {
                _employeeManager.InitBranchHistory(_employeeAdditionDto);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listEmployeeBranchHistories = workScope.GetAll<EmployeeBranchHistory>();

                Assert.Equal(_employeeAdditionDto.Id, listEmployeeBranchHistories.Last().EmployeeId);
            });
        }

        [Fact]
        public async void UpdateEmployeeSkill()
        {
            var workScope = Resolve<IWorkScope>();
            var employeeId = 905;
            var newSkillIds = new List<long> { 64, 65, 66 };

            WithUnitOfWork(() =>
            {
                var result = _employeeManager.UpdateEmployeeSkill(employeeId, newSkillIds);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listEmployeeSkills = workScope.GetAll<EmployeeSkill>()
                    .Where(employeeSkill => employeeSkill.EmployeeId == employeeId);

                Assert.True(listEmployeeSkills.Any(employeeSkill => newSkillIds.Contains(employeeSkill.SkillId)));
            });
        }

        [Fact]
        public async void UpdateEmployeeTeam()
        {
            var workScope = Resolve<IWorkScope>();
            var employeeId = 905;
            var newTeamds = new List<long> { 64, 65, 66 };

            WithUnitOfWork(() =>
            {
                var result = _employeeManager.UpdateEmployeeTeam(employeeId, newTeamds);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listEmployeeTeams = workScope.GetAll<EmployeeTeam>()
                    .Where(employeeTeam => employeeTeam.EmployeeId == employeeId);

                Assert.True(listEmployeeTeams.Any(employeeTeam => newTeamds.Contains(employeeTeam.TeamId)));
            });
        }

        [Fact]
        public async void CanNotDeleteEmployeeThatHasPaySlip()
        {
            var expectedMessage = "Employee has payslip >= 10/2022 => CAN NOT delete employee";
            var employeeId = 905;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _employeeManager.Delete(employeeId);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async void Delete()
        {
            var workScope = Resolve<IWorkScope>();
            var employeeId = 894;
            var listEmployeesBeforeDelete = new List<Employee>();

            await WithUnitOfWorkAsync(async () =>
            {
                listEmployeesBeforeDelete = workScope.GetAll<Employee>().ToList();

                var result = _employeeManager.Delete(employeeId);

                Assert.Equal(employeeId, result.Result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var listEmployeesAfterDelete = workScope.GetAll<Employee>();
                listEmployeesAfterDelete.Count().ShouldBeLessThan(listEmployeesBeforeDelete.Count);
            });
        }

        [Fact]
        public void ChangeEmployeeBranch()
        {
            var mockWorkScope = Resolve<IWorkScope>();
            var branchChangingDto = BranchChangingDto();
            var employeeBeforeUpdateBranch = new List<Employee>();

            WithUnitOfWork(() =>
            {
                employeeBeforeUpdateBranch = mockWorkScope.GetAll<Employee>()
                        .Where(employee => employee.Id == branchChangingDto.EmployeeId).ToList();
                _employeeManager.ChangeEmployeeBranch(branchChangingDto);
            });

            WithUnitOfWork(() =>
            {
                var employeeAfterUpdateBranch = mockWorkScope.GetAll<Employee>()
                    .Where(employee => employee.Id == branchChangingDto.EmployeeId);

                employeeAfterUpdateBranch.FirstOrDefault().BranchId.ShouldNotBeSameAs(employeeBeforeUpdateBranch.FirstOrDefault().BranchId);
                Assert.Equal(branchChangingDto.BranchId, employeeAfterUpdateBranch.FirstOrDefault().BranchId);
            });
        }

        [Fact]
        public async void CanNotChangeEmployeeBranchWithInvalidEmployeeId()
        {
            var branchChangingDto = BranchChangingDto();
            branchChangingDto.EmployeeId = 12;
            var expectedMessage = $"Can not found employee with Id={branchChangingDto.EmployeeId}";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _employeeManager.ChangeEmployeeBranch(branchChangingDto);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async void CanNotReCreateEmployeeToOtherToolWithInvalidEmployeeId()
        {
            var employeeId = 12;
            var expectedMessage = $"There is no employee with id: {employeeId}";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _employeeManager.ReCreateEmployeeToOtherTool(employeeId);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }
    }
}