using Amazon.S3.Model;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Report.Dto
{
    public class ReportSalaryDto
    {
        public InfoEmployeeDto InfoEmployee { get; set; }
        public List<ResultReport> ResultReports { get; set; }
        public double TotalSalary => ResultReports.Sum(x => x.Salary);
    }

    public class ResultReport
    {
        public string PayrollName { get; set; }
        public double Salary { get; set; }
    }
    public class InputMultiFilterReportSalaryPagingDto
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public List<long> TeamPayslipIds { get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<UserType> UserTypePayslips { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }
        public List<long> LevelIds {  get; set; }
        public List<long> PayrollIds { get; set; }
        public List<long> EmployeeIds { get; set; }
        public List<long> BranchPayslipIds { get; set; }
        public List<long> JobPositionPayslipIds { get; set; }
        public List<long> LevelPayslipIds { get; set; }
    }

    public class InfoEmployeeDto
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public string AvatarFullPath { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public BadgeInfoDto UserTypeInfo { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public Sex Sex { get; set; }
        public string Email { get; set; }
        public List<long> TeamIds { get; set; }
    }
}
