using HRMv2.Constants.Dictionary;
using HRMv2.Entities;
using HRMv2.Manager.Notifications.Templates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.EntityFrameworkCore.Seed.Tenants
{
    public class TenantEmailTemplateBuilder
    {
        private readonly HRMv2DbContext _context;
        private int? _tenantId;
        public TenantEmailTemplateBuilder(HRMv2DbContext context, int? tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }
        public void Create()
        {
            CreateMailTemplate();
        }
        private void CreateMailTemplate()
        {
            var mailTemplates = new List<EmailTemplate>();
            var mails = _context.EmailTemplates.IgnoreQueryFilters()
                .Where(q => q.TenantId == _tenantId)
                .Select(x => x.Type)
                .ToList();

            Enum.GetValues(typeof(MailFuncEnum))
                .Cast<MailFuncEnum>()
                .ToList()
                .ForEach(e =>
                {
                    if (!mails.Contains(e))
                    {
                        var isSeedMailExist = DictionaryHelper.SeedMailDic.ContainsKey(e);
                        mailTemplates.Add(
                            new EmailTemplate
                            {
                                Subject = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Subject : string.Empty,
                                Name = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Name : string.Empty,
                                BodyMessage = TemplateHelper.ContentEmailTemplate(e),
                                Description = isSeedMailExist ? DictionaryHelper.SeedMailDic[e].Description : string.Empty,
                                Type = e,
                                TenantId = _tenantId
                            }
                        );
                    }
                });

            _context.AddRange(mailTemplates);
            _context.SaveChanges();
        }
    }
}
