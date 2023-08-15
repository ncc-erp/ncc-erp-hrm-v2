using Abp.Domain.Repositories;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using NccCore.Uitls;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.ChangeEmployeeWorkingStatuses
{
    public class ChangeEmpoyeeWorkingStatusManagerTest : ChangeEmpoyeeWorkingStatusManagerTestBase
    {
        private readonly ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatusManager;
        public ChangeEmpoyeeWorkingStatusManagerTest()
        {
            _changeEmployeeWorkingStatusManager = ChangeEmployeeWokingStatusManagerInstance();
        }

        [Fact]
        public void ChangeStatusToQuit()
        {
            var toQuitDto = ToQuitDto();
            var benifitEmployeeDto = BenefitsOfEmployeeDto();
            var employeeRepository = Resolve<IRepository<Employee, long>>();
            var employeeWorkingHistoryRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var benifitEmployeeRepository = Resolve<IRepository<BenefitEmployee, long>>();
            var salaryChangeRequestEmployeeRepository = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();

            var listEmployeeWorkingHistorBeforeAction = new List<EmployeeWorkingHistory>();
            var listSalaryChangeRequestEmployeeBeforeAction = new List<SalaryChangeRequestEmployee>();

            WithUnitOfWork(() =>
            {
                // Get list employeeWorkingHistory before change status to quit
                listEmployeeWorkingHistorBeforeAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee before change status to quit
                listSalaryChangeRequestEmployeeBeforeAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Action
                _changeEmployeeWorkingStatusManager.ChangeStatusToQuit(toQuitDto);
            });

            WithUnitOfWork(() =>
            {
                // Get employee after change status to quit
                var employeeAfterChangeStatusToQuit = employeeRepository.Get(toQuitDto.EmployeeId);

                // Get benifit of employee after change status to quit
                var benifitEmployeeAfteAction = benifitEmployeeRepository.Get(benifitEmployeeDto.Id);

                // Get list employeeWorkingHistpry after change status to quit
                var listEmployeeWorkingHistorAfterAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee after change status to quit
                var listSalaryChangeRequestEmployeeAfterAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Compare employee before and after change status to quit
                Assert.Equal(EmployeeStatus.Quit, employeeAfterChangeStatusToQuit.Status);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.RealSalary);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.Salary);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.ProbationPercentage);

                // listEmployeeWorkingHistorAfterAction.Count must be greater than listEmployeeWorkingHistorBeforeAction.Count
                listEmployeeWorkingHistorAfterAction.Count.ShouldBeGreaterThan(listEmployeeWorkingHistorBeforeAction.Count);

                // Compare benifit of employee before and after change status to quit
                Assert.Equal(benifitEmployeeDto.StartDate, benifitEmployeeAfteAction.StartDate);
                Assert.Equal(benifitEmployeeDto.EndDate, benifitEmployeeAfteAction.EndDate);

                // listSalaryChangeRequestEmployeeAfterAction must be greater than listSalaryChangeRequestEmployeeBeforeAction
                listSalaryChangeRequestEmployeeAfterAction.Count.ShouldBeGreaterThan(listSalaryChangeRequestEmployeeBeforeAction.Count);
            });
        }

        [Fact]
        public void ChangeStatusToPause()
        {
            var toPauseDto = ToPauseDto();
            var benifitEmployeeDto = BenefitsOfEmployeeDto();
            var employeeRepository = Resolve<IRepository<Employee, long>>();
            var employeeWorkingHistoryRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var benifitEmployeeRepository = Resolve<IRepository<BenefitEmployee, long>>();
            var salaryChangeRequestEmployeeRepository = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();

            var listEmployeeWorkingHistorBeforeAction = new List<EmployeeWorkingHistory>();
            var listSalaryChangeRequestEmployeeBeforeAction = new List<SalaryChangeRequestEmployee>();

            WithUnitOfWork(() =>
            {
                // Get list employeeWorkingHistory before change status to pause
                listEmployeeWorkingHistorBeforeAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee before change status to pause
                listSalaryChangeRequestEmployeeBeforeAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Action
                _changeEmployeeWorkingStatusManager.ChangeStatusToPause(toPauseDto);
            });

            WithUnitOfWork(() =>
            {
                // Ger employee after change status to pause
                var employeeAfterChangeStatusToQuit = employeeRepository.Get(toPauseDto.EmployeeId);

                // Get benifit of employee after change status to pause
                var benifitEmployeeAfteAction = benifitEmployeeRepository.Get(benifitEmployeeDto.Id);

                // Get list employeeWorkingHistory after change status to pause
                var listEmployeeWorkingHistorAfterAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee after change status to pause
                var listSalaryChangeRequestEmployeeAfterAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Compare employee before and after change status to pause
                Assert.Equal(EmployeeStatus.Pausing, employeeAfterChangeStatusToQuit.Status);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.RealSalary);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.Salary);
                Assert.Equal(0, employeeAfterChangeStatusToQuit.ProbationPercentage);

                // listEmployeeWorkingHistorAfterAction.Count must be greater than listEmployeeWorkingHistorBeforeAction.Count
                listEmployeeWorkingHistorAfterAction.Count.ShouldBeGreaterThan(listEmployeeWorkingHistorBeforeAction.Count);

                // Compare benifit of employee before and after change status to pause
                Assert.Equal(benifitEmployeeDto.StartDate, benifitEmployeeAfteAction.StartDate);
                Assert.Equal(benifitEmployeeDto.EndDate, benifitEmployeeAfteAction.EndDate);

                // listSalaryChangeRequestEmployeeAfterAction must be greater than listSalaryChangeRequestEmployeeBeforeAction
                listSalaryChangeRequestEmployeeAfterAction.Count.ShouldBeGreaterThan(listSalaryChangeRequestEmployeeBeforeAction.Count);
            });
        }

        [Fact]
        public void ChangeStatusToMaternityLeave()
        {
            var toMaternityLeaveDto = ToMaternityLeaveDto();
            var benifitEmployeeDto = BenefitsOfEmployeeDto();
            var employeeRepository = Resolve<IRepository<Employee, long>>();
            var employeeWorkingHistoryRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var benifitEmployeeRepository = Resolve<IRepository<BenefitEmployee, long>>();
            var salaryChangeRequestEmployeeRepository = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();

            var listEmployeeWorkingHistorBeforeAction = new List<EmployeeWorkingHistory>();
            var listSalaryChangeRequestEmployeeBeforeAction = new List<SalaryChangeRequestEmployee>();

            WithUnitOfWork(() =>
            {
                // Get list employeeWorkingHistory before change status to maternityLeave
                listEmployeeWorkingHistorBeforeAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee before change status to maternityLeave
                listSalaryChangeRequestEmployeeBeforeAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Action
                _changeEmployeeWorkingStatusManager.ChangeStatusToMaternityLeave(toMaternityLeaveDto);
            });

            WithUnitOfWork(() =>
            {
                // Ger employee after change status to maternityLeave
                var employeeAfterChangeStatusToQuit = employeeRepository.Get(toMaternityLeaveDto.EmployeeId);

                // Get benifit of employee after change status to maternityLeave
                var benifitEmployeeAfteAction = benifitEmployeeRepository.Get(benifitEmployeeDto.Id);

                // Get list employeeWorkingHistory after change status to maternityLeave
                var listEmployeeWorkingHistorAfterAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee after change status to maternityLeave
                var listSalaryChangeRequestEmployeeAfterAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Compare employee before and after change status to maternityLeave
                Assert.Equal(EmployeeStatus.MaternityLeave, employeeAfterChangeStatusToQuit.Status);
                Assert.Equal(toMaternityLeaveDto.ToSalary, employeeAfterChangeStatusToQuit.RealSalary);
                Assert.Equal(toMaternityLeaveDto.ToSalary, employeeAfterChangeStatusToQuit.Salary);
                Assert.Equal(100, employeeAfterChangeStatusToQuit.ProbationPercentage);

                // listEmployeeWorkingHistorAfterAction.Count must be greater than listEmployeeWorkingHistorBeforeAction.Count
                listEmployeeWorkingHistorAfterAction.Count.ShouldBeGreaterThan(listEmployeeWorkingHistorBeforeAction.Count);

                // Compare benifit of employee before and after change status to maternityLeave
                Assert.Equal(benifitEmployeeDto.StartDate, benifitEmployeeAfteAction.StartDate);
                Assert.Equal(benifitEmployeeDto.EndDate, benifitEmployeeAfteAction.EndDate);

                // listSalaryChangeRequestEmployeeAfterAction must be greater than listSalaryChangeRequestEmployeeBeforeAction
                listSalaryChangeRequestEmployeeAfterAction.Count.ShouldBeGreaterThan(listSalaryChangeRequestEmployeeBeforeAction.Count);
            });
        }

        [Fact]
        public void CanNotChangeStatusToWorking()
        {
            var toWorkingDto = ToWorkingDto();
            var employeeWorkingHistoryRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            _ = WithUnitOfWorkAsync(async () =>
            {
                var currentStatus = employeeWorkingHistoryRepository.GetAll()
                .Where(x => x.EmployeeId == toWorkingDto.EmployeeId)
                .Where(x => x.DateAt.Month == toWorkingDto.ApplyDate.Month && x.DateAt.Year == toWorkingDto.ApplyDate.Year)
                .Select(x => new
                {
                    x.Status,
                    x.DateAt
                })
                .OrderBy(x => x.DateAt)
                .LastOrDefault();

                var expectedMessage = $"Can't not change working status from {currentStatus?.Status} ({DateTimeUtils.ToString(currentStatus.DateAt)}) to Working in the same month." +
                    $" Please delete the lastest working status history";

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _changeEmployeeWorkingStatusManager.ChangeStatusToWorking(toWorkingDto);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public void ChangeStatusToWorking()
        {
            var toWorkingDto = ToWorkingDto2();
            var benifitEmployeeDto = BenefitsOfEmployeeDto();
            var employeeRepository = Resolve<IRepository<Employee, long>>();
            var employeeWorkingHistoryRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var benifitEmployeeRepository = Resolve<IRepository<BenefitEmployee, long>>();
            var salaryChangeRequestEmployeeRepository = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();

            var listEmployeeWorkingHistorBeforeAction = new List<EmployeeWorkingHistory>();
            var listSalaryChangeRequestEmployeeBeforeAction = new List<SalaryChangeRequestEmployee>();

            WithUnitOfWork(() =>
            {
                // Get list employeeWorkingHistory before change status to working
                listEmployeeWorkingHistorBeforeAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee before change status to working
                listSalaryChangeRequestEmployeeBeforeAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Action
                _changeEmployeeWorkingStatusManager.ChangeStatusToWorking(toWorkingDto);
            });

            WithUnitOfWork(() =>
            {
                // Ger employee after change status to working
                var employeeAfterChangeStatusToQuit = employeeRepository.Get(toWorkingDto.EmployeeId);

                // Get benifit of employee after change status to working
                var benifitEmployeeAfteAction = benifitEmployeeRepository.Get(benifitEmployeeDto.Id);

                // Get list employeeWorkingHistory after change status to working
                var listEmployeeWorkingHistorAfterAction = employeeWorkingHistoryRepository.GetAll().ToList();

                // Get list salaryChangeRequestEmployee after change status to working
                var listSalaryChangeRequestEmployeeAfterAction = salaryChangeRequestEmployeeRepository.GetAll().ToList();

                // Compare employee before and after change status to working
                Assert.Equal(EmployeeStatus.Working, employeeAfterChangeStatusToQuit.Status);
                Assert.Equal(toWorkingDto.RealSalary, employeeAfterChangeStatusToQuit.RealSalary);
                Assert.Equal(toWorkingDto.BasicSalary, employeeAfterChangeStatusToQuit.Salary);
                Assert.Equal(toWorkingDto.ProbationPercentage, employeeAfterChangeStatusToQuit.ProbationPercentage);
                Assert.Equal(toWorkingDto.ToLevelId, employeeAfterChangeStatusToQuit.LevelId);
                Assert.Equal(toWorkingDto.ToJobPositionId, employeeAfterChangeStatusToQuit.JobPositionId);
                Assert.Equal(toWorkingDto.ToUserType, employeeAfterChangeStatusToQuit.UserType);

                // listEmployeeWorkingHistorAfterAction.Count must be greater than listEmployeeWorkingHistorBeforeAction.Count
                listEmployeeWorkingHistorAfterAction.Count.ShouldBeGreaterThan(listEmployeeWorkingHistorBeforeAction.Count);

                // Compare benifit of employee before and after change status to working
                Assert.Equal(benifitEmployeeDto.StartDate, benifitEmployeeAfteAction.StartDate);
                Assert.Equal(benifitEmployeeDto.EndDate, benifitEmployeeAfteAction.EndDate);

                // listSalaryChangeRequestEmployeeAfterAction must be greater than listSalaryChangeRequestEmployeeBeforeAction
                listSalaryChangeRequestEmployeeAfterAction.Count.ShouldBeGreaterThan(listSalaryChangeRequestEmployeeBeforeAction.Count);
            });
        }
    }
}
