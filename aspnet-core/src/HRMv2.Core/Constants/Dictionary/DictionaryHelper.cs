using HRMv2.Manager.Notifications.Templates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Constants.Dictionary
{
    public class DictionaryHelper
    {
        public static readonly Dictionary<string, string[]> FileTypeDic =
            new Dictionary<string, string[]>()
            {
                {"IMAGE", new string[] { "jpeg", "png", "svg", "jpg"} }
            };

        public static readonly Dictionary<NotifyTemplateEnum, MailInfoDto> SeedMailDic = new Dictionary<NotifyTemplateEnum, MailInfoDto>()
        {
            {
                NotifyTemplateEnum.Payslip,
                new MailInfoDto
                {
                    Name = "Payslip",
                    Description = "",
                    Subject = "[NCC]_THÔNG BÁO CHI TIẾT LƯƠNG",
                }
            },
            {
                NotifyTemplateEnum.ContractBM,
                new MailInfoDto
                {
                    Name = "Hợp đồng bảo mật",
                    Description = "",
                    Subject = "[NCC]_HỢP ĐỒNG BẢO MẬT",
                }
            },
            {
                NotifyTemplateEnum.ContractDT,
                new MailInfoDto
                {
                    Name = "Hợp đồng đào tạo",
                    Description = "",
                    Subject = "[NCC]_HỢP ĐỒNG ĐÀO TẠO",
                }
            },
            {
                NotifyTemplateEnum.ContractTV,
                new MailInfoDto
                {
                    Name = "Hợp đồng thử việc",
                    Description = "",
                    Subject = "[NCC]_HỢP ĐỒNG THỬ VIỆC",
                }
            },
            {
                NotifyTemplateEnum.ContractCTV,
                new MailInfoDto
                {
                    Name = "Hợp đồng cộng tác viên",
                    Description = "",
                    Subject = "[NCC]_HỢP ĐỒNG CỘNG TÁC VIÊN",
                }
            },
            {
                NotifyTemplateEnum.ContractLD,
                new MailInfoDto
                {
                    Name = "Hợp đồng lao động",
                    Description = "",
                    Subject = "[NCC]_HỢP ĐỒNG LAO ĐỘNG",
                }
            },
             {
                NotifyTemplateEnum.Debt,
                new MailInfoDto
                {
                    Name = "Debt",
                    Description = "",
                    Subject = "[NCC]_XÁC NHẬN KHOẢN VAY",
                }
            },
            {
                NotifyTemplateEnum.Bonus,
                new MailInfoDto
                {
                    Name = "Bonus Checkpoint",
                    Description = "",
                    Subject = "[NCC]_THƯỞNG CHECKPOINT"
                }
            },
            {
                NotifyTemplateEnum.Checkpoint,
                new MailInfoDto
                {
                    Name = "Checkpoint Result",
                    Description = "",
                    Subject = "[NCC]_KẾT QUẢ CHECKPOINT"
                }
            },
            {
                NotifyTemplateEnum.PayrollPendingCEO,
                new MailInfoDto
                {
                    Name = "Payroll PendingCEO",
                    Description = "",
                    Subject = "[NCC]_BẢNG LƯƠNG"
                }
            },
            {
                NotifyTemplateEnum.PayrollApprovedByCEO,
                new MailInfoDto
                {
                    Name = "Payroll ApprovedByCEO",
                    Description = "",
                    Subject = "[NCC]_BẢNG LƯƠNG"
                }
            },
             {
                NotifyTemplateEnum.PayrollRejectedByCEO,
                new MailInfoDto
                {
                    Name = "Payroll RejectedByCEO",
                    Description = "",
                    Subject = "[NCC]_BẢNG LƯƠNG"
                }
            },
            {
                NotifyTemplateEnum.PayrollExecuted,
                new MailInfoDto
                {
                    Name = "Payroll Executed",
                    Description = "",
                    Subject = "[NCC]_BẢNG LƯƠNG"
                }
            },
            {
                NotifyTemplateEnum.LinkToPreviewPayslip,
                new MailInfoDto
                {
                    Name = "Payslip Email Link",
                    Description = "",
                    Subject = "[NCC]_THÔNG BÁO CHI TIẾT LƯƠNG"
                }
            },
            {
                NotifyTemplateEnum.MezonDMLinkToPreviewPayslip,
                new MailInfoDto
                {
                    Name = "Payslip Mezon Direct Message Link",
                    Description = "",
                    Subject = "[NCC]_THÔNG BÁO CHI TIẾT LƯƠNG"
                }
            }

        };
    }
}