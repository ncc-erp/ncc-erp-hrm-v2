using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.BackgroundJob.SendMail
{
    public class SendMail : BackgroundJob<MailPreviewInfoDto>, ITransientDependency
    {
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        EmailManager _emailManager;
        public SendMail(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, EmailManager emailManager)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _emailManager = emailManager;
        }

        [UnitOfWork]
        public override void Execute(MailPreviewInfoDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _emailManager.Send(args);
                //uow.SaveChanges();
            }
        }
    }
}
