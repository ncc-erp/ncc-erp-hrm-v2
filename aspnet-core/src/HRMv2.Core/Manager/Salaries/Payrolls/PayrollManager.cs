using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.UI;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;
using HRMv2.Configuration;
using HRMv2.Constants;
using HRMv2.Entities;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Payrolls.Dto;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Timesheet;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Finfast;
using HRMv2.WebServices.Finfast.Dto;
using HRMv2.WebServices.Komu;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Authorization.Roles.StaticRoleNames;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Payrolls
{
    public class PayrollManager : BaseManager
    {
        private readonly TimesheetManager _timesheetManager;
        private readonly FinfastWebService _finfastService;
        private readonly DebtManager _debtManager;
        private readonly KomuService _komuService;
        private readonly ISettingManager _settingManager;
        private readonly EmailManager _emailManager;
        private readonly PayslipManager _payslipManager;

        public PayrollManager(TimesheetManager timesheetManager,
            FinfastWebService finfastWebService,
            DebtManager debtManager,
            KomuService komuService,
            ISettingManager settingManager,
            PayslipManager payslipManager,
            IWorkScope workScope,
            EmailManager emailManager) : base(workScope)
        {
            _timesheetManager = timesheetManager;
            _finfastService = finfastWebService;
            _debtManager = debtManager;
            _komuService = komuService;
            _settingManager = settingManager;
            _emailManager = emailManager;
            _payslipManager = payslipManager;
        }

        public IQueryable<GetPayrollDto> QueryAllPayroll()
        {
            return WorkScope.GetAll<Payroll>()
                .OrderByDescending(s => s.ApplyMonth)
                .Select(x => new GetPayrollDto
                {
                    Id = x.Id,
                    ApplyMonth = x.ApplyMonth,
                    StandardOpentalk = x.OpenTalk,
                    StandardWorkingDay = x.NormalWorkingDay,
                    Status = x.Status
                });
        }
        public List<GetPayrollDto> GetAll()
        {
            return QueryAllPayroll().ToList();
        }

        public GetPayrollDto Get(long id)
        {
            return QueryAllPayroll()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<GridResult<GetPayrollDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllPayroll();
            return await query.GetGridResult(query, input);
        }

        // TODO: Create_Test1 [need to add 'virtual' keyword on method GetSettingOffDates() of class TimesheetWebService to test]
        public async Task<CreatePayrollDto> Create(CreatePayrollDto input)
        {
            ValidateCreatePayroll(input.Year, input.Month);

            int companyWorkingDay = _timesheetManager.GetCompayWorkingDay(input.Year, input.Month);

            var newPayRoll = new Payroll
            {
                ApplyMonth = new DateTime(input.Year, input.Month, 1),
                Status = PayrollStatus.New,
                NormalWorkingDay = companyWorkingDay,
                OpenTalk = input.StandardOpenTalk
            };
            await WorkScope.InsertAsync(newPayRoll);

            return input;
        }

        public void ValidateCreatePayroll(int year, int month)
        {
            var existPayroll = QueryAllPayroll()
                .Where(x => x.ApplyMonth.Year == year)
                .Where(x => x.ApplyMonth.Month == month);

            if (existPayroll.Any())
            {
                throw new UserFriendlyException($"already has payroll for {month}/{year}");
            }
        }

        public List<DateTime> GetListDateFromPayroll()
        {
            var query = QueryAllPayroll().Select(x => x.ApplyMonth)
                .Distinct()
                .OrderByDescending(x => x.Date)
                .ToList();
            return query;
        }

        public async Task<UpdatePayrollDto> Update(UpdatePayrollDto input)
        {
            var entity = await WorkScope.GetAsync<Payroll>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<string> ChangeStatus(ChangePayrollStatusDto input)
        {
            var entity = await WorkScope.GetAsync<Payroll>(input.PayrollId);
            entity.Status = input.Status;
            await WorkScope.UpdateAsync(entity);

            switch (input.Status)
            {
                case PayrollStatus.New:
                case PayrollStatus.RejectedByKT:
                    ActivePunishmentBonusRefund(entity.ApplyMonth.Year, entity.ApplyMonth.Month);
                    break;
                case PayrollStatus.PendingKT:
                case PayrollStatus.PendingCEO:
                case PayrollStatus.RejectedByCEO:
                case PayrollStatus.ApprovedByCEO:
                case PayrollStatus.Executed:
                    DeActivePunishmentBonusRefund(entity.ApplyMonth.Year, entity.ApplyMonth.Month);
                    break;
                default:
                    break;
            }

            NotifyChangeStatus(input.Status, entity.ApplyMonth);
            return SendMailChangeStatus(input.Status, entity.ApplyMonth, entity.Id);

        }

        public string SendMailChangeStatus(PayrollStatus status, DateTime payrollDate, long payrollId)
        {
            switch (status)
            {
                case PayrollStatus.PendingCEO:
                    return SendMail(payrollDate, payrollId, status, MailFuncEnum.PayrollPendingCEO);
                case PayrollStatus.ApprovedByCEO:
                    return SendMail(payrollDate, payrollId, status, MailFuncEnum.PayrollApprovedByCEO);
                case PayrollStatus.RejectedByCEO:
                    return SendMail(payrollDate, payrollId, status, MailFuncEnum.PayrollRejectedByCEO);
            }
            return "";
        }

        // TODO: ChangeStatus_Test1, SendMailChangeStatus_Test, SendMail_Test, ExecuatePayroll_Test1 [need to change FirstOfDefault() to LastOrDefault() in method GetEmailTemplateDto() of class EmailManager to run tests]
        public string SendMail(DateTime payrollDate, long payrollId, PayrollStatus status, MailFuncEnum templateType)
        {
            var emailTemplate = _emailManager.GetEmailTemplateDto(templateType);
            var hrmv2Uri = HRMv2Consts.HRM_Uri;
            if (string.IsNullOrEmpty(emailTemplate.SendToEmail))
            {
                return $"Can not send mail because mail receiver is not set in template:&nbsp;Payroll&nbsp;{status.ToString()}";
            }
            var result = new PayrollMailTemplateDto
            {
                PayrollMonth = payrollDate,
                ConfirmUrl = hrmv2Uri + $"app/payroll/list-payroll/payroll-detail?id={payrollId}",
                PayrollStatus = status.ToString(),
                SendToEmail = emailTemplate.SendToEmail
            };
            MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(result, emailTemplate);
            _emailManager.Send(mailInput);
            return "";


        }


        public async Task<long> Delete(long id)
        {
            _payslipManager.DeletePayslips(id, null);

            await WorkScope.DeleteAsync<Payroll>(id);
            return id;
        }

        public string ExecuatePayroll(long payrollId)
        {
            var payroll = WorkScope.GetAll<Payroll>()
                .Where(x => x.Id == payrollId)
                .FirstOrDefault();

            if (payroll == default)
            {
                throw new UserFriendlyException($"Can't find payroll with id {payrollId}");
            }

            payroll.Status = PayrollStatus.Executed;

            var ListPayslip = WorkScope.GetAll<Payslip>()
                .Include(x => x.Employee)
                .Where(x => x.PayrollId == payrollId)
                .ToList();

            foreach (var payslip in ListPayslip)
            {
                var employee = WorkScope.GetAll<Employee>()
                    .Where(x => x.Id == payslip.EmployeeId)
                    .FirstOrDefault();

                employee.RemainLeaveDay = payslip.RemainLeaveDayAfter;
            }

            DeActivePunishmentBonusRefund(payroll.ApplyMonth.Year, payroll.ApplyMonth.Month);

            var payslipIds = ListPayslip.Select(x => x.Id).ToList();
            SetDoneDebt(payslipIds);

            CurrentUnitOfWork.SaveChanges();
            UpdatePunishmentFund(payroll.ApplyMonth, payroll.Id);
            NotifyChangeStatus(payroll.Status, payroll.ApplyMonth);
            return SendMail(payroll.ApplyMonth, payrollId, payroll.Status, MailFuncEnum.PayrollExecuted);
        }

        public void UpdatePunishmentFund(DateTime payrollApplyDate, long payrollId)
        {
            var punishmentFund = WorkScope.GetAll<PunishmentFund>()
                .Where(x => x.Date.Year == payrollApplyDate.Year && x.Date.Month == payrollApplyDate.Month)
                .Where(x => x.Amount > 0)
                .OrderBy(x => x.Date)
                .FirstOrDefault();

            var punishment = WorkScope.GetAll<PayslipDetail>()
                            .Where(x => x.Payslip.PayrollId == payrollId)
                            .Where(x => x.Type == PayslipDetailType.Punishment)
                            .Sum(s => Math.Abs(s.Money));
            var notPayPunishment = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Where(x => x.Salary < 0)
                .Sum(s => Math.Abs(s.Salary));
            //amount = abs(payroll.Punishment) - abs(payroll.phạt không thu được) = phạt thu được
            var amount = punishment - notPayPunishment;

            if (punishmentFund != default)
            {
                punishmentFund.Amount = amount;

            }
            else
            {
                var enity = new PunishmentFund();
                enity.Amount = amount;
                enity.Date = new DateTime(payrollApplyDate.Year, payrollApplyDate.Month, DateTime.DaysInMonth(payrollApplyDate.Year, payrollApplyDate.Month));
                enity.Note = "Insert punishment fund from payroll";
                WorkScope.InsertAndGetId<PunishmentFund>(enity);
            }
            CurrentUnitOfWork.SaveChanges();
        }

        // TODO: NotifyChangeStatus_Test1() [can't test result]
        public void NotifyChangeStatus(PayrollStatus status, DateTime payrollDate)
        {
            //Get tag Discord for Login User
            var loginUserEmail = WorkScope.GetAll<User>()
                .Where(x => x.Id == AbpSession.UserId)
                .Select(x => x.EmailAddress)
                .FirstOrDefault();
            var tagLoginUserDiscord = CommonUtil.GetDiscordTagUser(loginUserEmail);


            var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.PayrollChannelId);
            var message = "";
            var payrollName = $"payroll {payrollDate.Month}/{payrollDate.Year}";

            var ccEmail = "";
            var ccAccountDiscord = "";
            switch (status)
            {
                case PayrollStatus.PendingCEO:
                    ccEmail = GetFirstUserEmailHasRole(Tenants.CEO.ToUpper());
                    ccAccountDiscord = CommonUtil.GetDiscordTagUser(ccEmail);
                    message = $"{tagLoginUserDiscord} submited **{payrollName}** [PendingCEO] - cc: {ccAccountDiscord}";
                    break;
                case PayrollStatus.PendingKT:
                    ccEmail = GetFirstUserEmailHasRole(Tenants.KT.ToUpper());
                    ccAccountDiscord = CommonUtil.GetDiscordTagUser(ccEmail);
                    message = $"{tagLoginUserDiscord} submited **{payrollName}** [PendingKT] - cc: {ccAccountDiscord}";
                    break;
                case PayrollStatus.RejectedByKT:
                    ccEmail = GetFirstUserEmailHasRole(Tenants.SubKT.ToUpper());
                    ccAccountDiscord = CommonUtil.GetDiscordTagUser(ccEmail);
                    message = $"{tagLoginUserDiscord} rejected **{payrollName}** [RejectedByKT] - cc: {ccAccountDiscord}";
                    break;
                case PayrollStatus.RejectedByCEO:
                    ccEmail = GetFirstUserEmailHasRole(Tenants.KT.ToUpper());
                    ccAccountDiscord = CommonUtil.GetDiscordTagUser(ccEmail);
                    message = $"{tagLoginUserDiscord} rejected **{payrollName}** [RejectedByCEO] - cc: {ccAccountDiscord}";
                    break;
                case PayrollStatus.ApprovedByCEO:
                    ccEmail = GetFirstUserEmailHasRole(Tenants.KT.ToUpper());
                    ccAccountDiscord = CommonUtil.GetDiscordTagUser(ccEmail);
                    message = $"{tagLoginUserDiscord} approved **{payrollName}** [ApprovedByCEO] - cc: {ccAccountDiscord}";
                    break;
                case PayrollStatus.Executed:
                    message = $"{tagLoginUserDiscord} executed **{payrollName}** [Executed]";
                    break;
            }
            _komuService.NotifyToChannel(message, channelId);
        }

        private string GetFirstUserEmailHasRole(string roleName)
        {
            var q = from ur in WorkScope.GetAll<UserRole>()
                    join r in WorkScope.GetAll<Role, int>().Where(s => s.NormalizedName == roleName.ToUpper())
                    on ur.RoleId equals r.Id
                    join u in WorkScope.GetAll<User>() on ur.UserId equals u.Id
                    select u.EmailAddress;
            return q.FirstOrDefault();
        }



        // TODO: CreateFinfastOutcomeEntry_Test1() [can't test result]
        public void CreateFinfastOutcomeEntry(long payrollId)
        {
            var ListPayslip = WorkScope.GetAll<Payslip>()
                .Include(x => x.Employee)
                .Include(x => x.Payroll)
                .Where(x => x.PayrollId == payrollId)
                .ToList();

            List<OutcomingEntryDetailDto> listOutcomeDetail = new();
            var listBranch = WorkScope.GetAll<Branch>()
             .ToList();

            foreach (var payslip in ListPayslip)
            {
                var branch = listBranch.Where(x => x.Id == payslip.BranchId).FirstOrDefault();
                var detail = new OutcomingEntryDetailDto
                {
                    BranchCode = branch.Code,
                    UnitPrice = payslip.Salary >= 0 ? payslip.Salary : 0,
                    Name = $"{payslip.Employee.Email.Split("@")[0]}({branch.Code})",
                };
                listOutcomeDetail.Add(detail);
            }
            var payrollApplyMonth = ListPayslip.Select(x => x.Payroll.ApplyMonth).FirstOrDefault();

            var dto = new InputCreateOucomeRequestDto
            {
                Name = CommonUtil.GenerateFinfastOutcomeEntryName(payrollApplyMonth),
                Details = listOutcomeDetail
            };

            _finfastService.CreatOutcomeRequest(dto);
        }

        public object ValidCreateFinfastOutcomeEntry(long payrollId)
        {
            var branchIds = WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Select(x => x.BranchId)
                .Distinct()
                .ToList();

            var listBranchCode = WorkScope.GetAll<Branch>()
                .Where(x => branchIds.Contains(x.Id))
                .Select(x => x.Code)
                .ToList();

            var payrolApplyMonth = WorkScope.GetAll<Payroll>()
                .Where(x => x.Id == payrollId)
                .Select(x => x.ApplyMonth)
                .FirstOrDefault();

            var input = new InputValidCreateFinfastOucome
            {
                BranchCodes = listBranchCode,
                PayrollName = CommonUtil.GenerateFinfastOutcomeEntryName(payrolApplyMonth)
            };

            var failList = _finfastService.ValidCreateFinfastOutcomeEntry(input) ?? new List<string>();

            return new
            {
                FailList = failList,
                SuccessCount = listBranchCode.Count - failList.Count
            };
        }

        public void ActivePunishmentBonusRefund(int year, int month)
        {
            var bonuses = WorkScope.GetAll<Bonus>()
                .Where(x => x.ApplyMonth.Year == year)
                .Where(x => x.ApplyMonth.Month == month)
                .Where(x => !x.IsActive)
                .ToList();

            var punishments = WorkScope.GetAll<Punishment>()
                .Where(x => x.Date.Year == year)
                .Where(x => x.Date.Month == month)
                .Where(x => !x.IsActive)
                .ToList();

            var refunds = WorkScope.GetAll<Refund>()
                .Where(x => x.Date.Year == year)
                .Where(x => x.Date.Month == month)
                .Where(x => !x.IsActive)
                .ToList();

            foreach (var bonus in bonuses)
            {
                bonus.IsActive = true;
            }

            foreach (var punishment in punishments)
            {
                punishment.IsActive = true;
            }

            foreach (var refund in refunds)
            {
                refund.IsActive = true;
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public void DeActivePunishmentBonusRefund(int year, int month)
        {
            var bonuses = WorkScope.GetAll<Bonus>()
               .Where(x => x.ApplyMonth.Year == year)
               .Where(x => x.ApplyMonth.Month == month)
               .Where(x => x.IsActive)
               .ToList();

            var punishments = WorkScope.GetAll<Punishment>()
                .Where(x => x.Date.Year == year)
                .Where(x => x.Date.Month == month)
                .Where(x => x.IsActive)
                .ToList();

            var refunds = WorkScope.GetAll<Refund>()
                .Where(x => x.Date.Year == year)
                .Where(x => x.Date.Month == month)
                .Where(x => x.IsActive)
                .ToList();

            foreach (var bonus in bonuses)
            {
                bonus.IsActive = false;
            }

            foreach (var punishment in punishments)
            {
                punishment.IsActive = false;
            }

            foreach (var refund in refunds)
            {
                refund.IsActive = false;
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public void SetDoneDebt(List<long> payslipIds)
        {
            var debtIds = WorkScope.GetAll<PayslipDetail>()
                .Where(x => payslipIds.Contains(x.PayslipId))
                .Where(x => x.Type == PayslipDetailType.Debt)
                .Where(x => x.ReferenceId.HasValue)
                .Select(x => x.ReferenceId)
                .ToList();

            var dicDebtInfo = WorkScope.GetAll<DebtPaid>()
                .Include(x => x.Debt)
                .Where(x => debtIds.Contains(x.DebtId))
                .GroupBy(x => x.DebtId)
                .Select(x => new
                {
                    x.Key,
                    DebtInfo = new
                    {
                        TotalDebtPaid = x.Sum(x => x.Money),
                        Debt = x.Select(s => s.Debt).FirstOrDefault()
                    }
                })
                .ToDictionary(x => x.Key, x => x.DebtInfo);

            foreach (var id in debtIds)
            {
                if (dicDebtInfo.ContainsKey(id.Value))
                {
                    Debt debt = dicDebtInfo[id.Value].Debt;
                    double totalPaid = dicDebtInfo[id.Value].TotalDebtPaid;

                    bool isDebtValid = _debtManager.IsDebtValid(debt, totalPaid);

                    if (isDebtValid)
                    {
                        debt.Status = DebtStatus.Done;
                        WorkScope.UpdateAsync(debt);
                    }
                }
            }
        }
    }
}
