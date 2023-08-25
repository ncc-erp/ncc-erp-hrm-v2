
namespace HRMv2.Constants.Enum
{
    public class HRMEnum
    {
        public enum BenefitType
        {
            CheDoChung = 1,
            CheDoRieng = 2,
            CheDoRemote = 3
        }

        public enum Sex
        {
            Male = 1,
            Female = 2
        }

        public enum EmployeeStatus
        {
            Working = 1,
            Pausing = 2,
            Quit = 3,
            MaternityLeave = 4
        }
        public enum InsuranceStatus
        {
            BHXH = 1,
            PVI = 2,
            NONE = 3,
            PVIandBHXH = 4
        }

        public enum PunishmentStatus
        {
            Active = 0,
            InActive = 1
        }

        public enum DebtPaymentType
        {
            Salary = 1,
            RealMoney = 2
        }
        public enum DebtStatus
        {
            Inprogress = 1,
            Done = 2
        }

        public enum PayrollStatus
        {
            New = 1,
            PendingKT = 2,
            PendingCEO = 3,
            RejectedByKT = 4,
            RejectedByCEO = 5,
            ApprovedByCEO = 6,
            Executed = 7
        }
        public enum PayslipDetaiType
        {
            SalaryNormal = 1,
            SalaryOT = 2,
            Benefit = 3,
            Bonus = 4,
            Punishment = 5,
            Debt = 6
        }

        public enum SalaryRequestStatus
        {
            New = 1,
            Pending = 2,
            Approved = 3,
            Rejected = 4,
            Executed = 5,
        }


        public enum SalaryRequestType
        {
            Initial = 1,
            Change = 2,
            MaternityLeave = 3,
            BackToWork = 4,
            StopWorking = 5
        }

        public enum PayslipDetailType
        {
            SalaryNormal = 1,
            SalaryOT = 2,
            SalaryMaternityLeave = 3,
            Benefit = 4,
            Bonus =5,
            Punishment = 6,
            Debt = 7,
            Refund = 8
        }

        public enum SeniorityFilterType
        {
            Day = 1,
            Month = 2,
            year = 3
        }

        public enum UserType
        {
            Internship = 0,
            Collaborators = 1,
            Staff = 2,
            ProbationaryStaff = 3,
            Vendor = 5
        }

       

        public enum SeniorityComparision
        {
            Equal = 0,           
            LessThanOrEqual = 2,           
            GreaterThanOrEqual = 4,           
        }
        public enum ActionMode
        {
            Create = 0,
            Update = 1,
        }

        public enum MailFuncEnum
        {
            Payslip = 1,
            ContractBM = 2,
            ContractDT = 3,
            ContractTV = 4,
            ContractCTV = 5,
            ContractLD = 6,
            Debt = 7,
            Bonus = 8,
            Checkpoint = 9,
            PayrollPendingCEO = 10,
            PayrollApprovedByCEO = 11,
            PayrollRejectedByCEO= 12,
            PayrollExecuted = 13,

        }

        public enum TemplateType
        {
            Mail = 1,
            Print = 2
        }
        
        public enum PayslipConfirmStatus
        {
            NotConfirm = 0,
            ConfirmRight = 1,
            ConfirmWrong = 2
        }

        public enum TalentOnboardStatus
        {
            AcceptedOffer = 8,
            RejectedOffer = 9,
            Onboarded = 10
        }
        public enum RequestStatus
        {
            Pending = 1,
            Approved = 2,
            Rejected = 3
        }

        public enum PayslipStatusForExport
        {
            Both = 0,
            InLastMonthOnly = 1,
            InThisMonthOnly = 2,
        }
        public enum Comparision
        {
            Equal = 0,
            LessThan = 1,
            GreaterThan = 3,
        }
    }
}