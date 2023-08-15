using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.EmailTemplates
{
    [AbpAuthorize]
    public class EmailTemplateAppService : HRMv2AppServiceBase
    {
        private readonly EmailManager _emailManager;

        public EmailTemplateAppService(EmailManager emailManager)
        {
            _emailManager = emailManager;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Admin_EmailTemplate_View)]
        public async Task<List<EmailDto>> GetAll()
        {
            return await _emailManager.GetAllMailTemplate();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Admin_EmailTemplate_PreviewTemplate)]
        public GetMailPreviewInfoDto PreviewTemplate(long id)
        {
            return  _emailManager.PreviewTemplate(id);
        }

        [HttpGet]
        public GetMailPreviewInfoDto GetTemplateById(long id)
        {
            return _emailManager.GetTemplateById(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Admin_EmailTemplate_Edit)]  
        public async Task<UpdateTemplateDto> UpdateTemplate(UpdateTemplateDto input)
        {
            return await _emailManager.UpdateTemplate(input);
        }

        [HttpPost]
        public void SendMail(MailPreviewInfoDto input)
        {
            _emailManager.SendMail(input);
        }
    }
}
