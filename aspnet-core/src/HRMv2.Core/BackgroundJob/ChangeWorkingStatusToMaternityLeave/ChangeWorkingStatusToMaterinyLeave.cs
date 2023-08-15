﻿using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.BackgroundJob.ChangeWorkingStatusToMaternityLeave
{
    public class ChangeWorkingStatusToMaterinyLeave : BackgroundJob<ToMaternityLeaveDto>, ITransientDependency
    {
        public ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatusManager;

        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWork;
        public ChangeWorkingStatusToMaterinyLeave(ChangeEmployeeWorkingStatusManager changeEmployeeWorkingStatusManager, IAbpSession abpSession, IUnitOfWorkManager unitOfWork)
        {
            _changeEmployeeWorkingStatusManager = changeEmployeeWorkingStatusManager;
            _abpSession = abpSession;
            _unitOfWork = unitOfWork;
        }
        [UnitOfWork]
        public override void Execute(ToMaternityLeaveDto args)
        {
            _abpSession.Use(args.TenantId, args.CurrentUserLoginId);
            var uow = _unitOfWork.Current;

            using (uow.SetTenantId(args.TenantId))
            {
                _changeEmployeeWorkingStatusManager.ToMaternityLeave(args);
                uow.SaveChanges();
            }

        }
    }
}
