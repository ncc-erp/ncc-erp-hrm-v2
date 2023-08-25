using Abp.UI;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using NccCore.Uitls;
using Newtonsoft.Json;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Application.Tests.APIs.CalculateSalary
{
    public class CalculateSalary_Tests : HRMv2ApplicationTestBase
    {

        // tính lương tháng 8
        // 1 mức lương (5.000.000) apply từ 1/8
        // ngày phép trước khi tính:  1
        // off 3 ngày 1/8 (full), 16/8(full), 9/8(full)
        // opentalk 3 buổi
        // OT 5 giờ
        [Fact]
        public async Task TestCase1()
        {
            
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 1,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 23),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };
            

            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                new DateOTHourDto
                {
                    Date = new DateTime(2022,8,9),
                    OTHour = 3
                },
                 new DateOTHourDto
                {
                    Date = new DateTime(2022,8,10),
                    OTHour = 1
                },
                   new DateOTHourDto
                {
                    Date = new DateTime(2022,8,30),
                    OTHour = 1
                },

            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                }
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoChung,
                //    Money = 500000,
                //},

                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoRieng,
                //    Money = 400000,
                //    StartDate = new DateTime(2022, 6,1),
                //    EndDate = new DateTime(2022, 8, 30)
                //}
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022,8)
            };

            nccCaculator.CalculateSalary();



            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 5;
            var expectRefunleaveDay = 2;
            var expectMonthlyAddedLeaveDay = 1;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 4791667;
            var expectOtSalary = 130208;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, outputBenefitMoney);
            Assert.Equal(expectTotalBonus, outputBonusMoney);
            Assert.Equal(expectTotalPunishment, outputPunishment);
            Assert.Equal(expectTotalDebt, outputDebt);
            Assert.Equal(expectTotalRealSalary, nccCaculator.OutputPayslipDto.Salary);
        }

        // tương tự testcase 1 nhưng có 2 mức lương
        // 5m(1/8/2022), 10m(10/8/2022)
        [Fact]
        public async Task TestCase2()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 1,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 23),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                new DateOTHourDto
                {
                    Date = new DateTime(2022,8,9),
                    OTHour = 3
                },
                 new DateOTHourDto
                {
                    Date = new DateTime(2022,8,10),
                    OTHour = 1
                },
                   new DateOTHourDto
                {
                    Date = new DateTime(2022,8,30),
                    OTHour = 1
                },

            };
            
            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoChung,
                //    Money = 500000,
                //},

                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoRieng,
                //    Money = 400000,
                //    StartDate = new DateTime(2022, 6,1),
                //    EndDate = new DateTime(2022, 8, 30)
                //}
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();



            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 5;
            var expectRefunleaveDay = 2;
            var expectMonthlyAddedLeaveDay = 1;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 8333333;
            var expectOtSalary = 182292;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, outputBenefitMoney);
            Assert.Equal(expectTotalBonus, outputBonusMoney);
            Assert.Equal(expectTotalPunishment, outputPunishment);
            Assert.Equal(expectTotalDebt, outputDebt);
            Assert.Equal(expectTotalRealSalary, nccCaculator.OutputPayslipDto.Salary);
        }

        // test bù phép thử việc -> staff
        // 2 mức lương 5m (Thử việc, 1/6/2022), 10m (Staff, 1/8/2022)
        [Fact]
        public async Task TestCase3()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 2,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 23),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                new DateOTHourDto
                {
                    Date = new DateTime(2022,8,9),
                    OTHour = 3
                },
                 new DateOTHourDto
                {
                    Date = new DateTime(2022,8,10),
                    OTHour = 1
                },
                   new DateOTHourDto
                {
                    Date = new DateTime(2022,8,30),
                    OTHour = 1
                },

            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,5,1),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.ProbationaryStaff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();
            

            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 5;
            var expectRefunleaveDay = 3;
            var expectMonthlyAddedLeaveDay = 4;
            var ExpectLeaveDayAfter = 3;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 10000000;
            var expectOtSalary = 260417;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }



        // test lương nghỉ sinh tháng 8
        // nghỉ sinh từ 1/7/2022 (5m)
        [Fact]
        public async Task TestCase4()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.MaternityLeave,
                RemainLeaveDay = 2,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
               
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                

            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,3,1),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.MaternityLeave,
                    UserType = UserType.Staff
                },
                
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
               
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
               
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
               
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 0;
            var expectopenTalkCount = 0;
            var expectOffDay = 0;
            var expectOtHour = 0;
            var expectRefunleaveDay = 0;
            var expectMonthlyAddedLeaveDay = 0;
            var ExpectLeaveDayAfter = 2;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 0;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 10000000;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = 10000000;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        // test lương employee off giữa tháng
        // Quit ngày 21/8/2022
        // 1 mức lương 10m (1/8/2022)
        [Fact]
        public async Task TestCase5()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 21),
                Status = EmployeeStatus.Quit,
                RemainLeaveDay = 1,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
         
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>();
            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>();
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>();
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>();

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();



            var expectNormalWorkingDay = 13;
            var expectopenTalkCount = 2;
            var expectOffDay = 2;
            var expectOtHour = 0;
            var expectRefunleaveDay = 1;
            var expectMonthlyAddedLeaveDay = 0;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 6250000;
            var expectOtSalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, outputBenefitMoney);
            Assert.Equal(expectTotalBonus, outputBonusMoney);
            Assert.Equal(expectTotalPunishment, outputPunishment);
            Assert.Equal(expectTotalDebt, outputDebt);
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        //test join công ty từ giữa tháng (10/8/2022 - staff)
        // 1 mức lương 5m(10/8/2022)
        [Fact]
        public async Task TestCase6()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 10),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };

            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                }

            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 15;
            var expectopenTalkCount = 2;
            var expectOffDay = 1;
            var expectOtHour = 0;
            var expectRefunleaveDay = 1;
            var expectMonthlyAddedLeaveDay = 1;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 3541667;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        //test quay lại làm việc sau thời gian nghỉ sinh
        // 2 mức lương 5m(nghisinh - 10/2/2022), 10m(working - 10/8/2022)
        [Fact]
        public async Task TestCase7()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 10),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };

            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                },
                  new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,2,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.MaternityLeave,
                    UserType = UserType.Staff
                }
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 15;
            var expectopenTalkCount = 2;
            var expectOffDay = 1;
            var expectOtHour = 0;
            var expectRefunleaveDay = 1;
            var expectMonthlyAddedLeaveDay = 1;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 7083333;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 1521739;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = expectNormalSalary + expectMaternityLeavesalary;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }



        /// <summary>
        /// Join cty từ giữa tháng (10/08/2022 - staff)
        /// 1 mức lương 5M
        /// NormalTimesheetDate từ 10/08, ko log ngày 17/8
        /// off ngày 16/8
        /// 0 ngày phép
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestCase8()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 10),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };

            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                }

            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8, new List<DateTime> {
                    new DateTime(2022,8,1),
                    new DateTime(2022,8,2),
                    new DateTime(2022,8,3),
                    new DateTime(2022,8,4),
                    new DateTime(2022,8,5),
                    new DateTime(2022,8,6),
                    new DateTime(2022,8,7),
                    new DateTime(2022,8,8),
                    new DateTime(2022,8,9),                    
                    new DateTime(2022,8,17)})
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 14;//check lai
            var expectopenTalkCount = 2;
            var expectOffDay = 9;
            var expectOtHour = 0;
            var expectRefunleaveDay = 0;
            var expectMonthlyAddedLeaveDay = 0;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 3125000;// 3541667;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        /// <summary>
        /// Join cty từ giữa tháng (10/08/2022 - staff)
        /// 1 mức lương 5M
        /// NormalTimesheetDate từ 10/08, ko log ngày 17/8
        /// off ngày 16/8
        /// 0 ngày phép
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestCase9()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 10),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>() { new DateTime(2022, 8, 16) };
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };

            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                }

            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
                new CollectBenefitForPayslipDetailDto
                {
                    BenefitType = BenefitType.CheDoRemote,
                    EmployeeId = 1,
                    StartDate = inputEmployee.DateAt,
                    EndDate =  new DateTime(2022,8,31),
                    Money = 1000000,
                    ReferenceId = 1,
                    Note = "Ho tro gui xe oto"
                }
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8, new List<DateTime> {
                    new DateTime(2022,8,1),
                    new DateTime(2022,8,2),
                    new DateTime(2022,8,3),
                    new DateTime(2022,8,4),
                    new DateTime(2022,8,5),
                    new DateTime(2022,8,6),
                    new DateTime(2022,8,7),
                    new DateTime(2022,8,8),
                    new DateTime(2022,8,9),
                    new DateTime(2022,8,17)})
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 14;//check lai
            var expectopenTalkCount = 2;
            var expectOffDay = 9;
            var expectOtHour = 0;
            var expectRefunleaveDay = 0;
            var expectMonthlyAddedLeaveDay = 0;
            var ExpectLeaveDayAfter = 0;
            var expectCountDayHasBenefitRemote = 14;

            var expectNormalSalary = 3125000;// 3541667;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = Math.Round( expectCountDayHasBenefitRemote * 1000000 / inputPayroll.NormalWorkingDay);
            var expectTotalBonus = 0;
            var expectTotalPunishment = 0;
            var expectTotalDebt = 0;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectCountDayHasBenefitRemote, nccCaculator.CountDayHasBenefitRemote);

            var atualTotalBenefit = Math.Round( nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money));
            Assert.Equal(expectTotalBenefit, Math.Round(atualTotalBenefit));

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectCountDayHasBenefitRemote, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        /// <summary>
        /// Test cộng ngày phép và bù phép
        /// Join 24/6 t.viec
        /// lên staff 24/8
        /// off 2 buổi vào múc lương t.việc, 1 buổi mức lương staff
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestCase10()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 24),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,24),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,6,24),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.ProbationaryStaff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 0;
            var expectRefunleaveDay = 2;
            var expectMonthlyAddedLeaveDay = 2;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 6145833;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        [Fact]
        public async Task TestCase11()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 24),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,6,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.ProbationaryStaff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 0;
            var expectRefunleaveDay = 3;
            var expectMonthlyAddedLeaveDay = 3;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 8541667;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        /// <summary>
        /// test collect bonus, punishment, debt, refund
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestCase12()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 0,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 24),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,6,10),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.ProbationaryStaff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                },
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            List<InputPayslipDetailDto> inputRefund = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 3000000,
                },
                new InputPayslipDetailDto
                {
                    Money = 2000000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputRefunds = inputRefund,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();


            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 3;
            var expectOffDay = 3;
            var expectOtHour = 0;
            var expectRefunleaveDay = 3;
            var expectMonthlyAddedLeaveDay = 3;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 8541667;
            var expectOtSalary = 0;
            var expectMaternityLeavesalary = 0;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -150000;
            var expectTotalDebt = -100000;
            var expectTotalRefund = -5000000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt + expectTotalRefund;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            double outputMaternityLeaveSalary = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.SalaryMaternityLeave).Sum(x => x.Money);
            Assert.Equal(expectMaternityLeavesalary, Math.Round(outputMaternityLeaveSalary));
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);
            double outputRefund = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Refund).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, Math.Round(outputBenefitMoney));
            Assert.Equal(expectTotalBonus, Math.Round(outputBonusMoney));
            Assert.Equal(expectTotalPunishment, Math.Round(outputPunishment));
            Assert.Equal(expectTotalDebt, Math.Round(outputDebt));
            Assert.Equal(expectTotalRefund, Math.Round(outputRefund));
            Assert.Equal(expectTotalRealSalary, Math.Round(nccCaculator.OutputPayslipDto.Salary));
        }

        // Throw Exception khi standardDay = 0 
        // NormalWorkingDat = 0
        // Opentalk = 0;
        [Fact]
        public async Task TestCase13()
        {

            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 0,
                OpenTalk = 0,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 1,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 23),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 13),
                new DateTime(2022, 8, 20),
                new DateTime(2022, 8, 27),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                new DateOTHourDto
                {
                    Date = new DateTime(2022,8,9),
                    OTHour = 3
                },
                 new DateOTHourDto
                {
                    Date = new DateTime(2022,8,10),
                    OTHour = 1
                },
                   new DateOTHourDto
                {
                    Date = new DateTime(2022,8,30),
                    OTHour = 1
                },

            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                }
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoChung,
                //    Money = 500000,
                //},

                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoRieng,
                //    Money = 400000,
                //    StartDate = new DateTime(2022, 6,1),
                //    EndDate = new DateTime(2022, 8, 30)
                //}
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                nccCaculator.CalculateSalary();
            });

            var expectedMsg = $"StandardWorkingDay <= 0 {inputPayroll.StandardDay}";
            Assert.Equal(expectedMsg, exception.Message);
        }

        // tính lương tháng 8
        // 2 mức lương 5m(1/8/2022), 10m(10/8/2022)
        // ngày phép trước khi tính:  1
        // off 3 ngày 1/8 (full), 16/8(full), 9/8(full)
        // opentalk 2 buổi - 1 buổi mức lương 5m , 1 buổi mức lương 10m
        // OT 5 giờ

        [Fact]
        public async Task TestCase14()
        {
            var inputPayroll = new PayrollDto
            {
                ApplyMonth = new DateTime(2022, 8, 1),
                NormalWorkingDay = 23,
                OpenTalk = 2,
            };
            var inputEmployee = new EmployeeToCalDto
            {
                DateAt = new DateTime(2022, 8, 1),
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 1,
                UserType = UserType.Staff,
            };
            HashSet<DateTime> inputSettingOffDate = new HashSet<DateTime>
            {
                new DateTime(2022,8,7),
                new DateTime(2022,8,14),
                new DateTime(2022,8,21),
                new DateTime(2022,8,28),
            };
            List<OffDateDto> inputOffDates = new List<OffDateDto>
            {
                new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 9),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                  new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 16),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                },
                    new OffDateDto
                {
                    DateAt = new DateTime(2022, 8, 23),
                    DayValue = 1,
                    LeaveDay = 0,
                    DayOffTypeId = 0
                }
            };

            List<OffDateDto> inputOffDateLastMonth = new List<OffDateDto>();

            HashSet<DateTime> inputWorkAtHomeOnly = new HashSet<DateTime>();
            List<DateTime> inputOpentalkDates = new List<DateTime> {
                new DateTime(2022, 8, 6),
                new DateTime(2022, 8, 20),
                };


            List<DateOTHourDto> inputOTDates = new List<DateOTHourDto>
            {
                new DateOTHourDto
                {
                    Date = new DateTime(2022,8,9),
                    OTHour = 3
                },
                 new DateOTHourDto
                {
                    Date = new DateTime(2022,8,10),
                    OTHour = 1
                },
                   new DateOTHourDto
                {
                    Date = new DateTime(2022,8,30),
                    OTHour = 1
                },

            };

            List<SalaryInputForPayslipDto> inputSalaries = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,10),
                    Salary = 10000000,
                    SalaryType = SalaryRequestType.Change,
                    UserType = UserType.Staff
                },
                new SalaryInputForPayslipDto
                {
                    Date = new DateTime(2022,8,1),
                    Salary = 5000000,
                    SalaryType = SalaryRequestType.Initial,
                    UserType = UserType.Staff
                },
            };

            List<CollectBenefitForPayslipDetailDto> inputBenefit = new List<CollectBenefitForPayslipDetailDto>
            {
                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoChung,
                //    Money = 500000,
                //},

                //new CollectBenefitForPayslipDetailDto
                //{
                //    BenefitType = BenefitType.CheDoRieng,
                //    Money = 400000,
                //    StartDate = new DateTime(2022, 6,1),
                //    EndDate = new DateTime(2022, 8, 30)
                //}
            };

            List<InputPayslipDetailDto> inputBonus = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 1000000,
                }
            };
            List<InputPayslipDetailDto> inputPunishment = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 50000,
                }
            };
            List<InputPayslipDetailDto> inputDebt = new List<InputPayslipDetailDto>
            {
                new InputPayslipDetailDto
                {
                    Money = 100000,
                }
            };

            var nccCaculator = new NCCSalaryCalculator
            {
                InputPayroll = inputPayroll,
                InputEmployee = inputEmployee,
                InputSettingOffDates = inputSettingOffDate,
                InputOffDates = inputOffDates,
                InputWorkAtHomeOnlyDates = inputWorkAtHomeOnly,
                InputOpenTalkDates = inputOpentalkDates,
                InputOTs = inputOTDates,
                InputOffDateLastMonth = inputOffDateLastMonth,
                InputSalaries = inputSalaries,
                InputBenefits = inputBenefit,
                InputBonuses = inputBonus,
                InputPunishments = inputPunishment,
                InputDebts = inputDebt,
                InputNormalWorkingDates = DateTimeUtils.GetListDateInMonth(2022, 8)
            };

            nccCaculator.CalculateSalary();



            var expectNormalWorkingDay = 20;
            var expectopenTalkCount = 2;
            var expectOffDay = 3;
            var expectOtHour = 5;
            var expectRefunleaveDay = 2;
            var expectMonthlyAddedLeaveDay = 1;
            var ExpectLeaveDayAfter = 0;
            var expectWorkAtOfficeOrOnsiteDay = 0;

            var expectNormalSalary = 8229167;
            var expectOtSalary = 182292;
            var expectTotalBenefit = 0;
            var expectTotalBonus = 1000000;
            var expectTotalPunishment = -50000;
            var expectTotalDebt = -100000;
            var expectTotalRealSalary = expectNormalSalary + expectOtSalary + expectTotalBenefit
                + expectTotalBonus + expectTotalPunishment + expectTotalDebt;

            OutputPayslipDto output = null;

            await WithUnitOfWorkAsync(async () =>
            {
                output = nccCaculator.OutputPayslipDto;
            });

            Assert.Equal(expectNormalSalary, Math.Round(nccCaculator.OutputPayslipDate.NormalSalary));
            Assert.Equal(expectOtSalary, Math.Round(nccCaculator.OutputPayslipDate.OTSalary));
            Assert.Equal(expectMonthlyAddedLeaveDay, nccCaculator.OutputPayslipDto.AddedLeaveDay);
            Assert.Equal(ExpectLeaveDayAfter, nccCaculator.OutputPayslipDto.RemainLeaveDayAfter);
            Assert.Equal(expectRefunleaveDay, nccCaculator.OutputPayslipDto.RefundLeaveDay);
            Assert.Equal(expectNormalWorkingDay, nccCaculator.OutputPayslipDto.NormalDay);
            Assert.Equal(expectopenTalkCount, nccCaculator.OutputPayslipDto.OpentalkCount);
            Assert.Equal(expectOffDay, nccCaculator.OutputPayslipDto.OffDay);
            Assert.Equal(expectOtHour, nccCaculator.OutputPayslipDto.OTHour);
            Assert.Equal(expectWorkAtOfficeOrOnsiteDay, nccCaculator.OutputPayslipDto.WorkAtOfficeOrOnsiteDay);

            double outputBenefitMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Benefit).Sum(x => x.Money);
            double outputBonusMoney = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Bonus).Sum(x => x.Money);
            double outputPunishment = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Punishment).Sum(x => x.Money);
            double outputDebt = nccCaculator.OutputAllPayslipDetails.Where(x => x.Type == PayslipDetailType.Debt).Sum(x => x.Money);

            Assert.Equal(expectTotalBenefit, outputBenefitMoney);
            Assert.Equal(expectTotalBonus, outputBonusMoney);
            Assert.Equal(expectTotalPunishment, outputPunishment);
            Assert.Equal(expectTotalDebt, outputDebt);
            Assert.Equal(expectTotalRealSalary, nccCaculator.OutputPayslipDto.Salary);
        }

    }
}
