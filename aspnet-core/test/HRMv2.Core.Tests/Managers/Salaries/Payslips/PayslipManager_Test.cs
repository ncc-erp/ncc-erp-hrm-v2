using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using Abp.UI;
using AutoMapper.Internal;
using HRMv2.Entities;
using HRMv2.Hubs;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Salaries.Payslips
{
    public class PayslipManager_Test : HRMv2CoreTestBase
    {
        private PayslipManagerValue payslipManagerValue;
        private readonly PayslipManager _payslipManager;

        //private readonly HttpClient _httpClient;
        private readonly TimesheetWebService _timesheetService;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly CalculateSalaryHub _calculateSalaryHub;
        private readonly IOptions<TimesheetConfig> _timesheetConfig;
        private readonly IWorkScope _workScopeRoot;

        private InputCollectDataForPayslipDto inputCollectData;



        public PayslipManager_Test()
        {
            payslipManagerValue = new PayslipManagerValue();
            var _httpClient = Resolve<HttpClient>();
            var _iAbpSession = Resolve<IAbpSession>();
            var _iocResovler = Resolve<IIocResolver>();
            var timesheetServiceMock = new Mock<TimesheetWebService>(_httpClient, _iAbpSession, _iocResovler);
            inputCollectData = payslipManagerValue.inputCollectDataForPayslipDto;
            //TODO test case: add virtual to GetOTTimesheets, GetChamCongInfo, GetAllRequestDays, GetSettingOffDates in TimesheetWebService to pass test case
            //timesheetServiceMock.Setup(p => p.GetSettingOffDates(It.IsAny<int>(), It.IsAny<int>()))
            //                        .Returns(new HashSet<DateTime> { new DateTime(2022, 12, 12) });
            //timesheetServiceMock.Setup(p => p.GetOTTimesheets(inputCollectData))
            //                        .Returns(payslipManagerValue.valueReturnForOTTTimesheetsApi);
            //timesheetServiceMock.Setup(p => p.GetChamCongInfo(inputCollectData))
            //                        .Returns(payslipManagerValue.valueReturnForChamCongInfoApi);
            //timesheetServiceMock.Setup(p => p.GetAllRequestDays(inputCollectData))
            //                        .Returns(payslipManagerValue.valueReturnForGetAllRequestDaysApi);
            _timesheetService = timesheetServiceMock.Object;
            var _workScope = Resolve<IWorkScope>();
            var _emailSender = Resolve<IEmailSender>();
            var _timesheetConfig2 = Resolve<IOptions<TimesheetConfig>>();
            _emailManager = Substitute.For<EmailManager>(_workScope, _emailSender, _timesheetConfig2);
            var _iocResolver = Resolve<IIocResolver>();
            var _store = Resolve<IBackgroundJobStore>();
            var _timer = Resolve<AbpAsyncTimer>();
            _backgroundJobManager = Substitute.For<BackgroundJobManager>(_iocResolver, _store, _timer);
            _calculateSalaryHub = Substitute.For<CalculateSalaryHub>();
            _timesheetConfig = Resolve<IOptions<TimesheetConfig>>();
            _workScopeRoot = Resolve<IWorkScope>();
            
            _payslipManager = new PayslipManager(
                _timesheetService,
                _emailManager,
                _backgroundJobManager,
                _calculateSalaryHub,
                _timesheetConfig,
                _workScopeRoot);

            _payslipManager.ObjectMapper = Resolve<IObjectMapper>();
            _payslipManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();

            //_timesheetService.set
        }

        [Fact]
        public void QueryAllPayslip_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                var allPaysLipExpect = _workScopeRoot.GetAll<Payslip>().ToList();

                IQueryable<GetPayslipDto> getPayslipDtos = _payslipManager.QueryAllPayslip();

                Assert.True(getPayslipDtos.Count() > 0);
                Assert.Equal(allPaysLipExpect.Count, getPayslipDtos.Count());
                var allPayslipReceived = getPayslipDtos.ToList();
                Assert.Contains(57333, allPayslipReceived.Select(x => x.Id).ToList());
                Assert.Contains(146, allPayslipReceived.Select(x => x.PayrollId).ToList());
                Assert.Contains(880, allPayslipReceived.Select(x => x.EmployeeId).ToList());
            });
        }

        [Fact]
        public async Task GetPayslipEmployeePaging_Should_Be_Get_2()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayrollId = 146;
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayrollId, input);

                Assert.Equal(2, result.Items.Count);
                var resultPayslipGetted = _workScopeRoot.GetAll<Payslip>().Where(x => x.Id == result.Items.First().Id).First();
                Assert.Equal(inputPayrollId, resultPayslipGetted.PayrollId);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
            });
        }

        [Fact]
        public async Task GetPayslipEmployeePaging_Have_Search()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayrollId = 146;
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0,
                        SearchText= "An"
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayrollId, input);

                Assert.Equal(2, result.Items.Count);
                var resultPayslipGetted = _workScopeRoot.GetAll<Payslip>().Where(x => x.Id == result.Items.First().Id).First();
                Assert.Equal(inputPayrollId, resultPayslipGetted.PayrollId);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Contains("An", payslipEmployeeTest.FullName);
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
            });
        }


        [Fact]
        public void GetPayslipEmployeePaging_Filter_employee_status()
        {
            // filter by status employee
            WithUnitOfWork(async () =>
            {
                long inputPayRollId = 146;
                var allPayslip = _workScopeRoot.GetAll<Payslip>().ToList();
                var allEmployeesFilterShouldBe = _workScopeRoot.GetAll<Payslip>()
                                            .Where(x => x.PayrollId == inputPayRollId)
                                            .Where(x => x.Employee.Status == EmployeeStatus.Working )
                                            .Select(x => x.Id).ToList();
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    },
                    StatusIds = new List<EmployeeStatus> {
                        EmployeeStatus.Working
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayRollId, input);

                Assert.True(result.TotalCount > 0);
                Assert.Equal(input.GridParam.MaxResultCount, result.Items.Count);
                Assert.Equal(allEmployeesFilterShouldBe.Count, result.TotalCount);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
                // check status is working
                Assert.Equal(EmployeeStatus.Working ,payslipEmployeeTest.Status);
            });
        }

        [Fact]
        public void GetPayslipEmployeePaging_Filter_branch()
        {
            // filter by status employee
            WithUnitOfWork(async () =>
            {
                long inputPayRollId = 146;
                long inputBranchId = 94;
                var allPayslip = _workScopeRoot.GetAll<Payslip>().ToList();
                var allEmployeesFilterShouldBe = _workScopeRoot.GetAll<Payslip>()
                                            .Where(x => x.PayrollId == inputPayRollId)
                                            .Where(x => x.Employee.Branch.Id == inputBranchId)
                                            .Select(x => x.Id).ToList();
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    },
                    BranchIds = new List<long> {
                        inputBranchId
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayRollId, input);

                Assert.True(result.TotalCount > 0);
                Assert.Equal(input.GridParam.MaxResultCount, result.Items.Count);
                Assert.Equal(allEmployeesFilterShouldBe.Count, result.TotalCount);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
                // check branch
                Assert.Equal(inputBranchId, payslipEmployeeTest.BranchId);
            });
        }

        [Fact]
        public void GetPayslipEmployeePaging_filter_user_type()
        {
            // filter by status employee
            WithUnitOfWork(async () =>
            {
                long inputPayRollId = 146;
                UserType userTypeUsingFilter = UserType.Staff;
                var allPayslip = _workScopeRoot.GetAll<Payslip>().ToList();
                var allEmployeesFilterShouldBe = _workScopeRoot.GetAll<Payslip>()
                                            .Where(x => x.PayrollId == inputPayRollId)
                                            .Where(x => x.UserType == userTypeUsingFilter)
                                            .Select(x => x.Id).ToList();
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    },
                    Usertypes = new List<UserType> {
                        userTypeUsingFilter
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayRollId, input);

                Assert.True(result.TotalCount > 0);
                Assert.Equal(input.GridParam.MaxResultCount, result.Items.Count);
                Assert.Equal(allEmployeesFilterShouldBe.Count, result.TotalCount);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
                // check UserType
                Assert.Equal(userTypeUsingFilter, payslipEmployeeTest.UserType);
            });
        }
        
        [Fact]
        public void GetPayslipEmployeePaging_filter_LevelIds()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayRollId = 146;
                long levelidUsingFilter = 315;
                var allPayslip = _workScopeRoot.GetAll<Payslip>().ToList();
                var allEmployeesFilterShouldBe = _workScopeRoot.GetAll<Payslip>()
                                            .Where(x => x.PayrollId == inputPayRollId)
                                            .Where(x => x.LevelId == levelidUsingFilter)
                                            .Select(x => x.Id).ToList();
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    },
                    LevelIds = new List<long> {
                        levelidUsingFilter
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayRollId, input);

                Assert.True(result.TotalCount > 0);
                Assert.Equal(input.GridParam.MaxResultCount, result.Items.Count);
                Assert.Equal(allEmployeesFilterShouldBe.Count, result.TotalCount);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
                // check LevelId
                Assert.Equal(levelidUsingFilter, payslipEmployeeTest.LevelId);
            });
        }


        [Fact]
        public void GetPayslipEmployeePaging_filter_JobPositionIds()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayRollId = 146;
                long jobPositionIdsUsingFilter = 48;
                var allPayslip = _workScopeRoot.GetAll<Payslip>().ToList();
                var allEmployeesFilterShouldBe = _workScopeRoot.GetAll<Payslip>()
                                            .Where(x => x.PayrollId == inputPayRollId)
                                            .Where(x => x.JobPositionId == jobPositionIdsUsingFilter)
                                            .Select(x => x.Id).ToList();
                InputGetPayslipEmployeeDto input = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam()
                    {
                        MaxResultCount = 2,
                        SkipCount = 0
                    },
                    JobPositionIds = new List<long> {
                        jobPositionIdsUsingFilter
                    }
                };

                GridResult<GetPayslipEmployeeDto> result = await _payslipManager.GetPayslipEmployeePaging(inputPayRollId, input);

                Assert.True(result.TotalCount > 0);
                Assert.Equal(input.GridParam.MaxResultCount, result.Items.Count);
                Assert.Equal(allEmployeesFilterShouldBe.Count, result.TotalCount);
                var payslipEmployeeTest = result.Items[0];
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
                // check LevelId
                Assert.Equal(jobPositionIdsUsingFilter, payslipEmployeeTest.JobPositionId);
            });
        }

        [Fact]
        public void GetPayslipByPayrollId_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                long inputPayrollId = 146;
                var allPapSlipWithPayRollIdShouldReceived = _workScopeRoot.GetAll<Payslip>()
                     .Where(x => x.PayrollId == inputPayrollId).ToList();

                List<ExportPayrollIncludeLastMonthDto> result = _payslipManager.GetPayslipByPayrollId(inputPayrollId);

                Assert.True(result.Count > 0);
                Assert.Equal(allPapSlipWithPayRollIdShouldReceived.Count, result.Count());
                Payslip papyslipShouldBeRecieved = _workScopeRoot.GetAll<Payslip>().Where(x => x.PayrollId == inputPayrollId).First();
                Assert.Contains(papyslipShouldBeRecieved.Id, result.Select(r => r.Id).ToList());
            });
        }

        [Fact]
        public void GetPayslipEmployeeForExport_HappyTest()
        {
            long inputPayrollId = 146;
            WithUnitOfWork(() =>
            {
                List<ExportPayrollIncludeLastMonthDto> result = _payslipManager.GetPayslipEmployeeForExport(inputPayrollId);
                
                Assert.True(result.Count() > 20);
                var payslipEmployeeTest = result.First();
                var employeeSouldReceive = _workScopeRoot.GetAll<Employee>().Where(x => x.Id == 880).First();
                var payslipSouldReceive = _workScopeRoot.GetAll<Payslip>().Where(x => x.EmployeeId == 880).First ();
                Assert.Equal(payslipEmployeeTest.EmployeeId, employeeSouldReceive.Id);
                Assert.Equal(payslipEmployeeTest.FullName, employeeSouldReceive.FullName);
                Assert.Equal(payslipEmployeeTest.JobPositionId, employeeSouldReceive.JobPositionId);
                Assert.Equal(payslipEmployeeTest.BranchId, employeeSouldReceive.BranchId);
                Assert.Equal(payslipEmployeeTest.Email, employeeSouldReceive.Email);
                Assert.Equal(UserType.Staff, employeeSouldReceive.UserType);
                Assert.Equal(payslipEmployeeTest.LevelId, employeeSouldReceive.LevelId);
                Assert.Equal(payslipEmployeeTest.RealSalary, payslipSouldReceive.Salary);
                Assert.Equal(payslipEmployeeTest.NormalDay, payslipSouldReceive.NormalDay);
                Assert.Equal(payslipEmployeeTest.OTHour, payslipSouldReceive.OTHour);
                Assert.Equal(payslipEmployeeTest.OffDay, payslipSouldReceive.OffDay);
            });
        }

        //TODO: test case [check exception with invalid/not exits payrollid]

        //[Fact]
        //public void GetPayslipEmployeeForExport_Should_Throw_Exception_With_Payroll_Not_Exits()
        //{
        //    long inputPayrollId = -1;
        //    WithUnitOfWork(async () =>
        //    {
        //        var result = await Assert.ThrowsAsync<ArgumentException>(
        //            async () => _payslipManager.GetPayslipEmployeeForExport(inputPayrollId));

        //        Assert.Equal($"An item with the same key has already been", result.Message);
        //    });
        //}

        [Fact]
        public void GetPayslipResult_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                var intputPayslipId = 57250;
                var allPayslip = _workScopeRoot.GetAll<PayslipDetail>()
                     .Where(x => x.PayslipId == intputPayslipId).ToList();

                List<GetSalaryDetailDto> result = _payslipManager.GetPayslipResult(intputPayslipId);

                Assert.True( result.Count() > 0);
                Assert.Equal(allPayslip.Count, result.Count());
                Assert.Equal(result.First().PayslipId, intputPayslipId);
                Assert.DoesNotContain(result, x => x.PayslipId != intputPayslipId);
            });
        }

        [Fact]
        public void GetPayslipDetailByType_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                var intputPayslipId = 57333;
                var inputPayslipDetailType = PayslipDetailType.SalaryNormal;

                List<GetPayslipDetailByTypeDto> result = 
                    _payslipManager.GetPayslipDetailByType(intputPayslipId, inputPayslipDetailType);

                var salaryDetailDto = result[0];
                Assert.Single(result);
                Assert.Equal(intputPayslipId, salaryDetailDto.PayslipId);
                Assert.Equal(inputPayslipDetailType, salaryDetailDto.Type);
            });
        }

        // ---------------------------PayslipDetailPunishment------------------
        [Fact]
        public async Task CreatePayslipDetailPunishment_HappyTest()
        {
            const PayslipDetailType DEFAULT_PAYSLIP_DETAIL_TYPE = PayslipDetailType.Punishment;
            const bool DEFAULT_IS_PROJECT_COST = false;
            const int DEFAULT_MONEY_FACTOR = -1;
            long countRawPayslipDetail = 0;

            WithUnitOfWork(async () =>
            {
                countRawPayslipDetail = _workScopeRoot.GetAll<PayslipDetail>().ToList().Count;
                CreatePayslipDetailDto input = new CreatePayslipDetailDto()
                {
                    Id = 4,
                    PayslipId = 2,
                    Money = 100,
                    Note = "New PayslipDetail",
                    Type = PayslipDetailType.SalaryOT,
                    IsProjectCost = true
                };
                var result = await _payslipManager.CreatePayslipDetailPunishment(input);

                var check = _workScopeRoot.GetAsync<PayslipDetail>(input.Id);


                Assert.NotNull(check);
                Assert.Equal(DEFAULT_PAYSLIP_DETAIL_TYPE, check.Result.Type);
                Assert.Equal(DEFAULT_IS_PROJECT_COST, check.Result.IsProjectCost);
                Assert.Equal(DEFAULT_MONEY_FACTOR * input.Money, check.Result.Money);
            });

            WithUnitOfWork(async () =>
            {
                long newListCount = _workScopeRoot.GetAll<PayslipDetail>().ToList().Count;
                Assert.Equal(countRawPayslipDetail + 1, newListCount);
            });
        }

        //TODO: test case [throw error create duplicate,fail]
        
        //[Fact]
        //public async Task CreatePayslipDetailPunishment__Should_Throw_Exception_Duplicate()
        //{
        //    WithUnitOfWork(async () =>
        //    {
        //        CreatePayslipDetailDto input = new CreatePayslipDetailDto()
        //        {
        //            Id = 247458,
        //            PayslipId = 2,
        //            Money = 0,
        //            Note = "New PayslipDetail",
        //            Type = PayslipDetailType.SalaryOT,
        //            IsProjectCost = true
        //        };

        //        var result = await Assert.ThrowsAsync<ArgumentException>(
        //            async () => await _payslipManager.CreatePayslipDetailPunishment(input));

        //        Assert.Equal($"An item with the same key has already been added. Key: {input.Id}", result.Message);
        //    });
        //}

        [Fact]
        public void UpdatePayslipDetailPunishment_HappyTest()
        {
            const int DEFAULT_MONEY_FACTOR = -1;

            UpdatePayslipDetailDto input = new UpdatePayslipDetailDto()
            {
                Id = 247458,
                Money = 100,
                Note = "update PayslipDetail"
            };

            WithUnitOfWork(async () =>
            {
                var result = await _payslipManager.UpdatePayslipDetailPunishment(input);

                Assert.NotNull(result);
                Assert.True(result.Money != 0);
                Assert.True(result.Note.Length > 0);
            });

            WithUnitOfWork(async () =>
            {
                var check = _workScopeRoot.GetAsync<PayslipDetail>(input.Id);

                Assert.NotNull(check);
                Assert.Equal(input.Id, check.Result.Id);
                Assert.Equal(input.Note, check.Result.Note);
                Assert.Equal(input.Money * DEFAULT_MONEY_FACTOR, check.Result.Money);
            });
        }

        [Fact]
        public void UpdatePayslipDetailPunishment_Sould_Throw_ExceptionUserNotExits()
        {
            WithUnitOfWork(async () =>
            {
                UpdatePayslipDetailDto input = new UpdatePayslipDetailDto()
                {
                    Id = 100,
                    Money = 100,
                    Note = "update PayslipDetail"
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.UpdatePayslipDetailPunishment(input));
                
                Assert.Equal($"Payslip detail with Id = {input.Id} is not exist", result.Message);
            });
        }

        // ---------------------------PayslipDetailBonus------------------
        [Fact]
        public async Task CreatePayslipDetailBonus_HappyTest()
        {
            const PayslipDetailType DEFAULT_PAYSLIP_DETAIL_TYPE_BONUS = PayslipDetailType.Bonus;
            const bool DEFAULT_IS_PROJECT_COST_BONUS = true;

            WithUnitOfWork(async () =>
            {
                CreatePayslipDetailDto input = new CreatePayslipDetailDto()
                {
                    Id = 100,
                    PayslipId = 2,
                    Money = 100,
                    Note = "New PayslipDetail",
                    Type = PayslipDetailType.SalaryOT,
                    IsProjectCost= true
                };

                var result = await _payslipManager.CreatePayslipDetailBonus(input);

                var check = _workScopeRoot.GetAsync<PayslipDetail>(input.Id);
                Assert.NotNull(check);
                Assert.Equal(input.Id, check.Result.Id);
                Assert.Equal(input.Note, check.Result.Note);
                Assert.Equal(DEFAULT_PAYSLIP_DETAIL_TYPE_BONUS, check.Result.Type);
                Assert.Equal(DEFAULT_IS_PROJECT_COST_BONUS, check.Result.IsProjectCost);
            });
        }

        [Fact]
        public async Task CreatePayslipDetailBonus_ExceptionDuplidate()
        {
            WithUnitOfWork(async () =>
            {
                CreatePayslipDetailDto input = new CreatePayslipDetailDto()
                {
                    Id = 2,
                    PayslipId = 2,
                    Money = 100,
                    Note = "New PayslipDetail",
                    Type = PayslipDetailType.SalaryOT,
                    IsProjectCost = true
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.CreatePayslipDetailBonus(input));

                Assert.Equal($"An item with the same key has already been added. Key: {input.Id}", result.Message);
            });
        }

        [Fact]
        public void UpdatePayslipDetailBonus_HappyTest()
        {
            UpdatePayslipDetailDto input = new UpdatePayslipDetailDto()
            {
                Id = 247458,
                Money = 100,
                Note = "update PayslipDetail"
            };
            WithUnitOfWork(async () =>
            {
                var result = await _payslipManager.UpdatePayslipDetailBonus(input);

                Assert.NotNull(result);
                Assert.True(result.Money != 0);
                Assert.True(result.Note.Length > 0);
            });

            WithUnitOfWork(async () =>
            {
                var check = _workScopeRoot.GetAsync<PayslipDetail>(input.Id);
                Assert.NotNull(check);
                Assert.Equal(input.Id, check.Result.Id);
                Assert.Equal(input.Note, check.Result.Note);
            });
        }

        [Fact]
        public void UpdatePayslipDetailBonus_ExceptionNotExits()
        {
            WithUnitOfWork(async () =>
            {
                UpdatePayslipDetailDto input = new UpdatePayslipDetailDto()
                {
                    Id = 100,
                    Money = 100,
                    Note = "update PayslipDetail"
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.UpdatePayslipDetailBonus(input));
                Assert.Equal($"Payslip detail with Id = {input.Id} is not exist", result.Message);
            });
        }

        // ---------------------------PayslipDetail------------------
        [Fact]
        public void DeletePayslipDetail_HappyTest()
        {
            long inputId = 247458;
            var countAllPayslipDetailBeforeDelete = 0;
            WithUnitOfWork(async () =>
            {
                countAllPayslipDetailBeforeDelete = _workScopeRoot.GetAll<PayslipDetail>().Count();

                var idDeleted = await _payslipManager.DeletePayslipDetail(inputId);

                Assert.Equal(inputId, idDeleted);
            });

            WithUnitOfWork(async () =>
            {
                var newListPayslipDetail = _workScopeRoot.GetAll<PayslipDetail>();
                var payslipDetails = _workScopeRoot.GetAll<PayslipDetail>();
                var payslipDetail = await payslipDetails.AnyAsync(x => x.Id == inputId);
                Assert.False(payslipDetail);
                Assert.Equal(countAllPayslipDetailBeforeDelete - 1, newListPayslipDetail.Count());
            });
        }

        [Fact]
        public void DeletePayslipDetail_ExceptionNotExits()
        {
            WithUnitOfWork(async () =>
            {
                long inputId = -100;

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.DeletePayslipDetail(inputId));

                Assert.Equal($"Payslip detail with Id = {inputId} is not exist", result.Message);
            });
        }

        [Fact]
        public void ValidPayslipDetail_UserExits()
        {
            WithUnitOfWork(async () =>
            {
                long inputId = 247458;

                var result = await _payslipManager.ValidPayslipDetail(inputId);

                Assert.Equal(inputId, result);
            });
        }

        [Fact]
        public void ValidPayslipDetail_ExceptionNotExits()
        {
            WithUnitOfWork(async () =>
            {
                long inputId = 100;

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.ValidPayslipDetail(inputId));

                Assert.Equal($"Payslip detail with Id = {inputId} is not exist", result.Message);
            });
        }
        
        [Fact]
        public void GetEmployeeIdsInPayroll()
        {
            WithUnitOfWork(() =>
            {
                long inputPayrollId = 146;

                var resultListEmployeeId = _payslipManager.GetEmployeeIdsInPayroll(inputPayrollId);

                var payslips = _workScopeRoot.GetAll<Payslip>().Where(x => x.PayrollId == inputPayrollId).ToList();
                Assert.True(resultListEmployeeId.Count > 0);
                Assert.Equal(payslips.Count, resultListEmployeeId.Count);
            });
        }

        [Fact]
        public void GetPayslipDetail()
        {

            WithUnitOfWork(() =>
            {
                long inputPayslipId = 57333;
                var getPayslipSalaryViaPayslipId = _workScopeRoot.GetAsync<PayslipSalary>(inputPayslipId);

                GetPayslipDetailDto payslipDetailDto = _payslipManager.GetPayslipDetail(inputPayslipId);

                Assert.NotNull(payslipDetailDto);
                Assert.True(payslipDetailDto.InputSalary.Count >= 0);
                Assert.NotNull(payslipDetailDto.CalculateResult);
                var allPayslip = _workScopeRoot.GetAll<Payslip>().Where(x => x.Id == inputPayslipId);
                Assert.Equal(payslipDetailDto.TotalRealSalary, allPayslip.Select(x => x.Salary).FirstOrDefault());
            });
        }

        [Fact]
        public void GetSumaryInfomation_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                long inputPayrollId = 146;
                var list1 = _workScopeRoot.GetAll<PayslipDetail>()
                                .Where(x => x.Payslip.PayrollId == inputPayrollId)
                                .GroupBy(x => x.Type)
                                .Select(x => new
                                {
                                    Type = x.Key,
                                    TotalSalary = x.Sum(x => x.Money),
                                    Quantity = x.Count(),
                                }).ToList()
                                .Select(s => new SumaryInfoDto
                                {
                                    Name = Enum.GetName(typeof(PayslipDetailType), s.Type),
                                    TotalSalary = s.TotalSalary,
                                    Quantity = s.Quantity,
                                    Type = s.Type

                                }).ToList();
                var list2 = _workScopeRoot.GetAll<Payslip>()
                                    .Where(x => x.PayrollId == inputPayrollId)
                                    .GroupBy(s => s.UserType)
                                    .Select(s => new
                                    {
                                        UserType = s.Key,
                                        TotalSalary = s.Sum(s => s.Salary),
                                        Quatity = s.Count()
                                    }).ToList()
                                    .GroupBy(s => GetReportTypeName(s.UserType))
                                    .Select(s => new SumaryInfoDto
                                    {
                                        Name = s.Key,
                                        TotalSalary = s.Sum(x => x.TotalSalary),
                                        Quantity = s.Sum(x => x.Quatity)
                                    }).ToList();

                // act
                List<SumaryInfoDto> listSumaryInfoDto = _payslipManager.GetSumaryInfomation(inputPayrollId);

                // list = default "Toàn công ty"
                // + list PayslipDetail get by payrollId
                // + list Payslip get by payrollId 
                // + default "Phạt không thu được"
                Assert.True(listSumaryInfoDto.Count > 2);
                SumaryInfoDto firtSumaryInfoDto = listSumaryInfoDto.First();
                SumaryInfoDto lastSumaryInfoDto = listSumaryInfoDto.Last();
                Assert.Equal("Tổng chi (bắn sang Finfast)  = Tổng lương + Phạt không thu được", firtSumaryInfoDto.Name);
                Assert.Equal("Phạt không thu được", lastSumaryInfoDto.Name);
                Assert.Equal(list1.Count + list2.Count + 4, listSumaryInfoDto.Count);
            });
        }

        private string GetReportTypeName(UserType type)
        {
            if (type == UserType.Internship)
            {
                return "Intern";
            }

            if (type == UserType.Vendor)
            {
                return "Vendor";
            }
            return "Staff + CTV + T.Việc";
        }

        [Fact]
        public void GetPayslipCalculateResult_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                long inputPayslipId = 57333;
                var detail = _workScopeRoot.GetAll<PayslipDetail>()
                                .Where(x => x.PayslipId == inputPayslipId)
                                .GroupBy(x => x.Type)
                                .Select(x => new
                                {
                                    x.Key,
                                    TotalMoney = x.Sum(s => s.Money)
                                }).ToDictionary(x => x.Key, x => x.TotalMoney);

                CalculateResultDto calculateResult = _payslipManager.GetPayslipCalculateResult(inputPayslipId);

                Assert.NotNull(calculateResult);
                Assert.Equal(0, calculateResult.NormalSalary);
                Assert.Equal(0, calculateResult.OTSalary);
                Assert.Equal(2000000 + 500000 + 2000000, calculateResult.TotalBenefit);
                Assert.Equal(1000000, calculateResult.TotalBonus);
            });
        }

        [Fact]
        public void AddGeneratePayslipsToBackgroundJob_Should_Be_No_Error()
        {
            WithUnitOfWork(() =>
            {
                CollectPayslipDto inputCollectPayslipDto = new CollectPayslipDto
                {
                    PayrollId= 1,
                    EmployeeIds= new List<long> { 1, 2 }
                };

                GeneratePayslipResultDto resultPayslipsToBackground = 
                    _payslipManager.AddGeneratePayslipsToBackgroundJob(inputCollectPayslipDto);
                
                Assert.Empty(resultPayslipsToBackground.ErrorList);
            });
        }

        //TODO: test case pass khi chạy lẻ 
        //[Fact]
        //public void GetCalculatingSalaryInfoDto_Dont_Have_Param()
        //{
        //    WithUnitOfWork(() =>
        //    {
        //        var resultShouldBe = new CalculatingSalaryInfoDto();

        //        CalculatingSalaryInfoDto result =
        //            _payslipManager.GetCalculatingSalaryInfoDto(null);

        //        Assert.NotNull(result);
        //        Assert.NotNull(PayslipManager.DicTenantIdToCalculatingSalaryInfoDto[-1]);
        //        Assert.Equal(result.IsRuning , resultShouldBe.IsRuning);
        //        Assert.Equal(result.ProgressInfo, resultShouldBe.ProgressInfo);
        //    });

        //    WithUnitOfWork(() =>
        //    {
        //        Assert.NotNull(PayslipManager.DicTenantIdToCalculatingSalaryInfoDto[-1]);
        //    });
        //}

        //TODO: test case pass khi chạy lẻ 
        //[Fact]
        //public void GetCalculatingSalaryInfoDto_HaveParam()
        //{
        //    int inputTenantId = 1;
        //    var resultSuouldBe = new CalculatingSalaryInfoDto
        //    {
        //        IsRuning = true,
        //        ProgressInfo = "Processing..."
        //    };

        //    WithUnitOfWork(() =>
        //    {
        //        PayslipManager.DicTenantIdToCalculatingSalaryInfoDto.Add(inputTenantId, resultSuouldBe);

        //        CalculatingSalaryInfoDto result =
        //            _payslipManager.GetCalculatingSalaryInfoDto(inputTenantId);

        //        Assert.NotNull(result);
        //        Assert.NotNull(PayslipManager.DicTenantIdToCalculatingSalaryInfoDto[inputTenantId]);
        //        Assert.Equal(result.IsRuning, resultSuouldBe.IsRuning);
        //        Assert.Equal(result.ProgressInfo, resultSuouldBe.ProgressInfo);
        //    });

        //    WithUnitOfWork(() =>
        //    {
        //        var check = PayslipManager.DicTenantIdToCalculatingSalaryInfoDto[inputTenantId];
        //        Assert.NotNull(check);
        //        Assert.Equal(check.IsRuning, resultSuouldBe.IsRuning);
        //        Assert.Equal(check.ProgressInfo, resultSuouldBe.ProgressInfo);
        //    });
        //}

        // test GeneratePayslipsTryCacth
        [Fact]
        public void GeneratePayslipsTryCacth_HappyCase()
        {
            WithUnitOfWork(() =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> { 880, 881 }
                };

                GeneratePayslipResultDto result = _payslipManager.GeneratePayslipsTryCacth(input);

                Assert.NotNull(result);
            });
        }

        [Fact]
        public void GeneratePayslipsTryCacth_When_CalculatingSalaryInfo_Is_Running_Shuould_Add_Error()
        {
            int inputTenantId = 1;
            var setUpRunning = new CalculatingSalaryInfoDto
            {
                IsRuning = true,
                ProgressInfo = "Processing..."
            };

            WithUnitOfWork(() =>
            {
                PayslipManager.DicTenantIdToCalculatingSalaryInfoDto.Add(inputTenantId, setUpRunning);
            });

            WithUnitOfWork(() =>
            {
                var input = new CollectPayslipDto
                {
                    TenantId= inputTenantId,
                    PayrollId = 146,
                    EmployeeIds = new List<long> { 880, 881 }
                };

                GeneratePayslipResultDto result = _payslipManager.GeneratePayslipsTryCacth(input);

                Assert.Equal("Is Running => stop", result.ErrorList.First().Message);
                Assert.False(_payslipManager.GetCalculatingSalaryInfoDto(inputTenantId).IsRuning);
            });
        }

        //TODO: test case pass khi chạy lẻ 

        //[Fact]
        //public void GeneratePayslipsTryCacth_When_Have_Exception_Shuould_Add_Error()
        //{
        //    int inputTenantId = 1;
        //    var setUpRunning = new CalculatingSalaryInfoDto
        //    {
        //        IsRuning = false,
        //        ProgressInfo = "Processing..."
        //    };

        //    WithUnitOfWork(() =>
        //    {
        //        PayslipManager.DicTenantIdToCalculatingSalaryInfoDto.Add(inputTenantId, setUpRunning);
        //    });

        //    WithUnitOfWork(() =>
        //    {
        //        var input = new CollectPayslipDto
        //        {
        //            TenantId = inputTenantId,
        //            PayrollId = -1,
        //            EmployeeIds = new List<long> { -1, -2 }
        //        };

        //        GeneratePayslipResultDto result = _payslipManager.GeneratePayslipsTryCacth(input);

        //        Assert.True(result.ErrorList.Count > 0);
        //        Assert.False(_payslipManager.GetCalculatingSalaryInfoDto(inputTenantId).IsRuning);
        //    });
        //}

        //TODO: test case add vitural in GetSettingOffDates in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void GeneratePayslips_HappyTest()
        //{
        //    WithUnitOfWork(async () =>
        //    {
        //        var input = new CollectPayslipDto
        //        {
        //            PayrollId = 146,
        //            EmployeeIds = new List<long> { 880, 881 }
        //        };

        //        GeneratePayslipResultDto result = _payslipManager.GeneratePayslips(input);

        //        Assert.NotNull(result);
        //        Assert.True(result.ErrorList.Count == 0);
        //        Assert.True(result.PayslipIds.Count > 0);
        //    });
        //}

        [Fact]
        public void GeneratePayslips_Should_Throw_Exception_With_Not_Exits_Payslip ()
        {
            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = -1,
                    EmployeeIds = new List<long> { 880, 881 }
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.GeneratePayslips(input));

                Assert.True(result is UserFriendlyException);
                Assert.Equal($"Can't find the Payroll", result.Message);
            });
        }

        [Fact]
        public void GeneratePayslips_Should_GetDebtPlanError()
        {
            WithUnitOfWork(async () =>
            {
                var debtUpdate = _workScopeRoot.GetAll<Debt>().ToList().Where(x => x.EmployeeId == 883).FirstOrDefault();
                debtUpdate.Money = 9999999999999;
                debtUpdate.Status = DebtStatus.Inprogress;
                debtUpdate.PaymentType = DebtPaymentType.Salary;
                await _workScopeRoot.UpdateAsync(debtUpdate);
            });

            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> { 880, 883 }
                };

                GeneratePayslipResultDto result = _payslipManager.GeneratePayslips(input);

                var allDept = _workScopeRoot.GetAll<Debt>().ToList();
                Assert.NotNull(result);
                Assert.True(result.ErrorList.Count > 0);
                Assert.Null(result.PayslipIds);
                Assert.Contains("<strong>hung.trankien@ncc.asia</strong> debt #87 dont have payment plan", result.ErrorList.First().Message);
            });
        }

        [Fact]
        public void GeneratePayslips_Should_Throw_Exception_With_Payslip_Is_NOT_New_RejectedByKT_RejectedByKT_or_RejectedByCEO()
        {
            WithUnitOfWork(() =>
            {
                _workScopeRoot.InsertAsync(new Payroll
                {
                    Id = 1,
                    ApplyMonth= new DateTime(2022,12,12),
                    Status = PayrollStatus.ApprovedByCEO,
                    NormalWorkingDay= 1,
                    OpenTalk=1
                });
            });

            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 1,
                    EmployeeIds = new List<long> { 880, 881 }
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.GeneratePayslips(input));

                Assert.True(result is UserFriendlyException);
                Assert.Equal($"Only calculate salary if the the payroll status is: New, RejectedByKT, RejectedByKT or RejectedByCEO", result.Message);
            });
        }

        [Fact]
        public void GeneratePayslips_Should_Throw_Exception_With_Not_Exits_Employees()
        {
            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> { -1 }
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.GeneratePayslips(input));

                Assert.True(result is UserFriendlyException);
                Assert.Equal($"Không có nhân viên nào thỏa mãn điều kiện để nhận lương tháng này: {JsonConvert.SerializeObject(input)}", result.Message);
            });
        }
        
        
        // this function have been test in Test CalculatorSalary
        [Fact]
        public void TestNccCalculator_HappyTest()
        {
            WithUnitOfWork(() =>
            {
                Assert.True(1 == 1);
            });
        }

        [Fact]
        public void DeletePayslips_HappyCase()
        {
            long deletedPayslipId = 57333;
            var allPayslipCountRoot = 0;
            var allpayslipDetailsCountRoot = 0;
            var allpayslipSalariesCountRoot = 0;
            var alldebtPaidsCountRoot = 0;
            var allpayslipTeamsCountRoot = 0;

            WithUnitOfWork(() =>
            {
                allPayslipCountRoot = _workScopeRoot.GetAll<Payslip>().Count();
                allpayslipDetailsCountRoot = _workScopeRoot.GetAll<PayslipDetail>().Count();
                allpayslipSalariesCountRoot = _workScopeRoot.GetAll<PayslipSalary>().Count();
                alldebtPaidsCountRoot = _workScopeRoot.GetAll<DebtPaid>().Count();
                allpayslipTeamsCountRoot = _workScopeRoot.GetAll<PayslipTeam>().Count();

                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> { 880, 881 }
                };
                var deletePayslips = _workScopeRoot.GetAll<Payslip>()
                                    .Where(x => x.PayrollId == input.PayrollId)
                                    .WhereIf(input.EmployeeIds != null && input.EmployeeIds.Count > 0, x => input.EmployeeIds.Contains(x.EmployeeId))
                                    .ToList();
                deletedPayslipId = deletePayslips.First().Id;

                _payslipManager.DeletePayslips(input.PayrollId, input.EmployeeIds);
            });

            WithUnitOfWork(async () =>
            {
                var payslips = _workScopeRoot.GetAll<Payslip>();
                var payslipDetails = _workScopeRoot.GetAll<PayslipDetail>();
                var payslipSalaries = _workScopeRoot.GetAll<PayslipSalary>();
                var debtPaids = _workScopeRoot.GetAll<DebtPaid>();
                var payslipTeams = _workScopeRoot.GetAll<PayslipTeam>();
                var payslip = await payslips.AnyAsync(x => x.Id == deletedPayslipId);

                Assert.False(payslip);
                Assert.True(payslips.ToList().Count < allPayslipCountRoot);
                Assert.True(payslipDetails.ToList().Count < allpayslipDetailsCountRoot);
                Assert.True(payslipSalaries.ToList().Count < allpayslipSalariesCountRoot);
                Assert.True(payslipTeams.ToList().Count < allpayslipTeamsCountRoot);
                Assert.True(debtPaids.ToList().Count <= alldebtPaidsCountRoot);
            });
        }

        //TODO test case [should throw exception with invalid/not exits input]

        //[Fact]
        //public void DeletePayslips_Should_Throw_Exception()
        //{
        //    WithUnitOfWork( async () =>
        //    {

        //        var input = new CollectPayslipDto
        //        {
        //            PayrollId = -1,
        //            EmployeeIds = new List<long> { -1, -1 }
        //        };

        //        var result = await Assert.ThrowsAsync<UserFriendlyException>(
        //            async () => _payslipManager.DeletePayslips(input.PayrollId, input.EmployeeIds));

        //        Assert.Equal($"Payslip detail with payrollId = {input.PayrollId} is not exist", result.Message);
        //    });
        //}

        //TODO: test case add vitural in GetOTTimesheets in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void CollectOTTimesheets_HappyTest()
        //{
        //    var valueWithKeyQuang = payslipManagerValue.valueReturnForOTTTimesheetsApi[0].ListOverTimeHour;


        //    var valueWithKeyTung = payslipManagerValue.valueReturnForOTTTimesheetsApi[1].ListOverTimeHour;

        //    WithUnitOfWork(async () =>
        //    {
        //        var result = _payslipManager.CollectOTTimesheets(inputCollectData);

        //        Assert.NotNull(result);
        //        Assert.True(result.Count == 2);
        //        Assert.Contains("quang.levan@ncc.asia", result.Keys);
        //        Assert.Contains("tung.tranvan@ncc.asia", result.Keys);
        //        Assert.Equal(valueWithKeyQuang.Count, result["quang.levan@ncc.asia"].ListOverTimeHour.Count);
        //        Assert.Equal(valueWithKeyQuang.First().OTHour, result["quang.levan@ncc.asia"].ListOverTimeHour.First().OTHour);
        //        Assert.Equal(valueWithKeyTung.Count, result["tung.tranvan@ncc.asia"].ListOverTimeHour.Count);
        //        Assert.Equal(valueWithKeyTung.First().OTHour, result["tung.tranvan@ncc.asia"].ListOverTimeHour.First().OTHour);
        //    });
        //}

        //TODO: test case add vitural in GetSettingOffDates in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void CollectChamCongInfo_HappyTest()
        //{
        //    var OpenTalkDates = payslipManagerValue.valueReturnForChamCongInfoApi.First().OpenTalkDates;
        //    var NormalWorkingDates = payslipManagerValue.valueReturnForChamCongInfoApi.First().NormalWorkingDates;
        //    WithUnitOfWork(async () =>
        //    {
        //        var result = _payslipManager.CollectChamCongInfo(inputCollectData);

        //        Assert.NotNull(result);
        //        Assert.True(result.Count == 1);
        //        Assert.Contains("quang.levan@ncc.asia", result.Keys);
        //        Assert.Equal(OpenTalkDates.Count, result["quang.levan@ncc.asia"].OpenTalkDates.Count);
        //        Assert.Equal(OpenTalkDates.First(), result["quang.levan@ncc.asia"].OpenTalkDates.First());
        //        Assert.Equal(NormalWorkingDates.First(), result["quang.levan@ncc.asia"].NormalWorkingDates.First());
        //    });
        //}

        //TODO: test case add vitural in GetAllRequestDays in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void CollectRequestDates_HappyTest()
        //{
        //    WithUnitOfWork(async () =>
        //    {
        //        var result = _payslipManager.CollectRequestDates(inputCollectData);

        //        Assert.NotNull(result);
        //        Assert.True(result.Count == 1);
        //        Assert.Contains("quang.levan@ncc.asia", result.Keys);
        //        Assert.Equal(1, result["quang.levan@ncc.asia"].OffDateLastMonth.Count);
        //        Assert.Equal(1, result["quang.levan@ncc.asia"].OffDates.Count);
        //        Assert.Equal(2, result["quang.levan@ncc.asia"].WorkAtHomeOnlyDates.Count);
        //    });
        //}

        [Fact]
        public void Delete_HappyCase()
        {
            var inputId = 57333;
            var countBeforDelete = 0;

            WithUnitOfWork( async () =>
            {
                countBeforDelete = _workScopeRoot.GetAll<Payslip>().Count();

                long result = await _payslipManager.Delete(inputId);

                Assert.Equal(inputId, result);
            });

            WithUnitOfWork(async () =>
            {
                var payslips = _workScopeRoot.GetAll<Payslip>();
                var payslip = await payslips.AnyAsync(x => x.Id == inputId);

                Assert.False(payslip);
                Assert.True(payslips.Count() == countBeforDelete - 1);
            });
        }

        [Fact]
        public void Delete_Should_Throw_ExceptionNotExits()
        {
            var inputId = -1;
            WithUnitOfWork(async () =>
            {
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.Delete(inputId));

                Assert.Equal($"Can't find payslip with id {inputId}", result.Message);
            });
        }

        [Fact]
        public void CollectPunishment_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                DateTime input = new DateTime(2022, 12, 12);
                PunishmentEmployee punishmentEmployeeShouldReceived = _workScopeRoot.GetAll<PunishmentEmployee>()
                                     .Where(x => x.Punishment.Date.Month == input.Month)
                                     .Where(x => x.Punishment.Date.Year == input.Year)
                                     .First();

                Dictionary<long, List<InputPayslipDetailDto>> result = _payslipManager.CollectPunishment(input);

                Assert.NotNull(result);
                var employeeFirstInListTest = result[punishmentEmployeeShouldReceived.EmployeeId].First();
                Assert.Equal(employeeFirstInListTest.EmployeeId, punishmentEmployeeShouldReceived.EmployeeId);
                Assert.Equal(employeeFirstInListTest.Note, punishmentEmployeeShouldReceived.Note);
                Assert.Equal(employeeFirstInListTest.Money, punishmentEmployeeShouldReceived.Money);
                Assert.Equal(employeeFirstInListTest.ReferenceId, punishmentEmployeeShouldReceived.Id);
            });
        }

        [Fact]
        public void CollectRefund_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                DateTime inputPayrollApplyDate = new DateTime(2022, 12, 12);
                RefundEmployee refundEmployee = _workScopeRoot.GetAll<RefundEmployee>()
                                        .Where(x => x.Refund.Date.Month == inputPayrollApplyDate.Month)
                                        .Where(x => x.Refund.Date.Year == inputPayrollApplyDate.Year)
                                         .First();

                Dictionary<long, List<InputPayslipDetailDto>> result = _payslipManager.CollectRefund(inputPayrollApplyDate);

                Assert.NotNull(result);
                Assert.True(result.ContainsKey(refundEmployee.EmployeeId));
                var employeeFirstInListTest = result[refundEmployee.EmployeeId].First();
                Assert.Equal(employeeFirstInListTest.EmployeeId, refundEmployee.EmployeeId);
                Assert.Equal(employeeFirstInListTest.Note, refundEmployee.Note);
                Assert.Equal(employeeFirstInListTest.Money, refundEmployee.Money);
                Assert.Equal(employeeFirstInListTest.ReferenceId, refundEmployee.RefundId);
            });
        }

        [Fact]
        public void CollectBonus_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                DateTime inputPayrollApplyDate = new DateTime(2022, 12, 15, 13, 0, 0);
                BonusEmployee bonusEmployee = _workScopeRoot.GetAll<BonusEmployee>()
                                        .Where(x => x.Bonus.ApplyMonth.Month == inputPayrollApplyDate.Month)
                                        .Where(x => x.Bonus.ApplyMonth.Year == inputPayrollApplyDate.Year)
                                         .First();

                Dictionary<long, List<InputPayslipDetailDto>> result = _payslipManager.CollectBonus(inputPayrollApplyDate);

                Assert.NotNull(result);
                Assert.True(result.ContainsKey(bonusEmployee.EmployeeId));
                var employeeFirstInListTest = result[bonusEmployee.EmployeeId].First();
                Assert.Equal(employeeFirstInListTest.EmployeeId, bonusEmployee.EmployeeId);
                Assert.Equal(employeeFirstInListTest.Note, bonusEmployee.Note);
                Assert.Equal(employeeFirstInListTest.Money, bonusEmployee.Money);
                Assert.Equal(employeeFirstInListTest.ReferenceId, bonusEmployee.Id);
            });
        }

        [Fact]
        public async Task CollectBenefit_HappyTest()
        {
            WithUnitOfWork(async () =>
            {

                DateTime firstDayOfPayroll = new DateTime(2022, 10, 20);
                DateTime lastDayOfPayroll = new DateTime(2022, 11, 2);

                Dictionary<long, List<CollectBenefitForPayslipDetailDto>> result = _payslipManager.CollectBenefit(firstDayOfPayroll, lastDayOfPayroll);

                Assert.True(result.Count > 0);
                BenefitEmployee benefitEmployee = _workScopeRoot.GetAll<BenefitEmployee>()
                                        .Where(x => x.Benefit.IsActive)
                                        .Where(x => x.Benefit.Type == BenefitType.CheDoChung
                                        || (x.StartDate.Date <= lastDayOfPayroll && (x.EndDate == null || x.EndDate >= firstDayOfPayroll)))
                                        .First();
                Assert.True(result.ContainsKey(benefitEmployee.EmployeeId));
                var employeeFirstInListTest = result[benefitEmployee.EmployeeId].First();
                Assert.Equal(employeeFirstInListTest.ReferenceId, benefitEmployee.Id);
                Assert.Equal(employeeFirstInListTest.EmployeeId, benefitEmployee.EmployeeId);
                Assert.Equal(employeeFirstInListTest.Note, benefitEmployee.Benefit.Name);
                Assert.Equal(employeeFirstInListTest.Money, benefitEmployee.Benefit.Money);
                Assert.Equal(employeeFirstInListTest.BenefitType, benefitEmployee.Benefit.Type);
                Assert.Equal(employeeFirstInListTest.StartDate, benefitEmployee.StartDate);
                Assert.Equal(employeeFirstInListTest.EndDate, benefitEmployee.EndDate != null ? benefitEmployee.EndDate : lastDayOfPayroll);
            });
        }

        [Fact]
        public async Task CollectDebtPlan_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                DateTime payrollApplyDate = new DateTime(2023, 1, 2);

                Dictionary<long, List<InputPayslipDetailDto>> result = _payslipManager.CollectDebtPlan(payrollApplyDate);

                Assert.True(result.Count > 0);
                DebtPaymentPlan debtPaymentPlan = _workScopeRoot.GetAll<DebtPaymentPlan>()
                                            .Where(x => x.Date.Month == payrollApplyDate.Month)
                                            .Where(x => x.Date.Year == payrollApplyDate.Year)
                                            .Where(x => x.Debt.Status == DebtStatus.Inprogress)
                                            .Where(x => x.Debt.PaymentType == DebtPaymentType.Salary)
                                            .Where(x => x.Money > 0)
                                            .First();
                Debt debt = debtPaymentPlan.Debt;
                Assert.True(result.ContainsKey(debt.EmployeeId));
                var employeeFirstInListTest = result[debt.EmployeeId].First();
                Assert.Equal(employeeFirstInListTest.Note, 
                    $"Trừ lương hàng tháng vào khoản vay {debt.Money}đ, lãi suất {debt.InterestRate}%, từ {DateTimeUtils.ToString(debt.StartDate)} - {DateTimeUtils.ToString(debt.EndDate)}");
                Assert.Equal(employeeFirstInListTest.Money, debtPaymentPlan.Money);
                Assert.Equal(employeeFirstInListTest.ReferenceId, debtPaymentPlan.Id);
            });
        }

        [Fact]
        public void CollectPayslipEmployee_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                DateTime firstDayOfPayroll = new DateTime(2022, 1, 1);
                DateTime lastDayOfPayroll = new DateTime(2022, 12, 20);
                List<long> forEmployeeIds = new List<long> { 880, 881 };
                var results = _workScopeRoot.GetAll<EmployeeWorkingHistory>();

                List<EmployeeToCalDto> employees = _payslipManager.CollectPayslipEmployee(firstDayOfPayroll, lastDayOfPayroll, forEmployeeIds);

                Assert.NotNull(employees);
                Assert.Equal(2, employees.Count);
                Assert.Equal(880, employees[1].EmployeeId);
                Assert.Equal("AN.PHAMTHIEN@NCC.ASIA", employees[1].NormalizeEmailAddress);
                Assert.Equal(UserType.Staff, employees[1].UserType);
                Assert.Equal(315, employees[1].LevelId);
                Assert.Equal(94, employees[1].BranchId);
                Assert.Equal(48, employees[1].JobPositionId);
                Assert.Equal(0, employees[1].RemainLeaveDay);
                Assert.Equal(32, employees[1].BankId);
                Assert.Equal("19000585825", employees[1].BankAccountNumber);
                Assert.Equal("Techcombank", employees[1].BankName);
                Assert.Equal(1, employees[1].Teams.Count);
                Assert.Equal(new DateTime(2022, 10, 1), employees[1].DateAt);
                Assert.Equal(EmployeeStatus.Working, employees[1].Status);

                Assert.Equal(881, employees[0].EmployeeId);
                Assert.Equal("BAO.TRANNGOC@NCC.ASIA", employees[0].NormalizeEmailAddress);
                Assert.Equal(UserType.Staff, employees[0].UserType);
                Assert.Equal(319, employees[0].LevelId);
                Assert.Equal(95, employees[0].BranchId);
                Assert.Equal(47, employees[0].JobPositionId);
                Assert.Equal(0, employees[0].RemainLeaveDay);
                Assert.Equal(32, employees[0].BankId);
                Assert.Equal("19500212212", employees[0].BankAccountNumber);
                Assert.Equal("Techcombank", employees[0].BankName);
                Assert.Equal(1, employees[0].Teams.Count);
                Assert.Equal(new DateTime(2022, 10, 1), employees[0].DateAt);
                Assert.Equal(EmployeeStatus.Working, employees[0].Status);

            });
        }

        [Fact]
        public void GetPayslipSalary_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayslipId = 57333;
                List<PayslipSalary> allPayslipSalaryShouldReceived = _workScopeRoot.GetAll<PayslipSalary>()
                                                                .Where(x => x.PayslipId == inputPayslipId).ToList();

                List<PayslipContractSalaryDto> resultTest = _payslipManager.GetPayslipSalary(inputPayslipId);

                Assert.NotNull(resultTest);
                Assert.True(resultTest.Count > 0);
                Assert.Equal(resultTest.Count, allPayslipSalaryShouldReceived.Count);
                PayslipSalary payslipSalaryCheck = allPayslipSalaryShouldReceived.First();
                PayslipContractSalaryDto payslipSalaryTest = resultTest.First();
                Assert.Equal(payslipSalaryCheck.Date, payslipSalaryTest.FromDate);
                Assert.Equal(payslipSalaryCheck.Salary, payslipSalaryTest.Salary);
                Assert.Equal(payslipSalaryCheck.Note, payslipSalaryTest.Note);
            });
        }
        
        [Fact]
        public void CollectSalaryInput_HappyTest()
        {
            DateTime firstDayOfPayroll = new DateTime(2022, 9, 1);
            DateTime lastDayOfPayroll = new DateTime(2022, 12, 20);
            List<long> forEmployeeIds = new List<long> { 880, 881 };

            WithUnitOfWork(async () =>
            {
                List<EmployeeToCalDto> employeeList = _payslipManager.CollectPayslipEmployee(firstDayOfPayroll, lastDayOfPayroll, forEmployeeIds);

                Dictionary<long, List<SalaryInputForPayslipDto>> dicSalaryInput = 
                    _payslipManager.CollectSalaryInput(employeeList, firstDayOfPayroll, lastDayOfPayroll);

                Assert.NotNull(dicSalaryInput);
                Assert.True(dicSalaryInput.ContainsKey(880));
                Assert.NotNull(dicSalaryInput[880]);
                Assert.True(dicSalaryInput.ContainsKey(881));
                Assert.NotNull(dicSalaryInput[881]);
            });
        }

        // test GetChangeRequestEmployees
        [Fact]
        public void GetChangeRequestEmployees_HappyTest()
        {

            WithUnitOfWork(async () =>
            {
                List<long> forEmployeeIds = new List<long> { 880, 881 };

                IEnumerable<GetchangeRqEmployeeForPayslip> result = 
                    _payslipManager.GetChangeRequestEmployees(forEmployeeIds);

                int count = 0;
                using (IEnumerator<GetchangeRqEmployeeForPayslip> enumerator = result.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        GetchangeRqEmployeeForPayslip valueCurrentReceived = enumerator.Current;
                        Assert.NotNull(valueCurrentReceived.ApplyDate);
                        Assert.True(valueCurrentReceived.Salary != 0);
                        Assert.NotNull(valueCurrentReceived.Type);
                        Assert.NotNull(valueCurrentReceived.UserType);
                        count++;
                    }
                }
                Assert.NotNull(result);
                Assert.True(count > 0);
            });
        }

        [Fact]
        public void ReCalculateAllPayslipFromDetail_HappyTest()
        {
            long payslipId = 57333;
            WithUnitOfWork(async () =>
            {
                await _payslipManager.ReCalculatePayslipFromDetail(payslipId);
            });

            WithUnitOfWork(async () =>
            {
                Payslip payslip = await _workScopeRoot.GetAsync<Payslip>(payslipId);
                var payslipSalary = _workScopeRoot.GetAll<PayslipDetail>()
                            .Where(s => s.PayslipId == payslipId)
                            .Sum(s => s.Money);
                Assert.Equal(payslipSalary, payslip.Salary);
            });
        }

        [Fact]
        public async Task ReCalculateAllPayslipFromDetail_Should_Throw_Connot_Update_Beacause_Payroll_Status()
        {
            long payslipId = 100;
            Payslip payslipEntity = new Payslip
            {
                Id = payslipId,
                PayrollId = 1,
                Payroll = new Payroll
                {
                    ApplyMonth = DateTime.Now,
                    Status = CommonUtil.ListPayrollStatusCanUpdate.First(),
                    NormalWorkingDay = 0,
                    OpenTalk = 0
                },
                EmployeeId = 1,
                BranchId = 1,
                UserType = Constants.Enum.HRMEnum.UserType.Internship,
                LevelId = 1,
                JobPositionId = 1,
                RemainLeaveDayBefore = 1,
                RemainLeaveDayAfter = 1,
                AddedLeaveDay = 0,
                NormalDay = 0,
                OTHour = 0,
                RefundLeaveDay = 0,
                OpentalkCount = 0,
                WorkAtOfficeOrOnsiteDay = 0,
                OffDay = 0,
                Salary = 0,
                ConfirmStatus = 0,
                ComplainNote = "abc",
                BankAccountNumber = "abc",
                BankName = "abc",
                CreatorUserId = 1,
                LastModifierUserId = 1,
            };

            WithUnitOfWork(async () =>
            {
                long id = await _workScopeRoot.InsertAndGetIdAsync(payslipEntity);

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.ReCalculateAllPayslipFromDetail(id));

                Assert.Equal($"Can't change payslip because the payroll status is {payslipEntity.Payroll.Status}. " +
                    $"You can change payslip if the payroll status is: {CommonUtil.ListPayrollStatusCanUpdateToString}"
                    , result.Message);
            });
        }

        [Fact]
        public void ReCalculatePayslipFromDetail_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                long payslipId = 57333;

                await _payslipManager.ReCalculatePayslipFromDetail(payslipId);

                Payslip payslip = await _workScopeRoot.GetAsync<Payslip>(payslipId);
                var payslipSalary = _workScopeRoot.GetAll<PayslipDetail>()
                            .Where(s => s.PayslipId == payslipId)
                            .Sum(s => s.Money);
                Assert.Equal(payslipSalary, payslip.Salary);
            });
        }

        [Fact]
        public void ReCalculatePayslipFromDetail_Should_Throw_Not_Found()
        {
            long payslipId = 100;
            WithUnitOfWork(async () =>
            {
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.ReCalculatePayslipFromDetail(payslipId));

                Assert.Equal($"Not found payslip with Id {payslipId}", result.Message);
            });
        }

        [Fact]
        public async Task ReCalculatePayslipFromDetail_Should_Throw_Connot_Update_Beacause_Payroll_Status()
        {
            long payslipId = 100;
            Payslip payslipEntity = new Payslip
            {
                Id = payslipId,
                PayrollId = 1,
                Payroll = new Payroll
                {
                    ApplyMonth = DateTime.Now,
                    Status= CommonUtil.ListPayrollStatusCanUpdate.First(),
                    NormalWorkingDay= 0,
                    OpenTalk= 0
                },
                EmployeeId = 1,
                BranchId = 1,
                UserType = Constants.Enum.HRMEnum.UserType.Internship,
                LevelId = 1,
                JobPositionId = 1,
                RemainLeaveDayBefore = 1,
                RemainLeaveDayAfter = 1,
                AddedLeaveDay = 0,
                NormalDay = 0,
                OTHour = 0,
                RefundLeaveDay = 0,
                OpentalkCount = 0,
                WorkAtOfficeOrOnsiteDay = 0,
                OffDay = 0,
                Salary = 0,
                ConfirmStatus = 0,
                ComplainNote = "abc",
                BankAccountNumber = "abc",
                BankName = "abc",
                CreatorUserId = 1,
                LastModifierUserId = 1,
            };
            long id = 0;

            WithUnitOfWork(async () =>
            {
                id = await _workScopeRoot.InsertAndGetIdAsync(payslipEntity);
            });

            WithUnitOfWork(async () =>
            {
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.ReCalculatePayslipFromDetail(id));

                Assert.Equal($"Can't change payslip because the payroll status is {payslipEntity.Payroll.Status}. " +
                    $"You can change payslip if the payroll status is: {CommonUtil.ListPayrollStatusCanUpdateToString}"
                    , result.Message);
            });
        }

        //TODO: test case add vitural in GetSettingOffDates in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void AddEmployeesToPayroll_HappyTest()
        //{
        //    long payrollId = 1;
        //    Payroll payrollEnriry = new Payroll
        //    {
        //        Id = payrollId,
        //        ApplyMonth = new DateTime(2022, 12, 13),
        //        Status= PayrollStatus.PendingKT,
        //        NormalWorkingDay= 8,
        //        OpenTalk= 2
        //    };
        //    long id = 0;

        //    WithUnitOfWork(async () =>
        //    {
        //        id = await _workScopeRoot.InsertAndGetIdAsync(payrollEnriry);
        //    });

        //    WithUnitOfWork(async () =>
        //    {
        //        var input = new CollectPayslipDto
        //        {
        //            PayrollId = payrollId,
        //            EmployeeIds = new List<long> { 880, 881 }
        //        };

        //        GeneratePayslipResultDto result = _payslipManager.AddEmployeesToPayroll(input);

        //        Assert.NotNull(result);
        //        Assert.Null(result.PayslipIds);
        //        Assert.Equal(0, result.ErrorList.Count);
        //    });
        //}

        [Fact]
        public void AddEmployeesToPayroll_Should_Throw_Exception_Dont_Have_Employee_Select()
        {

            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> { }
                };
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.AddEmployeesToPayroll(input));

                Assert.Equal($"You have to select atleast one employee"
                    , result.Message);
            });
        }

        [Fact]
        public void AddEmployeesToPayroll_Should_Catch_Error_To_Reslut()
        {

            WithUnitOfWork(async () =>
            {
                var input = new CollectPayslipDto
                {
                    PayrollId = 146,
                    EmployeeIds = new List<long> {880, 881 }
                };
                var result = _payslipManager.AddEmployeesToPayroll(input);

                Assert.NotNull(result);
                Assert.Null(result.PayslipIds);
                Assert.True(result.ErrorList.Count > 0);
                Assert.Contains("an.phamthien@ncc.asia already in the Payroll", result.ErrorList.Select(x => x.Message).ToList());
                Assert.Contains("bao.tranngoc@ncc.asia already in the Payroll", result.ErrorList.Select(x => x.Message).ToList());
            });
        }

        //TODO: test case add vitural in GetSettingOffDates in TimesheetWebService and open cmt spy data in constructor

        //[Fact]
        //public void ReGeneratePayslip_HappyTest()
        //{
        //    WithUnitOfWork(async () =>
        //    {
        //        long payslipId = 57333;
        //        var allPayslipRaw = _workScopeRoot.GetAll<Payslip>().ToList();

        //        GeneratePayslipResultDto result = _payslipManager.ReGeneratePayslip(payslipId);

        //        Assert.NotNull(result);
        //        Assert.NotNull(result.ErrorList);
        //        Assert.True(result.PayslipIds.Count > 0);
        //        Assert.Equal(result.PayslipIds.First(), allPayslipRaw.Last().Id + 1);
        //    });
        //}

        // test SendMailToAllEmployee
        [Fact]
        public void SendMailToAllEmployee_HappyTest()
        {
            WithUnitOfWork(async () =>
            {
                SendMailAllEmployeeDto input = new SendMailAllEmployeeDto
                {
                    PayrollId = 146,
                    Deadline = new DateTime(2022, 12, 13)
                };
                var listPayslip = _workScopeRoot.GetAll<Payslip>()
                                .Where(x => x.PayrollId == input.PayrollId)
                                .ToList();

                string result = _payslipManager.SendMailToAllEmployee(input);

                Assert.NotNull(result);
                Assert.Contains($"Started sending {listPayslip.Count} email.", result);
            });
        }

        [Fact]
        public void SendMailToOneEmployee_Should_Throw_Not_Found()
        {
            SendMailOneemployeeDto input = new SendMailOneemployeeDto
            {
                PayslipId = 100
            };

            WithUnitOfWork(async () =>
            {
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () =>  _payslipManager.SendMailToOneEmployee(input));

                Assert.Equal($"Can't find payslip with id {input.PayslipId}", result.Message);
            });
        }

        [Fact]
        public void GetPayslipMailTemplate_HappyTest()
        {
            long payslipId = 57333;

            WithUnitOfWork(async () =>
            {
                GetPayslipMailContentDto result = _payslipManager.GetPayslipMailTemplate(payslipId);

                Assert.NotNull(result);
                Assert.Null(result.Deadline);
                Assert.NotNull(result.MailInfo);
                var mailInforReceived = result.MailInfo;
                Assert.True(result.MailInfo.Subject.Length > 1);
                Assert.Equal("an.phamthien@ncc.asia", mailInforReceived.SendToEmail);
                Assert.Equal(TemplateType.Mail, mailInforReceived.TemplateType);
                Assert.Equal(MailFuncEnum.Payslip, mailInforReceived.MailFuncType);
            });
        }

        [Fact]
        public void UpdatePayslipDeadline_HappyTest()
        {
            UpdateDeadlineDto input = new UpdateDeadlineDto
            {
                PayslipId = 57333,
                Deadline = new DateTime(2022, 12, 13)
            };

            WithUnitOfWork(async () =>
            {
                UpdateDeadlineDto result = await _payslipManager.UpdatePayslipDeadline(input);

                Assert.Equal(input.PayslipId, result.PayslipId);
                Assert.Equal(input.Deadline, result.Deadline);
            });

            WithUnitOfWork(async () =>
            {
                Payslip newPayslip = await _workScopeRoot.GetAsync<Payslip>(input.PayslipId);

                Assert.Equal(input.PayslipId, newPayslip.Id);
                Assert.Equal(input.Deadline, newPayslip.ComplainDeadline);
            });
        }

        [Fact]
        public void UpdatePayslipDeadline_Should_Throw_Not_Found()
        {
            UpdateDeadlineDto input = new UpdateDeadlineDto
            {
                PayslipId = -100,
                Deadline = DateTime.Now
            };

            WithUnitOfWork(async () =>
            {
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => await _payslipManager.UpdatePayslipDeadline(input));

                Assert.True(result is UserFriendlyException);
                Assert.Equal($"Can't find payslip with id {input.PayslipId}", result.Message);
            });
        }

        //TODO: test case create file \test\HRMv2.Core.Tests\bin\Debug\net6.0\wwwroot\template\Payslip-for-tech.xlsx

        //[Fact]
        //public void ExportTechcombank_HappyTest()
        //{
        //    WithUnitOfWork(async () =>
        //    {
        //        var techcombankId = _workScopeRoot.GetAll<Bank>()
        //             .Where(x => x.Code.ToLower().Trim() == "techcombank")
        //             .Select(x => x.Id)
        //             .FirstOrDefault();
        //        long payrollId = _workScopeRoot.GetAll<Payslip>()
        //            .Where(x => x.Salary > 0)
        //            .Where(x => x.BankId == techcombankId)
        //            .Select(x => x.PayrollId).FirstOrDefault();

        //        var resultFileBase64Dto = _payslipManager.ExportTechcombank(payrollId);

        //        Assert.NotNull(resultFileBase64Dto);
        //        Assert.Equal("Payslips-for-tech", resultFileBase64Dto.FileName);
        //        Assert.True(resultFileBase64Dto.Base64.Length > 0);
        //    });
        //}

        [Fact]
        public void ExportTechcombank_Should_Throw_PayRoll_Dont_Have_Payslip_Is_tech()
        {

            WithUnitOfWork(async () =>
            {
                long payrollId = _workScopeRoot.InsertAndGetId(new Payslip
                {
                    PayrollId = 1,
                    EmployeeId = 1,
                    BranchId = 1,
                    UserType = Constants.Enum.HRMEnum.UserType.Internship,
                    LevelId = 1,
                    JobPositionId = 1,
                    RemainLeaveDayBefore = 1,
                    RemainLeaveDayAfter = 1,
                    AddedLeaveDay = 0,
                    NormalDay = 0,
                    OTHour = 0,
                    RefundLeaveDay = 0,
                    OpentalkCount = 0,
                    WorkAtOfficeOrOnsiteDay = 0,
                    OffDay = 0,
                    Salary = 100,
                    ConfirmStatus = 0,
                    ComplainNote = "abc",
                    BankAccountNumber = "abc",
                    BankId = 35,
                    BankName = "bidv",
                    CreatorUserId = 1,
                    LastModifierUserId = 1,
                });

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.ExportTechcombank(payrollId));

                Assert.True(result is UserFriendlyException);
                Assert.Equal("Payroll don't have any techcombank payslip to export", result.Message);
            });
        }

        //TODO: test case create file \test\HRMv2.Core.Tests\bin\Debug\net6.0\wwwroot\template\Payslip-outside-tech.xlsx

        //[Fact]
        //public void ExportOutsideTech_HappyTest()
        //{

        //    WithUnitOfWork(async () =>
        //    {
        //        var techcombankId = _workScopeRoot.GetAll<Bank>()
        //             .Where(x => x.Code.ToLower().Trim() == "techcombank")
        //             .Select(x => x.Id)
        //             .FirstOrDefault();
        //        long payrollId = _workScopeRoot.GetAll<Payslip>()
        //            .Where(x => x.Salary > 0)
        //            .Where(x => x.BankId != techcombankId)
        //            .Select(x => x.PayrollId).FirstOrDefault();

        //        var resultFileBase64Dto = _payslipManager.ExportOutsideTech(payrollId);

        //        Assert.NotNull(resultFileBase64Dto);
        //        Assert.Equal("Payslip-ouside-tech", resultFileBase64Dto.FileName);
        //        Assert.True(resultFileBase64Dto.Base64.Length > 0);
        //    });
        //}

        [Fact]
        public void ExportOutsideTech_Should_Throw_PayRoll_Dont_Have_Payslip_Outside_Teck()
        {


            WithUnitOfWork(async () =>
            {
                long payrollId = -1;
                
                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                    async () => _payslipManager.ExportOutsideTech(payrollId));

                Assert.True(result is UserFriendlyException);
                Assert.Equal("Payroll don't have payslip to export", result.Message);

            });
        }

        //TODO: test case create file \test\HRMv2.Core.Tests\bin\Debug\net6.0\wwwroot\template\Payroll.xlsx

        //[Fact]
        //public void ExportPayroll_HappyTest()
        //{

        //    WithUnitOfWork(async () =>
        //    {
        //        long payrollId = 146;
        //        InputGetPayslipEmployeeDto inputGetPayslipEmployeeDto = new InputGetPayslipEmployeeDto
        //        {
        //            GridParam = new GridParam()
        //            {
        //                MaxResultCount = 2,
        //                SkipCount = 0,
        //                Sort = ""
        //            },
        //            StatusIds = new List<Constants.Enum.HRMEnum.EmployeeStatus> {
        //                Constants.Enum.HRMEnum.EmployeeStatus.Working
        //            }
        //        };

        //        var resultFileBase64Dto = await _payslipManager.ExportPayroll(payrollId, inputGetPayslipEmployeeDto);

        //        Assert.NotNull(resultFileBase64Dto);
        //        Assert.Equal("Payroll", resultFileBase64Dto.FileName);
        //        Assert.True(resultFileBase64Dto.Base64.Length > 0);
        //    });
        //}

        [Fact]
        public void ExportPayroll_Should_Throw_Payroll_Dont_Have_Payslip()
        {

            WithUnitOfWork(async () =>
            {
                long payrollId = -1;
                InputGetPayslipEmployeeDto inputGetPayslipEmployeeDto = new InputGetPayslipEmployeeDto
                {
                    GridParam = new GridParam(),
                    TeamIds = new List<long>(),
                    IsAndCondition = true,
                    StatusIds = new List<EmployeeStatus>(),
                    LevelIds = new List<long>(),
                    Usertypes = new List<UserType>(),
                    BranchIds = new List<long>(),
                    JobPositionIds = new List<long>()
                };

                var result = await Assert.ThrowsAsync<UserFriendlyException>(
                                    async () => await _payslipManager.ExportPayroll(payrollId, inputGetPayslipEmployeeDto));

                Assert.True(result is UserFriendlyException);
                Assert.Equal("Payroll don't have payslip to export", result.Message);
            });
        }

        //TODO: test case create file \test\HRMv2.Core.Tests\bin\Debug\net6.0\wwwroot\template\PayrollincludeLastMonth.xlsx

        //[Fact]
        //public void ExportPayrollIncludeLastMonth_HappyTest()
        //{

        //    WithUnitOfWork(async () =>
        //    {
        //        long payrollId = 146;

        //        var resultFileBase64Dto = _payslipManager.ExportPayrollIncludeLastMonth(payrollId);

        //        Assert.NotNull(resultFileBase64Dto);
        //        Assert.Equal("Payroll", resultFileBase64Dto.FileName);
        //        Assert.True(resultFileBase64Dto.Base64.Length > 0);
        //    });
        //}

        //TODO: test case [check exception with invalid/not exits payrollid]

        //[Fact]
        //public void ExportPayrollIncludeLastMonth_Should_Throw_Payroll_Dont_Have_Payslip()
        //{

        //    WithUnitOfWork(async () =>
        //    {
        //        long payrollId = 2;

        //        var result = await Assert.ThrowsAsync<UserFriendlyException>(
        //                            async () => _payslipManager.ExportPayrollIncludeLastMonth(payrollId));

        //        Assert.True(result is UserFriendlyException);
        //        Assert.Equal("Payroll don't have payslip to export", result.Message);
        //    });
        //}

        [Fact]
        public async Task TestUpdatePayslipInfo()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UpdatePayslipInfoDto input = new UpdatePayslipInfoDto
                {
                    Id = 57333,
                    RemainLeaveDayBefore = 3,
                    AddedLeaveDay = 3,
                    NormalDay = 23,
                    OpentalkCount = 1,
                    OffDay = 1,
                    OTHour = 0,
                    RefundLeaveDay = (float)1.5,
                    RemainLeaveDayAfter = (float)2.5,
                    NormalSalary = 5000000,
                    OTSalary = 2000000,
                };
                await _payslipManager.UpdatePayslipDetail(input);

                var afterUpdate_payslip = await _workScopeRoot.GetAsync<Payslip>(57333);
                var afterUpdate_payslipDetail = await _workScopeRoot.GetAll<PayslipDetail>().Where(x => x.PayslipId == 57333).ToListAsync();
                var TotalOtherSalary = afterUpdate_payslipDetail.Where(x => x.Type != PayslipDetailType.SalaryOT && x.Type != PayslipDetailType.SalaryNormal).Sum(x => x.Money);

                Assert.Equal(input.NormalDay+input.OpentalkCount*0.5, afterUpdate_payslip.NormalDay+afterUpdate_payslip.OpentalkCount*0.5);
                Assert.Equal(input.NormalSalary + input.OTSalary + TotalOtherSalary, afterUpdate_payslipDetail.Sum(x => x.Money));                

                Assert.Equal(input.RemainLeaveDayBefore, afterUpdate_payslip.RemainLeaveDayBefore);
                Assert.Equal(input.AddedLeaveDay, afterUpdate_payslip.AddedLeaveDay);
                Assert.Equal(input.NormalDay, afterUpdate_payslip.NormalDay);
                Assert.Equal(input.OffDay, afterUpdate_payslip.OffDay);
                Assert.Equal(input.OTHour, afterUpdate_payslip.OTHour);
                Assert.Equal(input.RefundLeaveDay, afterUpdate_payslip.RefundLeaveDay);
                Assert.Equal(input.RemainLeaveDayAfter, afterUpdate_payslip.RemainLeaveDayAfter);
                Assert.Equal(input.NormalSalary, afterUpdate_payslipDetail.Where(x => x.Type == PayslipDetailType.SalaryNormal).Sum(x => x.Money));
                Assert.Equal(input.OTSalary, afterUpdate_payslipDetail.Where(x => x.Type == PayslipDetailType.SalaryOT).Sum(x => x.Money));
            });
        }


        [Fact]
        public void TestGetPayslipInfoBeforeUpdate()
        {
            WithUnitOfWork(async () =>
            {
                long inputPayslipId = 57333;
                var Payslip = await _workScopeRoot.GetAsync<Payslip>(inputPayslipId);
                var PayslipDetail = await _workScopeRoot.GetAll<PayslipDetail>().Where(p => p.PayslipId == inputPayslipId).ToListAsync();

                var resultTest = _payslipManager.GetPayslipBeforeUpdateInfo(inputPayslipId);
                Assert.NotNull(resultTest);
                Assert.Equal(Payslip.Id, resultTest.Id);
                Assert.Equal(Payslip.RemainLeaveDayBefore, resultTest.RemainLeaveDayBefore);
                Assert.Equal(Payslip.AddedLeaveDay, resultTest.AddedLeaveDay);
                Assert.Equal(Payslip.NormalDay, resultTest.NormalDay);
                Assert.Equal(Payslip.OffDay, resultTest.OffDay);
                Assert.Equal(Payslip.OTHour, resultTest.OTHour);
                Assert.Equal(Payslip.RefundLeaveDay, resultTest.RefundLeaveDay);
                Assert.Equal(Payslip.RemainLeaveDayAfter, resultTest.RemainLeaveDayAfter);
                Assert.Equal(PayslipDetail.Where(p=>p.Type== PayslipDetailType.SalaryNormal).Sum(x=>x.Money), resultTest.NormalSalary);
                Assert.Equal(PayslipDetail.Where(p => p.Type == PayslipDetailType.SalaryOT).Sum(x => x.Money), resultTest.OTSalary);
            });
        }
    }

}
