using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Constants.Enum;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Anotations;
using NccCore.DynamicFilter;
using NccCore.Paging;
using NccCore.Uitls;
using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeDto : BaseEmployeeDto
    {
        public string UpdatedUser { get; set; }
        public DateTime StartWorkingDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string PlaceOfPermanent { get; set; }
        public string Address { get; set; }
        public string Bank { get; set; }
        public string BankAccountNumber { get; set; }
        public long? BankId { get; set; }
        public double Salary { get; set; }
        public double RealSalary { get; set; }
        public float RemainLeaveDay { get; set; }
        public double ProbationPercentage { get; set; }
        public string TaxCode { get; set; }
        public InsuranceStatus InsuranceStatus { get; set; }
        public string PersonalEmail { get; set; }

        public DateBetweenDto Seniority
        {
            get
            {
                if (StartWorkingDate != default && UserType == UserType.Staff)
                {
                    Period diff = DateTimeUtils.CalRangeBetweenDate(StartWorkingDate);
                    return new DateBetweenDto
                    {
                        Days = diff.Days,
                        Months = diff.Months,
                        Years = diff.Years
                    };
                }
                return null;
            }
        }
    }

    public class GetEmployeeDetailDto : GetEmployeeDto
    {
        public string ContractCode { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

    }

    public class GetEmployeeForExportDto : GetEmployeeDetailDto {
        public string SeniorityDay
        {
            get
            {
                if (StartWorkingDate != default && UserType == UserType.Staff && StartWorkingDate < CommonUtil.GetNow())
                {
                    return (CommonUtil.GetNow().Date - StartWorkingDate.Date).TotalDays.ToString() + "d";
                }
                return null;
            }
        }
        public string Surname => CommonUtil.GetSurNameByFullName(FullName);
        public string Name => CommonUtil.GetNameByFullName(FullName);
        public string JobPositionCode { get; set; }
        public string BranchCode { get; set; }
        public string LevelName { get; set; }
    }



    public class GetEmployeeBasicInfoDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

    }

    public class GetEmployeeBasicInfoForBreadcrumbDto : GetEmployeeBasicInfoDto
    {
        public string Avatar { get; set; }
        public string AvatarFullPath => FileUtil.FullFilePath(Avatar);
    }


    public class DateBetweenDto
    {
        public int Years { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
    }

    public class GetEmployeeBackToWork: GetEmployeeDto
    {
        public DateTime BackDate { get; set; }
        public DateTime ApplyDate { get; set; }
        public double RealSalary { get; set; }
    }

    public class InputMultiFilterEmployeePagingDto
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public bool IsAndCondition { get; set; }
        public List<EmployeeStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> Usertypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }
        public SeniorityFilterInput Seniority { get; set; }
        public int ?DaysLeftContractEndDate { get; set; }

    }

    public class GetEmployeeWorkingHistoryDto 
    {
        public long EmployeeId { get; set; }
        public DateTime BackDate { get; set; }
        public DateTime ApplyDate { get; set; }
        public EmployeeStatus? Status { get; set; }

    }

    public class SeniorityFilterInput
    {
        public SeniorityComparision Comparison { get; set;}
        public SeniorityFilterType SeniorityType { get; set; }
        public int SeniorityValue { get; set; }

        public DateTime GetDate()
        {
            var today = DateTimeUtils.GetNow();
            switch (this.SeniorityType)
            {
                case SeniorityFilterType.Day:
                    return today.AddDays(-this.SeniorityValue).Date;

                case SeniorityFilterType.Month:
                    return today.AddMonths(-this.SeniorityValue).Date;

                case SeniorityFilterType.year:
                    return today.AddYears(-this.SeniorityValue).Date;

            }

            return today.AddDays(-this.SeniorityValue).Date;

        }

    }

    public class GetEmployeeContractDto : GetEmployeeDto
    {
        public string ContractCode { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
    }




}
