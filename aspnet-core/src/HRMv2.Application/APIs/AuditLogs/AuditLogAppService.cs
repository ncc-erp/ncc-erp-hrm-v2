using Abp.Auditing;
using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Authorization.Users;
using HRMv2.Manager.Categories.AuditLogs;
using HRMv2.Manager.Categories.AuditLogs.Dto;
using HRMv2.Manager.Categories.Banks;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.AuditLogs
{
    [AbpAuthorize]
    public class AuditLogAppService : HRMv2AppServiceBase
    {
        private readonly AuditLogManager _auditLogManager;
        public AuditLogAppService(AuditLogManager auditLogManager)
        {
            _auditLogManager = auditLogManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_AuditLog_View)]
        public async Task<GridResult<GetAuditLogDto>> GetAllPagging(GridParam input)
        {
            return await _auditLogManager.GetAllPagging(input);
        }

        [HttpGet]
        public async Task<List<GetAllEmailAddressInAuditLogDto>> GetAllEmailAddressInAuditLog()
        {
            return await _auditLogManager.GetAllEmailAddressInAuditLog();
        }
    }
}
