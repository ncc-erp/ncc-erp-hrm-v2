using HRMv2.Utils;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class InputPayslipMailTemplate
    {
        public string EmployeeFullName { get; set; }
        public string PayrollYear { get; set; }
        public string PayrollMonth { get; set; }
        public string PayrollWorkingDay { get; set; }
        public string PayrollOpentalkCount { get; set; }
        public string PayslipWorkingDay { get; set; }
        public string PayslipOpentalk { get; set; }
        public string PayslipOTHour { get; set; }
        public string PayslipTotalWorkingDay { get; set; }
        public string PayslipRemainLeaveDayBefore { get; set; }
        public string PayslipRemainLeaveDayAfter { get; set; }
        public string PayslipAddedLeaveDay { get; set; }
        public string PayslipOffDays { get; set; }
        public string PayslipRefundDays { get; set; }
        public string NormalSalary { get; set; }
        public string OTSalary { get; set; }
        public string MaternityLeaveSalary { get; set; }
        public string SendToEmail { get; set; }
        public string TotalRealSalary { get; set; }
        public string ConfirmUrl { get; set; }
        public string ComplainUrl { get; set; }
        public string ComplainDeadline { get; set; }
        public string PayrollTotalWorkingDay => (float.Parse(PayrollWorkingDay) + float.Parse(PayrollOpentalkCount) * 0.5).ToString();

        public List<PayslipDetailEmailDto> ListPayslipDetail { get; set; }
        public List<PayslipSalaryEmailDto> ListPayslipSalary { get; set; }
        public List<MezonTokenEmailDto> ListMezonToken { get; set; }
        public string Subject => $"[NCC][{EmployeeFullName}] THÔNG BÁO CHI TIẾT LƯƠNG THÁNG {PayrollMonth}/{PayrollYear}";
        public string TotalBonus => CommonUtil.FormatDisplayMoney(CommonUtil.RoundMoneyVND(ListPayslipDetail.Where(x=> x.Money > 0).Sum(x => x.Money)));
        public string TotalMinus => CommonUtil.FormatDisplayMoney(CommonUtil.RoundMoneyVND(ListPayslipDetail.Where(x => x.Money < 0).Sum(x => x.Money)));
        public string TotalMezonToken => CommonUtil.FormatDisplayMoney(CommonUtil.RoundMoneyVND(ListMezonToken?.Sum(x => x.Amount) ?? 0));

        public string PayslipSalaries => "<table style='width:100%;border-collapse: collapse;'><tbody>"+string.Join("", ListPayslipSalary.Select(s =>
        @"<tr style=""height:50px;font-size:14pt""><td style=""height:50px;width:75%;border-width:0;vertical-align:top"">" + s.Note
            + @":</td><td style=""height:50px;font-size:14pt;width:25%;border-width:0;vertical-align:top""><strong>"
            + s.FormatMoney + "&nbsp;VND</strong><span style='font-size:12pt'> (" + s.FormatDate + ")</span></td></tr>")) + "</tbody></table>";
        public string PayslipBonuses => "<table style='width:100%;border-collapse: collapse;'><tbody>"+string.Join("", ListPayslipDetail.Where(x => x.Money > 0).Select(s =>
        @"<tr style=""height:50px;font-size:14pt""><td style=""height:50px;width: 75%;border-width:0;vertical-align:top"">" + s.Note
            + @":</td><td style=""height:50px;font-size:14pt;border-width:0;width: 25%;vertical-align:top;text-align:right;""><strong>"
            + s.FormatMoney + "&nbsp;VND</strong></td></tr>"))+"</tbody></table>";
        public string PayslipMinuses => "<table style='width:100%;border-collapse: collapse;'><tbody>"+ string.Join("", ListPayslipDetail.Where(x => (x.Money <= 0 && x.Type == PayslipDetailType.Punishment)||(x.Money < 0 && x.Type != PayslipDetailType.Punishment)).Select(s =>
        @"<tr style=""height:50px;font-size:14pt""><td style=""height:50px;width: 75%;border-width:0;vertical-align:top"">" + s.Note
            + @"</td><td style=""height:50px;font-size:14pt;border-width:0;width: 25%;vertical-align:top;text-align:right; color: hsl(357deg 85% 52%)""><strong>"
            + s.FormatMoney + "&nbsp;VND</strong></td></tr>"))+ "</tbody></table>";
        public string PayslipTokenBonuses => (ListMezonToken == null || ListMezonToken.Count == 0)
            ? ""
            : "<table style='width:100%;border-collapse: collapse;'><tbody>" + string.Join("", ListMezonToken.Select(s =>
            @"<tr style=""height:50px;font-size:14pt""><td style=""height:50px;width: 75%;border-width:0;vertical-align:top"">" + s.Note
                + @":</td><td style=""height:50px;font-size:14pt;border-width:0;width: 25%;vertical-align:top;text-align:right;""><strong>"
                + s.FormatAmount + "&nbsp;<span style='color:#1e3a8a'>MZD</span></strong></td></tr>")) + "</tbody></table>";
    }

    public class PayslipDetailEmailDto
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public string FormatMoney => CommonUtil.FormatDisplayMoney(Money);
        public PayslipDetailType Type { get; set; }
    }

    public class PayslipSalaryEmailDto
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public string FormatDate => DateTimeUtils.ToString(Date);
        public string FormatMoney => CommonUtil.FormatDisplayMoney(Money);

    }

    public class MezonTokenEmailDto
    {
        public long Amount { get; set; }
        public string Note { get; set; }
        public string FormatAmount => CommonUtil.FormatDisplayMoney(Amount);
    }


}
