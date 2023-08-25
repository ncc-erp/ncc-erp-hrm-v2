using Abp.Domain.Entities.Auditing;
using Ganss.Excel;
using HRMv2.Authorization.Users;
using HRMv2.Entities;
using HRMv2.EntityFrameworkCore;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Path = System.IO.Path;

namespace HRMv2.Tests.Seeders
{
    public class DataSeederConsumer
    {
        private List<TEntity> Create<TEntity, TKey>(string fileName, List<User> users = default(List<User>), Dictionary<string, string> customFields = default(Dictionary<string, string>))
            where TEntity : FullAuditedEntity<TKey>
        {

            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Tables", fileName);

            var excel = new ExcelMapper(path);
            if (customFields != null)
            {
                foreach (var item in customFields)
                {
                    excel.AddMapping(typeof(TEntity), item.Key, item.Value);
                }
            }
            var data = excel.Fetch<TEntity>().ToList();

            var first = data.FirstOrDefault();
            if (first == null)
            {
                return new List<TEntity>();
            }

            PropertyInfo propertyInfo = first.GetType().GetProperty("CreatorUser");
            PropertyInfo propertyInfo2 = first.GetType().GetProperty("LastModifierUser");

            if (propertyInfo != null && propertyInfo2 != null)
            {
                if (users != null)
                {
                    foreach (var item in data)
                    {
                        propertyInfo.SetValue(item, Convert.ChangeType(users.FirstOrDefault(x => x.Id == item.CreatorUserId), propertyInfo.PropertyType), null);
                        propertyInfo2.SetValue(item, Convert.ChangeType(users.FirstOrDefault(x => x.Id == item.LastModifierUserId), propertyInfo2.PropertyType), null);
                    }
                }
            }
            return data;
        }

        private List<TEntity> MapFK<TEntity, FKEntity>(List<TEntity> data, List<FKEntity> fkData, string fieldNeedMap, string fkField)
            where TEntity : FullAuditedEntity<long>
            where FKEntity : FullAuditedEntity<long>
        {
            foreach (var item in data)
            {
                var value = (long?)item.GetType().GetProperty(fkField).GetValue(item, null);
                var fk = fkData.FirstOrDefault(x => x.Id == value);
                PropertyInfo propertyInfo = item.GetType().GetProperty(fieldNeedMap);
                propertyInfo.SetValue(item, Convert.ChangeType(fk, propertyInfo.PropertyType), null);
            }
            return data;
        }
        private List<TEntity> MapICollection<TEntity, ICEntity>(List<TEntity> data, List<ICEntity> icData, string fieldNeedMap, string fkField)
            where TEntity : NccAuditEntity
            where ICEntity : FullAuditedEntity<long>
        {
            foreach (var item in data)
            {
                var ic = icData.Where(x => (long)x.GetType().GetProperty(fkField).GetValue(x, null) == item.Id).ToList();
                if (ic.Count == 0)
                {
                    ic = null;
                }
                PropertyInfo propertyInfo = item.GetType().GetProperty(fieldNeedMap);
                propertyInfo.SetValue(item, ic);
            }
            return data;
        }
        public void Seed(HRMv2DbContext context)
        {
            //var iseederType = typeof(ISeeder);
            //var seederTypes = Assembly.GetExecutingAssembly()
            //    .GetTypes()
            //    .Where(t => !t.IsInterface && t.IsAssignableTo(iseederType));
            //foreach (var seederType in seederTypes)
            //{
            //    var instance = (ISeeder)Activator.CreateInstance(seederType);
            //    instance.Seed(context);
            //}

            string path = Path.Combine(Directory.GetCurrentDirectory());

            if (path.Contains("HRMv2.Application.Tests"))
            {
                return;
            }

            //"Users",\
            var Users = Create<User, long>("AbpUsers.xlsx");
            context.Users.AddRange(Users);

            //"Branches",\
            var Branches = Create<Branch, long>("Branches.xlsx", Users);
            context.Branches.AddRange(Branches);

            //"Banks",\
            var Banks = Create<Bank, long>("Banks.xlsx", Users);
            context.Banks.AddRange(Banks);

            //"Benefits",\
            var Benefits = Create<Benefit, long>("Benefits.xlsx", Users);
            context.Benefits.AddRange(Benefits);

            //"Bonuses",
            var Bonuses = Create<Bonus, long>("Bonuses.xlsx", Users);
            context.Bonuses.AddRange(Bonuses);

            //"JobPositions",\
            var JobPositions = Create<JobPosition, long>("JobPositions.xlsx", Users);
            context.JobPositions.AddRange(JobPositions);

            //"Levels",\
            var Levels = Create<Level, long>("Levels.xlsx", Users);
            context.Levels.AddRange(Levels);

            //"Payrolls",\
            var Payrolls = Create<Payroll, long>("Payrolls.xlsx", Users);
            context.Payrolls.AddRange(Payrolls);

            //"PunishmentTypes",\
            var PunishmentTypes = Create<PunishmentType, long>("PunishmentTypes.xlsx", Users);
            context.PunishmentTypes.AddRange(PunishmentTypes);

            //"SalaryChangeRequests",\
            var SalaryChangeRequests = Create<SalaryChangeRequest, long>("SalaryChangeRequests.xlsx", Users);
            context.SalaryChangeRequests.AddRange(SalaryChangeRequests);

            //"Skills",\
            var Skills = Create<Skill, long>("Skills.xlsx", Users);
            context.Skills.AddRange(Skills);

            //"Teams",\
            var Teams = Create<Team, long>("Teams.xlsx", Users);
            context.Teams.AddRange(Teams);


            //"Employees",\
            var Employees = Create<Employee, long>("Employees.xlsx", Users, new Dictionary<string, string>() { { "FullName2", "FullName" } });
            Employees = MapFK(Employees, Banks, "Bank", "BankId");
            Employees = MapFK(Employees, JobPositions, "JobPosition", "JobPositionId");
            Employees = MapFK(Employees, Levels, "Level", "LevelId");
            Employees = MapFK(Employees, Branches, "Branch", "BranchId");

            //"EmployeeSkills",\
            var EmployeeSkills = Create<EmployeeSkill, long>("EmployeeSkills.xlsx", Users);
            EmployeeSkills = MapFK(EmployeeSkills, Employees, "Employee", "EmployeeId");
            EmployeeSkills = MapFK(EmployeeSkills, Skills, "Skill", "SkillId");

            //"EmployeeTeams",\
            var EmployeeTeams = Create<EmployeeTeam, long>("EmployeeTeams.xlsx", Users);
            EmployeeTeams = MapFK(EmployeeTeams, Employees, "Employee", "EmployeeId");
            EmployeeTeams = MapFK(EmployeeTeams, Teams, "Team", "TeamId");


            Employees = MapICollection(Employees, EmployeeSkills, "EmployeeSkills", "EmployeeId");
            Employees = MapICollection(Employees, EmployeeTeams, "EmployeeTeams", "EmployeeId");

            context.EmployeeSkills.AddRange(EmployeeSkills);
            context.EmployeeTeams.AddRange(EmployeeTeams);
            context.Employees.AddRange(Employees);

            //"EmployeeBranchHistories",\
            var EmployeeBranchHistories = Create<EmployeeBranchHistory, long>("EmployeeBranchHistories.xlsx", Users);
            EmployeeBranchHistories = MapFK(EmployeeBranchHistories, Branches, "Branch", "BranchId");
            context.EmployeeBranchHistories.AddRange(EmployeeBranchHistories);

            //"Punishments",\
            var Punishments = Create<Punishment, long>("Punishments.xlsx", Users);
            Punishments = MapFK(Punishments, PunishmentTypes, "PunishmentType", "PunishmentTypeId");
            context.Punishments.AddRange(Punishments);

            //"BenefitEmployees",\
            var BenefitEmployees = Create<BenefitEmployee, long>("BenefitEmployees.xlsx", Users);
            BenefitEmployees = MapFK(BenefitEmployees, Employees, "Employee", "EmployeeId");
            BenefitEmployees = MapFK(BenefitEmployees, Benefits, "Benefit", "BenefitId");
            context.BenefitEmployees.AddRange(BenefitEmployees);

            //"BonusEmployees",\
            var BonusEmployees = Create<BonusEmployee, long>("BonusEmployees.xlsx", Users);
            BonusEmployees = MapFK(BonusEmployees, Employees, "Employee", "EmployeeId");
            BonusEmployees = MapFK(BonusEmployees, Bonuses, "Bonus", "BonusId");
            context.BonusEmployees.AddRange(BonusEmployees);

            //"Debts",\
            var Debts = Create<Debt, long>("Debts.xlsx", Users);
            Debts = MapFK(Debts, Employees, "Employee", "EmployeeId");
            context.Debts.AddRange(Debts);

            //"EmployeeWorkingHistories",\
            var EmployeeWorkingHistories = Create<EmployeeWorkingHistory, long>("EmployeeWorkingHistories.xlsx", Users);
            EmployeeWorkingHistories = MapFK(EmployeeWorkingHistories, Employees, "Employee", "EmployeeId");
            context.EmployeeWorkingHistories.AddRange(EmployeeWorkingHistories);

            //"Payslips",\
            var Payslips = Create<Payslip, long>("Payslips.xlsx", Users);
            Payslips = MapFK(Payslips, Payrolls, "Payroll", "PayrollId");
            Payslips = MapFK(Payslips, Employees, "Employee", "EmployeeId");
            Payslips = MapFK(Payslips, Banks, "Bank", "BankId");
            context.Payslips.AddRange(Payslips);

            //"SalaryChangeRequestEmployees",\
            var SalaryChangeRequestEmployees = Create<SalaryChangeRequestEmployee, long>("SalaryChangeRequestEmployees.xlsx", Users);
            SalaryChangeRequestEmployees = MapFK(SalaryChangeRequestEmployees, SalaryChangeRequests, "SalaryChangeRequest", "SalaryChangeRequestId");
            SalaryChangeRequestEmployees = MapFK(SalaryChangeRequestEmployees, Employees, "Employee", "EmployeeId");
            context.SalaryChangeRequestEmployees.AddRange(SalaryChangeRequestEmployees);

            //"PunishmentEmployees",\
            var PunishmentEmployees = Create<PunishmentEmployee, long>("PunishmentEmployees.xlsx", Users);
            PunishmentEmployees = MapFK(PunishmentEmployees, Punishments, "Punishment", "PunishmentId");
            PunishmentEmployees = MapFK(PunishmentEmployees, Employees, "Employee", "EmployeeId");
            context.PunishmentEmployees.AddRange(PunishmentEmployees);

            //"DebtPaids",\
            var DebtPaids = Create<DebtPaid, long>("DebtPaids.xlsx", Users);
            DebtPaids = MapFK(DebtPaids, Debts, "Debt", "DebtId");
            context.DebtPaids.AddRange(DebtPaids);

            //"InputPayslipSalaries",\
            var InputPayslipSalaries = Create<PayslipSalary, long>("InputPayslipSalaries.xlsx", Users);
            InputPayslipSalaries = MapFK(InputPayslipSalaries, Payslips, "Payslip", "PayslipId");
            context.InputPayslipSalaries.AddRange(InputPayslipSalaries);

            //"DebtPaymentPlans",\
            var DebtPaymentPlans = Create<DebtPaymentPlan, long>("DebtPaymentPlans.xlsx", Users);
            DebtPaymentPlans = MapFK(DebtPaymentPlans, Debts, "Debt", "DebtId");
            context.DebtPaymentPlans.AddRange(DebtPaymentPlans);

            //"PayslipDetails",\
            var PayslipDetails = Create<PayslipDetail, long>("PayslipDetails.xlsx", Users);
            PayslipDetails = MapFK(PayslipDetails, Payslips, "Payslip", "PayslipId");
            context.PayslipDetails.AddRange(PayslipDetails);

            //"PayslipTeams",\
            var PayslipTeams = Create<PayslipTeam, long>("PayslipTeams.xlsx", Users);
            PayslipTeams = MapFK(PayslipTeams, Payslips, "Payslip", "PayslipId");
            context.PayslipTeams.AddRange(PayslipTeams);

            //"EmployeeContracts",\
            var EmployeeContracts = Create<EmployeeContract, long>("EmployeeContracts.xlsx", Users);
            EmployeeContracts = MapFK(EmployeeContracts, JobPositions, "JobPosition", "JobPositionId");
            EmployeeContracts = MapFK(EmployeeContracts, Levels, "Level", "LevelId");
            EmployeeContracts = MapFK(EmployeeContracts, SalaryChangeRequestEmployees, "SalaryChangeRequestEmployee", "SalaryRequestEmployeeId");
            context.EmployeeContracts.AddRange(EmployeeContracts);

            //"EmailTemplates",\
            var EmailTemplates = Create<EmailTemplate, long>("EmailTemplates.xlsx", Users);
            context.EmailTemplates.AddRange(EmailTemplates);

            //"TempEmployeeTSs",\
            var TempEmployeeTSs = Create<TempEmployeeTS, long>("TempEmployeeTSs.xlsx", Users);
            TempEmployeeTSs = MapFK(TempEmployeeTSs, Employees, "Employee", "EmployeeId");
            context.TempEmployeeTSs.AddRange(TempEmployeeTSs);

            //"TempEmployeeTalents",\

            var TempEmployeeTalents = Create<TempEmployeeTalent, long>("TempEmployeeTalents.xlsx", Users, new Dictionary<string, string>() { { "FullName2", "FullName" } });
            TempEmployeeTalents = MapFK(TempEmployeeTalents, JobPositions, "JobPosition", "JobPositionId");
            TempEmployeeTalents = MapFK(TempEmployeeTalents, Levels, "Level", "LevelId");
            TempEmployeeTalents = MapFK(TempEmployeeTalents, Branches, "Branch", "BranchId");
            context.TempEmployeeTalents.AddRange(TempEmployeeTalents);

            //"Refunds",\
            var Refunds = Create<Refund, long>("Refunds.xlsx", Users);
            context.Refunds.AddRange(Refunds);

            //"RefundEmployees"
            var RefundEmployees = Create<RefundEmployee, long>("RefundEmployees.xlsx", Users);
            RefundEmployees = MapFK(RefundEmployees, Refunds, "Refund", "RefundId");
            RefundEmployees = MapFK(RefundEmployees, Employees, "Employee", "EmployeeId");
            context.RefundEmployees.AddRange(RefundEmployees);

            context.SaveChanges();

        }
    }
}
