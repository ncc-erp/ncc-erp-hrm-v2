using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Notifications.SendDMToMezon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.BackgroundJob.SendDirectMessage
{
    public class SendDirectMessage : BackgroundJob<MezonPreviewInfoDto>, ITransientDependency
    {
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        private readonly SendDMService _sendDMService;

        public SendDirectMessage(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, SendDMService sendDMService)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _sendDMService = sendDMService;
        }

        [UnitOfWork]
        public override void Execute(MezonPreviewInfoDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _sendDMService.SendDMToUser(args);
            }

        }
    }
}
