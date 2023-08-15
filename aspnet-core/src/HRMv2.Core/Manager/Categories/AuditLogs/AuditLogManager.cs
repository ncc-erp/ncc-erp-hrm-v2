using Abp;
using Abp.Auditing;
using HRMv2.Authorization.Users;
using HRMv2.Manager.Categories.AuditLogs.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.AuditLogs
{
    public class AuditLogManager : BaseManager
    {
        public AuditLogManager(IWorkScope workScope) : base(workScope)
        { }

        public async Task<GridResult<GetAuditLogDto>> GetAllPagging(GridParam input)
        {
            var qEmailAddress = WorkScope.GetAll<User>().Select(s => new { s.Id , s.EmailAddress});
            var query = WorkScope.GetAll<AuditLog>()
                .Select(s => new GetAuditLogDto
                {
                    ExecutionDuration = s.ExecutionDuration,
                    ExecutionTime = s.ExecutionTime,
                    MethodName = s.MethodName,
                    Parameters = s.Parameters,
                    ServiceName = s.ServiceName,
                    UserId = s.UserId,
                    UserIdString = s.UserId.ToString(),
                    EmailAddress = qEmailAddress.Where(x => x.Id == s.UserId).Select(x => x.EmailAddress).FirstOrDefault()
                });

            return await query.GetGridResult(query, input);
        }
        public async Task<List<GetAllEmailAddressInAuditLogDto>> GetAllEmailAddressInAuditLog()
        {
            var userIdInAuditLog = await WorkScope.GetAll<AuditLog>().Where(s => s.UserId != null)
                .Select(s => s.UserId).Distinct().ToListAsync();

            var emailAddressByUserId = WorkScope.GetAll<User>().Where(s => userIdInAuditLog.Contains(s.Id)).Select(s => new GetAllEmailAddressInAuditLogDto
            {
                EmailAddress = s.EmailAddress,
                UserId = s.Id
            }).ToListAsync();

            return await emailAddressByUserId;
        }
    }
}
