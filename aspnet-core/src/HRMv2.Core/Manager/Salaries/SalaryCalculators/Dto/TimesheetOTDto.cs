using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.CalculateSalary.Dto
{
    public class TimesheetOTDto
    {
        public string NormalizedEmailAddress { get; set; }
        public List<DateOTHourDto> ListOverTimeHour { get; set; }
        public float TotalOTTimeHour
        {
            get
            {
                return ListOverTimeHour.Sum(x => x.OTHour);
            }
        }
    }
    public class DateOTHourDto
    {
        public DateTime Date { get; set; }
        public float OTHour { get; set; }

    }
}