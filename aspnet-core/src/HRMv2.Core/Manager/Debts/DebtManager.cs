using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Debts.Dto;
using HRMv2.Manager.Debts.PaidsManagger;
using HRMv2.Manager.Debts.PaymentPlansManager;
using HRMv2.Manager.Employees;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using static HRMv2.Utils.CommonUtil;
using static HRMv2.HRMv2Consts;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Utils;
using Abp.BackgroundJobs;
using HRMv2.BackgroundJob.SendMail;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace HRMv2.Manager.Debts
{
    public class DebtManager : BaseManager
    {
        public readonly PaidManager Paid;
        public readonly PaymentPlanManager PaymentPlan;
        private readonly EmployeeManager _employeeManager;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;

        public DebtManager(PaidManager Paid,
            PaymentPlanManager PaymentPlan,
            IWorkScope workScope,
            EmployeeManager employeeManager,
            EmailManager emailManager,
            BackgroundJobManager backgroundJobManager
            ) : base(workScope)
        {
            this.Paid = Paid;
            this.PaymentPlan = PaymentPlan;
            _employeeManager = employeeManager;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
        }
        public IQueryable<DebtDto> QueryAllDebt()
        {
            var totalPaid = Paid.GetAll()
                .Select(x => new {x.DebtId, x.Money})
                .GroupBy(x => x.DebtId)
                .Select(x => new { x.Key, totalPaid = x.Sum(x => x.Money)})
                .ToDictionary(x => x.Key, x => x.totalPaid);

            return WorkScope.GetAll<Debt>()
                .Select(x => new DebtDto
                {
                    Id = x.Id,
                    FullName = x.Employee.FullName,
                    Avatar = x.Employee.Avatar,
                    Email = x.Employee.Email,
                    Sex = x.Employee.Sex,
                    EmployeeId = x.Employee.Id,
                    BranchInfo = new BadgeInfoDto
                    {
                        Color = x.Employee.Branch.Color,
                        Name = x.Employee.Branch.Name
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Color = x.Employee.JobPosition.Color,
                        Name = x.Employee.JobPosition.Name
                    },
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
                    JobPositionId = x.Employee.JobPositionId,
                    Status = x.Employee.Status,
                    DebtStatus = x.Status,
                    InterestRate = x.InterestRate,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Money = x.Money,
                    PaymentType = x.PaymentType,
                    Note = x.Note,
                    TotalPaid = totalPaid.ContainsKey(x.Id) ? totalPaid[x.Id] : 0,
                    BranchId = x.Employee.BranchId,
                    LevelId = x.Employee.LevelId,
                    UserType = x.Employee.UserType,
                    CreatorUser = x.CreatorUser.FullName,
                    CreationTime = x.CreationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    UpdatedTime = x.LastModificationTime
                });
    
        }
        public List<DebtDto> GetAll()
        {
            return QueryAllDebt().ToList();
        }
        public DebtDto Get(long id)
        {
            return QueryAllDebt().Where(x => x.Id == id).FirstOrDefault();
        }

        public async Task<GridResult<DebtDto>> GetByEmployeeId(long id, GridParam input)
        {
            var query = QueryAllDebt()
                .Where(x => x.EmployeeId == id);
            return await query.GetGridResult(query, input);

        }

        public async Task<GridResult<DebtDto>> GetAllPaging(GetDebtEmployeeInputDto input)
        {
            var query = QueryAllDebt();

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.UserTypes != null && input.UserTypes.Count == 1) query = query.Where(x => input.UserTypes[0] == x.UserType);
            else if (input.UserTypes != null && input.UserTypes.Count > 1) query = query.Where(x => input.UserTypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.DebtStatusIds != null && input.DebtStatusIds.Count == 1) query = query.Where(x => input.DebtStatusIds[0] == (long)x.DebtStatus);
            else if (input.DebtStatusIds != null && input.DebtStatusIds.Count > 1) query.Where(x => input.DebtStatusIds.Contains((long)x.DebtStatus));
            
            if (input.PaymentTypeIds != null  && input.PaymentTypeIds.Count == 1) query = query.Where(x => input.PaymentTypeIds[0] == (long)x.PaymentType);
            else if (input.PaymentTypeIds != null && input.PaymentTypeIds.Count > 1) query.Where(x => input.PaymentTypeIds.Contains((long)x.PaymentType));
            
            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }

            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                        select employee;
                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;

            query = query.Where(s => employeeIds.Contains((long)s.EmployeeId));
            return await query.GetGridResult(query, input.GridParam);
        }
        public async Task<long> Create(CreateDebtDto input)
        {
            //await ValidCreate(input);
            var entity = ObjectMapper.Map<Debt>(input);
            entity.Status = input.DebtStatus;
            var id = await WorkScope.InsertAndGetIdAsync(entity);
            return id;
        }
        public async Task<UpdateDebtDto> Update(UpdateDebtDto input)
        {
            //await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Debt>(input.Id);
            ObjectMapper.Map(input, entity);
            entity.Status = input.DebtStatus;
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public long SetDone(long id)
        {
            var debt = WorkScope.GetAll<Debt>().Where(x => x.Id == id).FirstOrDefault();
            if (debt == default) throw new UserFriendlyException($"Cannot find debt with id = {id}");

            double totalDebtPaid = WorkScope.GetAll<DebtPaid>().Where(x => x.DebtId == debt.Id).Sum(p => p.Money);

            bool isValid = IsDebtValid(debt, totalDebtPaid);

            if (!isValid)
            {
                 throw new UserFriendlyException("User's total payment does not equal to principal + interest");
            }

            debt.Status = DebtStatus.Done;
            WorkScope.UpdateAsync(debt);
         
            return id;
        }

        public bool IsDebtValid(Debt debt, double totalDebtPaid)
        {
            var totalInterestMoney = CalculateInterestValue(debt.StartDate, debt.EndDate, debt.Money, debt.InterestRate);
            var totalMoney = debt.Money + totalInterestMoney;
            if (Math.Abs(totalDebtPaid - totalMoney) > DebtPaidAllow) {
                 return false;
            }
            return true;
        }

        public async Task<long> Delete(long id)
        {
            ValidDeleteDebt(id);
            await WorkScope.DeleteAsync<Debt>(id);
            return id;
        }

        void ValidDeleteDebt(long id)
        {
            var qdebtPaid = Paid.QueryAllDebt().Where(p => p.DebtId == id);
            if (qdebtPaid.Any()) throw new UserFriendlyException("Cannot delete this debt because it is currently in paying process or it is already done");
        }

        public void SendMailToOneEmployee(SendMailDto input)
        {
            var debt = WorkScope.GetAll<Debt>()
                .Where(x => x.Id == input.DebtId)
                .FirstOrDefault();
            if(debt == default)
            {
                throw new UserFriendlyException($"Can not found debt with Id = {input.DebtId}");
            }
            _emailManager.Send(input.MailContent);
        }

        public string SendMailToAllEmployee(GetDebtEmployeeInputDto input)
        {
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Debt);
            if (emailTemplate == default)
            {
                throw new UserFriendlyException($"Not found email template for debt");
            }
            var listDebts = GetAllPaging(input).Result.Items.Where(x=> x.DebtStatus == DebtStatus.Inprogress);
            var listPlanPayments = WorkScope.GetAll<DebtPaymentPlan>();
            var listDebtIds = listDebts.Select(x => x.Id).ToList();
            var getDicListDebtPaymentPlans = GetDicListDebtPaymentPlans(listDebtIds);

            List<ResultTemplateEmail<DebtMailTemplateDto>> emailDebts = listDebts.Select(d => new ResultTemplateEmail<DebtMailTemplateDto>
            {
                Result = new DebtMailTemplateDto
                {
                    EmployeeFullName = d.FullName,
                    SendToEmail = d.Email,
                    EmployeeIdCard = d.IdCard,
                    EmployeeIssuedBy = d.IssuedBy,
                    EmployeeIssuedOn = d.IssuedOn.HasValue ? d.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                    AmountLoan = CommonUtil.FormatDisplayMoney(d.Money),
                    LoanStartDate = d.StartDate.Date.ToString("dd/MM/yyyy"),
                    InterestRate = d.InterestRate.ToString() + "%",
                    ListDebtPaymentPlans = getDicListDebtPaymentPlans.ContainsKey(d.Id) ? getDicListDebtPaymentPlans[d.Id] : new List<PaidPlanDto>()
                }
             }).ToList();


            var delaySendMail = 0;
            foreach (var debt in emailDebts)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(debt.Result, emailTemplate);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += HRMv2Consts.DELAY_SEND_MAIL_SECOND;
            };

            return $"Started sending {emailDebts.Count} email.";
        }

        private Dictionary<long, List<PaidPlanDto>> GetDicListDebtPaymentPlans(List<long> debIds)
        {
            var res = WorkScope.GetAll<DebtPaymentPlan>()
               .Where(x => debIds.Contains(x.DebtId))
               .Select(x => new PaidPlanDto
               {
                   Date = x.Date,
                   Money = x.Money,
                   PaymentType = x.PaymentType,
                   DebtId = x.DebtId,
               })
               .ToList()
               .GroupBy(s => s.DebtId)
               .ToDictionary(s => s.Key, s => s.ToList());

            return res;
        }

        //TODO: refactor for unit test
        public MailPreviewInfoDto GetDebtTemplate(long debtId)
        {
            MailPreviewInfoDto template = _emailManager.GetEmailContentById(MailFuncEnum.Debt, debtId);
            return template;
        }

        public GetAllDebtEmployeeDto GetAllDebtEmployee()
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                var debtEmployees = WorkScope.GetAll<Debt>()
                .Where(x => x.PaymentType == DebtPaymentType.RealMoney)
                .Where(x => x.Status == DebtStatus.Inprogress)
                .Select(x => new GetDebtEmployeeDto
                {
                    Email = x.Employee.Email,
                    FullName = x.Employee.FullName,
                    Money = x.Money,
                    StartDate = x.StartDate,
                    Note = x.Note,
                    InterestRate = x.InterestRate

                }).ToList();

                var output = new GetAllDebtEmployeeDto()
                {
                    ListDebtEmployees = debtEmployees,
                    TotalLoan = debtEmployees.Sum(x => x.Money),
                    EmployeeCount = debtEmployees.DistinctBy(x => x.Email).Count()
                };
                return output;
            }
        }
    }
}
