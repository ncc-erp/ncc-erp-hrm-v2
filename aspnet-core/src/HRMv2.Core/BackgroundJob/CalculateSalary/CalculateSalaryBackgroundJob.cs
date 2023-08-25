using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;

namespace HRMv2.BackgroundJob.CalculateSalary
{
    public class CalculateSalaryBackgroundJob : BackgroundJob<CollectPayslipDto>, ITransientDependency
    {
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;

        PayslipManager _payslipManager;
        public CalculateSalaryBackgroundJob(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, PayslipManager payslipManager)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _payslipManager = payslipManager;
        }

        [UnitOfWork]
        public override void Execute(CollectPayslipDto args)
        {
            using(_abpSession.Use(args.TenantId, args.CurrentUserLoginId))
            {
                var uow = _unitOfWork.Current;

                using (uow.SetTenantId(args.TenantId))
                {
                    _payslipManager.GeneratePayslipsTryCacth(args);
                    uow.SaveChanges();
                }
            }            
            
        }
    }
}
