using HRMv2.Constants.Enum;
using HRMv2.Manager.WorkingHistories.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace HRMv2.Manager.Home.Dtos
{

    public class HomepageEmployeeStatisticDto
    {
        public string BranchName { get; set; }
        /// <summary>
        /// Total employee working on end date
        /// </summary>
        public int EmployeeTotal { get; set; }
        /// <summary>
        /// Total intern working on end date
        /// </summary>
        public int InternCount { get; set; }
        /// <summary>
        /// Total staff working on end date
        /// </summary>
        public int StaffCount { get; set; }
        /// <summary>
        /// Total CTV working on end date
        /// </summary>
        public int CTVCount { get; set; }
        /// <summary>
        /// Total employee thử việc working on end date
        /// </summary>
        public int TViecCount { get; set; }
        /// <summary>
        /// Total vendor working on end date
        /// </summary>
        public int VendorCount { get; set; }

        /// <summary>
        /// Total employee onboarded between (start date, end date)
        /// </summary>
        public int OnboardTotal => OnboardEmployees.Count;
        /// <summary>
        /// Total intern onboarded between (start date, end date)
        /// </summary>
        public int OnboardInternCount => OnboardEmployees.Where(s => s.UserType == HRMEnum.UserType.Internship).Count();

        /// <summary>
        /// Staff + CTV + TViec onboard in a month
        /// </summary>
        public int OnboardStaffCount => OnboardEmployees.Where(s => s.UserType != HRMEnum.UserType.Internship).Count();


        public int QuitJobTotal => QuitEmployees.Count;
        public int QuitJobInternCount => QuitEmployees.Where(s => s.UserType == HRMEnum.UserType.Internship).Count();

        /// <summary>
        /// Staff + CTV + TViec quit job in a month
        /// </summary>
        public int QuitJobStaffCount => QuitEmployees.Where(s => s.UserType != HRMEnum.UserType.Internship).Count();

        /// <summary>
        /// Tam nghi trong thang
        /// </summary>
        public int PausingCount => PausingEmployees.Count;

        /// <summary>
        /// Nghi sinh
        /// </summary>
        public int MaternityLeaveCount => MatenityLeaveEmployees.Count;

        public List<LastEmployeeWorkingHistoryDto> OnboardEmployees { get; set; }
        public List<LastEmployeeWorkingHistoryDto> QuitEmployees { get; set; }
        public List<LastEmployeeWorkingHistoryDto> PausingEmployees { get; set; }
        public List<LastEmployeeWorkingHistoryDto> MatenityLeaveEmployees { get; set; }
        /// <summary>
        /// List employee onboard and quit in (start date, end date)
        /// </summary>
        public List<LastEmployeeWorkingHistoryDto> OnboardAndQuitEmployees { get; set; }

        /// <summary>
        /// Số lượng empoloyee onboard và quit trong khoảng thời gian (start date, end date)
        /// </summary>
        public int OnboardAndQuitInTimeSpanCount => OnboardAndQuitEmployees.Count;
    }
}
