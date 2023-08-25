using Abp.UI;
using Abp.Webhooks;
using Castle.Core.Logging;
using DocumentFormat.OpenXml.Bibliography;
using HRMv2.Constants.Enum;
using HRMv2.Entities;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet.Dto;
using NccCore.Uitls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.SalaryCalculators
{
    public class NCCSalaryCalculator
    {
        private readonly ILogger Logger = NullLogger.Instance;
        // Input-----------------------------------------------

        public static int DAY_OF_MONTH_TO_ADD_LEAVE_DAY = 16;//working start date >= 15 -> no leave day to add
        public static float WORKING_DAY_PERCENTAGE_TO_ADD_LEAVE_DAY = 0.66666f;//working start date >= 15 -> no leave day to add
        public PayrollDto InputPayroll { get; set; }
        public EmployeeToCalDto InputEmployee { get; set; }
        /// <summary>
        /// Order by ApplyDate DESC
        /// </summary>
        public List<SalaryInputForPayslipDto> InputSalaries { get; set; }//orderby applydate desc
        public HashSet<DateTime> InputSettingOffDates { get; set; }
        public List<DateOTHourDto> InputOTs { get; set; }
        public List<DateTime> InputOpenTalkDates { get; set; }
        public List<DateTime> InputNormalWorkingDates { get; set; }
        public float InputRemainLeaveDayBefore => this.InputEmployee.RemainLeaveDay;

        /// <summary>
        /// List Opentalk date duoc tinh luong, thuong la 2 buoi cuoi thang
        /// </summary>
        private IEnumerable<DateTime> InputOpenTalkDatesForSalary => this.InputOpenTalkDates != null ? this.InputOpenTalkDates.OrderByDescending(s => s).Take(this.InputPayroll.OpenTalk) : null;
        private float OpentalkDateValueForSalary => InputOpenTalkDatesForSalary != null ? this.InputOpenTalkDatesForSalary.Count() * 0.5f : 0;
        /// <summary>
        /// OffDates and No Timesheet Dates
        /// </summary>
        public List<OffDateDto> OffAndNoTimesheetDates { get; set; }

        public List<OffDateDto> InputOffDates { get; set; }
        public List<OffDateDto> InputOffDateLastMonth { get; set; }
        private DateTime LastDateOfPayroll => DateTimeUtils.GetLastDayOfMonth(this.InputPayroll.ApplyMonth);
        private DateTime FirstDateOfPayroll => DateTimeUtils.GetFirstDayOfMonth(this.InputPayroll.ApplyMonth);
        public HashSet<DateTime> InputWorkAtHomeOnlyDates { get; set; }

        private DateTime InputWorkingEndDate
        {
            get
            {
                if (this.InputEmployee.Status != EmployeeStatus.Working)
                {
                    return this.InputEmployee.DateAt;
                }

                return this.LastDateOfPayroll.AddDays(1);
            }
        }
        private SalaryInputForPayslipDto FarthestSalary => this.InputSalaries.LastOrDefault();
        private SalaryInputForPayslipDto CurrentSalary => this.InputSalaries.FirstOrDefault();
        private DateTime WorkingStartDate
        {
            get
            {
                return FarthestSalary.Date > this.FirstDateOfPayroll ? FarthestSalary.Date : this.FirstDateOfPayroll;
            }
        }

        private DateTime WorkingStartDateHasLeaveDay
        {
            get
            {
                DateTime result = default;
                foreach (var item in InputSalaries)
                {

                    if (item.UserType == UserType.Staff)
                    {
                        result = item.Date;
                    }
                    else
                    {
                        break;
                    }
                }

                if (result != default)
                {
                    result = DateTimeUtils.Max(result, this.FirstDateOfPayroll).Date;
                }

                return result;


            }
        }

        public List<CollectBenefitForPayslipDetailDto> InputBenefits { get; set; }
        public List<InputPayslipDetailDto> InputPunishments { get; set; }
        public List<InputPayslipDetailDto> InputBonuses { get; set; }
        public List<InputPayslipDetailDto> InputDebts { get; set; }
        public List<InputPayslipDetailDto> InputRefunds { get; set; }

        // Output ---------------------------------------------------

        public PayslipDateDto OutputPayslipDate { get; set; }
        public OutputMaternityLeaveSalaryDto OutputMaternityLeave { get; set; }
        public float AddedLeaveDay { get; set; }
        public float RefundLeaveDay { get; set; }
        public float CountDayHasBenefitRemote { get; set; }
        public float RemainLeaveDayAfter => this.InputRemainLeaveDayBefore + this.AddedLeaveDay - this.RefundLeaveDay;
        private float TotalOTHour => InputOTs != null ? this.InputOTs.Sum(s => s.OTHour) : 0;

        public List<OutputPayslipDetail> OutputAllPayslipDetails { get; set; } = new List<OutputPayslipDetail>();

        public float TotalOffDay => OffAndNoTimesheetDates != null ? OffAndNoTimesheetDates.Sum(x => x.DayValue) : 0;

        public OutputPayslipDto OutputPayslipDto { get; set; } = new OutputPayslipDto();


        public void CalculateSalary()
        {
            AddNoTimesheetDatesToInputWorkAtHomeOnlyDates();
            CaculateOffAndNoTimesheetDates();
            this.CalculateNormalAndOTSalary();
            this.CalculateNghiSinhSalary();
            this.CalculateBenefits();
            this.CalculateBonuses();
            this.CalculateDebts();
            this.CalculatePunishments();
            this.CalculateRefunds();
            this.CaculatePayslipDto();

        }

        private void AddNoTimesheetDatesToInputWorkAtHomeOnlyDates()
        {
            var firstDate = this.FirstDateOfPayroll;
            var firstDateNextMonth = firstDate.AddMonths(1);
            if (InputWorkAtHomeOnlyDates == null)
            {
                InputWorkAtHomeOnlyDates = new HashSet<DateTime>();
            }
            for (var date = firstDate; date < firstDateNextMonth; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }
                if (InputSettingOffDates != null && InputSettingOffDates.Contains(date))
                {
                    continue;
                }

                if (InputNormalWorkingDates != null && InputNormalWorkingDates.Contains(date))
                {
                    continue;
                }

                if (!InputWorkAtHomeOnlyDates.Contains(date))
                {
                    InputWorkAtHomeOnlyDates.Add(date);
                }
            }
        }

        private void CaculateOffAndNoTimesheetDates()
        {
            this.OffAndNoTimesheetDates = new List<OffDateDto>();
            if (this.InputOffDates != null)
            {
                OffAndNoTimesheetDates.AddRange(this.InputOffDates);
            }
            var firstDate = this.FirstDateOfPayroll;
            var firstDateNextMonth = firstDate.AddMonths(1);

            for (var date = firstDate; date < firstDateNextMonth; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }
                if (InputSettingOffDates.Contains(date))
                {
                    continue;
                }

                if (InputNormalWorkingDates != null && InputNormalWorkingDates.Contains(date))
                {
                    continue;
                }

                var dto = InputOffDates == null ? default : InputOffDates.FirstOrDefault(s => s.DateAt == date);
                if (dto == default)
                {
                    OffAndNoTimesheetDates.Add(new OffDateDto
                    {
                        DateAt = date,
                        DayValue = 1,
                        DayOffTypeId = 0,
                        LeaveDay = 0
                    });
                }
                else
                {
                    dto.DayValue = 1;
                }
            }


        }

        private void CalculatePunishments()
        {
            this.InputPunishments?.ForEach(item =>
            {
                this.OutputAllPayslipDetails.Add(
                    new OutputPayslipDetail
                    {
                        Money = -item.Money,
                        ReferenceId = item.ReferenceId,
                        IsProjectCost = false,
                        Type = PayslipDetailType.Punishment,
                        Note = item.Note
                    }
                );
            });

        }

        private void CalculateRefunds()
        {
            this.InputRefunds?.ForEach(item =>
            {
                this.OutputAllPayslipDetails.Add(
                    new OutputPayslipDetail
                    {
                        Money = -item.Money,
                        ReferenceId = item.ReferenceId,
                        IsProjectCost = false,
                        Type = PayslipDetailType.Refund,
                        Note = item.Note
                    }
                );
            });

        }

        private void CalculateBonuses()
        {
            this.InputBonuses?.ForEach(item =>
            {
                this.OutputAllPayslipDetails.Add(
                    new OutputPayslipDetail
                    {
                        Money = item.Money,
                        ReferenceId = item.ReferenceId,
                        IsProjectCost = true,
                        Type = PayslipDetailType.Bonus,
                        Note = item.Note
                    }
                );
            });

        }


        private void CalculateDebts()
        {
            this.InputDebts?.ForEach(item =>
            {
                this.OutputAllPayslipDetails.Add(
                    new OutputPayslipDetail
                    {
                        Money = -item.Money,
                        ReferenceId = item.ReferenceId,
                        IsProjectCost = false,
                        Type = PayslipDetailType.Debt,
                        Note = item.Note
                    }
                );
            });

        }

        private void CaculatePayslipDto()
        {
            this.OutputPayslipDto.PayrollId = this.InputPayroll.Id;
            this.OutputPayslipDto.OpentalkCount = this.InputOpenTalkDates != default ? InputOpenTalkDates.Count : 0;
            this.OutputPayslipDto.EmployeeId = this.InputEmployee.EmployeeId;
            this.OutputPayslipDto.LevelId = this.InputEmployee.LevelId;
            this.OutputPayslipDto.UserType = this.InputEmployee.UserType;
            this.OutputPayslipDto.JobPositionId = this.InputEmployee.JobPositionId;
            this.OutputPayslipDto.BranchId = this.InputEmployee.BranchId;
            this.OutputPayslipDto.OffDay = this.TotalOffDay;
            this.OutputPayslipDto.NormalDay = this.OutputPayslipDate.TotalWorkingDay - this.OpentalkDateValueForSalary;
            this.OutputPayslipDto.OTHour = this.TotalOTHour;
            this.OutputPayslipDto.RefundLeaveDay = this.RefundLeaveDay;
            this.OutputPayslipDto.RemainLeaveDayAfter = this.RemainLeaveDayAfter;
            this.OutputPayslipDto.AddedLeaveDay = this.AddedLeaveDay;
            this.OutputPayslipDto.RemainLeaveDayBefore = this.InputEmployee.RemainLeaveDay;
            this.OutputPayslipDto.WorkAtOfficeOrOnsiteDay = this.CountDayHasBenefitRemote;
            this.OutputPayslipDto.Salary = this.OutputAllPayslipDetails.Sum(s => s.Money);
            this.OutputPayslipDto.BankId = InputEmployee.BankId;
            this.OutputPayslipDto.BankAccountNumber = InputEmployee.BankAccountNumber;
            this.OutputPayslipDto.BankName = InputEmployee.BankName;
        }



        private int CalculateMonthlyLeaveDayToAdd()
        {
            //check xem co phai staff ko

            if (CurrentSalary.UserType != HRMEnum.UserType.Staff)
            {
                return 0;
            }

            var workingStartDateHasLeaveDay = this.WorkingStartDateHasLeaveDay;
            if (workingStartDateHasLeaveDay == default)
            {
                return 0;
            }

            if (workingStartDateHasLeaveDay.Day >= DAY_OF_MONTH_TO_ADD_LEAVE_DAY)
            {
                return 0;
            }

            if (this.OutputPayslipDate.TotalWorkingDay < this.InputPayroll.StandardDay * WORKING_DAY_PERCENTAGE_TO_ADD_LEAVE_DAY)
            {
                return 0;
            }

            return 1;

        }

        private int CalculateLeaveDayToAddWhenBecomeStaff()
        {

            if (this.InputSalaries.Count <= 1)
            {
                return 0;
            }

            var staffSalary = InputSalaries.Where(x => x.UserType == UserType.Staff)
                .Where(x => DateTimeUtils.IsTheSameYearMonth(this.InputPayroll.ApplyMonth, x.Date))
                .FirstOrDefault();

            if (staffSalary == default)
            {
                return 0;
            }

            var probationStaffSalary = this.InputSalaries.Where(x => x.UserType == UserType.ProbationaryStaff)
                .FirstOrDefault();
            if (probationStaffSalary == default)
            {
                return 0;
            }

            int monthDiff = DateTimeUtils.CountMonthBetweenTwoDate(probationStaffSalary.Date, staffSalary.Date);

            return Math.Max(monthDiff, 0);
        }

        private int CalculateLeaveDayForNghiCoPhep()
        {
            if (this.InputOffDates == null || this.InputOffDates.Count == 0)
            {
                return 0;
            }
            var dicDateOffTypeIdToLeaveDayThisMonth = this.InputOffDates.Where(s => s.LeaveDay > 0).GroupBy(s => s.DayOffTypeId).ToDictionary(s => s.Key, s => s.Select(x => x.LeaveDay).FirstOrDefault());

            if (dicDateOffTypeIdToLeaveDayThisMonth.Count == 0)
            {
                return 0;
            }

            var dateOffTypeIdsLastMonth = this.InputOffDateLastMonth?.Select(s => s.DayOffTypeId).Distinct() ?? null;
            int result = 0;

            foreach (var item in dicDateOffTypeIdToLeaveDayThisMonth)
            {
                if (dateOffTypeIdsLastMonth != null && dateOffTypeIdsLastMonth.Contains(item.Key))
                {
                    continue;
                }
                result += item.Value;
            }
            return result;
        }

        private void CalculateAddedLeaveDay()
        {
            if (CurrentSalary.SalaryType == SalaryRequestType.MaternityLeave && CurrentSalary.Date <= FirstDateOfPayroll)
            {
                AddedLeaveDay = 0;
                return;
            }
            int MonthlyLeaveDayToAdd = this.CalculateMonthlyLeaveDayToAdd();
            int LeaveDayToAddWhenBecomeStaff = this.CalculateLeaveDayToAddWhenBecomeStaff();
            int LeaveDayForNghiCoPhep = this.CalculateLeaveDayForNghiCoPhep();

            this.AddedLeaveDay = MonthlyLeaveDayToAdd + LeaveDayForNghiCoPhep + LeaveDayToAddWhenBecomeStaff;
        }

        private void CalculateNghiSinhSalary()
        {
            var inputSalaryObj = InputSalaries.Where(x => x.SalaryType == SalaryRequestType.MaternityLeave).FirstOrDefault();
            if (inputSalaryObj == default)
            {
                return;
            }

            var startDate = inputSalaryObj.Date > this.FirstDateOfPayroll ? inputSalaryObj.Date : this.FirstDateOfPayroll;
            var endDate = inputSalaryObj.Date.AddMonths(6).AddDays(-1);
            endDate = endDate < this.LastDateOfPayroll ? endDate : this.LastDateOfPayroll;
            var workingDay = DateTimeUtils.CaculateStandardWorkingDay(startDate, endDate, this.InputSettingOffDates);

            var salary = workingDay * inputSalaryObj.Salary / this.InputPayroll.NormalWorkingDay;
            var note = $"Lương nghỉ sinh";
            if (startDate > FirstDateOfPayroll || endDate < LastDateOfPayroll)
            {
                note = $"Lương nghỉ sinh, hưởng từ {DateTimeUtils.ToString(startDate)} - {DateTimeUtils.ToString(endDate)}, số ngày hưởng: {workingDay}/{this.InputPayroll.NormalWorkingDay}";
            }

            this.OutputAllPayslipDetails.Add(new OutputPayslipDetail
            {
                IsProjectCost = true,
                Money = salary,
                Type = PayslipDetailType.SalaryMaternityLeave,
                Note = note
            });

        }

        private void CalculateNormalAndOTSalary()
        {
            var firstDayOfPayroll = this.FirstDateOfPayroll;
            var lastDayOfPayroll = this.LastDateOfPayroll;

            this.OutputPayslipDate = new PayslipDateDto
            {
                Details = new List<PayslipDateDetailDto>()
            };

            var standardDay = this.InputPayroll.StandardDay;
            if (standardDay <= 0)
            {
                throw new UserFriendlyException($"StandardWorkingDay <= 0 {standardDay}");
            }

            CalculateOutputPlayslipDate(firstDayOfPayroll, lastDayOfPayroll);
            CalculateAddedLeaveDay();
            CalculateRefundLeaveDay();

            this.OutputAllPayslipDetails.Add(new OutputPayslipDetail
            {
                IsProjectCost = true,
                Money = this.OutputPayslipDate.RoundNormalSalary,
                Type = PayslipDetailType.SalaryNormal,
                Note = "Lương Normal (bao gồm opentalk, bù phép)",
            });

            this.OutputAllPayslipDetails.Add(new OutputPayslipDetail
            {
                IsProjectCost = true,
                Money = this.OutputPayslipDate.RoundOTSalary,
                Type = PayslipDetailType.SalaryOT,
                Note = "Lương OT",
            });

        }

        private void CalculateOutputPlayslipDate(DateTime firstDayOfPayroll, DateTime lastDayOfPayroll)
        {
            var date = lastDayOfPayroll.Date;
            while (date >= firstDayOfPayroll)
            {
                var inputOT = this.InputOTs?.FirstOrDefault(s => s.Date == date);
                var dateSalary = GetDateSalary(date);
                var isNormalWorkingDate = !IsNotWorkingDate(date);
                var offDateValue = isNormalWorkingDate ? this.GetOffDateValue(date) : 1;
                PayslipDateDetailDto item = new PayslipDateDetailDto
                {
                    Date = date,
                    DateSalary = dateSalary,
                    OffDateValue = offDateValue,
                    IsNormalWorkingDate = isNormalWorkingDate,
                    OTHour = inputOT?.OTHour ?? 0,
                };

                this.OutputPayslipDate.Details.Add(item);

                date = date.AddDays(-1);
            }
        }

        private void CalculateRefundLeaveDay()
        {
            this.RefundLeaveDay = 0;
            var availableLeaveDayToRefund = this.InputRemainLeaveDayBefore + this.AddedLeaveDay;
            float totalDateValueForCaculator = 0;
            var availableOpentalkToRefund = this.InputPayroll.OpentalkDayValue - this.OpentalkDateValueForSalary;

            foreach (var item in this.OutputPayslipDate.Details)
            {
                var date = item.Date;

                totalDateValueForCaculator += 1 - item.OffDateValue;

                if (CanRefundLeaveDay(item.OffDateValue, item.Date, item.IsNormalWorkingDate, totalDateValueForCaculator))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday)
                    {
                        item.RefundOffDateValue = Math.Min(item.OffDateValue, availableLeaveDayToRefund);
                    }
                    else if (item.OffDateValue == 1 && availableOpentalkToRefund > 0)
                    {
                        item.RefundOffDateValue = Math.Min(0.5f, availableLeaveDayToRefund);
                        availableOpentalkToRefund -= item.RefundOffDateValue;
                    }

                    this.RefundLeaveDay += item.RefundOffDateValue;
                    availableLeaveDayToRefund -= item.RefundOffDateValue;
                    totalDateValueForCaculator += item.RefundOffDateValue;
                }

            }

        }

        private bool CanRefundLeaveDay(float offDateValue, DateTime date, bool isNormalWorkingDate, float totalDateValueForCaculate)
        {
            return offDateValue > 0
                && date.DayOfWeek != DayOfWeek.Sunday
                && !InputSettingOffDates.Contains(date)
                && isNormalWorkingDate
                && totalDateValueForCaculate < this.InputPayroll.StandardDay;
        }

        private void CalculateBenefits()
        {
            if (this.InputBenefits == null || this.InputBenefits.Count == 0)
            {
                return;
            }

            foreach (var item in this.InputBenefits)
            {
                OutputPayslipDetail outputObj = null;
                switch (item.BenefitType)
                {
                    case BenefitType.CheDoChung:
                        outputObj = CalculateBenefitCheDoChung(item);
                        break;

                    case BenefitType.CheDoRemote:
                        outputObj = CalculateBenefitCheDoRemote(item);
                        break;

                    case BenefitType.CheDoRieng:
                        outputObj = CalculateBenefitCheDoRieng(item);
                        break;
                }
                if (outputObj != default && !outputObj.IsExpired)
                {
                    this.OutputAllPayslipDetails.Add(outputObj);
                }
            }
        }

        private OutputPayslipDetail CalculateBenefitCheDoChung(CollectBenefitForPayslipDetailDto input)
        {
            var workingDay = this.OutputPayslipDate.TotalWorkingDay;
            var money = input.Money * workingDay / this.InputPayroll.StandardDay;
            return new OutputPayslipDetail
            {
                Type = PayslipDetailType.Benefit,
                Money = CommonUtil.RoundMoneyVND(money),
                Note = input.Note,
                IsProjectCost = true
            };
        }

        private OutputPayslipDetail CalculateBenefitCheDoRieng(CollectBenefitForPayslipDetailDto input)
        {
            var startDate = GetStartDateBenefit(input);

            var endDate = GetEndDateBenefit(input);

            var isExpire = endDate < this.FirstDateOfPayroll || startDate > this.LastDateOfPayroll;

            var workingDay = DateTimeUtils.CaculateStandardWorkingDay(startDate, endDate, this.InputSettingOffDates);

            var note = $"{input.Note}, số ngày hưởng: {workingDay}/{this.InputPayroll.NormalWorkingDay}";
            if (startDate > FirstDateOfPayroll || endDate < LastDateOfPayroll)
            {
                note = $"{input.Note}, hưởng từ {DateTimeUtils.ToString(startDate)} - {DateTimeUtils.ToString(endDate)}, số ngày hưởng: {workingDay}/{this.InputPayroll.NormalWorkingDay}";
            }

            var money = input.Money * workingDay / this.InputPayroll.NormalWorkingDay;
            return new OutputPayslipDetail
            {
                Type = PayslipDetailType.Benefit,
                Money = CommonUtil.RoundMoneyVND(money),
                Note = note,
                IsProjectCost = true,
                IsExpired = isExpire
            };
        }

        private OutputPayslipDetail CalculateBenefitCheDoRemote(CollectBenefitForPayslipDetailDto input)
        {
            var startDate = GetStartDateBenefit(input);

            var endDate = GetEndDateBenefit(input);

            var isExpire = endDate < this.FirstDateOfPayroll || startDate > this.LastDateOfPayroll;
            var workAtHomeDates = InputWorkAtHomeOnlyDates ?? new HashSet<DateTime>();
            var notHasBenefitRemoteDates = new HashSet<DateTime>(InputSettingOffDates.Concat(workAtHomeDates));
            var dayHasBenefit = DateTimeUtils.CaculateStandardWorkingDay(startDate, endDate, notHasBenefitRemoteDates);

            this.CountDayHasBenefitRemote = dayHasBenefit;

            var note = $"{input.Note}, số ngày hưởng: {dayHasBenefit}/{this.InputPayroll.NormalWorkingDay}";
            if (startDate > FirstDateOfPayroll || endDate < LastDateOfPayroll)
            {
                note = $"{input.Note}, hưởng từ {DateTimeUtils.ToString(startDate)} - {DateTimeUtils.ToString(endDate)}, số ngày hưởng: {dayHasBenefit}/{this.InputPayroll.NormalWorkingDay}";
            }

            var money = input.Money * dayHasBenefit / this.InputPayroll.NormalWorkingDay;
            return new OutputPayslipDetail
            {
                Type = PayslipDetailType.Benefit,
                Money = CommonUtil.RoundMoneyVND(money),
                Note = note,
                IsProjectCost = true,
                IsExpired = isExpire
            };
        }

        private DateTime GetStartDateBenefit(CollectBenefitForPayslipDetailDto input)
        {
            return DateTimeUtils.Max(this.FirstDateOfPayroll, input.StartDate).Date;
        }

        private DateTime GetEndDateBenefit(CollectBenefitForPayslipDetailDto input)
        {
            return input.EndDate.HasValue ? DateTimeUtils.Min(this.LastDateOfPayroll, input.EndDate.Value).Date
                                                : this.LastDateOfPayroll;
        }


        private float GetOffDateValue(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return 1;
            }

            if (this.InputSettingOffDates != null && this.InputSettingOffDates.Contains(date))
            {
                return 1;
            }

            if (date >= this.InputWorkingEndDate)
            {
                return 1;
            }

            if (date < this.WorkingStartDate)
            {
                return 1;
            }

            //if (this.InputNormalWorkingDates == null || !InputNormalWorkingDates.Contains(date))
            //{
            //    return 1;
            //}


            //opentalk
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                if (InputOpenTalkDates == null || this.InputOpenTalkDates.Count == 0)
                {
                    return 1;
                }
                return this.InputOpenTalkDatesForSalary.Contains(date) ? 0.5f : 1;
            }


            //request off
            var offDate = this.OffAndNoTimesheetDates?.FirstOrDefault(s => s.DateAt == date);
            var offDateValue = offDate != default ? offDate.DayValue : 0;
            return Math.Min(offDateValue, 1);

        }


        /// <summary>
        /// return 0 - ko chấm công
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private double GetDateSalary(DateTime date)
        {

            if ((InputEmployee.Status != EmployeeStatus.Working)
            && date >= InputEmployee.DateAt)
            {
                return 0;
            }

            var salary = this.InputSalaries.Where(s => s.SalaryType != SalaryRequestType.MaternityLeave)
                   .Where(s => s.Date <= date).OrderByDescending(s => s.Date)
                   .FirstOrDefault();

            if (salary == default)
            {
                return 0;
            }
            return salary.Salary / this.InputPayroll.StandardDay;
        }

        /// <summary>
        /// return 0 - ko chấm công
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool IsNotWorkingDate(DateTime date)
        {

            if ((InputEmployee.Status != EmployeeStatus.Working)
            && date >= InputEmployee.DateAt)
            {
                return true;
            }

            var salary = this.InputSalaries.Where(s => s.SalaryType != SalaryRequestType.MaternityLeave)
                   .Where(s => s.Date <= date).OrderByDescending(s => s.Date)
                   .FirstOrDefault();

            if (salary == default)
            {
                return true;
            }
            return false;
        }
    }
}
