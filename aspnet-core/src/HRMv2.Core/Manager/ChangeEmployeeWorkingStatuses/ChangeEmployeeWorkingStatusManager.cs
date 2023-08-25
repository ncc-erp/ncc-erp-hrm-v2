using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using HRMv2.BackgroundJob;
using HRMv2.BackgroundJob.ChangeWorkingStatusToMaternityLeave;
using HRMv2.BackgroundJob.ChangeWorkingStatusToPause;
using HRMv2.BackgroundJob.ChangeWorkingStatusToQuit;
using HRMv2.BackgroundJob.ChangeWorkingStatusToWorking;
using HRMv2.Configuration;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.SalaryRequests.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.IMS.Dto;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Project.Dto;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Talent.Dto;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.EntityFrameworkCore;
using NccCore.Uitls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;


namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses
{
    public class ChangeEmployeeWorkingStatusManager : BaseManager
    {
        public readonly BenefitManager _benefitManager;
        private readonly ContractManager _contractManager;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<Employee, long> _employeeRepository;
        private readonly IRepository<BenefitEmployee, long> _benefitEmployeeRepository;
        private readonly IRepository<SalaryChangeRequestEmployee, long> _salaryChangeRequestEmployeeRepository;
        private readonly IRepository<EmployeeWorkingHistory, long> _employeeWorkingHistoryRepository;
        private readonly IRepository<EmployeeContract, long> _employeeContractRepository;
        private readonly IRepository<BackgroundJobInfo, long> _storeJob;
        private readonly ProjectService _projectService;
        private readonly TimesheetWebService _timesheetWebService;
        private readonly IMSWebService _iMSWebService;
        private readonly TalentWebService _talentWebService;
        private readonly ISettingManager _settingManager;
        private readonly KomuService _komuService;

        public ChangeEmployeeWorkingStatusManager
            (IWorkScope workScope,
            BenefitManager benefitManager,
            ContractManager contractManager,
            IBackgroundJobManager backgroundJobManager,
            IRepository<Employee, long> employeeRepository,
            IRepository<EmployeeWorkingHistory, long> employeeWorkingHistoryRepository,
            IRepository<SalaryChangeRequestEmployee, long> salaryChangeRequestEmployeeRepository,
            IRepository<BenefitEmployee, long> benefitEmployeeRepository,
            IRepository<BackgroundJobInfo, long> storeJob,
            IRepository<EmployeeContract, long> employeeContractRepository,
            ProjectService projectService,
            TimesheetWebService timesheetWebService,
            IMSWebService iMSWebService,
            TalentWebService talentWebService,
            KomuService komuService,
            ISettingManager settingManager)
            : base(workScope)
        {
            _benefitManager = benefitManager;
            _contractManager = contractManager;
            _backgroundJobManager = backgroundJobManager;
            _employeeRepository = employeeRepository;
            _employeeWorkingHistoryRepository = employeeWorkingHistoryRepository;
            _salaryChangeRequestEmployeeRepository = salaryChangeRequestEmployeeRepository;
            _benefitEmployeeRepository = benefitEmployeeRepository;
            _storeJob = storeJob;
            _employeeContractRepository = employeeContractRepository;
            _projectService = projectService;
            _timesheetWebService = timesheetWebService;
            _iMSWebService = iMSWebService;
            _talentWebService = talentWebService;
            _settingManager = settingManager;
            _komuService = komuService;
        }

        public void ChangeStatusToQuit(ToQuitDto input)
        {

            DeleteOldRequestInBackgroundJob(input.EmployeeId);
            input.CurrentUserLoginId = (long)AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;
            if (input.ApplyDate.Date <= DateTimeUtils.GetNow() || input.IsConfirmed)
            {
                ToQuit(input);
            }
            else
            {
                if (IsAllowPlanToProjectUser(input.EmployeeId))
                {
                    PlanProjectUserQuitJob(input);
                }
                var delayHour = (input.ApplyDate.Date - DateTimeUtils.GetNow()).TotalHours + 5;

                Logger.Info($"ChangeStatusToQuit() ApplyDate: {input.ApplyDate.Date} => DelayHour: {delayHour}");

                _backgroundJobManager.Enqueue<ChangeWorkingStatusToQuit, ToQuitDto>(
                    input, BackgroundJobPriority.High, TimeSpan.FromHours(delayHour));

                var employeeInfo = WorkScope.GetAll<Employee>()
                    .Where(x => x.Id == input.EmployeeId)
                    .Select(x => new
                    {
                        Email = x.Email,
                        BranchName = x.Branch.Name,
                        UserType = x.UserType,
                        PositionName = x.JobPosition.Name,
                        CEOId = x.Branch.CEOId,
                        HRId = x.Branch.HRId
                    }).FirstOrDefault();

                var CEOUserName = WorkScope.GetAll<Employee>()
                    .Where(x => x.Id == employeeInfo.CEOId)
                    .Select(x => x.Email)
                    .FirstOrDefault();

                var HRUserName = WorkScope.GetAll<Employee>()
                    .Where(x => x.Id == employeeInfo.HRId)
                    .Select(x => x.Email)
                    .FirstOrDefault();

                var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuITChannelId);

                var tagCEO = !string.IsNullOrEmpty(CEOUserName) ? $"{CommonUtil.GetDiscordTagUser(CEOUserName)}, " : "";
                var tagHR = !string.IsNullOrEmpty(HRUserName) ? $"{CommonUtil.GetDiscordTagUser(HRUserName)}, " : "";

                var message = $"{tagCEO}{tagHR}HRM plan **{employeeInfo.Email}** {employeeInfo.BranchName} {CommonUtil.GetUserTypeNameVN(employeeInfo.UserType)}" +
                    $" {employeeInfo.PositionName} **Quit job** on {DateTimeUtils.ToString(input.ApplyDate)}";

                _komuService.NotifyToChannel(message, channelId);
            }
        }
        public void ToQuit(ToQuitDto input)
        {
            var employee = WorkScope.GetAll<Employee>()
                    .Where(x => x.Id == input.EmployeeId)
                    .Include(x => x.Branch)
                    .Include(x => x.JobPosition)
                    .FirstOrDefault();

            employee.Status = EmployeeStatus.Quit;
            employee.RealSalary = 0;
            employee.Salary = 0;
            employee.ProbationPercentage = 0;
            _employeeRepository.Update(employee);

            var employeeHistory = new EmployeeWorkingHistory
            {
                EmployeeId = input.EmployeeId,
                Status = EmployeeStatus.Quit,
                DateAt = input.ApplyDate,
                Note = input.Note,
                BackDate = default,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),
            };

            _employeeWorkingHistoryRepository.Insert(employeeHistory);

            if (input.ListCurrentBenefits != null && input.ListCurrentBenefits.Count > 0)
            {
                UpdateEmployeeBenefit(input.ListCurrentBenefits);
            }
            CreateSalaryChangeRequestStatusQuit(input);

            var inputChangeUserWorkingStatusToOtherTool = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate,

            };

            ConfirmUserQuit(inputChangeUserWorkingStatusToOtherTool);

            var CEOUserName = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employee.Branch.CEOId)
                .Select(x => x.Email)
                .FirstOrDefault();

            var HRUserName = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employee.Branch.HRId)
                .Select(x => x.Email)
                .FirstOrDefault();

            var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuITChannelId);
            var tagCEO = !string.IsNullOrEmpty(CEOUserName) ? $"{CommonUtil.GetDiscordTagUser(CEOUserName)}, " : "";
            var tagHR = !string.IsNullOrEmpty(HRUserName) ? $"{CommonUtil.GetDiscordTagUser(HRUserName)}, " : "";
            var message = $"{tagCEO}{tagHR}HRM confirm **{employee.Email}** {employee.Branch.Name} {CommonUtil.GetUserTypeNameVN(employee.UserType)}" +
                    $" {employee.JobPosition.Name} **Quit job** on {DateTimeUtils.ToString(input.ApplyDate)}";

            _komuService.NotifyToChannel(message, channelId);
        }


        public void ChangeStatusToPause(ToPauseDto input)
        {
            DeleteOldRequestInBackgroundJob(input.EmployeeId);
            input.CurrentUserLoginId = (long)AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;
            if (input.ApplyDate.Date <= DateTimeUtils.GetNow() || input.IsConfirmed)
            {
                ToPause(input);
            }
            else
            {
                if (IsAllowPlanToProjectUser(input.EmployeeId))
                {
                    PlanProjectUserPause(input);
                }
               
                var delayHour = (input.ApplyDate.Date - DateTimeUtils.GetNow()).TotalHours + 5;

                Logger.Info($"ChangeStatusToPause() ApplyDate: {input.ApplyDate.Date} => DelayHour: {delayHour}");

                _backgroundJobManager.Enqueue<ChangeWorkingStatusToPause, ToPauseDto>(
                    input, BackgroundJobPriority.High, TimeSpan.FromHours(delayHour));
            }
        }

        public void ToPause(ToPauseDto input)
        {
            var employee = _employeeRepository.Get(input.EmployeeId);
            employee.Status = EmployeeStatus.Pausing;
            employee.RealSalary = 0;
            employee.Salary = 0;
            employee.ProbationPercentage = 0;
            _employeeRepository.Update(employee);
            var employeeHistory = new EmployeeWorkingHistory
            {
                EmployeeId = input.EmployeeId,
                Status = EmployeeStatus.Pausing,
                DateAt = input.ApplyDate,
                BackDate = input.BackDate,
                Note = input.Note,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),

            };

            _employeeWorkingHistoryRepository.Insert(employeeHistory);

            if (input.ListCurrentBenefits != null && input.ListCurrentBenefits.Count > 0)
            {
                UpdateEmployeeBenefit( input.ListCurrentBenefits);
            }

            CreateSalaryChangeRequestStatusPause(input);
            var inputChangeUserWorkingStatusToOtherTool = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate

            };
            ConfirmUserPause(inputChangeUserWorkingStatusToOtherTool);
        }



        public void ChangeStatusToMaternityLeave(ToMaternityLeaveDto input)
        {
            DeleteOldRequestInBackgroundJob(input.EmployeeId);
            input.CurrentUserLoginId = (long)AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;

            if (input.ApplyDate.Date <= DateTimeUtils.GetNow() || input.IsConfirmed)
            {
                ToMaternityLeave(input);
            }
            else
            {
                if (IsAllowPlanToProjectUser(input.EmployeeId))
                {
                    PlanProjectUserMaternityLeave(input);
                }
                
                var delayHour = (input.ApplyDate.Date - DateTimeUtils.GetNow()).TotalHours + 5;

                Logger.Info($"ChangeStatusToMaternityLeave() ApplyDate: {input.ApplyDate.Date} => DelayHour: {delayHour}");

                _backgroundJobManager.Enqueue<ChangeWorkingStatusToMaterinyLeave, ToMaternityLeaveDto>(
                    input, BackgroundJobPriority.High, TimeSpan.FromHours(delayHour));
            }
        }
        public void ToMaternityLeave(ToMaternityLeaveDto input)
        {
            var employee = _employeeRepository.Get(input.EmployeeId);
            employee.Status = EmployeeStatus.MaternityLeave;
            employee.RealSalary = input.ToSalary;
            employee.ProbationPercentage = 100;
            employee.Salary = input.ToSalary;
            _employeeRepository.Update(employee);


            var employeeHistory = new EmployeeWorkingHistory
            {
                EmployeeId = input.EmployeeId,
                Status = EmployeeStatus.MaternityLeave,
                DateAt = input.ApplyDate,
                BackDate = input.BackDate,
                Note = input.Note,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),
            };
            _employeeWorkingHistoryRepository.Insert(employeeHistory);

            if (input.ListCurrentBenefits != null && input.ListCurrentBenefits.Count > 0)
            {
                UpdateEmployeeBenefit( input.ListCurrentBenefits);
            }

            CreateSalaryChangeRequestStatusMaternityLeave(input);
            var inputChangeUserWorkingStatusToOtherTool = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate,

            };
            ConfirmUserMaternityLeave(inputChangeUserWorkingStatusToOtherTool);
        }
        public void ChangeStatusToWorking(ToWorkingDto input)
        {
            CheckChangeStatusToWorking(input);

            DeleteOldRequestInBackgroundJob(input.EmployeeId);
            input.CurrentUserLoginId = (long)AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;

            if (input.ApplyDate.Date <= DateTimeUtils.GetNow() || input.IsConfirmed)
            {
                ToWorking(input);
            }
            else
            {
                var delayHour = (input.ApplyDate.Date - DateTimeUtils.GetNow()).TotalHours + 5;

                Logger.Info($"ChangeStatusToWorking() ApplyDate: {input.ApplyDate.Date} => DelayHour: {delayHour}");

                _backgroundJobManager.Enqueue<ChangeWorkingStatusToWorking, ToWorkingDto>(
                    input, BackgroundJobPriority.High, TimeSpan.FromHours(delayHour));
            }

        }
        public void ToWorking(ToWorkingDto input)
        {
            var isChange = HasToCreateChangeRequestWhenBackToWork(input);

            if (isChange)
            {
                CreateSalaryChangeRequestStatusWorking(input);
            }

            var employee = _employeeRepository.Get(input.EmployeeId);

            employee.Status = EmployeeStatus.Working;
            employee.RealSalary = input.RealSalary;
            employee.LevelId = input.ToLevelId;
            employee.UserType = input.ToUserType;
            employee.JobPositionId = input.ToJobPositionId;
            employee.ProbationPercentage = input.ProbationPercentage;
            employee.Salary = input.BasicSalary;
            employee.LastModificationTime = DateTimeUtils.GetNow();

            _employeeRepository.Update(employee);

            var employeeHistory = new EmployeeWorkingHistory
            {
                EmployeeId = input.EmployeeId,
                Status = EmployeeStatus.Working,
                DateAt = input.ApplyDate,
                Note = input.Note,
                BackDate = default,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId, 
                LastModificationTime = DateTimeUtils.GetNow(),
            };
            _employeeWorkingHistoryRepository.Insert(employeeHistory);

            if (input.ListCurrentBenefits != null && input.ListCurrentBenefits.Count > 0)
            {
                UpdateEmployeeBenefit(input.ListCurrentBenefits);
            }

            var inputChangeUserWorkingStatusToOtherTool = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate,

            };

            ConfirmUserBackToWork(inputChangeUserWorkingStatusToOtherTool);
        }

        public void CheckChangeStatusToWorking(ToWorkingDto input)
        {
            var currentStatus = _employeeWorkingHistoryRepository.GetAll()
                .Where(x => x.EmployeeId == input.EmployeeId)
                .Where(x => x.DateAt.Month == input.ApplyDate.Month && x.DateAt.Year == input.ApplyDate.Year)
                .Select(x => new
                {
                    x.Status,
                    x.DateAt
                })
                .OrderBy(x => x.DateAt)
                .LastOrDefault();


            if (currentStatus != default)
            {
                throw new UserFriendlyException($"Can't not change working status from {currentStatus.Status} ({DateTimeUtils.ToString(currentStatus.DateAt)}) to Working in the same month." +
                    $" Please delete the lastest working status history");
            }
        }

        public EmployeeInfoDto GetEmployeeBasicInfo(long employeeId)
        {
            var employee = _employeeRepository.GetAll()
                .Where(x => x.Id == employeeId)
                .Select(x => new EmployeeInfoDto
                {
                    EmployeeId = x.Id,
                    Status = x.Status,
                    JobPositionId = x.JobPositionId,
                    LevelId = x.LevelId,
                    UserType = x.UserType,
                    Salary = x.RealSalary
                }).FirstOrDefault();
            return employee;

        }

        public bool HasToCreateChangeRequestWhenBackToWork(ToWorkingDto input)
        {
            var salarychangeRequest = _salaryChangeRequestEmployeeRepository.GetAll()
                    .Where(x => x.EmployeeId == input.EmployeeId)
                    .Where(x => x.Type != SalaryRequestType.Change || x.SalaryChangeRequest.Status == SalaryRequestStatus.Executed)
                    .OrderByDescending(x => x.ApplyDate)
                    .ThenByDescending(x => x.CreationTime)
                    .FirstOrDefault();

            if (salarychangeRequest == default)
            {
                throw new UserFriendlyException("Not found SalaryChangeRequest of EmployeeId: " + input.EmployeeId);
            }

            var employee = _employeeRepository.Get(input.EmployeeId);

            var empContract = _employeeContractRepository.GetAll()
                .Where(x => x.SalaryRequestEmployeeId == salarychangeRequest.Id)
                .FirstOrDefault();

            if (employee.JobPositionId != input.ToJobPositionId ||
                    employee.UserType != input.ToUserType ||
                    employee.LevelId != input.ToLevelId ||
                    employee.ProbationPercentage != input.ProbationPercentage
               )

            {
                return true;
            }

            if (salarychangeRequest.HasContract != input.HasContract 
                || salarychangeRequest.ToSalary != input.RealSalary
                || salarychangeRequest.ApplyDate.Date != input.ApplyDate.Date) return true;

            if (empContract != null && empContract.EndDate != input.ContractEndDate) return true;

            return false;

        }

        public void UpdateEmployeeBenefit(List<GetBenefitsOfEmployeeDto> benefits)
        {
            var Ids = benefits.Select(s => s.Id);
            var entities = _benefitEmployeeRepository.GetAll().Where(s => Ids.Contains(s.Id)).ToList();

            foreach (var entity in entities)
            {
                var input = benefits.Where(s => s.Id == entity.Id).FirstOrDefault();
                if (input != default)
                {
                    entity.StartDate = input.StartDate;
                    entity.EndDate = input.EndDate;
                    entity.LastModificationTime = DateTimeUtils.GetNow();
                    _benefitEmployeeRepository.Update(entity);
                }
            }
        }



        public void CreateSalaryChangeRequestStatusWorking(ToWorkingDto input)
        {
            var employee = GetEmployeeBasicInfo(input.EmployeeId);

            var employeeWorkingStatus = new SalaryChangeRequestEmployee
            {
                Type = SalaryRequestType.BackToWork,
                EmployeeId = input.EmployeeId,
                ToUserType = input.ToUserType,
                ToLevelId = input.ToLevelId,
                FromUserType = employee.UserType,
                LevelId = employee.LevelId,
                ApplyDate = input.ApplyDate,
                ToSalary = input.RealSalary,
                Salary = employee.Salary,
                ToJobPositionId = input.ToJobPositionId,
                JobPositionId = employee.JobPositionId,
                HasContract = input.HasContract,
                TenantId = input.TenantId,
                Note = Enum.GetName(typeof(SalaryRequestType), SalaryRequestType.BackToWork),
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),

            };

            var requestId = _salaryChangeRequestEmployeeRepository.InsertAndGetId(employeeWorkingStatus);

            if (input.HasContract)
            {
                var createContractInput = new AddOrUpdateEmployeeRequestDto
                {
                    EmployeeId = input.EmployeeId,
                    HasContract = input.HasContract,
                    ToLevelId = input.ToLevelId,
                    ToJobPositionId = input.ToJobPositionId,
                    ToUserType = input.ToUserType,
                    BasicSalary = input.BasicSalary,
                    ToSalary = input.RealSalary,
                    ProbationPercentage = input.ProbationPercentage,
                    ApplyDate = input.ApplyDate,
                    ContractEndDate = input.ContractEndDate,
                    Id = requestId,
                    TenantId = input.TenantId,
                };
                _contractManager.CreateContractBySalaryRequest(createContractInput);
            }
        }


        public void CreateSalaryChangeRequestStatusMaternityLeave(ToMaternityLeaveDto input)
        {

            var employee = GetEmployeeBasicInfo(input.EmployeeId);
            var employeeWorkingStatus = new SalaryChangeRequestEmployee
            {
                Type = SalaryRequestType.MaternityLeave,
                EmployeeId = input.EmployeeId,
                ApplyDate = input.ApplyDate,
                FromUserType = employee.UserType,
                LevelId = employee.LevelId,
                Salary = employee.Salary,
                ToSalary = input.ToSalary,
                ToJobPositionId = employee.JobPositionId,
                ToUserType = employee.UserType,
                ToLevelId = employee.LevelId,
                JobPositionId = employee.JobPositionId,
                TenantId = input.TenantId,
                Note = Enum.GetName(typeof(SalaryRequestType), SalaryRequestType.MaternityLeave),
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),
            };

            _salaryChangeRequestEmployeeRepository.Insert(employeeWorkingStatus);
        }

        public void CreateSalaryChangeRequestStatusQuit(ToQuitDto input)
        {
            var employee = GetEmployeeBasicInfo(input.EmployeeId);
            var employeeWorkingStatus = new SalaryChangeRequestEmployee
            {
                Type = SalaryRequestType.StopWorking,
                EmployeeId = input.EmployeeId,
                ApplyDate = input.ApplyDate,
                FromUserType = employee.UserType,
                LevelId = employee.LevelId,
                Salary = employee.Salary,
                ToSalary = 0,
                ToJobPositionId = employee.JobPositionId,
                ToUserType = employee.UserType,
                ToLevelId = employee.LevelId,
                JobPositionId = employee.JobPositionId,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),
            };

            _salaryChangeRequestEmployeeRepository.Insert(employeeWorkingStatus);
        }

        public void CreateSalaryChangeRequestStatusPause(ToPauseDto input)
        {
            var employee = GetEmployeeBasicInfo(input.EmployeeId);
            var employeeWorkingStatus = new SalaryChangeRequestEmployee
            {
                Type = SalaryRequestType.StopWorking,
                EmployeeId = input.EmployeeId,
                ApplyDate = input.ApplyDate,
                FromUserType = employee.UserType,
                LevelId = employee.LevelId,
                Salary = employee.Salary,
                ToSalary = 0,
                ToJobPositionId = employee.JobPositionId,
                ToUserType = employee.UserType,
                ToLevelId = employee.LevelId,
                JobPositionId = employee.JobPositionId,
                TenantId = input.TenantId,
                LastModifierUserId = input.CurrentUserLoginId,
                LastModificationTime = DateTimeUtils.GetNow(),
            };

            _salaryChangeRequestEmployeeRepository.Insert(employeeWorkingStatus);
        }


        public void ExtendManternityLeave(ExtendWorkingStatusDto input)
        {
            ExtendWorkingStatus(input);

        }

        public void ExtendPausing(ExtendWorkingStatusDto input)
        {
            ExtendWorkingStatus(input);

        }


        public void ExtendWorkingStatus(ExtendWorkingStatusDto input)
        {

            var employeeHistory = _employeeWorkingHistoryRepository.GetAll()
                .Where(x => x.EmployeeId == input.EmployeeId)
                .OrderByDescending(x => x.DateAt)
                .FirstOrDefault();

           
            if (employeeHistory != null)
            {
                employeeHistory.BackDate = input.BackDate;
                employeeHistory.Note = input.Note;
                _employeeWorkingHistoryRepository.Update(employeeHistory);
            }


            if (input.ListCurrentBenefits != null && input.ListCurrentBenefits.Count > 0)
            {
                UpdateEmployeeBenefit(input.ListCurrentBenefits);
            }
        }

      

        public void DeleteOldRequestInBackgroundJob(long employeeId)
        {
            var jobTypeNameOfRequestToQuit = typeof(ChangeWorkingStatusToQuit).FullName;
            var jobTypeOfRequestToPause = typeof(ChangeWorkingStatusToPause).FullName;
            var jobTypeOfRequestToMaternity = typeof(ChangeWorkingStatusToMaterinyLeave).FullName;
            var jobTypeOfRequestToWorking = typeof(ChangeWorkingStatusToWorking).FullName;

            var filterEmployee = $"\"EmployeeId\":{employeeId},";
           _storeJob.GetAll()
                .Where(s => s.JobType.Contains(jobTypeNameOfRequestToQuit)
                 || s.JobType.Contains(jobTypeOfRequestToPause)
                 || s.JobType.Contains(jobTypeOfRequestToMaternity)
                 || s.JobType.Contains(jobTypeOfRequestToWorking))
                .Where(s => s.JobArgs.Contains(filterEmployee))
                .Select(s => s.Id)
                .ToList().ForEach(id => _backgroundJobManager.Delete(id.ToString()));

        }

        public GetLatestSalaryChangeRequestDto GetLatestSalaryChangeRequest(long employeeId)
        {
            var requestEmp = new GetLatestSalaryChangeRequestDto();

            var lastReq = _salaryChangeRequestEmployeeRepository.GetAll()
                    .Where(x => x.EmployeeId == employeeId)
                    .OrderByDescending(x => x.ApplyDate)
                    .ThenByDescending(x => x.CreationTime)
                    .FirstOrDefault();


            var employee = _employeeRepository.Get(employeeId);

            if (lastReq != null)
            {
                requestEmp.ApplyDate = lastReq.ApplyDate;
                requestEmp.ToLevelId = lastReq.ToLevelId;
                requestEmp.ToJobPositionId = lastReq.ToJobPositionId;
                requestEmp.ToLevelId = lastReq.ToLevelId;
                requestEmp.ToUserType = lastReq.ToUserType;
                requestEmp.HasContract = lastReq.HasContract;
                requestEmp.RealSalary = lastReq.ToSalary;
                requestEmp.ProbationPercentage = lastReq.Type == SalaryRequestType.MaternityLeave ? 100 : employee.ProbationPercentage;

                var empContract = _employeeContractRepository.GetAll()
                .Where(x => x.SalaryRequestEmployeeId == lastReq.Id)
                .FirstOrDefault();
                if (empContract != null) requestEmp.ContractEndDate = empContract.EndDate;
            }
            else
            {
                requestEmp = null;
            }

            return requestEmp;

        }

        private string GetEmployeeEmailById(long employeeId)
        {
            var employeeEmail = _employeeRepository.GetAll()
                .Where(x=> x.Id == employeeId)
                .Select(x=> x.Email)
                .FirstOrDefault();
            return employeeEmail;
        }

        public void ConfirmUserBackToWork(InputToUpdateUserStatusDto input)
        {

            _projectService.ConfirmUserBackToWork(input);
            
            _timesheetWebService.ConfirmUserBackToWork(input);
           
            _iMSWebService.ConfirmUserBackToWork(input);

           
            _talentWebService.ConfirmUserBackToWork(input);
            
        }

        public void ConfirmUserQuit(InputToUpdateUserStatusDto input)
        {
           
            _projectService.ConfirmUserQuit(input);
            
            _timesheetWebService.ConfirmUserQuit(input);

            _iMSWebService.ConfirmUserQuit(input);

            _talentWebService.ConfirmUserQuit(input);
        }

        public void ConfirmUserPause(InputToUpdateUserStatusDto input)
        {

            _projectService.ConfirmUserPause(input);

            _timesheetWebService.ConfirmUserPause(input);

            _iMSWebService.ConfirmUserPause(input);
  
            _talentWebService.ConfirmUserPause(input);
        }

        public void ConfirmUserMaternityLeave(InputToUpdateUserStatusDto input)
        {
 
            _projectService.ConfirmUserMaternityLeave(input);

            _timesheetWebService.ConfirmUserMaternityLeave(input);
    
            _iMSWebService.ConfirmUserMaternityLeave(input);
  
            _talentWebService.ConfirmUserMaternityLeave(input);
        }

        public void PlanProjectUserQuitJob(ToQuitDto input)
        {
            var inputToUpdate = new UpdateProjectUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate
            };
            _projectService.PlanUserQuitJob(inputToUpdate);
        }

        public void PlanProjectUserMaternityLeave(ToMaternityLeaveDto input)
        {
            var inputToUpdate = new UpdateProjectUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate
            };

            _projectService.PlanUserMaternityLeave(inputToUpdate);
        }

        public void PlanProjectUserPause(ToPauseDto input)
        {
            var inputToUpdate = new UpdateProjectUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.EmployeeId),
                DateAt = input.ApplyDate
            };
            _projectService.PlanUserPause(inputToUpdate);
        }

        public bool IsAllowPlanToProjectUser(long employeeId)
        {
            var employee = _employeeRepository.GetAll()
                .Where(x => x.Id == employeeId)
                .FirstOrDefault();
            if(employee != default && employee.Status == EmployeeStatus.Working)
            {
                return true;
            }
            return false;
        }

        


    }
}
