using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.MezonTokens;
using HRMv2.Manager.MezonTokens.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using Microsoft.AspNetCore.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.BackgroundJob.SentToken
{
    public class SendMezonTokenBackgroundJob : BackgroundJob<InputSendMezonTokenBJob>, ITransientDependency
    {

        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        MezonTokenManager _mezonTokenManager;
        public SendMezonTokenBackgroundJob(IAbpSession abpSession, IUnitOfWorkManager unitOfWork, MezonTokenManager mezonTokenManager)
        {
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
            _mezonTokenManager = mezonTokenManager;
        }

        [UnitOfWork]
        public override void Execute(InputSendMezonTokenBJob args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _mezonTokenManager.SendMezonTokenInBJob(args).GetAwaiter().GetResult();
                //uow.SaveChanges();
            }
        }
    }
}
