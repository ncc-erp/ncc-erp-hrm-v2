using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;
using HRMv2.MultiTenancy;
using HRMv2.Entities;

namespace HRMv2.EntityFrameworkCore
{
    public class HRMv2DbContext : AbpZeroDbContext<Tenant, Role, User, HRMv2DbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<IssuedBy> IssuedBys { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<PunishmentType> PunishmentTypes { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Punishment> Punishments { get; set; }
        public DbSet<PunishmentEmployee> PunishmentEmployees { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<BenefitEmployee> BenefitEmployees { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<BonusEmployee> BonusEmployees { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<DebtPaymentPlan> DebtPaymentPlans { get; set; }
        public DbSet<DebtPaid> DebtPaids { get; set; }
        public DbSet<EmployeeContract> EmployeeContracts { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayslipTeam> PayslipTeams { get; set; }
        public DbSet<PayslipSalary> InputPayslipSalaries { get; set; }
        public DbSet<PayslipDetail> PayslipDetails { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<EmployeeTeam> EmployeeTeams { get; set; }
        public DbSet<SalaryChangeRequest> SalaryChangeRequests { get; set; }
        public DbSet<SalaryChangeRequestEmployee> SalaryChangeRequestEmployees { get; set; }
        public DbSet<EmployeeWorkingHistory> EmployeeWorkingHistories { get; set; }
        public DbSet<EmployeeBranchHistory> EmployeeBranchHistories { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<TempEmployeeTalent> TempEmployeeTalents { get; set; }
        public DbSet<TempEmployeeTS> TempEmployeeTSs { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<RefundEmployee> RefundEmployees { get; set; }
        public DbSet<PunishmentFund> PunishmentFunds { get; set; }

        public HRMv2DbContext(DbContextOptions<HRMv2DbContext> options)
            : base(options)
        {
        }
    }
}
