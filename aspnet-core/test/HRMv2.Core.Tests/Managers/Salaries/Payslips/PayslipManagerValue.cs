using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.WebServices.Timesheet.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Core.Tests.Managers.Salaries.Payslips
{
    public class PayslipManagerValue
    {
        public InputCollectDataForPayslipDto inputCollectDataForPayslipDto = new InputCollectDataForPayslipDto
        {
            UpperEmails = new List<string> { "quang.levan@ncc.asia" },
            Year = 2022,
            Month = 12
        };

        public List<TimesheetOTDto> valueReturnForOTTTimesheetsApi = new List<TimesheetOTDto> {
                                        new TimesheetOTDto()
                                        {
                                            NormalizedEmailAddress= "quang.levan@ncc.asia",
                                            ListOverTimeHour= new List<DateOTHourDto>
                                            {
                                                new DateOTHourDto()
                                                {
                                                    Date= new DateTime(2022, 12, 1),
                                                    OTHour= 12
                                                },
                                                new DateOTHourDto()
                                                {
                                                    Date= new DateTime(2022, 12, 2),
                                                    OTHour= 11
                                                }
                                            }
                                        },
                                        new TimesheetOTDto()
                                        {
                                            NormalizedEmailAddress= "tung.tranvan@ncc.asia",
                                            ListOverTimeHour= new List<DateOTHourDto>
                                            {
                                                new DateOTHourDto()
                                                {
                                                    Date= new DateTime(2022, 11, 1),
                                                    OTHour= 6
                                                }
                                            }
                                        }
                                    };

        public List<ChamCongInfoDto> valueReturnForChamCongInfoApi = new List<ChamCongInfoDto> {
                                        new ChamCongInfoDto()
                                        {
                                            NormalizeEmailAddress= "quang.levan@ncc.asia",
                                            OpenTalkDates= new List<DateTime>
                                            {
                                                new DateTime(2022, 12, 1),
                                                new DateTime(2022, 12, 2)
                                            },
                                            NormalWorkingDates= new List<DateTime>
                                            {
                                                new DateTime(2022, 12, 1),
                                                new DateTime(2022, 12, 2)
                                            }
                                        },
                                        new ChamCongInfoDto()
                                        {
                                            NormalizeEmailAddress= "quang.levan@ncc.asia",
                                            OpenTalkDates= new List<DateTime>
                                            {
                                                new DateTime(2022, 12, 3)
                                            },
                                            NormalWorkingDates= new List<DateTime>
                                            {
                                                new DateTime(2022, 12, 1)
                                            }
                                        }
                                    };
        public List<GetRequestDateDto> valueReturnForGetAllRequestDaysApi = new List<GetRequestDateDto> {
                                        new GetRequestDateDto()
                                        {
                                            NormalizedEmailAddress= "quang.levan@ncc.asia",
                                            WorkAtHomeOnlyDates= new HashSet<DateTime>
                                            {
                                                new DateTime(2022, 12, 1),
                                                new DateTime(2022, 12, 2)
                                            },
                                            OffDates= new List<OffDateDto>
                                            {
                                                new OffDateDto()
                                                {
                                                    DateAt= new DateTime(2022, 12, 12),
                                                    DayValue= 2,
                                                    DayOffTypeId= 1,
                                                    LeaveDay=3
                                                }
                                            },
                                            OffDateLastMonth= new List<OffDateDto>
                                            {
                                                new OffDateDto()
                                                {
                                                    DateAt= new DateTime(2022, 12, 12),
                                                    DayValue= 9,
                                                    DayOffTypeId= 9,
                                                    LeaveDay=9
                                                }
                                            }
                                        }
                                    };
    }
}
