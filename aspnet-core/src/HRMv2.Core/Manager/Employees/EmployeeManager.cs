using Abp.UI;
using HRMv2.Constants;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.EmployeeContracts.Dto;
using HRMv2.Manager.Employees.Dto;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Timesheet;
using HRMv2.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMv2.UploadFileServices;
using HRMv2.Manager.SalaryRequests;
using HRMv2.Manager.SalaryRequests.Dto;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.NccCore;
using ClosedXML.Excel;
using System.IO;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Histories.Dto;
using HRMv2.WebServices.Timesheet.Dto;
using HRMv2.WebServices.Project.Dto;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.IMS.Dto;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using HRMv2.Constants.Enum;
using Abp.Extensions;
using Castle.Core.Internal;
using HRMv2.Net.MimeTypes;
using HRMv2.WebServices.Talent.Dto;
using Abp.BackgroundJobs;
using HRMv2.BackgroundJob.ChangeEmployeeBranch;
using Abp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NccCore.Helper;
using System.Threading;

namespace HRMv2.Manager.Employees
{
    public class EmployeeManager : BaseManager
    {
        private readonly ContractManager _contractManager;
        private readonly ProjectService _projectService;
        private readonly TimesheetWebService _timesheetService;
        private readonly TalentWebService _talentWebService;
        private readonly IMSWebService _IMSWebService;
        private readonly SalaryRequestManager _salaryRequestManager;
        private readonly HistoryManager _historyManager;
        private UploadFileService _uploadFileService;
        private readonly ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatusManager;
        private readonly string templateFolder = Path.Combine("wwwroot", "template");
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<BackgroundJobInfo, long> _storeJob;
        private readonly ILogger<EmployeeManager> Logger;

        public EmployeeManager(UploadFileService uploadFileService,
            ContractManager contractManager,
            HistoryManager historyManager,
            ProjectService projectService,
            TimesheetWebService timesheetService,
            IMSWebService iMSWebService,
            TalentWebService talentWebService,
            IWorkScope workScope,
            SalaryRequestManager salaryRequestManager,
            BenefitManager benefitManager,
            UserTypeManager userTypeManager,
            ChangeEmployeeWorkingStatusManager changeEmployeeWorkingStatusManager,
            IBackgroundJobManager backgroundJobManager,
            IRepository<BackgroundJobInfo, long> storeJob,
            ILogger<EmployeeManager> log
            ) : base(workScope)
        {
            _contractManager = contractManager;
            _uploadFileService = uploadFileService;
            _projectService = projectService;
            _timesheetService = timesheetService;
            _talentWebService = talentWebService;
            _IMSWebService = iMSWebService;
            _salaryRequestManager = salaryRequestManager;
            _historyManager = historyManager;
            _changeEmployeeWorkingStatusManager = changeEmployeeWorkingStatusManager;
            _backgroundJobManager = backgroundJobManager;
            _storeJob = storeJob;
            Logger = log;
        }
        public IQueryable<GetEmployeeDto> QueryAllEmployee()
        {
            return WorkScope.GetAll<Employee>()
                .Select(x => new GetEmployeeDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Avatar = x.Avatar,
                    Sex = x.Sex,
                    Email = x.Email,
                    StartWorkingDate = x.StartWorkingDate,
                    JobPositionId = x.JobPositionId,
                    BranchId = x.BranchId,
                    LevelId = x.LevelId,
                    UserType = x.UserType,
                    Status = x.Status,
                    Skills = x.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Branch.Name,
                        Color = x.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Level.Name,
                        Color = x.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.JobPosition.Name,
                        Color = x.JobPosition.Color
                    },
                    Phone = x.Phone,
                    Address = x.Address,
                    PlaceOfPermanent = x.PlaceOfPermanent,
                    Bank = x.BankId.HasValue ? x.Bank.Name : "",
                    BankAccountNumber = x.BankAccountNumber,
                    BankId = x.BankId,
                    Birthday = x.Birthday != null ? x.Birthday.Value : null,
                    IdCard = x.IdCard,
                    InsuranceStatus = x.InsuranceStatus,
                    IssuedBy = x.IssuedBy,
                    IssuedOn = x.IssuedOn != null ? x.IssuedOn.Value : null,
                    RealSalary = x.RealSalary,
                    Salary = x.Salary,
                    ProbationPercentage = x.ProbationPercentage,
                    RemainLeaveDay = x.RemainLeaveDay,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                });
        }

        public IQueryable<GetEmployeeForExportDto> QueryAllEmployeeDetail()
        {

            var employeeContract = WorkScope.GetAll<EmployeeContract>()
                .GroupBy(x => x.EmployeeId)
                .Select(x => new EmployeeContractBasicInfo
                {
                    EmployeeId = x.Key,
                    EndDate = x.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.CreationTime).FirstOrDefault().EndDate,
                    StartDate = x.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.CreationTime).FirstOrDefault().StartDate
                });
            var employee = WorkScope.GetAll<Employee>();

            var query = from x in employee
                        join ec in employeeContract
                        on x.Id equals ec.EmployeeId
                        select new GetEmployeeForExportDto
                        {
                            Id = x.Id,
                            FullName = x.FullName,
                            Avatar = x.Avatar,
                            Sex = x.Sex,
                            Email = x.Email,
                            StartWorkingDate = x.StartWorkingDate,
                            UserType = x.UserType,
                            Status = x.Status,
                            JobPositionId = x.JobPositionId,
                            BranchId = x.BranchId,
                            LevelId = x.LevelId,
                            BranchCode = x.Branch.Code,
                            LevelName = x.Level.Name,
                            JobPositionCode = x.JobPosition.Code,
                            Phone = x.Phone,
                            Address = x.Address,
                            PlaceOfPermanent = x.PlaceOfPermanent,
                            Bank = x.BankId.HasValue ? x.Bank.Name : "",
                            BankAccountNumber = x.BankAccountNumber,
                            BankId = x.BankId,
                            TaxCode = x.TaxCode,
                            Birthday = x.Birthday != null ? x.Birthday.Value : null,
                            IdCard = x.IdCard,
                            InsuranceStatus = x.InsuranceStatus,
                            IssuedBy = x.IssuedBy,
                            IssuedOn = x.IssuedOn != null ? x.IssuedOn.Value : null,
                            RealSalary = x.RealSalary,
                            Salary = x.Salary,
                            ProbationPercentage = x.ProbationPercentage,
                            RemainLeaveDay = x.RemainLeaveDay,
                            ContractStartDate = ec.StartDate,
                            ContractEndDate = ec.EndDate,
                            UpdatedTime = x.LastModificationTime,
                            UpdatedUser = x.LastModifierUser.FullName,
                        };
            return query;
        }

        public List<GetEmployeeDto> GetAll()
        {
            return QueryAllEmployee().ToList();
        }


        public GetEmployeeInfoDto Get(long employeeId)
        {
            var contract = _contractManager.QueryAllContract()
                .AsEnumerable()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefault();

            var employeeInfo = WorkScope.GetAll<Employee>().Where(x => x.Id == employeeId);



            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employeeId)
                .Select(x => new GetEmployeeDetailDto
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bank = x.BankId.HasValue ? x.Bank.Name : "",
                    BankAccountNumber = x.BankAccountNumber,
                    BankId = x.BankId,
                    Birthday = x.Birthday != null ? x.Birthday.Value : null,
                    IdCard = x.IdCard,
                    InsuranceStatus = x.InsuranceStatus,
                    IssuedBy = x.IssuedBy,
                    IssuedOn = x.IssuedOn != null ? x.IssuedOn.Value : null,
                    Phone = x.Phone,
                    ProbationPercentage = x.ProbationPercentage,
                    Salary = x.Salary,
                    RealSalary = x.RealSalary,
                    TaxCode = x.TaxCode,
                    PlaceOfPermanent = x.PlaceOfPermanent,
                    FullName = x.FullName,
                    Avatar = x.Avatar,
                    Sex = x.Sex,
                    Email = x.Email,
                    StartWorkingDate = x.StartWorkingDate,
                    JobPositionId = x.JobPositionId,
                    BranchId = x.BranchId,
                    LevelId = x.LevelId,
                    Status = x.Status,
                    UserType = x.UserType,
                    Skills = x.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Branch.Name,
                        Color = x.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Level.Name,
                        Color = x.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.JobPosition.Name,
                        Color = x.JobPosition.Color
                    },
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    ContractCode = contract != null ? contract.Code : null,
                    ContractStartDate = contract != null ? contract.StartDate : null,
                    ContractEndDate = contract != null ? contract.EndDate : null,
                    RemainLeaveDay = x.RemainLeaveDay,
                    PersonalEmail = x.PersonalEmail

                }).FirstOrDefault();


            bool isAllowEdit = ValidUpdateEmployee(employeeId);
            bool isAllowEditWorkingStatus = IsAllowEditToWorkingStatus(employeeId);
            bool isAllowEditBranch = IsAllowToEditBranch(employeeId);

            return new GetEmployeeInfoDto
            {
                EmployeeInfo = employee,
                IsAllowEdit = isAllowEdit,
                IsAllowEditBranch = isAllowEditBranch,
                IsAllowEditWorkingStatus = isAllowEditWorkingStatus
            };
        }

        public List<GetEmployeeBasicInfoDto> GetAllEmployeeBasicInfo()
        {
            var query = WorkScope.GetAll<Employee>()
                .Select(x => new GetEmployeeBasicInfoDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email
                });
            return query.ToList();
        }

        public GetEmployeeBasicInfoForBreadcrumbDto GetEmployeeBasicInfoForBreadcrumb(long employeeId)
        {
            var query = WorkScope.GetAll<Employee>().Where(x => x.Id == employeeId)
                .Select(x => new GetEmployeeBasicInfoForBreadcrumbDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Avatar = x.Avatar
                }).FirstOrDefault();
            return query;

        }

        public bool ValidUpdateEmployee(long employeeId)
        {
            var employeeRequest = WorkScope.GetAll<SalaryChangeRequestEmployee>()
             .Where(x => x.EmployeeId == employeeId);
            if (employeeRequest.Any(x => x.Type != SalaryRequestType.Initial))
            {
                return false;
            }
            return true;
        }

        public bool IsAllowEditToWorkingStatus(long employeeId)
        {
            var entities = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();

            var qSCRE = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                         .Where(x => x.EmployeeId == employeeId);
            var hasOnlyInitial = !qSCRE.Any(x => x.Type != SalaryRequestType.Initial);

            if (entities != null && entities.Count() == 1 && hasOnlyInitial)
            {
                return true;
            }
            return false;
        }

        public bool IsAllowToEditBranch(long employeeId)
        {
            var entities = WorkScope.GetAll<EmployeeBranchHistory>()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();
            if (entities != null && entities.Count() == 1)
            {
                return true;
            }
            return false;
        }



        public async Task<GridResult<GetEmployeeDto>> GetEmployeeExcept(GetEmployeeToAddDto input)
        {
            var query = QueryAllEmployee()
                .WhereIf(input.AddedEmployeeIds != null && !input.AddedEmployeeIds.IsEmpty(), x => !input.AddedEmployeeIds.Contains(x.Id));

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.Seniority != null)
            {
                var seniority = input.Seniority.GetDate();

                switch (input.Seniority.Comparison)
                {
                    case SeniorityComparision.Equal:

                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        break;

                    case SeniorityComparision.LessThanOrEqual:
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);

                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);

                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);
                        break;

                    case SeniorityComparision.GreaterThanOrEqual:
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != null && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        break;
                }
            }

            if(input.BirthdayFromDate.HasValue  && input.BirthdayToDate.HasValue)
            {
                var fromDate = input.BirthdayFromDate.Value;
                var toDate = input.BirthdayToDate.Value;

                query = query.Where(x => x.Birthday.HasValue)
                    .Where(x => x.Birthday.Value.Month > input.BirthdayFromDate.Value.Month
                    || (x.Birthday.Value.Month == fromDate.Month && x.Birthday.Value.Day >= fromDate.Day))
                    .Where(x => x.Birthday.Value.Month < toDate.Month
                    || (x.Birthday.Value.Month == toDate.Month && x.Birthday.Value.Day <= toDate.Day));
            }
            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }
            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams on employee.Id equals employeeId
                        select employee;

                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;
            query = query.Where(s => employeeIds.Contains(s.Id));
            return await query.GetGridResult(query, input.GridParam);

        }

        public async Task<GridResult<GetEmployeeForExportDto>> GetAllEmployeeForExport(GetEmployeeToAddDto input)
        {
            var query = QueryAllEmployeeDetail()
                .WhereIf(input.AddedEmployeeIds != null && !input.AddedEmployeeIds.IsEmpty(), x => !input.AddedEmployeeIds.Contains(x.Id));

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.Seniority != null)
            {
                var seniority = input.Seniority.GetDate();

                switch (input.Seniority.Comparison)
                {
                    case SeniorityComparision.Equal:

                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority == x.StartWorkingDate.Date);
                        break;

                    case SeniorityComparision.LessThanOrEqual:
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);

                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);

                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.StartWorkingDate != default)
                                .Where(x => seniority <= x.StartWorkingDate.Date || x.UserType != UserType.Staff);
                        break;

                    case SeniorityComparision.GreaterThanOrEqual:
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Day)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.Month)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        if (input.Seniority.SeniorityType == SeniorityFilterType.year)
                            query = query
                                .Where(x => x.UserType == UserType.Staff && x.StartWorkingDate != default)
                                .Where(x => seniority >= x.StartWorkingDate.Date);
                        break;
                }
            }

            if (input.BirthdayFromDate.HasValue && input.BirthdayToDate.HasValue)
            {
                var fromDate = input.BirthdayFromDate.Value;
                var toDate = input.BirthdayToDate.Value;

                query = query.Where(x => x.Birthday.HasValue)
                    .Where(x => x.Birthday.Value.Month > input.BirthdayFromDate.Value.Month
                    || (x.Birthday.Value.Month == fromDate.Month && x.Birthday.Value.Day >= fromDate.Day))
                    .Where(x => x.Birthday.Value.Month < toDate.Month
                    || (x.Birthday.Value.Month == toDate.Month && x.Birthday.Value.Day <= toDate.Day));
            }

            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }
            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams on employee.Id equals employeeId
                        select employee;

                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;
            query = query.Where(s => employeeIds.Contains(s.Id));
            return await query.GetGridResult(query, input.GridParam);

        }


        public async Task<CreateUpdateEmployeeDto> CreateEmployee(CreateUpdateEmployeeDto input, long? tempEmployeeId, bool isValid)
        {
            if (isValid)
            {
                await ValidCreate(input);
            }

            var entity = ObjectMapper.Map<Employee>(input);
            entity.StartWorkingDate = input.ContractStartDate;
            long employeeId = await WorkScope.InsertAndGetIdAsync(entity);
            input.Id = employeeId;

            await AddEmployeeSkills(employeeId, input.Skills);

            await AddEmployeeTeams(employeeId, input.Teams);

            InitWorkingHistory(input);

            InitBranchHistory(input);

            AddToBenefits(employeeId, entity.StartWorkingDate);

            await InitSalaryChangeRequestAndContract(input);

            if (tempEmployeeId != null)
            {
                OnboardTempEmployee(tempEmployeeId);
            }

            CreateOrUpdateToOtherTool(entity, ActionMode.Create);

            return input;
        }


        public void OnboardTempEmployee(long? id)
        {
            var tempEmployee = WorkScope.GetAll<TempEmployeeTalent>()
           .Where(x => x.Id == id)
           .FirstOrDefault();

            if (tempEmployee != default)
            {
                tempEmployee.Status = TalentOnboardStatus.Onboarded;
                WorkScope.UpdateAsync(tempEmployee);

                var dto = new UpdateTalentOnboardDto
                {
                    Email = tempEmployee.PersonalEmail
                };
                _talentWebService.UpdateOnboardStatus(dto);
            }
        }


        public void AddToBenefits(long employeeId, DateTime StartDate)
        {
            var addBenefits = WorkScope.GetAll<Benefit>()
             .Where(x => x.IsBelongToAllEmployee)
             .ToList();

            List<BenefitEmployee> listBenefitToAdd = new();
            foreach (var benefit in addBenefits)
            {
                var benefitEmployee = new BenefitEmployee
                {
                    BenefitId = benefit.Id,
                    EmployeeId = employeeId,
                    StartDate = StartDate,
                    EndDate = null
                };
                listBenefitToAdd.Add(benefitEmployee);
            }
            WorkScope.InsertRangeAsync(listBenefitToAdd);
        }

        public async Task<CreateUpdateEmployeeDto> Update(CreateUpdateEmployeeDto input)
        {
            ValidUpdate(input);
            //them validate, kiem tra nhung truong co thay doi
            var entity = await WorkScope.GetAsync<Employee>(input.Id);
            ObjectMapper.Map(input, entity);

            var qSCRE = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                         .Where(x => x.EmployeeId == entity.Id);
            var hasOnlyInitial = !qSCRE.Any(x => x.Type != SalaryRequestType.Initial);

            if (hasOnlyInitial)
            {
                var requestId = qSCRE.Select(x => x.Id).FirstOrDefault();
                await UpdateInitSalaryRequestAndContract(input, requestId);

                var initWorkingHistory = WorkScope.GetAll<EmployeeWorkingHistory>()
                         .Where(x => x.EmployeeId == entity.Id)
                         .OrderBy(x => x.DateAt)
                         .FirstOrDefault();

                if (input.ContractStartDate.Date != initWorkingHistory.DateAt.Date)
                {

                    initWorkingHistory.DateAt = input.ContractStartDate;

                }

                if (initWorkingHistory.Status != input.Status)
                {
                    initWorkingHistory.Status = input.Status;
                    UpdateEmployeeStatusToOtherTool(input);
                }

                await WorkScope.UpdateAsync(initWorkingHistory);

            }
            else
            {
                _contractManager.UpdateContracEndDate(entity.Id, input.ContractEndDate);
            }

            await WorkScope.UpdateAsync(entity);
            await UpdateEmployeeSkill(entity.Id, input.Skills);
            await UpdateEmployeeTeam(entity.Id, input.Teams);
            await UpdateInitBranchHistory(input);

            CreateOrUpdateToOtherTool(entity, ActionMode.Update);

            return input;
        }

        public void InitWorkingHistory(CreateUpdateEmployeeDto employee)
        {
            var dto = new CreateWorkingHistoryDto
            {
                EmployeeId = employee.Id,
                DateAt = employee.ContractStartDate,
                Status = EmployeeStatus.Working,
                Note = "Inital"
            };
            _historyManager.CreateWorkingHistory(dto);
        }




        public void InitBranchHistory(CreateUpdateEmployeeDto employee)
        {
            var dto = new CreateBranchHistoryDto
            {
                EmployeeId = employee.Id,
                DateAt = employee.ContractStartDate,
                BranchId = employee.BranchId,
                Note = "Inital"
            };
            _historyManager.CreateBranchHistory(dto);
        }

        public async Task UpdateInitBranchHistory(CreateUpdateEmployeeDto input)
        {
            var entities = WorkScope.GetAll<EmployeeBranchHistory>()
                .Where(x => x.EmployeeId == input.Id)
                .ToList();

            if (entities == null || entities.Count != 1)
            {
                return;
            }

            var initialEntity = entities.FirstOrDefault();

            if (initialEntity.DateAt.Date != input.ContractStartDate.Date)
            {
                initialEntity.DateAt = input.ContractStartDate;

            }
            if (initialEntity.BranchId != input.BranchId) initialEntity.BranchId = input.BranchId;

            await WorkScope.UpdateAsync(initialEntity);
        }


        public async Task UpdateInitSalaryRequestAndContract(CreateUpdateEmployeeDto employee, long requestId)
        {
            var salaryRequestEmployeeDto = new AddOrUpdateEmployeeRequestDto
            {
                Id = requestId,
                EmployeeId = employee.Id,
                LevelId = employee.LevelId,
                ToLevelId = employee.LevelId,
                FromUserType = employee.UserType,
                ToUserType = employee.UserType,
                BasicSalary = employee.Salary,
                ToSalary = employee.RealSalary,
                ApplyDate = employee.ContractStartDate,
                JobPositionId = employee.JobPositionId,
                ToJobPositionId = employee.JobPositionId,
                ContractCode = employee.ContractCode,
                SalaryChangeRequestId = null,
                ProbationPercentage = employee.ProbationPercentage,
                ContractEndDate = employee.ContractEndDate,
                Salary = employee.RealSalary,
                HasContract = true,
                Type = SalaryRequestType.Initial,
                Note = CommonUtil.GetSalaryRequestTypeName(SalaryRequestType.Initial)
            };
            await _salaryRequestManager.UpdateSalaryRequestEmployee(salaryRequestEmployeeDto);
        }


        public async Task AddEmployeeSkills(long employeeId, List<long> skillIds)
        {
            if (skillIds == null || skillIds.Count == 0)
            {
                return;
            }

            List<EmployeeSkill> listToAdd = skillIds.Select(skillId => new EmployeeSkill
            {
                EmployeeId = employeeId,
                SkillId = skillId
            }).ToList();

            await WorkScope.InsertRangeAsync(listToAdd);
        }

        public async Task AddEmployeeTeams(long employeeId, List<long> teamIds)
        {
            if (teamIds == null || teamIds.Count == 0)
            {
                return;
            }

            List<EmployeeTeam> listToAdd = teamIds.Select(skillId => new EmployeeTeam
            {
                EmployeeId = employeeId,
                TeamId = skillId
            }).ToList();

            await WorkScope.InsertRangeAsync(listToAdd);
        }

        public async Task UpdateEmployeeSkill(long employeeId, List<long> newSkillIds)
        {
            var currentEmployeeSkills = WorkScope.GetAll<EmployeeSkill>()
                 .Where(x => x.EmployeeId == employeeId)
                 .Select(s => new { s.Id, s.SkillId })
                 .ToList();

            var currentSkillIds = currentEmployeeSkills.Select(s => s.SkillId).ToList();

            var deleteEmployeeSkillIds = currentEmployeeSkills.Where(s => !newSkillIds.Contains(s.SkillId)).Select(s => s.Id);

            var addSkillIds = newSkillIds.Except(currentSkillIds).ToList();

            foreach (var id in deleteEmployeeSkillIds)
            {
                await WorkScope.DeleteAsync<EmployeeSkill>(id);
            }

            await AddEmployeeSkills(employeeId, addSkillIds);
        }


        public async Task UpdateEmployeeTeam(long employeeId, List<long> teamIds)
        {
            var userTeams = WorkScope.GetAll<EmployeeTeam>()
                 .Where(x => x.EmployeeId == employeeId)
                 .ToList();
            var currentTeamIds = userTeams.Select(x => x.TeamId);
            var deleteTeamIds = currentTeamIds.Except(teamIds);
            var deleteTeams = userTeams.Where(x => deleteTeamIds.Contains(x.TeamId));
            var listToAdd = teamIds.Where(x => !currentTeamIds.Contains(x)).ToList();

            foreach (var item in deleteTeams)
            {
                await WorkScope.DeleteAsync<EmployeeTeam>(item.Id);
            }

            await AddEmployeeTeams(employeeId, listToAdd);
        }

        public async Task<long> InitSalaryChangeRequestAndContract(CreateUpdateEmployeeDto employee)
        {
            var salaryRequestEmployeeDto = new AddOrUpdateEmployeeRequestDto
            {
                EmployeeId = employee.Id,
                LevelId = employee.LevelId,
                ToLevelId = employee.LevelId,
                FromUserType = employee.UserType,
                ToUserType = employee.UserType,
                Salary = employee.RealSalary,
                ToSalary = employee.RealSalary,
                BasicSalary = employee.Salary,
                ApplyDate = employee.ContractStartDate,
                JobPositionId = employee.JobPositionId,
                ToJobPositionId = employee.JobPositionId,
                ProbationPercentage = employee.ProbationPercentage,
                ContractEndDate = employee.ContractEndDate.HasValue ? (DateTime)employee.ContractEndDate : null,
                HasContract = true,
                Note = CommonUtil.GetSalaryRequestTypeName(SalaryRequestType.Initial)
            };

            return await _salaryRequestManager.AddEmployeeTosalaryRequest(salaryRequestEmployeeDto);
        }


        public async Task<long> Delete(long id)
        {
            var employee = await WorkScope.GetAsync<Employee>(id);

            var payslips = WorkScope.GetAll<Payslip>().Include(s => s.Payroll)
                .Where(s => s.EmployeeId == id)
                .ToList();

            var october = new DateTime(2022, 10, 1).Date;
            if (payslips.Any(s => s.Payroll.ApplyMonth >= october))
            {
                throw new UserFriendlyException($"Employee has payslip >= 10/2022 => CAN NOT delete employee");
            }

            var payslipIds = payslips.Select(s => s.Id).ToList();

            if ((employee.Status == EmployeeStatus.Working || employee.Status == EmployeeStatus.MaternityLeave) && payslips.Count > 0)
            {
                throw new UserFriendlyException($"Employee is {employee.Status} and has {payslipIds.Count} payslip => Can't delete");
            }

            var employeeChangeRequest = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(s => s.EmployeeId == id)
                .ToList();


            var changeRequestIds = employeeChangeRequest.Select(s => s.Id).ToList();

            WorkScope.GetAll<EmployeeContract>()
                .Where(s => changeRequestIds.Contains(s.SalaryRequestEmployeeId))
                .ToList().ForEach(s => s.IsDeleted = true);

            employeeChangeRequest.ForEach(s => s.IsDeleted = true);

            WorkScope.GetAll<EmployeeWorkingHistory>()
                .Where(s => s.EmployeeId == id)
                .ToList().ForEach(s => s.IsDeleted = true);

            WorkScope.GetAll<EmployeeBranchHistory>()
                .Where(s => s.EmployeeId == id)
                .ToList().ForEach(s => s.IsDeleted = true);

            WorkScope.GetAll<BenefitEmployee>()
                .Where(s => s.EmployeeId == id)
                .ToList().ForEach(s => s.IsDeleted = true);

            WorkScope.GetAll<BonusEmployee>()
                .Where(s => s.EmployeeId == id)
                .ToList().ForEach(s => s.IsDeleted = true);

            WorkScope.GetAll<PunishmentEmployee>()
                .Where(s => s.EmployeeId == id)
                .ToList().ForEach(s => s.IsDeleted = true);


            WorkScope.GetAll<PayslipDetail>()
                .Where(s => payslipIds.Contains(s.PayslipId))
                .ToList().ForEach(s => s.IsDeleted = true);

            payslips.ForEach(s => s.IsDeleted = true);

            employee.IsDeleted = true;

            CurrentUnitOfWork.SaveChanges();
            return id;
        }

        public async Task<string> UploadAvatar([FromForm] AvatarDto input)
        {
            if (input.File == null)
            {
                throw new UserFriendlyException("No file upload");
            }
            Employee employee = await WorkScope.GetAsync<Employee>(input.EmployeeId);
            String avatarPath = await _uploadFileService.UploadAvatar(input.File);
            employee.Avatar = avatarPath;
            await WorkScope.UpdateAsync(employee);

            if (UploadFileConstant.UploadFileProvider != UploadFileConstant.InternalUploadFile)
            {
                var avatarData = new UploadAvatarDto
                {
                    AvatarPath = avatarPath,
                    EmailAddress = employee.Email
                };
                _projectService.UpdateAvatarToProject(avatarData);
                _timesheetService.UpdateAvatarToTimesheet(avatarData);
            }
            return avatarPath;

        }

        private async Task ValidCreate(CreateUpdateEmployeeDto input)
        {
            var isExist = await WorkScope.GetAll<Employee>()
                .AnyAsync(x => x.Email.Trim() == input.Email.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Email is Already Exist");
            }
            if(input.FullName.Split(' ').Length == 1)
            {
                throw new UserFriendlyException($"Full name is invalid");
            }
        }

        private void ValidUpdate(CreateUpdateEmployeeDto input)
        {
            var isExist = WorkScope.GetAll<Employee>()
                .Any(x => x.Id != input.Id && x.Email == input.Email);
            if (isExist)
            {
                throw new UserFriendlyException($"Email is Already Exist");
            }
            if (input.FullName.Split(' ').Length == 1)
            {
                throw new UserFriendlyException($"Full name is invalid");
            }
        }
        private void ValidDelete(long id)
        {
            var entity = WorkScope.GetAsync<Employee>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Employee with id {id}");
            }
        }

        public void ChangeEmployeeBranch(ChangeBranchDto input)
        {
            AllowUpdateBranch(input);
            input.CurrentUserLoginId = (long)AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;

            DeleteOldRequestInBackgroundJob(input.EmployeeId);
            if (input.Date.Date <= DateTimeUtils.GetNow().Date)
            {
                UpdateNewBranch(input);
            }
            else
            {
                var delayHour = (input.Date.Date - DateTimeUtils.GetNow()).TotalHours + 5;
                _backgroundJobManager.Enqueue<ChangeEmployeeBranch, ChangeBranchDto>(input, BackgroundJobPriority.High, TimeSpan.FromHours(delayHour));
            }
        }

        public void AllowUpdateBranch(ChangeBranchDto input)
        {
            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == input.EmployeeId).FirstOrDefault();
            if(employee == default)
            {
                throw new UserFriendlyException($"Can not found employee with Id={input.EmployeeId}");
            }
            if(employee.BranchId == input.BranchId)
            {
                throw new UserFriendlyException($"Employee already has this branch");
            }
        }

        public void DeleteOldRequestInBackgroundJob(long employeeId)
        {

            var jobTypeOfChangeBranch = typeof(ChangeEmployeeBranch).FullName;

            var filterEmployee = $"\"EmployeeId\":{employeeId},";
            _storeJob.GetAll()
                 .Where(s => s.JobType.Contains(jobTypeOfChangeBranch))
                 .Where(s => s.JobArgs.Contains(filterEmployee))
                 .Select(s => s.Id)
                 .ToList().ForEach(id => _backgroundJobManager.Delete(id.ToString()));

        }

        public void UpdateNewBranch(ChangeBranchDto input)
        {
            var employee = WorkScope.GetAsync<Employee>(input.EmployeeId).Result;
            employee.BranchId = input.BranchId;
            var employeeHistory = new EmployeeBranchHistory
            {
                EmployeeId = input.EmployeeId,
                BranchId = input.BranchId,
                DateAt = input.Date,
                Note = $"Change branch of employee"
            };
            WorkScope.Insert(employeeHistory);
            CurrentUnitOfWork.SaveChanges();
        }



        public async Task<FileBase64Dto> ExportEmployee(GetEmployeeToAddDto input)
        {
            var templateFilePath = Path.Combine(templateFolder, "Create-Employees.xlsx");

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    FillMetaBank(package);
                    FillMetaPosition(package);
                    FillDataToExport(package, input);

                    string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "ExportEmplopyee",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }

        }

        public void FillDataToExport(ExcelPackage package, GetEmployeeToAddDto input)
        {
            var employees = GetAllEmployeeForExport(input).Result.Items;
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;
            worksheet.Column(6).Style.Numberformat.Format = "@";
            worksheet.Column(20).Style.Numberformat.Format = "@";
            worksheet.Column(21).Style.Numberformat.Format = "@";
            worksheet.Column(23).Style.Numberformat.Format = "@";

            foreach (var employee in employees)
            {

                worksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                worksheet.Cells[rowIndex, 2].Value = employee.Surname;
                worksheet.Cells[rowIndex, 3].Value = employee.Name;
                worksheet.Cells[rowIndex, 4].Value = employee.Email;
                worksheet.Cells[rowIndex, 5].Value = employee.Sex;
                worksheet.Cells[rowIndex, 6].Value = employee.Phone;
                worksheet.Cells[rowIndex, 7].Value = employee.Birthday;
                worksheet.Cells[rowIndex, 8].Value = employee.BranchCode;
                worksheet.Cells[rowIndex, 9].Value = employee.UserTypeName;
                worksheet.Cells[rowIndex, 10].Value = employee.LevelName;
                worksheet.Cells[rowIndex, 11].Value = employee.JobPositionCode;
                worksheet.Cells[rowIndex, 12].Value = employee.Status;
                worksheet.Cells[rowIndex, 13].Value = employee.ContractStartDate;
                worksheet.Cells[rowIndex, 14].Value = employee.ContractEndDate;
                worksheet.Cells[rowIndex, 15].Value = employee.Salary;
                worksheet.Cells[rowIndex, 16].Value = employee.ProbationPercentage;
                worksheet.Cells[rowIndex, 17].Value = employee.RealSalary;
                worksheet.Cells[rowIndex, 18].Value = employee.RemainLeaveDay;
                worksheet.Cells[rowIndex, 19].Value = employee.Bank;
                worksheet.Cells[rowIndex, 20].Value = employee.BankAccountNumber;
                worksheet.Cells[rowIndex, 21].Value = employee.TaxCode;
                worksheet.Cells[rowIndex, 22].Value = employee.InsuranceStatus;
                worksheet.Cells[rowIndex, 23].Value = employee.IdCard;
                worksheet.Cells[rowIndex, 24].Value = employee.PlaceOfPermanent;
                worksheet.Cells[rowIndex, 25].Value = employee.Address;
                worksheet.Cells[rowIndex, 26].Value = employee.IssuedOn;
                worksheet.Cells[rowIndex, 27].Value = employee.IssuedBy;
                worksheet.Cells[rowIndex, 28].Value = employee.SeniorityDay;
                worksheet.Cells[rowIndex, 29].Value = employee.AvatarFullPath;
                rowIndex++;

            }
        }

        private void CreateOrUpdateToOtherTool(Employee input, ActionMode mode)
        {
            string branchCode = GetBranchCodeById(input.BranchId);
            string levelCode = GetLevelCodeById(input.LevelId);
            string email = input.Email.ToLower().Trim();
            string jobPosition = GetPositionCodeById(input.JobPositionId);

            var listSkillName = WorkScope.GetAll<EmployeeSkill>()
                                         .Where(s => s.EmployeeId == input.Id)
                                         .Select(s => s.Skill.Name)
                                         .ToList();

            var employee = new CreateOrUpdateUserOtherToolDto
            {
                FullName = input.FullName,
                BranchCode = branchCode,
                LevelCode = levelCode,
                EmailAddress = email,
                Sex = input.Sex,
                Type = input.UserType,
                PositionCode = jobPosition,
                SkillNames = listSkillName,
                WorkingStartDate = input.StartWorkingDate,
            };
            if (mode == ActionMode.Create)
            {
                CreateEmployeeToOtherTool(employee);
            }
            else
            {
                UpdateEmployeeToOtherTool(employee);
            }
        }
        public void ReCreateEmployeeToOtherTool(long employeeId)
        {
            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employeeId)
                .FirstOrDefault();
            if (employee == default)
            {
                throw new UserFriendlyException($"There is no employee with id: {employeeId}");
            }
            CreateOrUpdateToOtherTool(employee, ActionMode.Create);

        }

        public void UpdateEmployeeInfoToOtherTool(EmployeeIdDto input)
        {

            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();
            if (employee == default)
            {
                throw new UserFriendlyException($"There is no employee with id: {input.Id}");
            }
            CreateOrUpdateToOtherTool(employee, ActionMode.Update);

        }

        private void CreateEmployeeToOtherTool(CreateOrUpdateUserOtherToolDto input)
        {
            _timesheetService.CreateTimesheetUser(input);

            _projectService.CreateProjectUser(input);

            _IMSWebService.CreateIMSUser(input);

            _talentWebService.CreateTalentUser(input);
        }


        private void UpdateEmployeeToOtherTool(CreateOrUpdateUserOtherToolDto input)
        {
            _timesheetService.UpdateTimesheetUser(input);

            _projectService.UpdateProjectUser(input);

            _IMSWebService.UpdateIMSUser(input);

            _talentWebService.UpdateTalentUser(input);
        }

        private void UpdateEmployeeStatusToOtherTool(CreateUpdateEmployeeDto input)
        {
            var inputToUpdate = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.Id),
                DateAt = input.ContractStartDate,

            };
            switch (input.Status)
            {
                case EmployeeStatus.Quit:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserQuit(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.Working:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserBackToWork(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.Pausing:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserPause(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.MaternityLeave:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserMaternityLeave(inputToUpdate);
                        break;
                    }
            }
        }

        private string GetEmployeeEmailById(long employeeId)
        {
            var employeeEmail = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employeeId)
                .Select(x => x.Email)
                .FirstOrDefault();
            return employeeEmail;
        }

        public async Task<FileBase64Dto> GetDataMetaToCreateEmployeeByFile()
        {

            var templateFilePath = Path.Combine(templateFolder, "Create-Employees.xlsx");

            using (var stream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {

                using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        FillMetaBank(package);
                        FillMetaPosition(package);
                        FillMetaLevel(package);
                        FillMetaBranch(package);
                        FillMetaUserType(package);
                        FillMetaInsuranceStatusCreate(package);
                        FillMetaSex(package);
                        string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());

                        return new FileBase64Dto
                        {
                            FileName = "CreateEmployees",
                            FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                            Base64 = fileBase64
                        };
                    }
                }
            }

        }
        public async Task<FileBase64Dto> GetDataMetaToUpdateEmployeeByFile()
        {

            var templateFilePath = Path.Combine(templateFolder, "Update-Employees.xlsx");

            using (var stream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {

                using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        FillMetaBank(package);
                        FillMetaInsuranceStatusUpdate(package);
                        string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());

                        return new FileBase64Dto
                        {
                            FileName = "UpdateEmployees",
                            FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                            Base64 = fileBase64
                        };
                    }
                }
            }

        }
        private void FillMetaPosition(ExcelPackage excelPackageIn)
        {
            var listPositionCodes = WorkScope.GetAll<JobPosition>()
                .Select(x => x.Code)
                .ToList();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listPositionCodes)
            {
                sheet.Cells[rowIndex, 7].Value = code;
                rowIndex++;
            }
        }

        private void FillMetaBank(ExcelPackage excelPackageIn)
        {
            var listBankCodes = WorkScope.GetAll<Bank>()
               .Select(x => x.Code)
               .ToList();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listBankCodes)
            {
                sheet.Cells[rowIndex, 1].Value = code;
                rowIndex++;
            }
        }

        private void FillMetaLevel(ExcelPackage excelPackageIn)
        {
            var listLevelName = WorkScope.GetAll<Level>()
               .Select(x => x.Name)
               .ToList();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var name in listLevelName)
            {
                sheet.Cells[rowIndex, 4].Value = name;
                rowIndex++;
            }
        }

        private void FillMetaBranch(ExcelPackage excelPackageIn)
        {
            var listBranch = WorkScope.GetAll<Branch>()
               .Select(x => x.Code)
               .ToList();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listBranch)
            {
                sheet.Cells[rowIndex, 6].Value = code;
                rowIndex++;
            }
        }

        private void FillMetaInsuranceStatusCreate(ExcelPackage excelPackageIn)
        {
            var listInsuranceStatus = Enum.GetValues(typeof(InsuranceStatus)).Cast<InsuranceStatus>();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listInsuranceStatus)
            {
                sheet.Cells[rowIndex, 5].Value = code;
                rowIndex++;
            }
        }
        private void FillMetaInsuranceStatusUpdate(ExcelPackage excelPackageIn)
        {
            var listInsuranceStatus = Enum.GetValues(typeof(InsuranceStatus)).Cast<InsuranceStatus>();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listInsuranceStatus)
            {
                sheet.Cells[rowIndex, 2].Value = code;
                rowIndex++;
            }
        }

        private void FillMetaSex(ExcelPackage excelPackageIn)
        {
            var listSex = Enum.GetValues(typeof(Sex)).Cast<Sex>();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listSex)
            {
                sheet.Cells[rowIndex, 3].Value = code;
                rowIndex++;
            }
        }

        private void FillMetaUserType(ExcelPackage excelPackageIn)
        {
            var listUserType = Enum.GetValues(typeof(UserType)).Cast<UserType>();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var code in listUserType)
            {
                sheet.Cells[rowIndex, 2].Value = code;
                rowIndex++;
            }
        }


        private void ValidImport(IFormFile input)
        {
            if (input == null || !Path.GetExtension(input.FileName).Equals(".xlsx"))
            {
                throw new UserFriendlyException("File null or is not .xlsx file");
            }
        }
        private async Task<List<UpdtaeEmployeeFromFileDto>> GetDataUpdateFromFile([FromForm] InputFileDto input)
        {
            var datas = new List<UpdtaeEmployeeFromFileDto>();
            using (var stream = new MemoryStream())
            {
                input.File.CopyTo(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    var rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var data = new UpdtaeEmployeeFromFileDto();
                        data.Email = worksheet.Cells[row, 1].GetCellValue<string>() ?? "";
                        data.Phone = worksheet.Cells[row, 2].GetCellValue<string>() ?? "";
                        data.Phone = data.Phone.Replace(" ", "").Replace("-", "");
                        data.Phone = data.Phone.ToLower() == "null" ? "" : data.Phone;
                        if (data.Phone != "" && !data.Phone.StartsWith("0") && !data.Phone.StartsWith("84")) { data.Phone = "0" + data.Phone; }
                        data.Birthday = worksheet.Cells[row, 3].GetValue<DateTime?>() ?? null;
                        data.BankCode = worksheet.Cells[row, 4].GetCellValue<string>() ?? "";
                        data.BankAccountNumber = worksheet.Cells[row, 5].GetCellValue<string>() ?? "";
                        data.TaxCode = worksheet.Cells[row, 6].GetCellValue<string>() ?? "";
                        data.InsuranceStatusCode = worksheet.Cells[row, 7].GetCellValue<string>() ?? "";
                        data.IdCard = worksheet.Cells[row, 8].GetCellValue<string>() ?? "";
                        data.PlaceOfPermanent = worksheet.Cells[row, 9].GetCellValue<string>() ?? "";
                        data.Address = worksheet.Cells[row, 10].GetCellValue<string>() ?? "";
                        data.IssuedOn = worksheet.Cells[row, 11].GetCellValue<DateTime?>() ?? null;
                        data.IssuedBy = worksheet.Cells[row, 12].GetCellValue<string>() ?? "";
                        data.Row = row;

                        datas.Add(data);

                    }

                }
            }
            return datas;
        }
        private async Task<List<ImportEmployeeFromFileDto>> GetDataCreateFromFile([FromForm] InputFileDto input)
        {
            var datas = new List<ImportEmployeeFromFileDto>();
            using (var stream = new MemoryStream())
            {
                input.File.CopyTo(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    var rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var data = new ImportEmployeeFromFileDto();
                        data.Surname = worksheet.Cells[row, 2].GetValue<string>() ?? "";
                        data.Name = worksheet.Cells[row, 3].GetValue<string>() ?? "";
                        data.Email = worksheet.Cells[row, 4].GetCellValue<string>() ?? "";
                        data.SexCode = worksheet.Cells[row, 5].GetCellValue<string>() ?? "";
                        data.Phone = worksheet.Cells[row, 6].GetCellValue<string>() ?? "";
                        data.Phone = data.Phone.Replace(" ", "").Replace("-", "");
                        data.Phone = data.Phone.ToLower() == "null" ? "" : data.Phone;

                        if (data.Phone != "" && !data.Phone.StartsWith("0") && !data.Phone.StartsWith("84")) { data.Phone = "0" + data.Phone; }

                        data.Birthday = worksheet.Cells[row, 7].GetValue<DateTime?>() ?? null;
                        data.BranchCode = worksheet.Cells[row, 8].GetValue<string>() ?? null;
                        data.UserTypeName = worksheet.Cells[row, 9].GetCellValue<string>() ?? "";
                        data.LevelCode = worksheet.Cells[row, 10].GetCellValue<string>() ?? "";
                        data.JobPositionCode = worksheet.Cells[row, 11].GetCellValue<string>() ?? "";
                        data.StatusName = worksheet.Cells[row, 12].GetCellValue<string>() ?? "";
                        data.ContractStartDate = worksheet.Cells[row, 13].GetValue<DateTime>();
                        data.ContractEndDate = worksheet.Cells[row, 14].GetCellValue<DateTime?>() ?? null;
                        data.Salary = worksheet.Cells[row, 15].GetCellValue<double>();
                        data.ProbationPercentage = worksheet.Cells[row, 16].GetCellValue<double>();
                        data.RealSalary = worksheet.Cells[row, 17].GetCellValue<double>();
                        data.RemainLeaveDay = worksheet.Cells[row, 18].GetCellValue<float>();
                        data.BankCode = worksheet.Cells[row, 19].GetCellValue<string>() ?? "";
                        data.BankAccountNumber = worksheet.Cells[row, 20].GetCellValue<string>() ?? "";
                        data.TaxCode = worksheet.Cells[row, 21].GetCellValue<string>() ?? "";
                        data.InsuranceStatusCode = worksheet.Cells[row, 22].GetCellValue<string>() ?? "";
                        data.IdCard = worksheet.Cells[row, 23].GetCellValue<string>() ?? "";
                        data.PlaceOfPermanent = worksheet.Cells[row, 24].GetCellValue<string>() ?? "";
                        data.Address = worksheet.Cells[row, 25].GetCellValue<string>() ?? "";
                        data.IssuedOn = worksheet.Cells[row, 26].GetCellValue<DateTime?>() ?? null;
                        data.IssuedBy = worksheet.Cells[row, 27].GetCellValue<string>() ?? "";
                        data.Row = row;

                        datas.Add(data);

                    }

                }
            }
            return datas;
        }



        public async Task<Object> CreateEmployeeFromFile([FromForm] InputFileDto input)
        {
            ValidImport(input.File);
            var datas = await GetDataCreateFromFile(input);
            var failedList = new List<ResponseFailImportEmployeeDto>();
            var successList = new List<string>();
            var dictBranch = WorkScope.GetAll<Branch>()
                                      .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                      .ToDictionary(s => s.Key, s => s.Id);
            var dictLevel = WorkScope.GetAll<Level>()
                                     .Select(s => new { Key = s.Name.ToLower(), s.Id })
                                     .ToDictionary(s => s.Key, s => s.Id);
            var dictBank = WorkScope.GetAll<Bank>()
                                    .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);
            var dictJobposition = WorkScope.GetAll<JobPosition>()
                                           .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                           .ToDictionary(s => s.Key, s => s.Id);


            var importEmails = datas.Select(s => s.Email).ToList();

            var alreadyExistEmails = WorkScope.GetAll<Employee>()
                            .Select(x => x.Email.ToLower())
                            .Where(s => importEmails.Contains(s))
                            .Distinct().ToHashSet();

            foreach (var data in datas)
            {
                var trimmedEmail = data.Email.Trim();

                if (trimmedEmail.EndsWith("."))
                {
                    failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Email is invalid" });
                    continue;
                }
                try
                {

                    var addr = new System.Net.Mail.MailAddress(data.Email);
                    if (addr.Address != trimmedEmail)
                    {
                        failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Email is invalid" });
                        continue;
                    }

                }
                catch
                {
                    failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Email is invalid" });
                    continue;
                }


                if (string.IsNullOrEmpty(data.Email) || alreadyExistEmails.Contains(data.Email))
                {
                    failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Email Already Exist or Null" });
                    continue;
                }
                if (!ValidDataToImport(data, failedList))
                {
                    continue;
                }



                data.BankId = dictBank.ContainsKey(data.BankCode.ToLower()) ? dictBank[data.BankCode.ToLower()] : null;
                data.JobPositionId = dictJobposition[data.JobPositionCode.ToLower()];
                data.LevelId = dictLevel[data.LevelCode.ToLower()];
                data.BranchId = dictBranch[data.BranchCode.ToLower()];
                data.UserType = (UserType)CommonUtil.GetValueOfUsertype(data.UserTypeName);


                await CreateEmployee(data, null, false);
                successList.Add(data.Email);
            }
            return new { successList, failedList };
        }

        public bool ValidDataToUpdate(UpdtaeEmployeeFromFileDto data, List<ResponseFailImportEmployeeDto> failedList)
        {
            var dictBank = WorkScope.GetAll<Bank>()
                                    .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);
            if (!string.IsNullOrEmpty(data.BankCode) && !dictBank.ContainsKey(data.BankCode.ToLower()))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Bank" });
                return false;
            }
            if (!string.IsNullOrEmpty(data.InsuranceStatusCode) && CommonUtil.GetValueOfInsuranceStatus(data.InsuranceStatusCode) == -1)
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Insurance Status" });
                return false;
            }
            return true;
        }

        public bool ValidDataToImport(ImportEmployeeFromFileDto data, List<ResponseFailImportEmployeeDto> failedList)
        {
            var dictBranch = WorkScope.GetAll<Branch>()
                                      .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                      .ToDictionary(s => s.Key, s => s.Id);
            var dictLevel = WorkScope.GetAll<Level>()
                                     .Select(s => new { Key = s.Name.ToLower(), s.Id })
                                     .ToDictionary(s => s.Key, s => s.Id);
            var dictBank = WorkScope.GetAll<Bank>()
                                    .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);
            var dictJobposition = WorkScope.GetAll<JobPosition>()
                                           .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                           .ToDictionary(s => s.Key, s => s.Id);

            if (string.IsNullOrEmpty(data.Surname))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Surname can not be null" });
                return false;
            }

            if (string.IsNullOrEmpty(data.Name))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Name can not be null" });
                return false;
            }

            if (string.IsNullOrEmpty(data.BranchCode))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Branch  can not be null" });
                return false;

            }
            if (string.IsNullOrEmpty(data.UserTypeName))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field UserType can not be null" });
                return false;
            }

            if (string.IsNullOrEmpty(data.LevelCode))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Level can not be null" });
                return false;

            }
            if (string.IsNullOrEmpty(data.JobPositionCode))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Job Position can not be null" });
                return false;

            }


            if (data.ContractStartDate == default)
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Contract StartDate can not be null" });
                return false;

            }



            if (!dictJobposition.ContainsKey(data.JobPositionCode.ToLower()))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found JobPosition" });
                return false;
            }
            if (!dictLevel.ContainsKey(data.LevelCode.ToLower()))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Level" });
                return false;
            }

            if (!dictBranch.ContainsKey(data.BranchCode.ToLower()))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Branch" });
                return false;
            }
            if (CommonUtil.GetValueOfUsertype(data.UserTypeName) == -1)
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Usertype" });
                return false;
            }
            if (!string.IsNullOrEmpty(data.BankCode) && !dictBank.ContainsKey(data.BankCode.ToLower()))
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Bank" });
                return false;
            }
            if (!string.IsNullOrEmpty(data.SexCode) && CommonUtil.GetValueOfSex(data.SexCode) == -1)
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Sex" });
                return false;
            }
            if (!string.IsNullOrEmpty(data.InsuranceStatusCode) && CommonUtil.GetValueOfInsuranceStatus(data.InsuranceStatusCode) == -1)
            {
                failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Insurance Status" });
                return false;
            }
            return true;
        }


        public async Task<Object> UpdateEmployeeFromFile([FromForm] InputFileDto input)
        {
            ValidImport(input.File);
            var datas = await GetDataUpdateFromFile(input);

            var failedList = new List<ResponseFailImportEmployeeDto>();
            var successList = new List<string>();

            var dictBank = WorkScope.GetAll<Bank>()
                                   .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                   .ToDictionary(s => s.Key, s => s.Id);
            var importEmails = datas.Select(s => s.Email).ToList();

            var dictEmployee = WorkScope.GetAll<Employee>()
                            .Select(employeeInfo => new { Key = employeeInfo.Email.ToLower(), employeeInfo })
                                      .ToDictionary(x => x.Key, employeeInfo => employeeInfo);

            foreach (var data in datas)
            {


                if (string.IsNullOrEmpty(data.Email) || !dictEmployee.ContainsKey(data.Email.ToLower()))
                {
                    failedList.Add(new ResponseFailImportEmployeeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found employee" });
                    continue;
                }
                var employee = dictEmployee[data.Email.ToLower()].employeeInfo;

                if (!ValidDataToUpdate(data, failedList))
                {
                    continue;
                }


                data.BankId = dictBank.ContainsKey(data.BankCode.ToLower()) ? dictBank[data.BankCode.ToLower()] : null;
                employee.Phone = string.IsNullOrEmpty(data?.Phone) ? employee.Phone : data?.Phone;
                employee.Birthday = data.Birthday ?? employee.Birthday;
                employee.BankId = data.BankId ?? employee.BankId;
                employee.BankAccountNumber = string.IsNullOrEmpty(data.BankAccountNumber) ? employee.BankAccountNumber : data.BankAccountNumber;
                employee.TaxCode = string.IsNullOrEmpty(data.TaxCode) ? employee.TaxCode : data.TaxCode;
                employee.InsuranceStatus = !string.IsNullOrEmpty(data.InsuranceStatusCode) ? data.InsuranceStatus : employee.InsuranceStatus;
                employee.IdCard = string.IsNullOrEmpty(data.IdCard) ? employee.IdCard : data.IdCard;
                employee.Address = string.IsNullOrEmpty(data.Address) ? employee.Address : data.Address;
                employee.PlaceOfPermanent = string.IsNullOrEmpty(data.PlaceOfPermanent) ? employee.PlaceOfPermanent : data.PlaceOfPermanent;
                employee.IssuedOn = data.IssuedOn ?? employee.IssuedOn;
                employee.IssuedBy = string.IsNullOrEmpty(data.IssuedBy) ? employee.IssuedBy : data.IssuedBy;

                successList.Add(data.Email);

            }

            CurrentUnitOfWork.SaveChanges();

            return new { successList, failedList };
        }
        public List<EmployeeHasBirthdayInMonthDto> GetEmployeesBirthdayInMonth(int month)
        {
            return QueryAllEmployee()
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == month)
                .Select(x => new EmployeeHasBirthdayInMonthDto
                {
                    FullName = x.FullName,
                    Email = x.Email,
                    Birthday = x.Birthday.Value
                }).ToList();
        }

        public List<EmployeeHasBirthdayInMonthDto> GetEmployeesByBirthday(int month, int day)
        {
            return WorkScope.GetAll<Employee>()
                .Where(s => s.Status == EmployeeStatus.Working || s.Status == EmployeeStatus.MaternityLeave)
                .Where(x => x.Birthday.HasValue && x.Birthday.Value.Month == month && x.Birthday.Value.Day == day)
                .Select(x => new EmployeeHasBirthdayInMonthDto
                {
                    FullName = x.FullName,
                    Email = x.Email,
                    Birthday = x.Birthday.Value
                }).ToList();
        }

        public async Task<FileBase64Dto> ExportEmployeeStatistic(InputExportEmployeeStatisticDto input)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "EmployeeStatistic.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

            GetEmployeeStatisticDto employeeStatistic = GetEmployeeStatistic(input);

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataToExport(template, employeeStatistic);

                    string fileBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "EmployeeStatistic",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }

        public GetEmployeeStatisticDto GetEmployeeStatistic(InputExportEmployeeStatisticDto input)
        {
            var qEmployeeHistory = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Where(x => x.DateAt >= input.StartDate && x.DateAt <= input.EndDate)
                .Select(x => new { x.EmployeeId, x.Status }).ToList();

            var onboardEmployeeIds = qEmployeeHistory
                .Where(x => x.Status == EmployeeStatus.Working)
                .Distinct()
                .Select(x => x.EmployeeId)
                .ToList();

            var quitEmployeeIds = qEmployeeHistory
                .Where(x => x.Status == EmployeeStatus.Quit)
                .Distinct()
                .Select(x => x.EmployeeId)
                .ToList();

            var dicEmployees = WorkScope.GetAll<Employee>()
                .Include(x => x.Branch)
                .Where(x => onboardEmployeeIds.Contains(x.Id) || quitEmployeeIds.Contains(x.Id))
                 .ToDictionary(x => x.Id, x => new EmployeeStatisticDto
                 {
                     Id = x.Id,
                     Email = x.Email,
                     BranchName = x.Branch.Name,
                     UserType = x.UserType
                 });


            List<EmployeeStatisticDto> onboardEmployees = new();
            List<EmployeeStatisticDto> quitEmployees = new();

            foreach (var id in onboardEmployeeIds)
            {
                if (dicEmployees.ContainsKey(id))
                {
                    onboardEmployees.Add(dicEmployees[id]);
                }
            }

            foreach (var id in quitEmployeeIds)
            {
                if (dicEmployees.ContainsKey(id))
                {
                    quitEmployees.Add(dicEmployees[id]);
                }
            }

            return new GetEmployeeStatisticDto
            {
                OnboardedEmployees = onboardEmployees,
                QuitEmployees = quitEmployees
            };
        }

        private void FillDataToExport(ExcelPackage package, GetEmployeeStatisticDto data)
        {
            var onboardEmployeeSheet = package.Workbook.Worksheets[0];
            var quitEmployeeSheet = package.Workbook.Worksheets[1];
            var onboardRowIndex = 2;
            var quitRowIndex = 2;

            foreach (var employee in data.OnboardedEmployees)
            {
                onboardEmployeeSheet.Cells[onboardRowIndex, 1].Value = employee.Email;
                onboardEmployeeSheet.Cells[onboardRowIndex, 2].Value = employee.UserType;
                onboardEmployeeSheet.Cells[onboardRowIndex, 3].Value = employee.BranchName;
                onboardRowIndex++;
            }

            foreach (var employee in data.QuitEmployees)
            {
                quitEmployeeSheet.Cells[quitRowIndex, 1].Value = employee.Email;
                quitEmployeeSheet.Cells[quitRowIndex, 2].Value = employee.UserType;
                quitEmployeeSheet.Cells[quitRowIndex, 3].Value = employee.BranchName;
                quitRowIndex++;
            }
        }

        public void QuitJobToOtherTool(EmployeeIdDto input)
        {
            var inputToUpdate = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(input.Id),
                DateAt = CommonUtil.GetNow(),

            };
            _changeEmployeeWorkingStatusManager.ConfirmUserQuit(inputToUpdate);
        }
        public async Task<GetPhoneNumber> GetEmployeePhone(string email)
        {
            return await WorkScope.GetAll<Employee>()
                .Select(s => new GetPhoneNumber
                {
                    Email = s.Email,
                    PhoneNumber = s.Phone
                })
                .Where(s => s.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public List<GetAllEmployeeDto> GetAllEmployee()
        {
            var employees = WorkScope.GetAll<Employee>()
                .Select(x => new GetAllEmployeeDto
                {
                    Email = x.Email,
                    FullName = x.FullName,
                    BranchCode = x.Branch.Code,
                    UserType = x.UserType,
                    Status = x.Status,
                    JobPositionCode = x.JobPosition.Code
                }).ToList();
            return employees;
        }
        public async Task<GetEmployeeByEmailDto> GetEmployeeByEmail(string email)
        {
            return await WorkScope.GetAll<Employee>()
                 .Select(x => new GetEmployeeByEmailDto
                 {
                     Email = x.Email,
                     FullName = x.FullName,
                     BranchCode = x.Branch.Code,
                     BranchName = x.Branch.Name
                 })
                 .Where(s => s.Email.ToLower() == email.ToLower())
                 .FirstOrDefaultAsync();
        }

        public async void UpdateAllWorkingEmployeeInfoToOtherTools()
        {
            var employees =  WorkScope.GetAll<Employee>()
                .Where(x => x.Status == EmployeeStatus.Working)
                .Select(x => new CreateOrUpdateUserOtherToolDto
                {
                    FullName = x.FullName,
                    BranchCode = x.Branch.Code,
                    LevelCode = x.Level.Code,
                    EmailAddress = x.Email,
                    Sex = x.Sex,
                    Type = x.UserType,
                    PositionCode = x.JobPosition.Code,
                    WorkingStartDate = x.StartWorkingDate,
                }
                )
                .ToList();
            if(employees == null || employees.Count() == 0)  return;

            foreach(var employee in employees)
            {
                try
                {
                    Logger.LogDebug($"Start Sync data for user: {employee.EmailAddress}");
                    UpdateEmployeeToOtherTool(employee);
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    base.Logger.Error(employee.EmailAddress + " error: " + e.Message);
                }
            }
        }
    }
}
