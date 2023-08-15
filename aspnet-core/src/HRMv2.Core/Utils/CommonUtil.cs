using Abp.Timing;
using HRMv2.Entities;
using HRMv2.Manager.BackgroundJobInfos.Dto;
using HRMv2.Manager.Categories.UserTypes.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Utils
{
    public class CommonUtil
    {
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }
        public static string NowToYYYYMMddHHmmss()
        {
            return GetNow().ToString("yyyyMMddHHmmss");
        }
        public static UserTypeDto GetUserType(UserType userType)
        {
            return USERTYPE_COLOR.Where(x => (UserType)x.Id == userType).FirstOrDefault();
        
        }

        public static List<UserTypeDto> USERTYPE_COLOR = new List<UserTypeDto>{
           new UserTypeDto{Id = UserType.Internship.GetHashCode(), Name="TTS", Color= "#007bff", Code= "INTERNSHIP", ContractPeriodMonth = 12, ShortName="TTS" },
           new UserTypeDto{Id = UserType.Collaborators.GetHashCode(), Name="CTV", Color= "#ffc107", Code= "COLLABRATORS", ContractPeriodMonth = 0, ShortName="CTV"},
           new UserTypeDto{Id = UserType.Staff.GetHashCode(), Name="Staff", Color= "#28a745", Code="STAFF", ContractPeriodMonth = 0, ShortName="Staff"},
           new UserTypeDto{Id = UserType.ProbationaryStaff.GetHashCode(), Name="T.Việc", Color= "#e08b29", Code= "PROBATIONARYSTAFF", ContractPeriodMonth= 2, ShortName= "T.Việc"},
           new UserTypeDto{Id = UserType.Vendor.GetHashCode(), Name="Vendor", Color= "#6c757d", Code= "Vendor", ContractPeriodMonth= 0, ShortName= "Vendor"}

        };

        public static int GetValueOfUsertype(string userTypeName)
        {

            var userTypeValue = ((UserType[])Enum.GetValues(typeof(UserType)))
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .Where(c => c.Name.ToLower() == userTypeName.ToLower())
                .Select(c => new {c.Value, c.Name})
                .FirstOrDefault();
            if(userTypeValue == default) { return -1; }
            return userTypeValue.Value;

        }

        public static int GetValueOfSex(string sexName)
        {
            var sexValue = ((Sex[])Enum.GetValues(typeof(Sex)))
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .Where(c => c.Name.ToLower() == sexName.ToLower())
                .Select(c => c.Value)
                .FirstOrDefault();
            if (sexValue == default) { sexValue = -1; }
            return sexValue;

        }

        public static int GetValueOfInsuranceStatus(string insuranceStatusName)
        {
            var insuranceStatusValue = ((InsuranceStatus[])Enum.GetValues(typeof(InsuranceStatus)))
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .Where(c => c.Name.ToLower() == insuranceStatusName.ToLower())
                .Select(c => c.Value)
                .FirstOrDefault();
            if (insuranceStatusValue == default) { insuranceStatusValue = -1; }
            return insuranceStatusValue;

        }

        public static int GetValueOfEmployeeStatus(string statusName)
        {
            var statusValue = ((EmployeeStatus[])Enum.GetValues(typeof(EmployeeStatus)))
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .Where(c => c.Name.ToLower() == statusName.ToLower())
                .Select(c => c.Value)
                .FirstOrDefault();
            if (statusValue == default) { statusValue = -1; }
            return statusValue;

        }

        public static double CalculateInterestValue(DateTime startDate, DateTime endDate, double principal, double interestRate)
        {
            var totalDay = (endDate - startDate).TotalDays;
            var interestRatePerDay = (interestRate / 365);
            var realInterestRate = interestRatePerDay * totalDay;
            var totalInterest = principal * (realInterestRate / 100);
            return totalInterest;
        }

        public static string GetSalaryRequestTypeName(SalaryRequestType type)
        {
            switch(type)
            {
                case SalaryRequestType.Initial: return "Lương khởi tạo";
                case SalaryRequestType.Change: return "Tăng lương";
                case SalaryRequestType.MaternityLeave: return "Nghỉ sinh";
                case SalaryRequestType.BackToWork: return "BackToWork";
                case SalaryRequestType.StopWorking: return "StopWorking";
            }   
            return type.ToString();
        }

        public static string GetWorkingStatusName(EmployeeStatus type)
        {
            switch (type)
            {
                case EmployeeStatus.Working: return "Working";
                case EmployeeStatus.MaternityLeave: return "Maternity";
                case EmployeeStatus.Pausing: return "Pausing";
                case EmployeeStatus.Quit: return "Quit";
            }
            return type.ToString();
        }

        public static string GetInsuranceStatusName(InsuranceStatus type)
        {
            switch (type)
            {
                case InsuranceStatus.BHXH: return "BHXH";
                case InsuranceStatus.NONE: return "NONE";
                case InsuranceStatus.PVI: return "PVI";
            }
            return type.ToString();
        }

        public static string GetUserTypeNameVN(UserType type)
        {
            switch (type)
            {
                case UserType.Internship: return "TTS"; 
                case UserType.Staff: return "Staff";
                case UserType.ProbationaryStaff: return "T.Việc";
                case UserType.Collaborators: return "CTV";
                case UserType.Vendor: return "Vendor";
            }
            return type.ToString();
        }

        private static string GenerateInputSalaryNote(UserType userType)
        {
            switch (userType)
            {
                case UserType.Internship:
                    return "TTS";
                case UserType.Collaborators:
                    return "CTV";
                case UserType.ProbationaryStaff:
                    return "";
                case UserType.Staff:
                    return "Staff";
                    default : return userType.ToString();
            }

        }

        private static string GenerateInputSalaryNote(SalaryRequestType changType)
        {
            switch (changType)
            {
                case SalaryRequestType.Initial:
                    return "";
                case SalaryRequestType.Change:
                    return "";
                case SalaryRequestType.MaternityLeave:
                    return "nghỉ sinh";
                case SalaryRequestType.StopWorking:
                    return "Nghỉ việc/tạm nghỉ";
                case SalaryRequestType.BackToWork:
                    return "quay lại làm việc";
                default: return changType.ToString();
            }

        }

        public static string GenerateInputSalaryNote(UserType userType, SalaryRequestType changType)
        {
            if (userType == UserType.ProbationaryStaff)
            {
                return "Lương thử việc";
            }

            if (changType == SalaryRequestType.StopWorking)
            {
                return "Nghỉ việc";
            }

            return $"{GenerateInputSalaryNote(userType)} {GenerateInputSalaryNote(changType)}";
        }
        public static BadgeInfoDto GetRequestUpdateInfoStatusName(RequestStatus type)
        {
            switch (type)
            {
                case RequestStatus.Pending: return new BadgeInfoDto { Name = "Pending", Color = "#E08B29" };
                case RequestStatus.Approved: return new BadgeInfoDto { Name = "Approved", Color = "#28a745" };
                case RequestStatus.Rejected: return new BadgeInfoDto { Name = "Rejected", Color = "#6c757d" };

            }
            return null;
        }

        public static string FormatDisplayMoney(double money)
        {
            if(money == 0)
            {
                return money.ToString();
            }
            return String.Format("{0:0,0}", Math.Round(money));
        }

        /// <summary>
        /// Làm tròn đến hàng nghìn
        /// dùng cho làm tròn tiền VNĐ
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static double RoundMoneyVND(double money)
        {
            return Math.Round(money);
        }

        public static HashSet<PayrollStatus> ListPayrollStatusCanUpdate = new HashSet<PayrollStatus> { PayrollStatus.New, PayrollStatus.RejectedByKT, PayrollStatus.RejectedByCEO };
        public static string ListPayrollStatusCanUpdateToString = string.Join(", ", ListPayrollStatusCanUpdate.ToArray());

        public static bool IsCanUpdatePayslip(PayrollStatus status)
        {
            return ListPayrollStatusCanUpdate.Contains(status);
        }
        public static string GetNameByFullName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(" ") + 1);
        }

        public static string GetSurNameByFullName(string fullName)
        {
            return fullName.Substring(0, fullName.LastIndexOf(" "));
        }

        public static string ContractTypeName(UserType userType)
        {
                switch (userType)
                {
                    case UserType.ProbationaryStaff:
                        return "HĐTV-NCC";
                    case UserType.Staff:
                        return "HĐLĐ-NCC";

                    case UserType.Collaborators:
                        return "HĐCTV-NCC";
                    case UserType.Internship:
                        return "HĐĐT-NCC";
                    default: return userType.ToString();

                }       
    }

        public static string GenerateContractCode(string email, int month, int year, UserType userType)
        {
            var contractCode = $"{email.Split("@")[0]}/{month}/{year}/{ContractTypeName(userType)}";

            return contractCode;
        }

        public static DateTime? GenerateContractEndDate(UserType userType, DateTime contractStartDate)
        {
            switch (userType)
            {
                case UserType.ProbationaryStaff:
                    return contractStartDate.AddMonths(HRMv2Consts.TVIEC_CONTRACT_PERIOD_MONTH);
                default:
                    return null;
            }
        }

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static UserType MaptalentUsertype(UserType typeFromTalent)
        {
            var userTypeMapers = new UserType[] { UserType.Internship, UserType.Staff};
            return userTypeMapers[(int)typeFromTalent];
        }

        public static string GetPaymentPlansToText(List<PaidPlanDto> input)
        {
            var rs = "";
            var index = 1;
            foreach(var s in input)
            {
                rs += @"<table style=""border-collapse:collapse;width:100%;border-width:0""><tr style=""border - width:0"">
                <td style=""border-width:0;text-align: center;width:10%"">" + index + @"</td><td style=""text-align: right;height:50px;font-size:14pt;border-width:0;width:25%""><strong>" + FormatDisplayMoney(s.Money) + @"&nbsp;VND</strong><span style=""font-size:12pt""></span></td><td style=""text-align: center;height:50px;font-size:14pt;border-width:0;width:65%"">" + s.MethodPaidNote + "</td></tr></table>";
                index++;
            }
            return rs;
        }
        public static string NumberToText(double inputNumber)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            string sNumber = inputNumber.ToString("");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;  

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {

                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result != "" ? char.ToUpper(result[0]) + result.Substring(1) : "";
        }

        public static string GetUserNameByEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return "null";
            }
            return email.Split("@")[0];
        }
        public static string GetDiscordTagUser(string email)
        {
            return "${" + GetUserNameByEmail(email) + "}";
        }

        public static TemplateType GetTemplateType(MailFuncEnum type)
        {
            switch (type)
            {
                case MailFuncEnum.Payslip:
                case MailFuncEnum.Debt:
                case MailFuncEnum.Bonus:
                case MailFuncEnum.Checkpoint:
                    return TemplateType.Mail;
                case MailFuncEnum.ContractDT:
                case MailFuncEnum.ContractCTV:
                case MailFuncEnum.ContractBM:
                case MailFuncEnum.ContractLD:
                case MailFuncEnum.ContractTV:
                    return TemplateType.Print;
            }
            return TemplateType.Mail;
        }
        public static string GetMailType(string jobAgr)
        {
            var mailFuncTypePayslip = $"\"MailFuncType\":{(int)MailFuncEnum.Payslip}";
            var mailFuncTypeDebt = $"\"MailFuncType\":{(int)MailFuncEnum.Debt}";
            var mailFuncTypeBonus = $"\"MailFuncType\":{(int)MailFuncEnum.Bonus}";
            var mailFuncTypeCheckpoint = $"\"MailFuncType\":{(int)MailFuncEnum.Checkpoint}";
            if (jobAgr.Contains(mailFuncTypePayslip))
            {
                return "Gửi mail bảng lương";
            }
            if (jobAgr.Contains(mailFuncTypeDebt))
            {
                return "Gửi mail Debt";
            }
            if (jobAgr.Contains(mailFuncTypeBonus))
            {
                return "Gửi mail Bonus";
            }
            if (jobAgr.Contains(mailFuncTypeCheckpoint))
            {
                return "Gửi mail thông báo kết quả Checkpoint";
            }
            return "";
        }

        public static List<GetBGJobsDescription> BackgroundJobDescription(string jobAgr)
        {
            var desByMailType = "";
            if (jobAgr.Contains("MailFuncType"))
            {
                desByMailType = GetMailType(jobAgr);
            }
            var listDes = new List<GetBGJobsDescription>()
            {
                new GetBGJobsDescription()
                {
                    SubJobType = "ChangeWorkingStatusToPause",
                    Description= "Nhân viên tạm nghỉ"
                },
                new GetBGJobsDescription()
                {
                    SubJobType = "ChangeWorkingStatusToMaterinyLeave",
                    Description= "Nhân viên nghỉ sinh"
                },
                new GetBGJobsDescription()
                {
                    SubJobType = "ChangeWorkingStatusToQuit",
                    Description= "Nhân viên nghỉ việc"
                },
                new GetBGJobsDescription()
                {
                    SubJobType = "ChangeWorkingStatusToWorking",
                    Description= "Nhân viên trở lại làm việc"
                },
                new GetBGJobsDescription()
                {
                    SubJobType = "CalculateSalaryBackgroundJob",
                    Description= "Tính lương"
                },
                new GetBGJobsDescription()
                {
                    SubJobType = "SendMail",
                    Description= desByMailType
                },


            };
            return listDes;
        }

        public static string GenerateFinfastOutcomeEntryName(DateTime payrollApplyMonth)
        {
            return $"Chi Bảng lương tháng {payrollApplyMonth.Month}/{payrollApplyMonth.Year}";
        }
    }
}
