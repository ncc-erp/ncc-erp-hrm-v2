using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;

namespace HRMv2.BackgroundJob.ChangeWorkingStatusToPause{
    public class ChangeWorkingStatusToPause : BackgroundJob<ToPauseDto>, ITransientDependency
    {
        private readonly ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatusManager;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;

        public ChangeWorkingStatusToPause(ChangeEmployeeWorkingStatusManager changeEmployeeWorkingStatusManager, IAbpSession abpSession, IUnitOfWorkManager unitOfWork)
        {
            _changeEmployeeWorkingStatusManager = changeEmployeeWorkingStatusManager;
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
    
        }
        [UnitOfWork]
        public override void Execute(ToPauseDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _changeEmployeeWorkingStatusManager.ToPause(args);
                uow.SaveChanges();
            }
        }
    }
}
