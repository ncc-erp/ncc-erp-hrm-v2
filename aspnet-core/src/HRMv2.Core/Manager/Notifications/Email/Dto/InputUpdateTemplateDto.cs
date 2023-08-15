using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    [AutoMapTo(typeof(EmailTemplate))]

    public class InputUpdateTemplateDto : EmailDto
    {
    }
}
