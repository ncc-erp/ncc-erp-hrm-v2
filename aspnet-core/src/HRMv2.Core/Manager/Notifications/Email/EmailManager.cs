using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.UI;
using HRMv2.Constants.Dictionary;
using HRMv2.Entities;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Notifications.Templates;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Notifications.Email
{
    public class EmailManager : BaseManager
    {
        private readonly IEmailSender _emailSender;
        private readonly IOptions<TimesheetConfig> _timesheetConfig;

        public EmailManager(IWorkScope workScope,
            IEmailSender emailSender,
            IOptions<TimesheetConfig> timesheetConfig
            ) : base(workScope)
        {
            _emailSender = emailSender;
            _timesheetConfig = timesheetConfig;
        }


        public IQueryable<EmailDto> IQGetEmailTemplate()
        {
            return WorkScope.GetAll<EmailTemplate>()
                    .Select(s => new EmailDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        BodyMessage = s.BodyMessage.Replace("\"", "'"),
                        Type = s.Type,
                        CCs = s.CCs,
                        SendToEmail = s.SendToEmail
                    });
        }

        public void SendMail(MailPreviewInfoDto input)
        {
            Send(input);
        }

        public void Send(MailPreviewInfoDto message)
        {
            if (message.CCs.Any())
            {
                SendToCC(message);
            }
            else
            {
                SendDefault(message);
            }
        }

        private void SendDefault(MailPreviewInfoDto message)
        {
            _emailSender.SendAsync(
                          to: message.SendToEmail,
                          subject: message.Subject,
                          body: message.BodyMessage,
                          isBodyHtml: true
                      );
        }
        private void SendToCC(MailPreviewInfoDto message)
        {
            var mailMessage = new MailMessage()
            {
                Body = message.BodyMessage,
                Subject = message.Subject
            };
            mailMessage.To.Add(message.SendToEmail);
            message.CCs.ForEach(cc => mailMessage.CC.Add(cc));
            mailMessage.IsBodyHtml = true;
            _emailSender.SendAsync(mailMessage);
        }

        public async Task<List<EmailDto>> GetAllMailTemplate()
        {
            return await IQGetEmailTemplate()
                .OrderBy(s => s.Type)
                .ToListAsync();
        }

        public GetMailPreviewInfoDto PreviewTemplate(long templateId)
        {
            var template = WorkScope.GetAll<EmailTemplate>()
                .Where(x => x.Id == templateId)
                .FirstOrDefault();

            if(template == default)
            {
                throw new UserFriendlyException($"Can't find template with id {templateId}");
            }
       /*     ResultTemplateEmail<InputPayslipMailTemplate>*/

            var data = EmailDispatchData(template.Type, null);

            var result = GenerateEmailContent(data.Result, template);
            return new GetMailPreviewInfoDto
            {
                Id = result.TemplateId,
                Name = template.Name,
                Description = template.Description,
                BodyMessage = result.BodyMessage,
                Subject = result.Subject,
                CCs = result.CCs,
                PropertiesSupport = data.PropertiesSupport,
                Type = template.Type,
                SendToEmail = result.SendToEmail
            };
        }

        public GetMailPreviewInfoDto GetTemplateById(long templateId)
        {
            var template = WorkScope.GetAll<EmailTemplate>()
                .Where(x => x.Id == templateId)
                .FirstOrDefault();

            if (template == default)
            {
                throw new UserFriendlyException($"Can't find template with id {templateId}");
            }

            var data = EmailDispatchData(template.Type, null);
            var ccs = string.IsNullOrEmpty(template.CCs) ? new List<string>() : template.CCs.Split(",").ToList();

            return new GetMailPreviewInfoDto
            {
                Id = template.Id,
                Type = template.Type,
                BodyMessage = template.BodyMessage.Replace("\"", "'"),
                Subject = template.Subject,
                CCs = ccs,
                Name = template.Name,
                Description = template.Description,
                PropertiesSupport = data.PropertiesSupport,
                SendToEmail = template.SendToEmail,
            };
        }

        public async Task<UpdateTemplateDto> UpdateTemplate(UpdateTemplateDto input)
        {
            var entity = ObjectMapper.Map<EmailTemplate>(input);
            entity.CCs = string.Join(",", input.ListCC);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public MailPreviewInfoDto GetEmailContentById(MailFuncEnum mailType, long id)
        {
            var template = WorkScope.GetAll<EmailTemplate>().Where(x => x.Type == mailType).FirstOrDefault();

            var data = EmailDispatchData(mailType, id);

            return GenerateEmailContent(data.Result, template);
        }

        public MailPreviewInfoDto GetContractContentById(MailFuncEnum mailType, long contractId)
        {
            var template = WorkScope.GetAll<EmailTemplate>().Where(x => x.Type == mailType).FirstOrDefault();

            var data = EmailDispatchData(mailType, contractId);

            return GenerateEmailContent(data.Result, template);
        }

        public EmailTemplateDto GetEmailTemplateDto(MailFuncEnum type)
        {
            var emailTemplateDto = WorkScope.GetAll<EmailTemplate>()
                .Where(s => s.Type == type)
                .Select(s => new EmailTemplateDto
                {
                    Id = s.Id,   
                    Type = s.Type,
                    BodyMessage = s.BodyMessage,
                    CCs = s.CCs,
                    Name = s.Name,
                    Subject = s.Subject,
                    SendToEmail = s.SendToEmail
                    
                }).FirstOrDefault();

            return emailTemplateDto;
        }


        private dynamic EmailDispatchData(MailFuncEnum EmailType, long? id)
        {
            switch (EmailType)
            {
                case MailFuncEnum.Payslip:
                    return GetDataPayslip(id);
                case MailFuncEnum.ContractBM:
                    return GetConfidentialContractData(id);
                case MailFuncEnum.ContractDT:
                    return GetTrainingContractData(id);
                case MailFuncEnum.ContractTV:
                    return GetProbationaryContractData(id);
                case MailFuncEnum.ContractCTV:
                    return GetCollaboratorContractData(id);
                case MailFuncEnum.ContractLD:
                    return GetLaborContractData(id);
                case MailFuncEnum.Debt:
                    return GeDataDebt(id);
                case MailFuncEnum.Bonus:
                    return GetBonusData(id);
                case MailFuncEnum.Checkpoint:
                    return GetCheckpointData(id);
                case MailFuncEnum.PayrollPendingCEO:
                    return GetPayrollPendingCEOData(id);
                case MailFuncEnum.PayrollApprovedByCEO:
                    return GetPayrollApprovedByCEOData(id);
                case MailFuncEnum.PayrollRejectedByCEO:
                    return GetPayrollRejectedByCEOData(id);
                case MailFuncEnum.PayrollExecuted:
                    return GetPayrollExecutedData(id);
                default:
                    return null;
            }
        }

        private ResultTemplateEmail<PayrollMailTemplateDto> GetPayrollData(long? payrollId)
        {
            var payroll = WorkScope.GetAll<Payroll>()
                .Where(x => x.Id == payrollId)
                .FirstOrDefault();

            var hrmv2Uri = HRMv2Consts.HRM_Uri;
            var result = new PayrollMailTemplateDto
            {
                PayrollMonth = payroll.ApplyMonth,
                ConfirmUrl = hrmv2Uri + $"app/payroll/list-payroll/payroll-detail?id={payroll.Id}",
                PayrollStatus = payroll.Status.ToString(),
            };

            return new ResultTemplateEmail<PayrollMailTemplateDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<PayrollMailTemplateDto> GetPayrollApprovedByCEOData(long? payrollId)
        {

            if (payrollId == null)
            {
                return new ResultTemplateEmail<PayrollMailTemplateDto>
                {
                    Result = TemplateHelper.GetPayrollApprovedByCEOFakeData()
                };
            }
            return GetPayrollData(payrollId);
        }
        private ResultTemplateEmail<PayrollMailTemplateDto> GetPayrollRejectedByCEOData(long? payrollId)
        {

            if (payrollId == null)
            {
                return new ResultTemplateEmail<PayrollMailTemplateDto>
                {
                    Result = TemplateHelper.GetPayrollRejectedByCEOFakeData()
                };
            }
            return GetPayrollData(payrollId);
        }
        private ResultTemplateEmail<PayrollMailTemplateDto> GetPayrollExecutedData(long? payrollId)
        {

            if (payrollId == null)
            {
                return new ResultTemplateEmail<PayrollMailTemplateDto>
                {
                    Result = TemplateHelper.GetPayrollExecutedFakeData()
                };
            }
            return GetPayrollData(payrollId);
        }
        private ResultTemplateEmail<PayrollMailTemplateDto> GetPayrollPendingCEOData(long? payrollId)
        {
            if (payrollId == null)
            {
                return new ResultTemplateEmail<PayrollMailTemplateDto>
                {
                    Result = TemplateHelper.GetPayrollPendingCEOFakeData()
                };
            }

            return GetPayrollData(payrollId);
        }
        private ResultTemplateEmail<CheckpointMailTemplateDto> GetCheckpointData(long? requestId)
        {
            if (requestId == null)
            {
                return new ResultTemplateEmail<CheckpointMailTemplateDto>
                {
                    Result = TemplateHelper.GetCheckpointFakeData()
                };
            }
            var data = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.Id == requestId)
                .Select(x => new GetSalaryChangeRequestForSendMailDto
                {
                    EmployeeFullName = x.Employee.FullName,
                    EmployeeEmail = x.Employee.Email,
                    CheckpointName = x.SalaryChangeRequest.Name,
                    OldLevelId = x.LevelId,
                    NewLevelId = x.ToLevelId,
                    OldSalary = x.Salary,
                    NewSalary = x.ToSalary,
                    ApplyDate = x.ApplyDate
                })
                .FirstOrDefault();

            var dictLevel = WorkScope.GetAll<Level>()
                                 .Select(s => new { Key = s.Id,Name = s.Name.ToLower() })
                                 .ToDictionary(s => s.Key, s => s.Name);

            var result = new CheckpointMailTemplateDto
            {
                EmployeeFullName = data.EmployeeFullName,
                SendToEmail = data.EmployeeEmail,
                CheckpointName = data.CheckpointName,
                OldSalary = CommonUtil.FormatDisplayMoney(data.OldSalary),
                NewSalary = CommonUtil.FormatDisplayMoney(data.NewSalary),
                OldLevel = dictLevel[data.OldLevelId],
                NewLevel = dictLevel[data.NewLevelId],
                ApplyDate = data.ApplyDate.ToString("dd/MM/yyyy")
            };
            return new ResultTemplateEmail<CheckpointMailTemplateDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<BonusMailTemplateDto> GetBonusData(long? bonusEmployeeId)
        {
            if(bonusEmployeeId == null)
            {
                return new ResultTemplateEmail<BonusMailTemplateDto>
                {
                    Result = TemplateHelper.GetBonusFakeData()
                };
            }
            var data = WorkScope.GetAll<BonusEmployee>()
                .Where(x=> x.Id == bonusEmployeeId)
                .Select(x=> new GetBonusEmployeeForSendMailDto
                {
                    EmployeeFullName = x.Employee.FullName,
                    EmployeeEmail = x.Employee.Email,
                    ApplyMonth = x.Bonus.ApplyMonth,
                    Money = x.Money,
                    BonusId = x.BonusId,
                    BonusName = x.Bonus.Name
                })
                .FirstOrDefault();

            var result = new BonusMailTemplateDto
            {
                EmployeeFullName = data.EmployeeFullName,
                BonusMoney = CommonUtil.FormatDisplayMoney(data.Money),
                BonusName = data.BonusName,
                SendToEmail = data.EmployeeEmail,
                ApplyMonth = data.ApplyMonth.ToString("MM/yyyy"),
            };
            return new ResultTemplateEmail<BonusMailTemplateDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<ConfidentialityContractDto> GetConfidentialContractData(long? contractId)
        {
            if(contractId == null)
            {
                return new ResultTemplateEmail<ConfidentialityContractDto>
                {
                    Result = TemplateHelper.GetConfidentialityContractFakeData()
                };
            }
            var data = GetEmployeeContractData((long)contractId);

            var result = new ConfidentialityContractDto
            {
                EmployeeFullName = data.FullName,
                EmployeeBasicSalary = data.BasicSalary.ToString(),
                EmployeeBirthday = data.Birthday.HasValue ? data.Birthday.Value.ToString("dd/MM/yyyy") : "",
                EmployeeBranch = data.BranchName,
                EmployeePhone = data.Phone,
                EmployeeIdCard = data.IdCard,
                EmployeeIssuedBy = data.IssuedBy,
                EmployeeIssuedOn = data.IssuedOn.HasValue ? data.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                EmployeePosition = data.JobPositionName,
                EmployeePlaceOfResidence = data.Address,
                EmployeeProbationPercentage = data.ProbationPercentage.ToString(),
                ContractEndDate = data.ContractEndDate.HasValue ? data.ContractEndDate.Value.ToString("dd/MM/yyyy") : "",
                ContractStartDate = data.ContractStartDate.Value.ToString("dd/MM/yyyy"),
                CEOFullName = data.CEOFullName,
                CompanyAddress = data.CompanyAddress,
                CompanyPhone = data.CompanyPhone,
                CompanyTaxCode = data.CompanyTaxCode,
                
            };
            return new ResultTemplateEmail<ConfidentialityContractDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<TrainingContractDto> GetTrainingContractData(long? contractId)
        {
            if (contractId == null)
            {
                return new ResultTemplateEmail<TrainingContractDto>
                {
                    Result = TemplateHelper.GetTrainingContractFakeData()
                };
            }
            var data = GetEmployeeContractData((long)contractId);

            var result = new TrainingContractDto
            {
                EmployeeFullName = data.FullName,
                EmployeeBasicSalary = data.BasicSalary.ToString(),
                EmployeeBirthday = data.Birthday.HasValue ? data.Birthday.Value.ToString("dd/MM/yyyy") : "",
                EmployeeBranch = data.BranchName,
                EmployeePhone = data.Phone,
                EmployeeIdCard = data.IdCard,
                EmployeeIssuedBy = data.IssuedBy,
                EmployeeIssuedOn = data.IssuedOn.HasValue ? data.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                EmployeePosition = data.JobPositionName,
                EmployeePlaceOfResidence = data.Address,
                EmployeeProbationPercentage = data.ProbationPercentage.ToString(),
                ContractEndDate = data.ContractEndDate.HasValue ? data.ContractEndDate.Value.ToString("dd/MM/yyyy") : "",
                ContractStartDate = data.ContractStartDate.Value.ToString("dd/MM/yyyy"),
                CEOFullName = data.CEOFullName,
                CompanyAddress = data.CompanyAddress,
            };
            return new ResultTemplateEmail<TrainingContractDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<ProbationaryContractDto> GetProbationaryContractData(long? contractId)
        {
            if (contractId == null)
            {
                return new ResultTemplateEmail<ProbationaryContractDto>
                {
                    Result = TemplateHelper.GetProbationaryContractFakeData()
                };
            }
            var data = GetEmployeeContractData((long)contractId);

            var result = new ProbationaryContractDto
            {
                EmployeeFullName = data.FullName,
                EmployeeBasicSalary = data.BasicSalary.ToString(),
                EmployeeBirthday = data.Birthday.HasValue ? data.Birthday.Value.ToString("dd/MM/yyyy") : "",
                EmployeeBranch = data.BranchName,
                EmployeePhone = data.Phone,
                EmployeeIdCard = data.IdCard,
                EmployeeIssuedBy = data.IssuedBy,
                EmployeeIssuedOn = data.IssuedOn.HasValue ? data.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                EmployeePosition = data.JobPositionName,
                EmployeePlaceOfResidence = data.Address,
                EmployeeProbationPercentage = data.ProbationPercentage.ToString(),
                ContractEndDate = data.ContractEndDate.HasValue ? data.ContractEndDate.Value.ToString("dd/MM/yyyy") : "",
                ContractStartDate = data.ContractStartDate.Value.ToString("dd/MM/yyyy"),
                ContractCode = data.ContractCode,
                CEOFullName = data.CEOFullName,
                CompanyAddress = data.CompanyAddress,
            };

            return new ResultTemplateEmail<ProbationaryContractDto>
            {
                Result = result,
            };
        }

        private ResultTemplateEmail<CollaboratorContractDto> GetCollaboratorContractData(long? contractId)
        {
            if (contractId == null)
            {
                return new ResultTemplateEmail<CollaboratorContractDto>
                {
                    Result = TemplateHelper.GetCollabContractFakeData()
                };
            }
            var data = GetEmployeeContractData((long)contractId);

            var result = new CollaboratorContractDto
            {
                EmployeeFullName = data.FullName,
                EmployeeBasicSalary = data.BasicSalary.ToString(),
                EmployeeBirthday = data.Birthday.HasValue ? data.Birthday.Value.ToString("dd/MM/yyyy") : "",
                EmployeeBranch = data.BranchName,
                EmployeePhone = data.Phone,
                EmployeeIdCard = data.IdCard,
                EmployeeIssuedBy = data.IssuedBy,
                EmployeeIssuedOn = data.IssuedOn.HasValue ? data.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                EmployeePosition = data.JobPositionName,
                EmployeePlaceOfResidence = data.Address,
                EmployeeProbationPercentage = data.ProbationPercentage.ToString(),
                ContractEndDate = data.ContractEndDate.HasValue ? data.ContractEndDate.Value.ToString("dd/MM/yyyy") : "",
                ContractStartDate = data.ContractStartDate.Value.ToString("dd/MM/yyyy"),
                ContractCode = data.ContractCode,
                CEOFullName = data.CEOFullName,
                CompanyAddress = data.CompanyAddress,
            };
            return new ResultTemplateEmail<CollaboratorContractDto>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<LaborContractDto> GetLaborContractData(long? contractId)
        {
            if (contractId == null)
            {
                return new ResultTemplateEmail<LaborContractDto>
                {
                    Result = TemplateHelper.GetLaborContractFakeData()
                };
            }
            var data = GetEmployeeContractData((long)contractId);

            var result = new LaborContractDto
            {
                EmployeeFullName = data.FullName,
                EmployeeBasicSalary = data.BasicSalary.ToString(),
                EmployeeBirthday = data.Birthday.HasValue ? data.Birthday.Value.ToString("dd/MM/yyyy") : "",
                EmployeeBranch = data.BranchName,
                EmployeePhone = data.Phone,
                EmployeeIdCard = data.IdCard,
                EmployeeIssuedBy = data.IssuedBy,
                EmployeeIssuedOn = data.IssuedOn.HasValue ? data.IssuedOn.Value.ToString("dd/MM/yyyy") : "",
                EmployeePosition = data.JobPositionName,
                EmployeePlaceOfResidence = data.Address,
                EmployeeProbationPercentage = data.ProbationPercentage.ToString(),
                ContractEndDate = data.ContractEndDate.HasValue ? data.ContractEndDate.Value.ToString("dd/MM/yyyy") : "",
                ContractStartDate = data.ContractStartDate.Value.ToString("dd/MM/yyyy"),
                ContractCode = data.ContractCode,
                CEOFullName = data.CEOFullName,
                CompanyAddress = data.CompanyAddress,
            };
            

            return new ResultTemplateEmail<LaborContractDto>
            {
                Result = result
            };
        }

        private GetEmployeeContractDataDto GetEmployeeContractData(long contractId)
        {

            var contract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.Id == contractId)
                .Select(x=> new EmployeeContractForExportDto
                {
                    Code = x.Code,
                    BasicSalary = x.BasicSalary,
                    EmployeeId = x.EmployeeId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    JobPositionName = x.JobPosition.NameInContract,
                    ProbationPercentage = x.ProbationPercentage
                }).FirstOrDefault();

            var result = WorkScope.GetAll<Employee>()
                        .Where(x => x.Id == contract.EmployeeId)
                        .Select(x=> new GetEmployeeContractDataDto
                        {
                            FullName = x.FullName,
                            BasicSalary = contract.BasicSalary,
                            ProbationPercentage = contract.ProbationPercentage,
                            Address = x.Address,
                            Birthday = x.Birthday,
                            ContractStartDate = contract.StartDate,
                            ContractEndDate = contract.EndDate,
                            IdCard = x.IdCard,
                            IssuedBy = x.IssuedBy,
                            IssuedOn = x.IssuedOn,
                            ContractCode = contract.Code,
                            Phone = x.Phone,
                            JobPositionName = contract.JobPositionName,
                            CompanyTaxCode = x.Branch.CompanyTaxCode,
                            CompanyPhone = x.Branch.CompanyPhone,
                            CompanyAddress = x.Branch.Address,
                            BranchName = x.Branch.NameInContract,
                            CEOId = x.Branch.CEOId
                            
                        }).FirstOrDefault();
            result.CEOFullName = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == result.CEOId)
                .Select(x => x.FullName)
                .FirstOrDefault();

            return result;

        }

        private ResultTemplateEmail<InputPayslipMailTemplate> GetDataPayslip(long? payslipId)
        {
            if (payslipId == null)
            {
                return new ResultTemplateEmail<InputPayslipMailTemplate> {
                    Result = TemplateHelper.GetPayslipFakeData()
                };
            }

            var payslip = WorkScope.GetAll<Payslip>()
                .Include(x => x.Employee)
                .Include(x => x.Payroll)
                .Where(x => x.Id == payslipId)
                .FirstOrDefault();

            var payslipSalaries = WorkScope.GetAll<PayslipSalary>()
                .Where(x => x.PayslipId == payslipId)
                .Select(x => new PayslipSalaryEmailDto
                {
                    Date = x.Date.Date,
                    Money = x.Salary,
                    Note = x.Note
                })
                .ToList();

            var payslipdetails = WorkScope.GetAll<PayslipDetail>()
                .Where(x => x.PayslipId == payslipId)
                .Select(x => new PayslipDetailEmailDto
                {
                    Money = x.Money,
                    Note = x.Note
                })
                .ToList();

            var timesheetUri = _timesheetConfig.Value.Uri;

            var result = new InputPayslipMailTemplate
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
                ListPayslipDetail = payslipdetails,
                ListPayslipSalary = payslipSalaries,
                ConfirmUrl = timesheetUri + $"public/confirm-mail?id={payslip.Id}",
                ComplainUrl = timesheetUri + $"public/complain-mail?id={payslip.Id}",
                ComplainDeadline = payslip.ComplainDeadline.HasValue 
                ? payslip.ComplainDeadline.Value.ToString("HH:mm dd/MM/yyyy ") 
                : "..."
            };

            return new ResultTemplateEmail<InputPayslipMailTemplate>
            {
                Result = result
            };
        }

        private ResultTemplateEmail<DebtMailTemplateDto> GeDataDebt(long? debtId)
        {
            if (debtId == null)
            {
                return new ResultTemplateEmail<DebtMailTemplateDto>
                {
                    Result = TemplateHelper.GetDebtFakeDate()
                };
            }

            var data = WorkScope.GetAll<Debt>()
                .Where(x => x.Id == debtId)
                .Select(x=> new GetDebtDto
                {
                    EmployeeFullName = x.Employee.FullName,
                    EmployeeIdCard = x.Employee.IdCard,
                    EmployeeIssuedBy = x.Employee.IssuedBy,
                    EmployeeIssuedOn = x.Employee.IssuedOn,
                    AmountLoan = x.Money,
                    LoanStartDate = x.StartDate,
                    InterestRate = x.InterestRate,
                    EmployeeEmail = x.Employee.Email

                })
                .FirstOrDefault();

            var listDebtPaymentPlans = WorkScope.GetAll<DebtPaymentPlan>()
                .Where(x=> x.DebtId == debtId)
                .Select(x=> new PaidPlanDto
                {
                    Date = x.Date,
                    Money = x.Money,
                    PaymentType = x.PaymentType
                }).ToList();

            var result = new DebtMailTemplateDto
            {
                EmployeeFullName = data.EmployeeFullName,
                SendToEmail = data.EmployeeEmail,
                EmployeeIdCard = data.EmployeeIdCard,
                EmployeeIssuedBy = data.EmployeeIssuedBy,
                EmployeeIssuedOn = data.EmployeeIssuedOn.HasValue ? data.EmployeeIssuedOn.Value.ToString("dd/MM/yyyy") : "",
                AmountLoan = CommonUtil.FormatDisplayMoney(data.AmountLoan),
                LoanStartDate = data.LoanStartDate.Date.ToString("dd/MM/yyyy"),
                InterestRate = data.InterestRate.ToString() + "%",
                ListDebtPaymentPlans = listDebtPaymentPlans
            };

            return new ResultTemplateEmail<DebtMailTemplateDto>
            {
                Result = result
            };
        }

        public MailPreviewInfoDto GenerateEmailContent<TDto, TEntity>(TDto data, TEntity mailEntity) where TDto : class where TEntity : class
        {
            Type typeOfEntity = typeof(TEntity);
            Type typeOfDto = typeof(TDto);
            var templateId = typeOfEntity.GetProperty("Id").GetValue(mailEntity) as long?;

            var bodyMessage = typeOfEntity.GetProperty("BodyMessage").GetValue(mailEntity) as string;
            var subject = typeOfDto.GetProperty("Subject").GetValue(data) as string;

            var properties = typeOfDto.GetProperties().Select(s => s.Name).ToArray();
            foreach (var property in properties)
            {
                var a = typeOfDto.GetProperty(property).GetValue(data);
                bodyMessage = bodyMessage.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
                subject = subject.Replace("{{" + property + "}}", typeOfDto.GetProperty(property).GetValue(data) as string);
            }
            var sendTo = (typeOfDto.GetProperty("SendToEmail").GetValue(data) != null ?
                typeOfDto.GetProperty("SendToEmail").GetValue(data):
                typeOfEntity.GetProperty("SendToEmail").GetValue(mailEntity)) as string;

            var ccs = typeOfEntity.GetProperty("CCs").GetValue(mailEntity) as string;
            var listCCs = string.IsNullOrEmpty(ccs) ? new List<string>() : ccs.Split(",").ToList();

            var type = typeOfEntity.GetProperty("Type").GetValue(mailEntity) as MailFuncEnum?;

            return new MailPreviewInfoDto
            {
                MailFuncType = type.HasValue ? type.Value : default,
                BodyMessage = bodyMessage.Replace("\"", "'"),
                Subject = subject,
                TemplateId = templateId.HasValue ? templateId.Value : 0,
                SendToEmail = sendTo,
                CCs = listCCs
            };
        }

        public void CreateDefaultMailTemplate(int tenantId)
        {
            var mailTemplates = new List<EmailTemplate>();
            var mails = WorkScope.GetAll<EmailTemplate>()
                .Where(q => q.TenantId == tenantId).Select(x => x.Type).ToList();

                 Enum.GetValues(typeof(MailFuncEnum))
                .Cast<MailFuncEnum>()
                .ToList()
                .ForEach(e =>
                {
                    if (!mails.Contains(e))
                    {
                        var isSeedMailExist = DictionaryHelper.SeedMailDic.ContainsKey(e);
                        mailTemplates.Add(
                            new EmailTemplate
                            {
                                Subject = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Subject : string.Empty,
                                Name = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Name : string.Empty,
                                BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                                Description = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Description : string.Empty,
                                Type = e,
                                TenantId = tenantId
                            }
                        );
                    }
                });
           WorkScope.InsertRange(mailTemplates);
        }
    }
}
