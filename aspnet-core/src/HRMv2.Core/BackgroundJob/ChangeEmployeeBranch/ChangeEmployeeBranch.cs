using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.BackgroundJob.ChangeEmployeeBranch
{
    public class ChangeEmployeeBranch : BackgroundJob<ChangeBranchDto>, ITransientDependency
    {
        public readonly EmployeeManager _employeeManager;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        public ChangeEmployeeBranch(EmployeeManager employeeManager, IAbpSession abpSession, IUnitOfWorkManager unitOfWork)
        {
            _employeeManager = employeeManager;
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
        }
        [UnitOfWork]
        public override void Execute(ChangeBranchDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                this._employeeManager.UpdateNewBranch(args);
                uow.SaveChanges();
            }
        }
    }
}
