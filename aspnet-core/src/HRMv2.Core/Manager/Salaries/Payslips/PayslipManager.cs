using Abp.BackgroundJobs;
using Abp.Linq.Extensions;
using Abp.UI;
using HRMv2.BackgroundJob.SendMail;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using Abp.Webhooks;
using HRMv2.BackgroundJob.CalculateSalary;
using HRMv2.BackgroundJob.ChangeWorkingStatusToQuit;
using Abp.Runtime.Session;
using HRMv2.Hubs;
using Microsoft.Extensions.Options;
using HRMv2.Manager.Employees.Dto;
using System.IO;
using OfficeOpenXml;
using HRMv2.Net.MimeTypes;
using Microsoft.AspNetCore.Mvc;
using HRMv2.Authorization.Roles;
using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using AutoMapper.Internal;
using HRMv2.Manager.Bonuses.Dto;

namespace HRMv2.Manager.Salaries.Payslips
{
    public class PayslipManager : BaseManager
    {
        private readonly TimesheetWebService _timesheetService;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly CalculateSalaryHub _calculateSalaryHub;
        private readonly IOptions<TimesheetConfig> _timesheetConfig;

        public PayslipManager(TimesheetWebService timesheetService,
            EmailManager emailManager,
            BackgroundJobManager backgroundJobManager,
            CalculateSalaryHub calculateSalaryHub,
            IOptions<TimesheetConfig> timesheetConfig,
        IWorkScope workScope) : base(workScope)
        {
            _emailManager = emailManager;
            _timesheetService = timesheetService;
            _backgroundJobManager = backgroundJobManager;
            _calculateSalaryHub = calculateSalaryHub;
            _timesheetConfig = timesheetConfig;
        }

        public IQueryable<GetPayslipDto> QueryAllPayslip()
        {
            return WorkScope.GetAll<Payslip>()
                .Select(x => new GetPayslipDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    FullName = x.Employee.FullName,
                    Email = x.Employee.Email,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    Skills = x.Employee.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.Employee.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
                    ToBranchId = x.BranchId,
                    ToLevelId = x.LevelId,
                    ToJobPositionId = x.JobPositionId,
                    ToUserType = x.UserType,
                    CreationTime = x.CreationTime,
                    PayrollId = x.PayrollId,
                    NormalHour = x.NormalDay,
                    OffDay = x.OffDay,
                    OpentalkCount = x.OpentalkCount,
                    OTHour = x.OTHour,
                    RealOffsetDay = x.RefundLeaveDay,
                    RemainLeaveDayBefore = x.RemainLeaveDayBefore,
                    WorkAtOfficeOrOnsiteDay = x.WorkAtOfficeOrOnsiteDay,
                });
        }

        public async Task<List<PunishmentDto>> GetAvailablePunishmentsInMonth(long payslipId)
        {
            var payslip = WorkScope.GetAll<Payslip>()
                .Where(p => p.Id == payslipId)
                .Select(p => new
                {
                    p.Payroll.ApplyMonth,
                    p.Id,
                    p.PayrollId
                })
                .FirstOrDefault();

            //referenceid => punishmentEmployeeId (list) => punishment (list)
            var ReferencedIds = WorkScope.GetAll<PayslipDetail>()
                .Where(p => p.PayslipId == payslip.Id)
                .Where(p => p.ReferenceId.HasValue)
                .Select(p => p.ReferenceId)
                .ToList();

            //đã xài
            var punishmentIds = WorkScope.GetAll<PunishmentEmployee>()
                 .Where(p => ReferencedIds.Contains(p.Id))
                 .Select(p => p.PunishmentId)
                 .ToList();

            var result = WorkScope.GetAll<Punishment>()
                .OrderBy(s => s.Name)
                .Where(s => s.IsActive && s.Date.Month == payslip.ApplyMonth.Month && s.Date.Year == payslip.ApplyMonth.Year)
                .Where(s => !punishmentIds.Contains(s.Id))
                .Select(x => new PunishmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Date = x.Date,
                    IsActive = x.IsActive,
                    PunishmentTypeId = x.PunishmentTypeId
                });
            return await result.ToListAsync();
        }

        public List<GetBonusDto> GetAvailableBonuses(long payslipId)
        {
            var payrollDateTime = WorkScope.GetAll<Payslip>()
                .Where(p => p.Id == payslipId)
                .Select(p => p.Payroll.ApplyMonth)
                .FirstOrDefault();
            var bonusEmployeeIdList = WorkScope.GetAll<PayslipDetail>()
                .Where(p => p.PayslipId == payslipId && p.Type == PayslipDetailType.Bonus)
                .Where(p => p.ReferenceId.HasValue)
                .Select(p => p.ReferenceId.Value)
                .ToList();
            var addedBonusIdList = WorkScope.GetAll<BonusEmployee>()
                .Where(be => bonusEmployeeIdList.Contains(be.Id))
                .Select(be => be.BonusId)
                .ToList();
            var bonuses = WorkScope.GetAll<Bonus>()
                .Where(b => b.IsActive == true)
                .Where(b => b.ApplyMonth.Year == payrollDateTime.Year && b.ApplyMonth.Month == payrollDateTime.Month)
                .Where(b => !addedBonusIdList.Contains(b.Id))
                .Select(b => new GetBonusDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsActive = b.IsActive,
                    ApplyMonth = b.ApplyMonth
                })
                .ToList();
            return bonuses;
        }

        public async Task<GridResult<GetPayslipEmployeeDto>> GetPayslipEmployeePaging(long payrollId, InputGetPayslipEmployeeDto input)
        {
            var query = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Select(x => new GetPayslipEmployeeDto
                {
                    Id = x.Id,
                    RealSalary = x.Salary,
                    PayslipUserType = x.UserType,
                    FullName = x.Employee.FullName,
                    Email = x.Employee.Email,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    RemainLeaveDays = x.RemainLeaveDayAfter,
                    Skills = x.Employee.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.Employee.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
                    BankInfo = new BankInfoDto
                    {
                        BankName = x.BankName,
                        BankAccountNumber = x.BankAccountNumber
                    },
                    BranchId = x.BranchId,
                    LevelId = x.LevelId,
                    JobPositionId = x.JobPositionId,
                    UserType = x.UserType,
                    EmployeeId = x.EmployeeId,
                    Status = x.Employee.Status,
                    StandardNormalDay = x.Payroll.NormalWorkingDay,
                    StandardOpentalk = x.Payroll.OpenTalk,
                    NormalDay = x.NormalDay,
                    OTHour = x.OTHour,
                    LeaveDayBefore = x.RemainLeaveDayBefore,
                    AddedLeaveDay = x.AddedLeaveDay,
                    RefundLeaveDay = x.RefundLeaveDay,
                    OffDay = x.OffDay,
                    OpentalkCount = x.OpentalkCount,
                    CreationTime = x.CreationTime,
                    CreatorName = x.CreatorUser.FullName,
                    LastModificationTime = x.LastModificationTime,
                    LastModifierName = x.LastModifierUser != null ? x.LastModifierUser.FullName : null,
                    ConfirmStatus = x.ConfirmStatus,
                    ComplainDeadline = x.ComplainDeadline,
                    ComplainNote = x.ComplainNote,

                });

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 0) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 0) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            GridResult<GetPayslipEmployeeDto> result;

            if (input.TeamIds != null && input.TeamIds.Count > 0)
            {
                if (input.TeamIds.Count == 1 || !input.IsAndCondition)
                {
                    var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                    query = from employee in query
                            join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                            select employee;
                    result = await query.GetGridResult(query, input.GridParam);
                }

                var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;
                query = query.Where(s => employeeIds.Contains(s.EmployeeId));
            }

            result = await query.GetGridResult(query, input.GridParam);

            if (result != null && result.Items.Count > 0)
            {
                var payslipIds = query.Select(x => x.Id).ToList();

                var qPayslipDetail = WorkScope.GetAll<PayslipDetail>()
                    .Where(x => payslipIds.Contains(x.PayslipId))
                    .GroupBy(x => new { id = x.PayslipId, type = x.Type });

                var dicPayslipDetails = qPayslipDetail
                    .Select(s => new
                    {
                        s.Key,
                        Money = s.Sum(x => x.Money)
                    }).ToDictionary(s => s.Key, s => s.Money);

                var dicBenefitDetails = qPayslipDetail
                    .Select(s => new
                    {
                        s.Key,
                        ListBenefit = s.Select(x => new BenefitPayslipDetailDto
                        {
                            Money = x.Money,
                            Note = x.Note
                        })
                    }).ToDictionary(s => s.Key, s => s.ListBenefit);

                var dicPayslipSalary = WorkScope.GetAll<PayslipSalary>()
                    .Where(x => x.Payslip.PayrollId == payrollId)
                    .GroupBy(x => x.PayslipId)
                    .Select(x => new
                    {
                        x.Key,
                        InputSalarys = x.Select(s => new InputsalaryDto
                        {
                            Note = s.Note,
                            Salary = s.Salary,
                            FromDate = s.Date
                        })
                    })
                    .ToDictionary(x => x.Key, x => x.InputSalarys);

                var dicLevel = WorkScope.GetAll<Level>()
                   .ToDictionary(x => x.Id, x => x.Name);
                var dicPosition = WorkScope.GetAll<JobPosition>()
                    .ToDictionary(x => x.Id, x => x.Name);
                var dicBranch = WorkScope.GetAll<Branch>()
                    .ToDictionary(x => x.Id, x => x.Name);

                foreach (var item in result.Items)
                {
                    item.PayslipBanrch = dicBranch.ContainsKey(item.BranchId) ? dicBranch[item.BranchId] : null;
                    item.PayslipLevel = dicLevel.ContainsKey(item.LevelId) ? dicLevel[item.LevelId] : null;
                    item.PayslipPosition = dicPosition.ContainsKey(item.JobPositionId) ? dicPosition[item.JobPositionId] : null;

                    item.NormalSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryNormal })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryNormal }] : 0;

                    item.OTSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryOT })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryOT }] : 0;

                    item.MaternityLeaveSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryMaternityLeave })
                   ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryMaternityLeave }] : 0;

                    item.TotalBonus = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Bonus })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Bonus }] : 0;

                    item.TotalPunishment = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Punishment })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Punishment }] : 0;

                    item.TotalDebt = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Debt })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Debt }] : 0;

                    item.ListBenefit = dicBenefitDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Benefit })
                    ? dicBenefitDetails[new { id = item.Id, type = PayslipDetailType.Benefit }].ToList() : null;

                    item.ListInputSalary = dicPayslipSalary.ContainsKey(item.Id) ? dicPayslipSalary[item.Id].ToList() : null;
                }
            }

            return result;
        }


        public List<ExportPayrollIncludeLastMonthDto> GetPayslipByPayrollId(long payrollId)
        {
            var result = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Select(x => new ExportPayrollIncludeLastMonthDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    RealSalary = x.Salary,
                    FullName = x.Employee.FullName,
                    Email = x.Employee.Email,
                    RemainLeaveDays = x.RemainLeaveDayAfter,
                    BankAccountNumber = x.BankAccountNumber,
                    BankName = x.BankName,
                    StandardNormalDay = x.Payroll.NormalWorkingDay,
                    StandardOpentalk = x.Payroll.OpenTalk,
                    NormalDay = x.NormalDay,
                    OTHour = x.OTHour,
                    LeaveDayBefore = x.RemainLeaveDayBefore,
                    AddedLeaveDay = x.AddedLeaveDay,
                    RefundLeaveDay = x.RefundLeaveDay,
                    OffDay = x.OffDay,
                    OpentalkCount = x.OpentalkCount,
                    ComplainNote = x.ComplainNote,
                    BranchId = x.BranchId,
                    JobPositionId = x.JobPositionId,
                    LevelId = x.LevelId,
                    PayslipUserType = x.UserType
                }).ToList();

            if (result != null && result.Count > 0)
            {
                var payslipIds = result.Select(x => x.Id).ToList();

                var qPayslipDetail = WorkScope.GetAll<PayslipDetail>()
                    .Where(x => payslipIds.Contains(x.PayslipId))
                    .GroupBy(x => new { id = x.PayslipId, type = x.Type });

                var dicPayslipDetails = qPayslipDetail
                    .Select(s => new
                    {
                        s.Key,
                        Money = s.Sum(x => x.Money)
                    }).ToDictionary(s => s.Key, s => s.Money);

                var dicBenefitDetails = qPayslipDetail
                    .Select(s => new
                    {
                        s.Key,
                        ListBenefit = s.Select(x => new BenefitPayslipDetailDto
                        {
                            Money = x.Money,
                            Note = x.Note
                        })
                    }).ToDictionary(s => s.Key, s => s.ListBenefit);

                var dicPayslipSalary = WorkScope.GetAll<PayslipSalary>()
                    .Where(x => x.Payslip.PayrollId == payrollId)
                    .GroupBy(x => x.PayslipId)
                    .Select(x => new
                    {
                        x.Key,
                        InputSalarys = x.Select(s => new InputsalaryDto
                        {
                            Note = s.Note,
                            Salary = s.Salary,
                            FromDate = s.Date
                        })
                    })
                    .ToDictionary(x => x.Key, x => x.InputSalarys);

                var dicLevel = WorkScope.GetAll<Level>()
                   .ToDictionary(x => x.Id, x => x.Name);
                var dicPosition = WorkScope.GetAll<JobPosition>()
                    .ToDictionary(x => x.Id, x => x.Name);
                var dicBranch = WorkScope.GetAll<Branch>()
                    .ToDictionary(x => x.Id, x => x.Name);

                foreach (var item in result)
                {
                    item.PayslipBanrch = dicBranch.ContainsKey(item.BranchId) ? dicBranch[item.BranchId] : null;
                    item.PayslipLevel = dicLevel.ContainsKey(item.LevelId) ? dicLevel[item.LevelId] : null;
                    item.PayslipPosition = dicPosition.ContainsKey(item.JobPositionId) ? dicPosition[item.JobPositionId] : null;

                    item.NormalSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryNormal })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryNormal }] : 0;

                    item.OTSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryOT })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryOT }] : 0;

                    item.MaternityLeaveSalary = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.SalaryMaternityLeave })
                   ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.SalaryMaternityLeave }] : 0;

                    item.TotalBonus = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Bonus })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Bonus }] : 0;

                    item.TotalPunishment = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Punishment })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Punishment }] : 0;

                    item.TotalDebt = dicPayslipDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Debt })
                    ? dicPayslipDetails[new { id = item.Id, type = PayslipDetailType.Debt }] : 0;

                    item.ListBenefit = dicBenefitDetails.ContainsKey(new { id = item.Id, type = PayslipDetailType.Benefit })
                    ? dicBenefitDetails[new { id = item.Id, type = PayslipDetailType.Benefit }].ToList() : null;

                    item.ListInputSalary = dicPayslipSalary.ContainsKey(item.Id) ? dicPayslipSalary[item.Id].ToList() : null;
                }
            }
            return result;
        }


        //TODO: test case [check exception with invalid/not exits payrollid]
        public List<ExportPayrollIncludeLastMonthDto> GetPayslipEmployeeForExport(long payrollId)
        {
            var payroll = WorkScope.GetAll<Payroll>()
                .Where(x => x.Id == payrollId)
                .FirstOrDefault();

            var lastMonthPayrollId = WorkScope.GetAll<Payroll>()
               .Where(x => x.ApplyMonth < payroll.ApplyMonth)
               .OrderByDescending(x => x.ApplyMonth)
               .Select(x => x.Id)
               .FirstOrDefault();

            List<ExportPayrollIncludeLastMonthDto> thisMonthPayslips = GetPayslipByPayrollId(payrollId);
            List<ExportPayrollIncludeLastMonthDto> lastMonthPayslips = GetPayslipByPayrollId(lastMonthPayrollId);

            var lastMonthPayslipEmployeeIds = lastMonthPayslips.Select(x => x.EmployeeId).ToList();
            var thisMonthPayslipEmployeeIds = thisMonthPayslips.Select(x => x.EmployeeId).ToList();

            var dicLastMonthPayslip = lastMonthPayslips
                .ToDictionary(x => x.EmployeeId, x => x);

            foreach (var item in thisMonthPayslips)
            {
                if (lastMonthPayslipEmployeeIds.Contains(item.EmployeeId))
                {
                    item.ExportStatus = PayslipStatusForExport.Both;
                }
                else
                {
                    item.ExportStatus = PayslipStatusForExport.InThisMonthOnly;
                }

                if (dicLastMonthPayslip.ContainsKey(item.EmployeeId))
                {
                    var lastMonthPayslip = dicLastMonthPayslip[item.EmployeeId];
                    item.RealSalaryLastMonth = lastMonthPayslip.RealSalary;
                    item.NormalSalaryLastMonth = lastMonthPayslip.NormalSalaryLastMonth;
                    item.LeaveDayLastMonth = lastMonthPayslip.RemainLeaveDays;
                    item.ListInputSalaryLastMonth = lastMonthPayslip.ListInputSalary;
                }
            }

            var payslipNotInThisMonthButInLastMonth = lastMonthPayslips.Where(x => !thisMonthPayslipEmployeeIds.Contains(x.EmployeeId)).ToList();

            foreach (var payslip in payslipNotInThisMonthButInLastMonth)
            {
                payslip.ExportStatus = PayslipStatusForExport.InLastMonthOnly;

                payslip.RealSalaryLastMonth = payslip.RealSalary;
                payslip.RealSalary = 0;

                payslip.NormalSalaryLastMonth = payslip.NormalSalary;
                payslip.NormalSalary = 0;

                payslip.LeaveDayLastMonth = payslip.RemainLeaveDays;
                payslip.RemainLeaveDays = 0;

                payslip.ListInputSalaryLastMonth = payslip.ListInputSalary;
                payslip.ListInputSalary = new List<InputsalaryDto>();
            }

            List<ExportPayrollIncludeLastMonthDto> result = thisMonthPayslips.Concat(payslipNotInThisMonthButInLastMonth).OrderBy(x => x.ExportStatus).ToList();

            return result;
        }

        public List<GetSalaryDetailDto> GetPayslipResult(long payslippId)
        {
            return WorkScope.GetAll<PayslipDetail>()
                .Where(x => x.PayslipId == payslippId)
                .Select(x => new GetSalaryDetailDto
                {
                    PayslipId = x.PayslipId,
                    Type = x.Type,
                    Money = x.Money,
                    Note = x.Note,
                    IsProjectCost = x.IsProjectCost
                }).ToList();
        }

        public List<GetPayslipDetailByTypeDto> GetPayslipDetailByType(long payslipId, PayslipDetailType type)
        {
            return WorkScope.GetAll<PayslipDetail>()
                .Where(x => x.PayslipId == payslipId)
                .Where(x => x.Type == type)
                .Select(x => new GetPayslipDetailByTypeDto
                {
                    Id = x.Id,
                    PayslipId = x.PayslipId,
                    Type = x.Type,
                    Money = Math.Abs(x.Money),
                    Note = x.Note,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
                }).ToList();
        }

        //TODO: test case [throw error create duplicate,fail]

        public async Task<string> CreatePayslipDetailPunishmentAndCreateEmployeePunishment(CreatePayslipDetailPunishmentDto input)
        {
            var employeeId = WorkScope
                .GetAll<Payslip>()
                .Where(e => e.Id == input.PayslipId)
                .Select(e => e.EmployeeId)
                .FirstOrDefault();

            var punishmentEmployee = WorkScope
                .GetAll<PunishmentEmployee>()
                .Where(pe => pe.EmployeeId == employeeId)
                .Where(pe => pe.PunishmentId == input.PunishmentId)
                .FirstOrDefault();

            var entity = ObjectMapper.Map<PayslipDetail>(input);
            entity.Type = PayslipDetailType.Punishment;
            entity.Money = -Math.Abs(entity.Money);
            entity.IsProjectCost = false;

            if (punishmentEmployee == default)
            {
                var newPunishMentEmployee = new PunishmentEmployee
                {
                    Money = Math.Abs(input.Money),
                    Note = input.Note,
                    EmployeeId = employeeId,
                    PunishmentId = input.PunishmentId,
                };
                newPunishMentEmployee.Id = await WorkScope.InsertAndGetIdAsync(newPunishMentEmployee);

                entity.ReferenceId = newPunishMentEmployee.Id;
                WorkScope.Insert(entity);
                return "Inserted PayslipDetail and inserted PunishmentEmployee successfull";
            }
            
            punishmentEmployee.Money = Math.Abs(entity.Money);
            punishmentEmployee.Note = entity.Note;
            entity.ReferenceId = punishmentEmployee.Id;

            await WorkScope.InsertAsync(entity);
            await WorkScope.UpdateAsync(punishmentEmployee);
            return "Inserted PayslipDetail and updated PunishmentEmployee successfull";             
        }

        public async Task<string> UpdatePayslipDetailPunishment(UpdatePayslipDetailDto input)
        {
            var payslipDetailExt = WorkScope.GetAll<PayslipDetail>()
                .Where(s => s.Id == input.Id)
                .Select(s => new
                {
                    PayslipDetail = s,
                    s.Payslip.EmployeeId
                }).FirstOrDefault();

            if (payslipDetailExt == default)
                throw new UserFriendlyException("Not found PayslipDetail Id = " + input.Id);

            payslipDetailExt.PayslipDetail.Note = input.Note;
            payslipDetailExt.PayslipDetail.Money = -Math.Abs(input.Money);

            await WorkScope.UpdateAsync(payslipDetailExt.PayslipDetail);

            var punishmentEmployee = WorkScope.GetAll<PunishmentEmployee>()
                .Where(s => s.Id == payslipDetailExt.PayslipDetail.ReferenceId)
                .FirstOrDefault();

            if (punishmentEmployee != default)
            {
                punishmentEmployee.Money = Math.Abs(input.Money);
                punishmentEmployee.Note = input.Note;
                await WorkScope.UpdateAsync(punishmentEmployee);

                return $"Updated PayslipDetail and PunishmentEmployee successfull";
            }

            return $"Updated PayslipDetail, not found PunishmentEmployee";

        }

        public async Task<string> CreatePayslipDetailBonus(CreatePayslipBonusDto input)
        {
            var employeeId = await WorkScope.GetAll<Payslip>()
                .Where(p => p.Id == input.PayslipId)
                .Select(p => p.EmployeeId)
                .FirstOrDefaultAsync();
            var bonusEmployee = await WorkScope.GetAll<BonusEmployee>()
                .Where(be => be.BonusId == input.BonusId && be.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (bonusEmployee != default)
            {
                var isExistPayslipDetail = WorkScope.GetAll<PayslipDetail>()
                    .Where(p => p.Type == PayslipDetailType.Bonus
                    && p.PayslipId == input.PayslipId
                    && p.ReferenceId == bonusEmployee.Id).Any();

                if (isExistPayslipDetail)
                    throw new UserFriendlyException($"This payslip already has bonus {input.Note} with Id: {input.BonusId}");
            }

            var entity = new PayslipDetail()
            {
                PayslipId = input.PayslipId,
                Money = Math.Abs(input.Money),
                Note = input.Note,
                Type = PayslipDetailType.Bonus,
                IsProjectCost = true
            };

            string result;
            if (bonusEmployee != default)
            {
                bonusEmployee.Note = input.Note;
                bonusEmployee.Money = Math.Abs(input.Money);
                await WorkScope.UpdateAsync(bonusEmployee);
                entity.ReferenceId = bonusEmployee.Id;
                result = $"Added bonus {input.Note} successfully to Payslip, and updated in Bonus";
            }
            else
            {
                var bonusEmployeeAdd = new BonusEmployee
                {
                    EmployeeId = employeeId,
                    BonusId = input.BonusId,
                    Note = input.Note,
                    Money = input.Money
                };
                entity.ReferenceId = await WorkScope.InsertAndGetIdAsync(bonusEmployeeAdd);
                result = $"Added bonus {input.Note} successfully to Payslip and Bonus";
            }

            await WorkScope.InsertAsync(entity);
            return result;
        }

        public async Task<string> UpdatePayslipDetailBonus(UpdatePayslipDetailDto input)
        {

            var entity = await WorkScope.GetAsync<PayslipDetail>(input.Id);
            entity.Money = input.Money;
            entity.Note = input.Note;
            await WorkScope.UpdateAsync(entity);

            if (!entity.ReferenceId.HasValue)
            {
                return "Updated PayslipDetail only, PayslipDetail.ReferenceId null";
            }

            var bonusEmployee = await WorkScope.GetAll<BonusEmployee>()
                .Where(be => be.Id == entity.ReferenceId.Value)
                .FirstOrDefaultAsync();
            if (bonusEmployee == default)
            {
                return $"Updated PayslipDetail only, not found BonusEmployee Id {entity.ReferenceId.Value}";
            }

            bonusEmployee.Note = input.Note;
            bonusEmployee.Money = input.Money;
            await WorkScope.UpdateAsync(bonusEmployee);
            return $"Updated bonus {input.Note} successfully in Payslip and Bonus";
        }

        public async Task<long> DeletePayslipDetail(long id)
        {
            var payslipDetailExt = WorkScope.GetAll<PayslipDetail>()
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    PayslipDetail = s,
                    s.Payslip.Payroll.ApplyMonth,
                    s.Payslip.EmployeeId
                }).FirstOrDefault();

            if (payslipDetailExt == default)
                throw new UserFriendlyException($"Not found PayslipDetail Id = {id}");

            var payslipDetail = payslipDetailExt.PayslipDetail;
            await WorkScope.DeleteAsync<PayslipDetail>(id);

            if (payslipDetail.Type == PayslipDetailType.Punishment)
            {                
                var punishmentEmployee = WorkScope.GetAll<PunishmentEmployee>()
                .Where(s => s.Id == payslipDetail.ReferenceId)
                .Where(s => s.EmployeeId == payslipDetailExt.EmployeeId)
                .Where(s => s.Punishment.Date.Year == payslipDetailExt.ApplyMonth.Year
                && s.Punishment.Date.Month == payslipDetailExt.ApplyMonth.Month)
                .FirstOrDefault();

                if (punishmentEmployee != null)
                {
                    await WorkScope.DeleteAsync(punishmentEmployee);
                }
            }

            if(payslipDetail.Type == PayslipDetailType.Bonus)
            {
                var bonusEmployee = await WorkScope.GetAll<BonusEmployee>()
                    .Where(be => be.Id == payslipDetail.ReferenceId.Value)
                    .Where(be => be.EmployeeId == payslipDetailExt.EmployeeId)
                    .Where(be => be.Bonus.ApplyMonth.Year == payslipDetailExt.ApplyMonth.Year
                    && be.Bonus.ApplyMonth.Month == payslipDetailExt.ApplyMonth.Month)
                    .FirstOrDefaultAsync();
                if(bonusEmployee != default)
                {
                    await WorkScope.DeleteAsync(bonusEmployee);
                }
            }    
            return id;
        }


        public async Task<long> ValidPayslipDetail(long id)
        {
            var isExist = await WorkScope.GetAll<PayslipDetail>()
                .Where(x => x.Id == id)
                .AnyAsync();

            if (!isExist)
            {
                throw new UserFriendlyException($"Payslip detail with Id = {id} is not exist");
            }

            return id;

        }


        public List<long> GetEmployeeIdsInPayroll(long payrollId)
        {
            return WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Select(x => x.EmployeeId)
                .ToList();
        }


        public GetPayslipDetailDto GetPayslipDetail(long id)
        {
            List<PayslipContractSalaryDto> inputSalary = GetPayslipSalary(id);
            CalculateResultDto calculateResult = GetPayslipCalculateResult(id);

            return WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == id)
                .Select(x => new GetPayslipDetailDto
                {
                    ParollMonth = x.Payroll.ApplyMonth,
                    EmployeeFullName = x.Employee.FullName,
                    TotalRealSalary = x.Salary,
                    LeaveDayAfter = x.RemainLeaveDayAfter,
                    StandardWorkingDay = x.Payroll.NormalWorkingDay,
                    StandardOpenTalk = x.Payroll.OpenTalk,
                    LeaveDayBefore = x.RemainLeaveDayBefore,
                    MonthlyAddedLeaveDay = x.AddedLeaveDay,
                    NormalworkingDay = x.NormalDay,
                    OffDay = x.OffDay,
                    OpenTalkCount = x.OpentalkCount,
                    OTHour = x.OTHour,
                    WorkAtOfficeOrOnsiteDay = x.WorkAtOfficeOrOnsiteDay,
                    RefundLeaveDay = x.RefundLeaveDay,
                    InputSalary = inputSalary,
                    CalculateResult = calculateResult,
                    ConfirmStatus = x.ConfirmStatus,
                    ComplainDeadline = x.ComplainDeadline,
                    ComplainNote = x.ComplainNote
                }).FirstOrDefault();
        }

        public async Task<long> ValidPayslip(long id)
        {
            var isExist = await WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == id)
                .AnyAsync();

            if (!isExist)
            {
                throw new UserFriendlyException($"Payslip with Id = {id} is not exist");
            }

            return id;

        }

        public GetPayslipInfoBeforeUpdateDto GetPayslipBeforeUpdateInfo(long payslipId)
        {
            return WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == payslipId)
                .Select(x => new GetPayslipInfoBeforeUpdateDto
                {
                    Id = x.Id,
                    RemainLeaveDayBefore = x.RemainLeaveDayBefore,
                    AddedLeaveDay = x.AddedLeaveDay,
                    NormalDay = x.NormalDay,
                    OpentalkCount = x.OpentalkCount,
                    RemainLeaveDayAfter = x.RemainLeaveDayAfter,
                    OffDay = x.OffDay,
                    OTHour = x.OTHour,
                    RefundLeaveDay = x.RefundLeaveDay,
                    Salary = x.Salary,
                    NormalSalary = WorkScope.GetAll<PayslipDetail>()
                                            .Where(dt => dt.Type == PayslipDetailType.SalaryNormal && dt.PayslipId == payslipId)
                                            .Sum(m => m.Money),
                    OTSalary = WorkScope.GetAll<PayslipDetail>()
                                            .Where(dt => dt.Type == PayslipDetailType.SalaryOT && dt.PayslipId == payslipId)
                                            .Sum(m => m.Money)
                }).FirstOrDefault();
        }

        public async Task<UpdatePayslipInfoDto> UpdatePayslipDetail(UpdatePayslipInfoDto input)
        {
            await ValidPayslip(input.Id);
            var entity = await WorkScope.GetAsync<Payslip>(input.Id);
            if (entity.RemainLeaveDayBefore != input.RemainLeaveDayBefore 
                || entity.AddedLeaveDay != input.AddedLeaveDay 
                || entity.NormalDay != input.NormalDay 
                || entity.OpentalkCount != input.OpentalkCount 
                || entity.OffDay != input.OffDay
                || entity.OTHour != input.OTHour
                || entity.RefundLeaveDay != input.RefundLeaveDay
                ||entity.RemainLeaveDayAfter != input.RemainLeaveDayAfter)
            {
                entity.RemainLeaveDayBefore = input.RemainLeaveDayBefore;
                entity.AddedLeaveDay = input.AddedLeaveDay;
                entity.NormalDay = input.NormalDay;
                entity.OpentalkCount = input.OpentalkCount;
                entity.OffDay = input.OffDay;
                entity.OTHour = input.OTHour;
                entity.RefundLeaveDay = input.RefundLeaveDay;
                entity.RemainLeaveDayAfter = input.RemainLeaveDayAfter;
            }

            var details = await WorkScope.GetAll<PayslipDetail>().Where(p=>p.PayslipId==input.Id).ToListAsync();
            //normal salary
            var detail_NormalSalary = details.Where(dt => dt.Type == PayslipDetailType.SalaryNormal).ToList();
            UpdateDetail(detail_NormalSalary, input.NormalSalary);

            //OT salary
            var detail_OTSalary = details.Where(dt => dt.Type == PayslipDetailType.SalaryOT).ToList();
            UpdateDetail(detail_OTSalary, input.OTSalary);

            entity.Salary = details.Sum(s=>s.Money);

            await CurrentUnitOfWork.SaveChangesAsync();
            return input;
        }

        private void UpdateDetail(List<PayslipDetail> details, double value)
        {

            if (details.Sum(s => s.Money) == value)
                return;

            if (details.Count >= 1)
            {
                details[0].Money = value;
                if (details.Count > 1)                   
                    details.Skip(1).ForAll(s => s.Money = 0);
            }

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

        /// <summary>
        /// Hàm này lấy ra thông tin chi tiết danh sách các nhân viên chưa trả tiền phạt/ lương âm
        /// </summary>
        /// <param name="payrollId"></param>
        /// <returns>
        ///     Return các thông tin sau:
        ///     {
        ///         Id: Id của payslip;
        ///         FullName: Tên nhân viên;
        ///         Email: Email nhân viên;
        ///         Avatar: string;
        ///         Sex: Giới tính;
        ///         BranchInfo: {Tên; Màu}
        ///         LevelInfo: {Tên; Màu}
        ///         JobPositionInfo: {Tên; Màu}
        ///         UserTypeInfo: {Tên; màu}
        ///         RealSalary: Lương thực lĩnh (điều kiện < 0)
        ///         EmployeeId: Id nhân viên
        ///     }
        /// </returns>
        public async Task<List<GetNotPayslipEmployeeDto>> GetAllPenaltyNotCollected(long payrollId)
        {
            var query = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId && x.Salary < 0)
                .Select(x => new GetNotPayslipEmployeeDto
                {
                    Id = x.Id,
                    RealSalary = x.Salary,
                    FullName = x.Employee.FullName,
                    Email = x.Employee.Email,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
                    UserType = x.UserType,
                    EmployeeId = x.EmployeeId,
                });

            return await query.ToListAsync();
        }
        public List<SumaryInfoDto> GetSumaryInfomation(long payrollId)
        {
            var qPayslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId).ToList();

            var list1 = WorkScope.GetAll<PayslipDetail>()
                                .Where(x => x.Payslip.PayrollId == payrollId)
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

            var list2 = qPayslip
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

            var results = new List<SumaryInfoDto>();

            var notPayPunishment = qPayslip
            .Where(x => x.Salary < 0).ToList();

            results.Add(new SumaryInfoDto
            {
                Name = "Tổng chi (bắn sang Finfast)  = Tổng lương - Phạt không thu được",
                Quantity = list2.Sum(s => s.Quantity),
                TotalSalary = list2.Sum(s => s.TotalSalary) - notPayPunishment.Sum(s => s.Salary),
            });

            results.Add(new SumaryInfoDto
            {
                Name = "Tổng lương",
                TotalSalary = list2.Sum(s => s.TotalSalary),
                Quantity = list2.Sum(s => s.Quantity)
            });

            results.AddRange(list1);
            results.AddRange(list2);

            var notPaidPunishment = new SumaryInfoDto
            {
                Name = "Phạt không thu được",
                TotalSalary = notPayPunishment.Sum(s => s.Salary),
                Quantity = notPayPunishment.Count(),
            };

            var punishment = list1.Where(s => s.Type == PayslipDetailType.Punishment).FirstOrDefault();
            if (punishment != default)
            {
                var payPunishment = new SumaryInfoDto
                {
                    Name = "Phạt thu được",
                    TotalSalary = punishment.TotalSalary - notPaidPunishment.TotalSalary,
                    Quantity = punishment.Quantity
                };
                results.Add(payPunishment);
            }
            results.Add(notPaidPunishment);
            return results;
        }


        public CalculateResultDto GetPayslipCalculateResult(long payslipId)
        {
            var detail = WorkScope.GetAll<PayslipDetail>()
                .Where(x => x.PayslipId == payslipId)
                .GroupBy(x => x.Type)
                .Select(x => new
                {
                    x.Key,
                    TotalMoney = x.Sum(s => s.Money)
                }).ToDictionary(x => x.Key, x => x.TotalMoney);

            return new CalculateResultDto
            {
                NormalSalary = detail.ContainsKey(PayslipDetailType.SalaryNormal) ? detail[PayslipDetailType.SalaryNormal] : 0,
                OTSalary = detail.ContainsKey(PayslipDetailType.SalaryOT) ? detail[PayslipDetailType.SalaryOT] : 0,
                MaternityLeaveSalary = detail.ContainsKey(PayslipDetailType.SalaryMaternityLeave) ? detail[PayslipDetailType.SalaryMaternityLeave] : 0,
                TotalBenefit = detail.ContainsKey(PayslipDetailType.Benefit) ? detail[PayslipDetailType.Benefit] : 0,
                TotalBonus = detail.ContainsKey(PayslipDetailType.Bonus) ? detail[PayslipDetailType.Bonus] : 0,
                TotalDebt = detail.ContainsKey(PayslipDetailType.Debt) ? detail[PayslipDetailType.Debt] : 0,
                TotalPunishment = detail.ContainsKey(PayslipDetailType.Punishment) ? detail[PayslipDetailType.Punishment] : 0,
                TotalRefund = detail.ContainsKey(PayslipDetailType.Refund) ? detail[PayslipDetailType.Refund] : 0,
            };
        }

        private void CheckValidGeneratePayslips(PayrollDto payroll)
        {
            if (payroll == default)
            {
                throw new UserFriendlyException($"Can't find the Payroll");
            }

            if (payroll.Status == PayrollStatus.ApprovedByCEO
                || payroll.Status == PayrollStatus.PendingCEO
                || payroll.Status == PayrollStatus.ApprovedByCEO
                || payroll.Status == PayrollStatus.PendingCEO
                || payroll.Status == PayrollStatus.Executed)
            {
                throw new UserFriendlyException($"Only calculate salary if the the payroll status is: New, RejectedByKT, RejectedByKT or RejectedByCEO");
            }
        }

        private List<GenerateErrorDto> GetDebtPlanError(List<long> employeeIds)
        {
            var listError = new List<GenerateErrorDto>();
            
            var debts = WorkScope.GetAll<Debt>()
                .Where(x => x.Status == DebtStatus.Inprogress)
                .Where(x => x.PaymentType == DebtPaymentType.Salary)
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Select(x => new
                {
                    x.Id,
                    x.Money,
                    x.InterestRate,
                    x.StartDate,
                    x.EndDate,
                    x.Employee.Email
                })
                .AsNoTracking()
                .ToList();

            var debtIds = debts.Select(s => s.Id).ToList();

            var dicPaymentPlan = WorkScope.GetAll<DebtPaymentPlan>()
               .Select(s => new { s.DebtId, s.Money })
               .Where(x => debtIds.Contains(x.DebtId))
               .GroupBy(x => x.DebtId)
               .Select(x => new
               {
                   x.Key,
                   TotalPlanMoney = x.Sum(s => s.Money)
               }).ToDictionary(x => x.Key, x => x.TotalPlanMoney);


            foreach (var debt in debts)
            {
                if (dicPaymentPlan.ContainsKey(debt.Id))
                {
                    var totalPlanMoney = dicPaymentPlan[debt.Id];
                    var debtInterest = CommonUtil.CalculateInterestValue(debt.StartDate, debt.EndDate, debt.Money, debt.InterestRate);
                    var moneyHaveToPay = debt.Money + CommonUtil.RoundMoneyVND(debtInterest);

                    if (Math.Abs(totalPlanMoney - moneyHaveToPay) > 10000)
                    {
                        var error = new GenerateErrorDto
                        {
                            Message = $"<strong>{debt.Email}</strong> Debt #{debt.Id} Payment plan (total: <strong>{CommonUtil.FormatDisplayMoney(totalPlanMoney)})</strong>" +
                            $" is NOT EQUAL to debt Principal + Interest (<strong>{CommonUtil.FormatDisplayMoney(moneyHaveToPay)}</strong>)",
                            ReferenceId = debt.Id 
                        };

                        listError.Add(error);
                    }
                }
                else
                {
                    listError.Add( new GenerateErrorDto { Message = $"<strong>{debt.Email}</strong> debt #{debt.Id} dont have payment plan", ReferenceId = debt.Id });
                }
            }

            return listError;
        }


        public static Dictionary<int, CalculatingSalaryInfoDto> DicTenantIdToCalculatingSalaryInfoDto = new Dictionary<int, CalculatingSalaryInfoDto>();
        public GeneratePayslipResultDto AddGeneratePayslipsToBackgroundJob(CollectPayslipDto input)
        {
            var result = new GeneratePayslipResultDto
            {
                ErrorList = new List<GenerateErrorDto>(),
            };
            var isAlreadyExist = WorkScope.GetAll<BackgroundJobInfo>()
                .Where(s => !s.IsAbandoned)
                .Where(s => s.JobType.Contains(typeof(CalculateSalaryBackgroundJob).FullName))
                .Any();
            if (isAlreadyExist)
            {
                result.ErrorList.Add(new GenerateErrorDto { Message = "Already added Calculate Salary to Background Job" });
                return result;
            }
            _backgroundJobManager.Enqueue<CalculateSalaryBackgroundJob, CollectPayslipDto>(input, BackgroundJobPriority.High, TimeSpan.FromSeconds(0));
            return result;
        }

        public CalculatingSalaryInfoDto GetCalculatingSalaryInfoDto(int? tenantId)
        {
            var tenantIdValue = tenantId.HasValue ? tenantId.Value : -1;
            if (!DicTenantIdToCalculatingSalaryInfoDto.ContainsKey(tenantIdValue))
            {
                DicTenantIdToCalculatingSalaryInfoDto.Add(tenantIdValue, new CalculatingSalaryInfoDto());
            }

            return DicTenantIdToCalculatingSalaryInfoDto[tenantIdValue];
        }

        public GeneratePayslipResultDto GeneratePayslipsTryCacth(CollectPayslipDto input)
        {
            CalculatingSalaryInfoDto calculatingSalaryInfoDto = GetCalculatingSalaryInfoDto(input.TenantId);

            try
            {
                if (calculatingSalaryInfoDto.IsRuning)
                {
                    return new GeneratePayslipResultDto
                    {
                        ErrorList = new List<GenerateErrorDto>() { new GenerateErrorDto { Message = "Is Running => stop" } },
                    };
                }
                calculatingSalaryInfoDto.IsRuning = true;
                return GeneratePayslips(input);
            }
            catch (Exception e)
            {

                return new GeneratePayslipResultDto
                {
                    ErrorList = new List<GenerateErrorDto>() { new GenerateErrorDto { Message = e.Message } },
                };
            }
            finally
            {
                calculatingSalaryInfoDto.IsRuning = false;
            }
        }

        public GeneratePayslipResultDto GeneratePayslips(CollectPayslipDto input)
        {
            CalculatingSalaryInfoDto calculatingSalaryInfoDto = GetCalculatingSalaryInfoDto(input.TenantId);

            calculatingSalaryInfoDto.ProgressInfo = "Started calculating salary";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Start" });

            var result = new GeneratePayslipResultDto
            {
                ErrorList = new List<GenerateErrorDto>(),
            };

            var payroll = WorkScope.GetAll<Payroll>()
                .Where(x => x.Id == input.PayrollId)
                .Select(x => new PayrollDto
                {
                    Id = x.Id,
                    ApplyMonth = x.ApplyMonth,
                    NormalWorkingDay = x.NormalWorkingDay,
                    OpenTalk = x.OpenTalk,
                    Status = x.Status
                })
                .FirstOrDefault();

            CheckValidGeneratePayslips(payroll);

            var firstDayOfPayroll = DateTimeUtils.GetFirstDayOfMonth(payroll.ApplyMonth);
            var lastDayOfPayroll = DateTimeUtils.GetLastDayOfMonth(firstDayOfPayroll);

            calculatingSalaryInfoDto.ProgressInfo = "Collecting employee info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting employee info" });


            List<EmployeeToCalDto> employees = CollectPayslipEmployee(firstDayOfPayroll, lastDayOfPayroll, input.EmployeeIds);

            Logger.Info($"GeneratePayslips(): CollectPayslipEmployee done with result count = {employees?.Count ?? 0}");
            calculatingSalaryInfoDto.ProgressInfo = $"Done collecting employee info with result count={employees?.Count ?? 0}";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting employee info" });

            if (employees.IsEmpty())
            {
                throw new UserFriendlyException($"Không có nhân viên nào thỏa mãn điều kiện để nhận lương tháng này: {JsonConvert.SerializeObject(input)}");
            }

            var employeeEmails = employees.Select(s => s.NormalizeEmailAddress.ToLower().Trim()).ToList();
            var employeeIds = employees.Select(s => s.EmployeeId).ToList();

            var debtError = GetDebtPlanError(employeeIds);
            
            if(debtError.Count > 0)
            {
                result.ErrorList.AddRange(debtError);
                _calculateSalaryHub.SendMessage(new { Message = debtError, Process = "", Status = "Error" });
                return result;
            }

            calculatingSalaryInfoDto.ProgressInfo = "Deleting all payslips in the payroll";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Deleting payslips" });

            DeletePayslips(input.PayrollId, input.EmployeeIds);

            calculatingSalaryInfoDto.ProgressInfo = "Done deleting all payslips in the payroll";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "" });

            Logger.Info($"GeneratePayslips(): Deleted Payslips: input={JsonConvert.SerializeObject(input)}");



            var year = payroll.ApplyMonth.Year;
            var month = payroll.ApplyMonth.Month;
            var inputCollectData = new InputCollectDataForPayslipDto
            {
                UpperEmails = employeeEmails,
                Year = year,
                Month = month
            };

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Benefit info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });
            Dictionary<long, List<CollectBenefitForPayslipDetailDto>> dicBenefits = CollectBenefit(firstDayOfPayroll, lastDayOfPayroll);
            Logger.Info($"GeneratePayslips(): CollectBenefit done, dicBenefits.Count = {dicBenefits?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Punishment info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });


            Dictionary<long, List<InputPayslipDetailDto>> dicPunishments = CollectPunishment(payroll.ApplyMonth);
            Logger.Info($"GeneratePayslips(): CollectPunishment done, dicPunishments.Count = {dicPunishments?.Count ?? 0}");



            calculatingSalaryInfoDto.ProgressInfo = $"Collecting refund info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });
            Dictionary<long, List<InputPayslipDetailDto>> dicRefunds = CollectRefund(payroll.ApplyMonth);
            Logger.Info($"GeneratePayslips(): CollectRefund done, dicPunishments.Count = {dicRefunds?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Debt info";

            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<long, List<InputPayslipDetailDto>> dicDebtPlans = CollectDebtPlan(payroll.ApplyMonth);
            Logger.Info($"GeneratePayslips(): CollectDebtPlan done, dicDebtPlans.Count = {dicDebtPlans?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Bonus info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<long, List<InputPayslipDetailDto>> dicBonuses = CollectBonus(payroll.ApplyMonth);
            Logger.Info($"GeneratePayslips(): CollectBonus done, dicBonuses.Count = {dicBonuses?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Salary Input info";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<long, List<SalaryInputForPayslipDto>> dicSalaryInput = CollectSalaryInput(employees, firstDayOfPayroll, lastDayOfPayroll);
            Logger.Info($"GeneratePayslips(): CollectSalaryInput done, dicSalaryInput.Count = {dicSalaryInput?.Count ?? 0}");

            //Collect from TS
            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Setting Off Date from Timesheet tool";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            HashSet<DateTime> settingOffDates = _timesheetService.GetSettingOffDates(year, month);
            Logger.Info($"GeneratePayslips(): _timesheetService.GetSettingOffDates done, settingOffDates.Count = {settingOffDates?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting timesheet OT from Timesheet tool";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<string, TimesheetOTDto> dicTimesheetOTs = CollectOTTimesheets(inputCollectData);
            Logger.Info($"GeneratePayslips(): CollectOTTimesheets done, dicTimesheetOTs.Count = {dicTimesheetOTs?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting ChamCong Info from Timesheet tool";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<string, ChamCongInfoDto> dicChamCongInfo = CollectChamCongInfo(inputCollectData);
            Logger.Info($"GeneratePayslips(): CollectChamCongInfo done, dicChamCongInfo.Count = {dicChamCongInfo?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Collecting Request Off/Remote/Onsite Info from Timesheet tool";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Collecting data" });

            Dictionary<string, GetRequestDateDto> dicRequestDates = CollectRequestDates(inputCollectData);
            Logger.Info($"GeneratePayslips(): CollectRequestDates done, dicRequestDates.Count = {dicRequestDates?.Count ?? 0}");

            calculatingSalaryInfoDto.ProgressInfo = $"Starting Calculate Salary for {employees.Count} employees";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Calculating" });


            List<NCCSalaryCalculator> listNCCSalaryCalculator = new List<NCCSalaryCalculator>();

            int index = 0;
            var count = employees.Count;
            foreach (var employee in employees)
            {
                try
                {
                    var employeeId = employee.EmployeeId;
                    var employeeEmail = employee.NormalizeEmailAddress;
                    calculatingSalaryInfoDto.ProgressInfo = $"Calculating for: {employeeEmail} - {index}/{count}";
                    _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = $"{index}/{count}", Status = "Calculating" });

                    List<PayslipDetail> payslipDetail = new List<PayslipDetail>();
                    var inputSalaries = dicSalaryInput.ContainsKey(employeeId) ? dicSalaryInput[employeeId] : null;

                    if (inputSalaries.IsEmpty())
                    {
                        result.ErrorList.Add(new GenerateErrorDto { Message = $"employee: {employee.NormalizeEmailAddress} don't have salary input", ReferenceId = employee.EmployeeId });
                        continue;
                    }

                    var nccSalaryCaculator = new NCCSalaryCalculator
                    {
                        InputPayroll = payroll,
                        InputEmployee = employee,
                        InputSettingOffDates = settingOffDates,
                        InputOffDates = dicRequestDates.ContainsKey(employeeEmail) ? dicRequestDates[employeeEmail].OffDates : null,
                        InputWorkAtHomeOnlyDates = dicRequestDates.ContainsKey(employeeEmail) ? dicRequestDates[employeeEmail].WorkAtHomeOnlyDates : null,
                        InputOpenTalkDates = dicChamCongInfo.ContainsKey(employeeEmail) ? dicChamCongInfo[employeeEmail].OpenTalkDates : null,
                        InputNormalWorkingDates = dicChamCongInfo.ContainsKey(employeeEmail) ? dicChamCongInfo[employeeEmail].NormalWorkingDates : null,
                        InputOTs = dicTimesheetOTs.ContainsKey(employeeEmail) ? dicTimesheetOTs[employeeEmail].ListOverTimeHour : null,
                        InputOffDateLastMonth = dicRequestDates.ContainsKey(employeeEmail) ? dicRequestDates[employeeEmail].OffDateLastMonth : null,
                        InputSalaries = inputSalaries,
                        InputBenefits = dicBenefits.ContainsKey(employeeId) ? dicBenefits[employeeId] : null,
                        InputBonuses = dicBonuses.ContainsKey(employeeId) ? dicBonuses[employeeId] : null,
                        InputPunishments = dicPunishments.ContainsKey(employeeId) ? dicPunishments[employeeId] : null,
                        InputDebts = dicDebtPlans.ContainsKey(employeeId) ? dicDebtPlans[employeeId] : null,
                        InputRefunds = dicRefunds.ContainsKey(employeeId) ? dicRefunds[employeeId] : null
                    };

                    nccSalaryCaculator.CalculateSalary();
                    Logger.Debug(JsonConvert.SerializeObject(nccSalaryCaculator));

                    listNCCSalaryCalculator.Add(nccSalaryCaculator);
                    index++;
                }
                catch (Exception ex)
                {
                    var errorMessage = $"index: {index}, employeeId: {employee.EmployeeId}, email: {employee.NormalizeEmailAddress}, error: {ex.Message}";
                    Logger.Error(errorMessage);
                    result.ErrorList.Add(new GenerateErrorDto { Message = errorMessage });
                }
            }

            Logger.Info($"GeneratePayslips(): Done calucating salary for {count} employee");

            calculatingSalaryInfoDto.ProgressInfo = $"Inserting the result into DB";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "" });

            result.PayslipIds = InsertCalculateSalaryResultToDB(listNCCSalaryCalculator, input.TenantId);

            calculatingSalaryInfoDto.ProgressInfo = $"Done Calculating Salary for payroll: #{input.PayrollId} {DateTimeUtils.ToMMYYYY(payroll.ApplyMonth)}";
            _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Done" });

            if (result.ErrorList != null && result.ErrorList.Count > 0)
            {
                _calculateSalaryHub.SendMessage(new { Message = result, Process = "", Status = "Error" });
            }

            calculatingSalaryInfoDto.IsRuning = false;

            return result;
        }

        private List<long> InsertCalculateSalaryResultToDB(List<NCCSalaryCalculator> listNCCSalaryCalculator, int? tenantId)
        {
            CalculatingSalaryInfoDto calculatingSalaryInfoDto = GetCalculatingSalaryInfoDto(tenantId);
            List<long> payslipIds = new List<long>();
            var count = listNCCSalaryCalculator.Count;
            var index = 1;
            foreach (var nccSalaryCaculator in listNCCSalaryCalculator)
            {
                calculatingSalaryInfoDto.ProgressInfo = $"Inserting into DB {index}/{count}: {nccSalaryCaculator.InputEmployee.NormalizeEmailAddress}";
                _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = $"{index}/{count}", Status = "Inserting DB" });

                var payslipId = InsertPayslipEmployee(nccSalaryCaculator);
                payslipIds.Add(payslipId);

                calculatingSalaryInfoDto.ProgressInfo = $"Done inserting into DB {index}/{count}: {nccSalaryCaculator.InputEmployee.NormalizeEmailAddress}";
                _calculateSalaryHub.SendMessage(new { Message = calculatingSalaryInfoDto.ProgressInfo, Process = "", Status = "Inserting DB" });
                index++;
            }
            CurrentUnitOfWork.SaveChanges();
            return payslipIds;
        }

        public List<string> TestNccCalculator(InpuTestNccCalculator input)
        {
            var nccCalculator = new NCCSalaryCalculator
            {
                InputPayroll = input.InputPayroll,
                InputEmployee = input.InputEmployee,
                InputSettingOffDates = input.InputSettingOffDates,
                InputOffDates = input.InputOffDates,
                InputWorkAtHomeOnlyDates = input.InputWorkAtHomeOnlyDates,
                InputOpenTalkDates = input.InputOpenTalkDates,
                InputOTs = input.InputOTs,
                InputOffDateLastMonth = input.InputOffDateLastMonth,
                InputSalaries = input.InputSalaries,
                InputBenefits = input.InputBenefits,
                InputBonuses = input.InputBonuses,
                InputPunishments = input.InputPunishments,
                InputDebts = input.InputDebts,
            };

            nccCalculator.CalculateSalary();
            List<string> result = new();

            CalculateResult output = new CalculateResult
            {
                NormalSalary = nccCalculator.OutputPayslipDate.NormalSalary,
                OTsalary = nccCalculator.OutputPayslipDate.OTSalary,
                MaternityLeavesalary = nccCalculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money),
                NormalDay = nccCalculator.OutputPayslipDto.NormalDay,
                OpentalkCount = nccCalculator.OutputPayslipDto.OpentalkCount,
                OffDay = nccCalculator.OutputPayslipDto.OffDay,
                OTHour = nccCalculator.OutputPayslipDto.OTHour,
                RefundLeaveDay = nccCalculator.OutputPayslipDto.RefundLeaveDay,
                AddedLeaveDay = nccCalculator.OutputPayslipDto.AddedLeaveDay,
                LeaveDayAfter = nccCalculator.OutputPayslipDto.RemainLeaveDayAfter,
                WorkAtOfficeOrOnsiteDay = nccCalculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay,
                TotalBenefit = nccCalculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money),
                TotalBonus = nccCalculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money),
                TotalPunishment = nccCalculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money),
                TotalDebt = nccCalculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money),
            };
            output.TotalRealSalary = nccCalculator.OutputPayslipDto.Salary;


            result.Add("NormalSalary: " + GetResultMessage(output.NormalSalary, input.NormalSalary));
            result.Add("OTSalary: " + GetResultMessage(output.OTsalary, input.OtSalary));
            result.Add("MaternitySalary: " + GetResultMessage(output.MaternityLeavesalary, input.MaternityLeavesalary));
            result.Add("NormalWorkingDays: " + GetResultMessage(output.NormalDay, input.NormalWorkingDay));
            result.Add("OpenTalks: " + GetResultMessage(output.OpentalkCount, input.OpenTalkCount));
            result.Add("OffDays: " + GetResultMessage(output.OffDay, input.OffDay));
            result.Add("OtHour: " + GetResultMessage(output.OTHour, input.OtHour));
            result.Add("RefundLeaveDay: " + GetResultMessage(output.RefundLeaveDay, input.RefunleaveDay));
            result.Add("MonthlyAddedLeaveDay: " + GetResultMessage(output.AddedLeaveDay, input.MonthlyAddedLeaveDay));
            result.Add("LeaveDayAfter: " + GetResultMessage(output.LeaveDayAfter, input.LeaveDayAfter));
            result.Add("DayHasRemoteBenefit: " + GetResultMessage(output.WorkAtOfficeOrOnsiteDay, input.WorkAtOfficeOrOnsiteDay));
            result.Add("TotalBenefit: " + GetResultMessage(output.TotalBenefit, input.TotalBenefit));
            result.Add("TotalBonus: " + GetResultMessage(output.TotalBonus, input.TotalBonus));
            result.Add("TotalPunishment: " + GetResultMessage(output.TotalPunishment, input.TotalPunishment));
            result.Add("TotalDebt: " + GetResultMessage(output.TotalDebt, input.TotalDebt));
            result.Add("TotalRealSalary: " + GetResultMessage(output.TotalRealSalary, input.TotalRealSalary));
            return result;
        }

        private string GetResultMessage(double output, double expect)
        {
            return IsEqual(output, expect) ? "Passed" : $"Failed (output: {output}, expect: {expect})";
        }

        private bool IsEqual(double output, double expect)
        {
            if (Math.Round(output) == Math.Round(expect))
            {
                return true;
            };
            return false;
        }

        /// <summary>
        /// Insert 1 row into Payslip,
        /// n row into PayslipDetail, 
        /// 0 or n row into DebtPaid,
        /// 1 or n row into PayslipSalary (Input Salaries for calculate)
        /// 0 or n row into PayslipTeam
        /// </summary>
        /// <param name="nccSalaryCaculator"></param>
        /// <returns>payslipId</returns>
        private long InsertPayslipEmployee(NCCSalaryCalculator nccSalaryCaculator)
        {
            var payslipEntity = ObjectMapper.Map<Payslip>(nccSalaryCaculator.OutputPayslipDto);
            payslipEntity.ComplainDeadline = null;
            payslipEntity.Salary = CommonUtil.RoundMoneyVND(payslipEntity.Salary);

            long payslipId = WorkScope.InsertAndGetId(payslipEntity);

            var payslipDetails = new List<PayslipDetail>();

            var debtPaids = new List<DebtPaid>();
            foreach (var dto in nccSalaryCaculator.OutputAllPayslipDetails)
            {
                var entity = ObjectMapper.Map<PayslipDetail>(dto);
                entity.Money = entity.Money;
                entity.PayslipId = payslipId;
                payslipDetails.Add(entity);
                entity.Id = WorkScope.InsertAndGetId(entity);

                if (dto.Type == PayslipDetailType.Debt)
                {
                    debtPaids.Add(new DebtPaid
                    {
                        DebtId = dto.ReferenceId.Value,
                        Date = nccSalaryCaculator.InputPayroll.ApplyMonth,
                        Money = Math.Abs(entity.Money),
                        PaymentType = DebtPaymentType.Salary,
                        Note = $"Trừ khoản vay #{dto.ReferenceId.Value} vào lương tháng {DateTimeUtils.ToMMYYYY(nccSalaryCaculator.InputPayroll.ApplyMonth)}",
                        PayslipDetailId = entity.Id,
                    });
                }
            }
            WorkScope.InsertRange(debtPaids);

            var payslipSalaries = new List<PayslipSalary>();
            nccSalaryCaculator.InputSalaries.ForEach(dto =>
            {
                var entity = new PayslipSalary
                {
                    PayslipId = payslipId,
                    Salary = dto.Salary,
                    Date = dto.Date,
                    Note = CommonUtil.GenerateInputSalaryNote(dto.UserType, dto.SalaryType)
                };
                payslipSalaries.Add(entity);
            });

            WorkScope.InsertRange(payslipSalaries);

            if (nccSalaryCaculator.InputEmployee.Teams != null && nccSalaryCaculator.InputEmployee.Teams.Count > 0)
            {
                var payslipTeams = new List<PayslipTeam>();
                nccSalaryCaculator.InputEmployee.Teams.ForEach(teamId =>
                {
                    payslipTeams.Add(new PayslipTeam
                    {
                        PayslipId = payslipId,
                        TeamId = teamId
                    });
                });

                WorkScope.InsertRange(payslipTeams);
            }


            return payslipId;
        }

        //TODO test case [should throw exception with invalid/not exits input]
        public void DeletePayslips(long payrollId, List<long> employeeIds)
        {
            var deletePayslips = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .WhereIf(employeeIds != null && employeeIds.Count > 0, x => employeeIds.Contains(x.EmployeeId))
                .ToList();

            if (deletePayslips == null || deletePayslips.IsEmpty())
            {
                return;
            }
            var playslipIds = deletePayslips.Select(s => s.Id).ToList();

            var payslipDetails = WorkScope.GetAll<PayslipDetail>()
                .Where(s => playslipIds.Contains(s.PayslipId))
                .ToList();

            var payslipDetailIds = payslipDetails.Select(s => s.Id).ToList();

            var payslipSalaries = WorkScope.GetAll<PayslipSalary>()
                .Where(s => playslipIds.Contains(s.PayslipId))
                .ToList();

            var debtPaids = WorkScope.GetAll<DebtPaid>()
                .Where(s => s.PaymentType == DebtPaymentType.Salary)
                .Where(s => s.PayslipDetailId.HasValue && payslipDetailIds.Contains(s.PayslipDetailId.Value))
                .ToList();

            var payslipTeams = WorkScope.GetAll<PayslipTeam>()
                .Where(s => playslipIds.Contains(s.PayslipId))
                .ToList();

            var sessionUserId = AbpSession.UserId;

            foreach (var payslip in deletePayslips)
            {
                payslip.IsDeleted = true;
                payslip.DeleterUserId = sessionUserId;
                payslip.DeletionTime = DateTimeUtils.GetNow();
            }

            foreach (var item in payslipDetails)
            {
                item.IsDeleted = true;
                item.DeleterUserId = sessionUserId;
                item.DeletionTime = DateTimeUtils.GetNow();
            }

            foreach (var item in payslipSalaries)
            {
                item.IsDeleted = true;
                item.DeleterUserId = sessionUserId;
                item.DeletionTime = DateTimeUtils.GetNow();
            }

            foreach (var item in debtPaids)
            {
                item.IsDeleted = true;
                item.DeleterUserId = sessionUserId;
                item.DeletionTime = DateTimeUtils.GetNow();
            }

            foreach (var item in payslipTeams)
            {
                item.IsDeleted = true;
                item.DeleterUserId = sessionUserId;
                item.DeletionTime = DateTimeUtils.GetNow();
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public Dictionary<string, TimesheetOTDto> CollectOTTimesheets(InputCollectDataForPayslipDto input)
        {
            var results = _timesheetService.GetOTTimesheets(input);

            if (results == default)
            {
                return new Dictionary<string, TimesheetOTDto>();
            }

            return results.GroupBy(x => x.NormalizedEmailAddress)
                 .ToDictionary(x => x.Key, x => x.FirstOrDefault());
        }


        public Dictionary<string, ChamCongInfoDto> CollectChamCongInfo(InputCollectDataForPayslipDto input)
        {
            var results = _timesheetService.GetChamCongInfo(input);

            if (results == default)
            {
                return new Dictionary<string, ChamCongInfoDto>();
            }
            return results.GroupBy(x => x.NormalizeEmailAddress)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());
        }

        public Dictionary<string, GetRequestDateDto> CollectRequestDates(InputCollectDataForPayslipDto input)
        {
            var requestDates = _timesheetService.GetAllRequestDays(input);
            if (requestDates == default)
            {
                return new Dictionary<string, GetRequestDateDto>();
            }

            return requestDates
                .GroupBy(x => x.NormalizedEmailAddress)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());
        }

        public async Task<long> Delete(long id)
        {
            var payslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (payslip == default)
            {
                throw new UserFriendlyException($"Can't find payslip with id {id}");
            }

            var deleteEmployeeIds = new List<long>();

            deleteEmployeeIds.Add(payslip.EmployeeId);

            DeletePayslips(payslip.PayrollId, deleteEmployeeIds);

            return id;
        }

        public Dictionary<long, List<InputPayslipDetailDto>> CollectPunishment(DateTime payrollApplyDate)
        {
            return WorkScope.GetAll<PunishmentEmployee>()
                 .Where(x => x.Punishment.Date.Month == payrollApplyDate.Month)
                 .Where(x => x.Punishment.Date.Year == payrollApplyDate.Year)
                 .Select(x => new InputPayslipDetailDto
                 {
                     EmployeeId = x.EmployeeId,
                     Note = x.Note,
                     Money = x.Money,
                     ReferenceId = x.Id,
                 })
                 .AsEnumerable()
                 .GroupBy(x => x.EmployeeId)
                 .ToDictionary(x => x.Key, x => x.ToList());
        }

        public Dictionary<long, List<InputPayslipDetailDto>> CollectRefund(DateTime payrollApplyDate)
        {
            return WorkScope.GetAll<RefundEmployee>()
                 .Where(x => x.Refund.Date.Month == payrollApplyDate.Month)
                 .Where(x => x.Refund.Date.Year == payrollApplyDate.Year)
                 .Select(x => new InputPayslipDetailDto
                 {
                     EmployeeId = x.EmployeeId,
                     Note = x.Note,
                     Money = x.Money,
                     ReferenceId = x.RefundId
                 })
                 .AsEnumerable()
                 .GroupBy(x => x.EmployeeId)
                 .ToDictionary(x => x.Key, x => x.ToList());
        }

        public Dictionary<long, List<InputPayslipDetailDto>> CollectBonus(DateTime payrollApplyDate)
        {
            return WorkScope.GetAll<BonusEmployee>()
                 .Where(x => x.Bonus.ApplyMonth.Month == payrollApplyDate.Month)
                 .Where(x => x.Bonus.ApplyMonth.Year == payrollApplyDate.Year)
                 .Select(x => new InputPayslipDetailDto
                 {
                     EmployeeId = x.EmployeeId,
                     Note = x.Note,
                     Money = x.Money,
                     ReferenceId = x.Id
                 })
                 .AsEnumerable()
                 .GroupBy(x => x.EmployeeId)
                 .ToDictionary(x => x.Key, x => x.ToList());
        }

        public Dictionary<long, List<CollectBenefitForPayslipDetailDto>> CollectBenefit(DateTime firstDayOfPayroll, DateTime lastDayOfPayroll)
        {
            return WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.Benefit.IsActive)
                .Where(x => x.Benefit.Type == BenefitType.CheDoChung
                || (x.StartDate.Date <= lastDayOfPayroll && (x.EndDate == null || x.EndDate >= firstDayOfPayroll)))
                 .Select(x => new CollectBenefitForPayslipDetailDto
                 {
                     ReferenceId = x.Id,
                     EmployeeId = x.EmployeeId,
                     Note = x.Benefit.Name,
                     Money = x.Benefit.Money,
                     BenefitType = x.Benefit.Type,
                     StartDate = x.StartDate,
                     EndDate = x.EndDate != null ? x.EndDate : lastDayOfPayroll
                 })
                 .AsEnumerable()
                 .GroupBy(x => x.EmployeeId)
                 .ToDictionary(x => x.Key, x => x.ToList());
        }




        public Dictionary<long, List<InputPayslipDetailDto>> CollectDebtPlan(DateTime payrollApplyDate)
        {
            return WorkScope.GetAll<DebtPaymentPlan>()
                 .Where(x => x.Date.Month == payrollApplyDate.Month)
                 .Where(x => x.Date.Year == payrollApplyDate.Year)
                 .Where(x => x.Debt.Status == DebtStatus.Inprogress)
                 .Where(x => x.Debt.PaymentType == DebtPaymentType.Salary)
                 .Where(x => x.Money > 0)
                 .Select(s => new
                 {
                     PlanMoney = s.Money,
                     s.Debt.EmployeeId,
                     s.DebtId,
                     s.Debt.Money,
                     s.Debt.InterestRate,
                     s.Debt.StartDate,
                     s.Debt.EndDate,
                     s.Id
                 }).AsEnumerable()
                 .Select(x => new InputPayslipDetailDto
                 {
                     EmployeeId = x.EmployeeId,
                     Note = $"Trừ lương hàng tháng vào khoản vay {x.Money}đ, lãi suất {x.InterestRate}%, từ {DateTimeUtils.ToString(x.StartDate)} - {DateTimeUtils.ToString(x.EndDate)}",
                     Money = x.PlanMoney,
                     ReferenceId = x.DebtId,
                 }).GroupBy(x => x.EmployeeId)
                 .ToDictionary(x => x.Key, x => x.ToList());
        }

        private IEnumerable<EmployeeWorkingStatusDto> GetEmployeeIdsToPayroll(DateTime firstDayOfPayroll, DateTime lastDayOfPayroll, List<long> forEmployeeIds)
        {
            var last6MonthsDate = firstDayOfPayroll.AddMonths(-6);

            var results = WorkScope.GetAll<EmployeeWorkingHistory>()
                .WhereIf(forEmployeeIds != null && forEmployeeIds.Count > 0, x => forEmployeeIds.Contains(x.EmployeeId))
                 .Where(x => x.DateAt.Date <= lastDayOfPayroll)
                 .GroupBy(s => s.EmployeeId)
                 .Select(s => new EmployeeWorkingStatusDto
                 {
                     EmployeeId = s.Key,
                     Status = s.OrderByDescending(x => x.DateAt).FirstOrDefault().Status,
                     DateAt = s.OrderByDescending(x => x.DateAt).FirstOrDefault().DateAt,
                 }).AsEnumerable()
                 .Where(x =>
                        x.Status == EmployeeStatus.Working
                        || (x.Status == EmployeeStatus.MaternityLeave && x.DateAt > last6MonthsDate)//check bug vung bien
                        || ((x.Status == EmployeeStatus.Quit || x.Status == EmployeeStatus.Pausing) && x.DateAt > firstDayOfPayroll))
                 .AsEnumerable();

            return results;
        }

        private List<EmployeeToCalDto> GetEmployeeInfo(IEnumerable<EmployeeWorkingStatusDto> employeeList)
        {
            if (employeeList == null || employeeList.IsEmpty())
            {
                return null;
            }

            var dicEmployeeStatus = employeeList.ToDictionary(s => s.EmployeeId);
            var employeeIds = dicEmployeeStatus.Keys.AsEnumerable();
            var results = WorkScope.GetAll<Employee>()
                .Where(s => employeeIds.Contains(s.Id))
                .Select(s => new EmployeeToCalDto
                {
                    EmployeeId = s.Id,
                    NormalizeEmailAddress = s.Email.ToUpper().Trim(),
                    BranchId = s.BranchId,
                    JobPositionId = s.JobPositionId,
                    LevelId = s.LevelId,
                    RemainLeaveDay = s.RemainLeaveDay,
                    UserType = s.UserType,
                    BankId = s.BankId,
                    BankAccountNumber = s.BankAccountNumber,
                    BankName = s.Bank.Name,
                    Teams = s.EmployeeTeams.Select(x => x.TeamId).ToList(),
                }).ToList();

            results.ForEach(s =>
            {
                s.DateAt = dicEmployeeStatus[s.EmployeeId].DateAt;
                s.Status = dicEmployeeStatus[s.EmployeeId].Status;
            });

            return results;
        }
        public List<EmployeeToCalDto> CollectPayslipEmployee(DateTime firstDayOfPayroll, DateTime lastDayOfPayroll, List<long> forEmployeeIds)
        {
            IEnumerable<EmployeeWorkingStatusDto> employees = GetEmployeeIdsToPayroll(firstDayOfPayroll, lastDayOfPayroll, forEmployeeIds);
            return GetEmployeeInfo(employees);
        }

        public List<PayslipContractSalaryDto> GetPayslipSalary(long payslipId)
        {
            return WorkScope.GetAll<PayslipSalary>()
                  .Where(x => x.PayslipId == payslipId)
                  .Select(x => new PayslipContractSalaryDto
                  {
                      FromDate = x.Date,
                      Salary = x.Salary,
                      Note = x.Note
                  }).ToList();
        }

        /*public Task<List<PenaltyNotCollectedDto>> GetAllPenaltyNotCollectedDto()
        {

        }*/

        private IEnumerable<GetchangeRqEmployeeForPayslip> GetIEnumableGetchangeRqEmployeeForPayslip(List<EmployeeToCalDto> employeeList)
        {
            var employeeIds = employeeList.Select(x => x.EmployeeId).ToList();

            return GetChangeRequestEmployees(employeeIds);

        }


        public Dictionary<long, List<SalaryInputForPayslipDto>> CollectSalaryInput(List<EmployeeToCalDto> employeeList, DateTime firstDayOfPayroll, DateTime lastDayOfPayroll)
        {
            var dicResult = GetIEnumableGetchangeRqEmployeeForPayslip(employeeList)
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(s => s.Key
                , s => s.OrderByDescending(x => x.ApplyDate)
                .Select(x => new SalaryInputForPayslipDto
                {
                    Date = x.ApplyDate.Date,
                    Salary = x.Salary,
                    SalaryType = x.Type,
                    UserType = x.UserType
                }).ToList());

            foreach (var employeeId in dicResult.Keys)
            {
                var listSalary = dicResult[employeeId];

                var listSalaryResult = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDayOfPayroll, lastDayOfPayroll);

                dicResult[employeeId] = listSalaryResult;
            }

            return dicResult;
        }


        public IEnumerable<GetchangeRqEmployeeForPayslip> GetChangeRequestEmployees(List<long> employeeIds)
        {
            return WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.Type == SalaryRequestType.Initial
                 || x.Type == SalaryRequestType.MaternityLeave
                 || x.Type == SalaryRequestType.BackToWork
                 || x.Type == SalaryRequestType.StopWorking
                 || (x.SalaryChangeRequest != null && x.SalaryChangeRequest.Status == SalaryRequestStatus.Executed))
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .Select(s => new GetchangeRqEmployeeForPayslip
                {
                    EmployeeId = s.EmployeeId,
                    ApplyDate = s.ApplyDate.Date,
                    Salary = s.ToSalary,
                    Type = s.Type,
                    UserType = s.ToUserType

                }).AsEnumerable();
        }


        public void ReCalculateAllPayslipFromDetail(long payrollId)
        {

            var payrollStatus = WorkScope.GetAll<Payroll>()
                .Where(s => s.Id == payrollId)
                .Select(s => s.Status).FirstOrDefault();

            if (!CommonUtil.IsCanUpdatePayslip(payrollStatus))
            {
                throw new UserFriendlyException($"Can't change payslip because the payroll status is {payrollStatus}. " +
                    $"You can change payslip if the payroll status is: {CommonUtil.ListPayrollStatusCanUpdateToString}");
            }

            var dicPayslipIdToSalary = WorkScope.GetAll<PayslipDetail>()
               .Where(x => x.Payslip.PayrollId == payrollId)
               .GroupBy(x => x.PayslipId)
               .Select(x => new
               {
                   PayslipId = x.Key,
                   Salary = x.Sum(s => s.Money)
               }).AsEnumerable()
               .ToDictionary(s => s.PayslipId, s => s.Salary);

            WorkScope.GetAll<Payslip>()
                .Where(s => s.PayrollId == payrollId)
                .ToList().ForEach(s => s.Salary = dicPayslipIdToSalary.ContainsKey(s.Id) ? dicPayslipIdToSalary[s.Id] : s.Salary);

            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<double> ReCalculatePayslipFromDetail(long payslipId)
        {
            var payslip = await WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == payslipId)
                .Select(s => new { PayrollStatus = s.Payroll.Status, Payslip = s })
                .FirstOrDefaultAsync();

            if (payslip == default)
            {
                throw new UserFriendlyException($"Not found payslip with Id {payslipId}");
            }

            if (!CommonUtil.IsCanUpdatePayslip(payslip.PayrollStatus))
            {
                throw new UserFriendlyException($"Can't change payslip because the payroll status is {payslip.PayrollStatus}. " +
                    $"You can change payslip if the payroll status is: {CommonUtil.ListPayrollStatusCanUpdateToString}");
            }

            var payslipSalary = WorkScope.GetAll<PayslipDetail>()
                .Where(s => s.PayslipId == payslipId)
                .Sum(s => s.Money);

            payslip.Payslip.Salary = payslipSalary;
            await WorkScope.UpdateAsync(payslip.Payslip);

            return payslipSalary;
        }


        public GeneratePayslipResultDto AddEmployeesToPayroll(CollectPayslipDto input)
        {
            var result = new GeneratePayslipResultDto{
                ErrorList = new List<GenerateErrorDto>()
            };
            if (input.EmployeeIds == null || input.EmployeeIds.IsEmpty())
            {
                throw new UserFriendlyException("You have to select atleast one employee");
            }

            var alreadyEmployeeInPayroll = WorkScope.GetAll<Payslip>()
                .Where(x => input.EmployeeIds.Contains(x.EmployeeId))
                .Where(x => x.PayrollId == input.PayrollId)
                .Select(x => new
                {
                    x.Employee.Email,
                    x.EmployeeId
                }).ToList();

            var alreadyEmployeeIds = alreadyEmployeeInPayroll.Select(x => x.EmployeeId);

            alreadyEmployeeInPayroll.ForEach(x => result.ErrorList.Add(new GenerateErrorDto { Message = $"{x.Email} already in the Payroll" }));

            input.EmployeeIds = input.EmployeeIds.Except(alreadyEmployeeIds).ToList();

            var generateResult = GeneratePayslips(input);

            if(generateResult.ErrorList != null && generateResult.ErrorList.Count > 0)
            {
                result.ErrorList.AddRange(generateResult.ErrorList);
            }

            return result;
        }


        public GeneratePayslipResultDto ReGeneratePayslip(long payslipId)
        {

            var input = WorkScope.GetAll<Payslip>().Where(s => s.Id == payslipId)
                .Select(s => new CollectPayslipDto
                {
                    PayrollId = s.PayrollId,
                    EmployeeIds = new List<long> { s.EmployeeId }
                }).FirstOrDefault();

            var result = GeneratePayslips(input);
            return result;

        }

        private void UpdatePayslipComplainDeadLine(List<Payslip> payslips, DateTime deadline)
        {
            foreach (var payslip in payslips)
            {
                payslip.ComplainDeadline = deadline;
            };
            CurrentUnitOfWork.SaveChanges();
        }

        public string SendMailToAllEmployee(SendMailAllEmployeeDto input)
        {
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Payslip);
            if (emailTemplate == default)
            {
                throw new UserFriendlyException($"Not found email template for payslip");
            }

            var listPayslip = WorkScope.GetAll<Payslip>()
                .Include(s => s.Employee)
                .Include(s => s.Payroll)
                .Where(x => x.PayrollId == input.PayrollId)
                .ToList();

            if (listPayslip.IsEmpty())
            {
                throw new UserFriendlyException($"There is no payslip in the payroll {input.PayrollId}");
            }

            UpdatePayslipComplainDeadLine(listPayslip, input.Deadline);

            var payslipIds = listPayslip.Select(s => s.Id).ToList();

            var dicPayslipIdToInputSalaries = GetDicInputPayslipSalaries(payslipIds);

            var dicPayslipIdToPayslipDetails = GetDicPayslipDetails(payslipIds);

            var timesheetUri = _timesheetConfig.Value.Uri;

            List<ResultTemplateEmail<InputPayslipMailTemplate>> emailPayslips = listPayslip.Select(payslip => new ResultTemplateEmail<InputPayslipMailTemplate>
            {
                Result = new InputPayslipMailTemplate
                {
                    EmployeeFullName = payslip.Employee.FullName,
                    PayrollMonth = payslip.Payroll.ApplyMonth.Month.ToString(),
                    PayrollYear = payslip.Payroll.ApplyMonth.Year.ToString(),
                    PayslipWorkingDay = payslip.NormalDay.ToString(),
                    PayslipOTHour = payslip.OTHour.ToString(),
                    PayslipOpentalk = payslip.OpentalkCount.ToString(),
                    PayrollWorkingDay = payslip.Payroll.NormalWorkingDay.ToString(),
                    PayrollOpentalkCount = payslip.Payroll.OpenTalk.ToString(),
                    PayslipRemainLeaveDayBefore = payslip.RemainLeaveDayBefore.ToString(),
                    PayslipAddedLeaveDay = payslip.AddedLeaveDay.ToString(),
                    PayslipRemainLeaveDayAfter = payslip.RemainLeaveDayAfter.ToString(),
                    PayslipRefundDays = payslip.RefundLeaveDay.ToString(),
                    PayslipOffDays = payslip.OffDay.ToString(),
                    TotalRealSalary = CommonUtil.FormatDisplayMoney(payslip.Salary),
                    SendToEmail = payslip.Employee.Email,
                    ListPayslipDetail = dicPayslipIdToPayslipDetails.ContainsKey(payslip.Id) ? dicPayslipIdToPayslipDetails[payslip.Id] : new List<PayslipDetailEmailDto>(),
                    ListPayslipSalary = dicPayslipIdToInputSalaries.ContainsKey(payslip.Id) ? dicPayslipIdToInputSalaries[payslip.Id] : new List<PayslipSalaryEmailDto>(),
                    ConfirmUrl = $"{timesheetUri}public/confirm-mail?id={payslip.Id}",
                    ComplainUrl = $"{timesheetUri}public/complain-mail?id={payslip.Id}",
                    ComplainDeadline = payslip.ComplainDeadline.HasValue ? payslip.ComplainDeadline.Value.ToString("HH:mm dd/MM/yyyy ") : "..."
                }
            }).ToList();


            var delaySendMail = 0;
            foreach (var payslip in emailPayslips)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(payslip.Result, emailTemplate);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += HRMv2Consts.DELAY_SEND_MAIL_SECOND;
            };

            return $"Started sending {emailPayslips.Count} email.";
        }

        private Dictionary<long, List<PayslipSalaryEmailDto>> GetDicInputPayslipSalaries(List<long> payslipIds)
        {
            var dicPayslipIdToInputSalaries = WorkScope.GetAll<PayslipSalary>()
               .Where(x => payslipIds.Contains(x.PayslipId))
               .Select(x => new PayslipSalaryEmailDto
               {
                   PayslipId = x.PayslipId,
                   Date = x.Date.Date,
                   Money = x.Salary,
                   Note = x.Note,
               })
               .ToList()
               .GroupBy(s => s.PayslipId)
               .ToDictionary(s => s.Key, s => s.ToList());

            return dicPayslipIdToInputSalaries;
        }

        private Dictionary<long, List<PayslipDetailEmailDto>> GetDicPayslipDetails(List<long> payslipIds)
        {
            var dicPayslipIdToPayslipDetails = WorkScope.GetAll<PayslipDetail>()
               .Where(x => payslipIds.Contains(x.PayslipId))
               .Select(x => new PayslipDetailEmailDto
               {
                   PayslipId = x.PayslipId,
                   Money = x.Money,
                   Note = x.Note
               })
               .ToList()
               .GroupBy(s => s.PayslipId)
               .ToDictionary(s => s.Key, s => s.ToList());

            return dicPayslipIdToPayslipDetails;
        }




        public void SendMailToOneEmployee(SendMailOneemployeeDto input)
        {
            var payslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .FirstOrDefault();

            if (payslip == default)
            {
                throw new UserFriendlyException($"Can't find payslip with id {input.PayslipId}");
            }

            payslip.ComplainDeadline = input.Deadline;
            WorkScope.UpdateAsync(payslip);

            _emailManager.Send(input.MailContent);
        }

        public GetPayslipMailContentDto GetPayslipMailTemplate(long payslipId)
        {
            var deadLine = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == payslipId)
                .Select(x => x.ComplainDeadline)
                .FirstOrDefault();

            MailPreviewInfoDto mailTemplate = _emailManager.GetEmailContentById(MailFuncEnum.Payslip, payslipId);
            return new GetPayslipMailContentDto
            {
                MailInfo = mailTemplate,
                Deadline = deadLine
            };
        }

        public async Task<UpdateDeadlineDto> UpdatePayslipDeadline(UpdateDeadlineDto input)
        {
            var payslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .FirstOrDefault();

            if (payslip == default)
            {
                throw new UserFriendlyException($"Can't find payslip with id {input.PayslipId}");
            }

            payslip.ComplainDeadline = input.Deadline;

            await WorkScope.UpdateAsync(payslip);

            return input;
        }


        public FileBase64Dto ExportTechcombank(long payrollId)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "Payslip-for-tech.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

            List<PayslipForBankDto> payslips = GetPayslipDataForExport(payrollId, true);

            if (payslips.IsEmpty() || payslips.Count == 0)
            {
                throw new UserFriendlyException("Payroll don't have any techcombank payslip to export");
            }

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataToExportTech(template, payslips);

                    string resultBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "Payslips-for-tech",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = resultBase64   
                    };
                }
            }
        }

        public FileBase64Dto ExportOutsideTech(long payrollId)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "Payslip-outside-tech.xlsx");
                
            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

            List<PayslipForBankDto> payslips = GetPayslipDataForExport(payrollId, false);

            if (payslips.IsEmpty() || payslips.Count == 0)
            {
                throw new UserFriendlyException("Payroll don't have payslip to export");
            }

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataToExportOutsideTech(template, payslips);

                    string fileBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "Payslip-ouside-tech",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }

        public async Task<FileBase64Dto> ExportPayroll(long payrollId, InputGetPayslipEmployeeDto input)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "Payroll.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

            input.GridParam.MaxResultCount = 1000000;
            List<GetPayslipEmployeeDto> payslips = (await GetPayslipEmployeePaging(payrollId, input)).Items.ToList();

            if (payslips.IsEmpty() || payslips.Count == 0)
            {
                throw new UserFriendlyException("Payroll don't have payslip to export");
            }

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataToExportPayroll(template, payslips);

                    string fileBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "Payroll",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }

        //TODO: test case [check exception with invalid/not exits payrollid]
        public FileBase64Dto ExportPayrollIncludeLastMonth(long payrollId)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "PayrollincludeLastMonth.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

            List<ExportPayrollIncludeLastMonthDto> payslips = GetPayslipEmployeeForExport(payrollId);

            if (payslips.IsEmpty() || payslips.Count == 0)
            {
                throw new UserFriendlyException("Payroll don't have payslip to export");
            }

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataExportPayrollIncludeLastMonth(template, payslips);

                    string fileBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "Payroll",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }


        private void FillDataToExportTech(ExcelPackage package, List<PayslipForBankDto> payslips)
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;

            foreach (var payslip in payslips)
            {
                worksheet.Cells[rowIndex, 1].Value = $"REF{rowIndex - 1}";
                worksheet.Cells[rowIndex, 2].Value = payslip.Salary;
                worksheet.Cells[rowIndex, 3].Value = payslip.EmployeeName;
                worksheet.Cells[rowIndex, 4].Value = payslip.BankAccountNumber;
                worksheet.Cells[rowIndex, 5].Value = payslip.Note;
                rowIndex++;
            }
        }


        private void FillDataToExportOutsideTech(ExcelPackage package, List<PayslipForBankDto> payslips)
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;

            foreach (var payslip in payslips)
            {
                worksheet.Cells[rowIndex, 1].Value = $"REF{(rowIndex - 1).ToString("D4")}";
                worksheet.Cells[rowIndex, 2].Value = payslip.Salary;
                worksheet.Cells[rowIndex, 3].Value = payslip.EmployeeName;
                worksheet.Cells[rowIndex, 4].Value = payslip.BankAccountNumber;
                worksheet.Cells[rowIndex, 5].Value = payslip.Note;
                worksheet.Cells[rowIndex, 6].Value = payslip.BankName;
                rowIndex++;
            }
        }

        private void FillDataToExportPayroll(ExcelPackage package, List<GetPayslipEmployeeDto> payslips)
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;

            foreach (var payslip in payslips)
            {
                var inputSalaries = string.Join("\n", payslip.ListInputSalary.Select(x => x.Salary));
                var bankInfoStr = $"{payslip.BankInfo.BankAccountNumber}\n{payslip.BankInfo.BankName}";
                var benefitStr = "";

                if(payslip.ListBenefit != null && payslip.ListBenefit.Count > 0)
                {
                    foreach (var benefit in payslip.ListBenefit)
                    {
                        benefitStr += $"{benefit.Money} {benefit.Note}\n";
                    }
                }

                worksheet.Cells[rowIndex, 1].Value = payslip.FullName;
                worksheet.Cells[rowIndex, 2].Value = payslip.Email;
                worksheet.Cells[rowIndex, 3].Value = inputSalaries;
                worksheet.Cells[rowIndex, 4].Value = payslip.RealSalary;
                worksheet.Cells[rowIndex, 5].Value = payslip.NormalSalary;
                worksheet.Cells[rowIndex, 6].Value = payslip.OTSalary;
                worksheet.Cells[rowIndex, 7].Value = payslip.MaternityLeaveSalary;
                worksheet.Cells[rowIndex, 8].Value = payslip.NormalDay;
                worksheet.Cells[rowIndex, 9].Value = payslip.OpentalkCount;
                worksheet.Cells[rowIndex, 10].Value = payslip.OffDay;
                worksheet.Cells[rowIndex, 11].Value = payslip.RefundLeaveDay;
                worksheet.Cells[rowIndex, 12].Value = payslip.LeaveDayBefore;
                worksheet.Cells[rowIndex, 13].Value = payslip.AddedLeaveDay;
                worksheet.Cells[rowIndex, 14].Value = payslip.RemainLeaveDays;
                worksheet.Cells[rowIndex, 15].Value = payslip.TotalBonus;
                worksheet.Cells[rowIndex, 16].Value = payslip.TotalPunishment;
                worksheet.Cells[rowIndex, 17].Value = payslip.TotalDebt;
                worksheet.Cells[rowIndex, 18].Value = benefitStr;
                worksheet.Cells[rowIndex, 19].Value = payslip.PayslipUserTypeName;
                worksheet.Cells[rowIndex, 20].Value = payslip.PayslipBanrch;
                worksheet.Cells[rowIndex, 21].Value = payslip.PayslipLevel;
                worksheet.Cells[rowIndex, 22].Value = payslip.PayslipPosition;
                worksheet.Cells[rowIndex, 23].Value = bankInfoStr;
                worksheet.Cells[rowIndex, 24].Value = payslip.ComplainNote;
                rowIndex++;
            }
        }

        private void FillDataExportPayrollIncludeLastMonth(ExcelPackage package, List<ExportPayrollIncludeLastMonthDto> payslips)
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;

            foreach (var payslip in payslips)
            {
                var inputSalaries = "";
                var inputSalaryLastMonth = "";
                var bankInfoStr = $"{payslip.BankAccountNumber}\n{payslip.BankName}";
                var benefitStr = "";
                int statusCol = 3;
                int inputThisMonthCol = 4;
                int inputLastMonthCol = 5;

                SetStatusCellColor(worksheet, rowIndex, statusCol, payslip.ExportStatus);
                SetInputSalaryCellFormat(worksheet, rowIndex, inputThisMonthCol, payslip.ListInputSalary);
                SetInputSalaryCellFormat(worksheet, rowIndex, inputLastMonthCol, payslip.ListInputSalaryLastMonth);

                if (payslip.ListInputSalary != null && payslip.ListInputSalary.Count > 0)
                {
                    inputSalaries = string.Join("\n", payslip.ListInputSalary.Select(x => x.Salary));
                }

                if (payslip.ListInputSalaryLastMonth != null && payslip.ListInputSalaryLastMonth.Count > 0)
                {
                    inputSalaryLastMonth = string.Join("\n", payslip.ListInputSalaryLastMonth.Select(x => x.Salary));
                }

                if (payslip.ListBenefit != null && payslip.ListBenefit.Count > 0)
                {
                    foreach (var benefit in payslip.ListBenefit)
                    {
                        benefitStr += $"{benefit.Money} {benefit.Note}\n";
                    }
                }

                worksheet.Cells[rowIndex, 1].Value = payslip.FullName;
                worksheet.Cells[rowIndex, 2].Value = payslip.Email;
                worksheet.Cells[rowIndex, 3].Value = payslip.ExportStatus;
                worksheet.Cells[rowIndex, 4].Value = inputSalaries;
                worksheet.Cells[rowIndex, 5].Value = inputSalaryLastMonth;
                worksheet.Cells[rowIndex, 6].Value = payslip.RealSalary;
                worksheet.Cells[rowIndex, 7].Value = payslip.RealSalaryLastMonth;
                worksheet.Cells[rowIndex, 8].Value = payslip.NormalSalary;
                worksheet.Cells[rowIndex, 9].Value = payslip.NormalSalaryLastMonth;
                worksheet.Cells[rowIndex, 10].Value = payslip.RemainLeaveDays;
                worksheet.Cells[rowIndex, 11].Value = payslip.LeaveDayLastMonth;
                worksheet.Cells[rowIndex, 12].Value = payslip.OTSalary;
                worksheet.Cells[rowIndex, 13].Value = payslip.MaternityLeaveSalary;
                worksheet.Cells[rowIndex, 14].Value = payslip.NormalDay;
                worksheet.Cells[rowIndex, 15].Value = payslip.OpentalkCount;
                worksheet.Cells[rowIndex, 16].Value = payslip.OffDay;
                worksheet.Cells[rowIndex, 17].Value = payslip.RefundLeaveDay;
                worksheet.Cells[rowIndex, 18].Value = payslip.LeaveDayBefore;
                worksheet.Cells[rowIndex, 19].Value = payslip.AddedLeaveDay;
                worksheet.Cells[rowIndex, 20].Value = payslip.TotalBonus;
                worksheet.Cells[rowIndex, 21].Value = payslip.TotalPunishment;
                worksheet.Cells[rowIndex, 22].Value = payslip.TotalDebt;
                worksheet.Cells[rowIndex, 23].Value = payslip.PayslipBanrch;
                worksheet.Cells[rowIndex, 24].Value = payslip.PayslipUserTypeName;
                worksheet.Cells[rowIndex, 25].Value = payslip.PayslipLevel;
                worksheet.Cells[rowIndex, 26].Value = payslip.PayslipPosition;
                worksheet.Cells[rowIndex, 27].Value = bankInfoStr;
                worksheet.Cells[rowIndex, 28].Value = benefitStr;
                worksheet.Cells[rowIndex, 29].Value = payslip.ComplainNote;
                rowIndex++;
            }
        }

        public void SetStatusCellColor(ExcelWorksheet worksheet, int row, int col, PayslipStatusForExport status)
        {
            if (status == PayslipStatusForExport.Both)
            {
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#007bff"));
            }
            if (status == PayslipStatusForExport.InThisMonthOnly)
            {
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#28a745"));
            }
            if (status == PayslipStatusForExport.InLastMonthOnly)
            {
                worksheet.Cells[row, col].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#dc3545"));
            }
        }

        public void SetInputSalaryCellFormat(ExcelWorksheet worksheet, int row, int col, List<InputsalaryDto> listInputSalary)
        {
            if (listInputSalary != null && listInputSalary.Count > 1)
            {
                worksheet.Cells[row, col].Style.Numberformat.Format = "@";
            }
        }

        private List<PayslipForBankDto> GetPayslipDataForExport(long payrollId, bool isTechcombank)
        {
            var techcombankId = WorkScope.GetAll<Bank>()
                 .Where(x => x.Code.ToLower().Trim() == "techcombank")
                 .Select(x => x.Id)
                 .FirstOrDefault();

            if (isTechcombank && techcombankId == default)
            {
                throw new UserFriendlyException("Không tìm thấy bản ghi bank có code.ToLower() là techcombank");
            }

            var query = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Where(x => x.Salary > 0);

            query = isTechcombank ? query.Where(x => x.BankId == techcombankId) : query.Where(x => x.BankId != techcombankId);

            return query
                .Select(x => new PayslipForBankDto
                {
                    EmployeeName = CommonUtil.RemoveSign4VietnameseString(x.Employee.FullName).ToUpper().Trim(),
                    BankName = x.Bank.Name,
                    BankAccountNumber = x.BankAccountNumber,
                    Salary = x.Salary,
                    Note = $"NCC THANH TOAN LUONG THANG {x.Payroll.ApplyMonth.Month}.{x.Payroll.ApplyMonth.Year}",
                }).ToList();
        }

        private void ValidUpdateRemainLeaveDaysAfter([FromForm] InputToUpdateRemainLeaveDaysDto input)
        {
            if (input.File == null || !Path.GetExtension(input.File.FileName).Equals(".xlsx"))
            {
                throw new UserFriendlyException("File upload is invalid");
            }
        }


         public async Task<Object> UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary([FromForm] InputToUpdateRemainLeaveDaysDto input)
         {
            ValidUpdateRemainLeaveDaysAfter(input);
            var failedList = new List<ResponseFailedDto>();
            var successList = new List<string>();

            var mapEmailToRemainLeaveDaysAfter =  WorkScope.GetAll<Payslip>()
               .Where(x => x.PayrollId == input.PayrollId)
               .Select(info => new { Key = info.Employee.Email, info })
               .ToDictionary(x => x.Key, info => info);

            using (var stream = new MemoryStream())
            {
                await input.File.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    var rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var email = worksheet.Cells[row, 1].GetCellValue<string>();
                        var remainLeaveDaysAfter = worksheet.Cells[row, 2].GetCellValue<float>();
                        if (string.IsNullOrEmpty(email))
                        {
                            failedList.Add(new ResponseFailedDto { Row = row, Email = email, RemainLeaveDays = remainLeaveDaysAfter, ReasonFail = "Email null or empty" });
                            continue;
                        }

                        email = email.Trim().ToLower();

                        if (!mapEmailToRemainLeaveDaysAfter.ContainsKey(email))
                        {
                            failedList.Add(new ResponseFailedDto { Row = row, Email = email, RemainLeaveDays = remainLeaveDaysAfter, ReasonFail = "Email not found" });
                            continue;
                        }

                        if (string.IsNullOrEmpty(remainLeaveDaysAfter.ToString()))
                        {
                            remainLeaveDaysAfter = 0;
                        }

                        var payslip = mapEmailToRemainLeaveDaysAfter[email].info;
                        payslip.RemainLeaveDayAfter = remainLeaveDaysAfter;
                        successList.Add(email);

                    }

                    CurrentUnitOfWork.SaveChanges();
                }
            }

            return new { successList, failedList };
        } 
    }
}
