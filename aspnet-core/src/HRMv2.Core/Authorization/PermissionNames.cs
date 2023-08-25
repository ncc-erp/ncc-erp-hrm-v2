using Abp.MultiTenancy;
using System.Collections.Generic;
using static HRMv2.Authorization.Roles.StaticRoleNames;

namespace HRMv2.Authorization
{
    public static class PermissionNames
    {
        public const string Home = "Home";
        public const string Admin = "Admin";

        //User
        public const string Admin_User = "Admin.User";
        public const string Admin_User_View = "Admin.User.View";
        public const string Admin_User_Create = "Admin.User.Create";
        public const string Admin_User_Edit = "Admin.User.Edit";
        public const string Admin_User_EditUserRole = "Admin.User.EditUserRole";
        public const string Admin_User_Delete = "Admin.User.Delete";
        public const string Admin_User_ResetPassword = "Admin.User.ResetPassword";

        //Role
        public const string Admin_Role = "Admin.Role";
        public const string Admin_Role_View = "Admin.Role.View";
        public const string Admin_Role_Create = "Admin.Role.Create";
        public const string Admin_Role_Edit = "Admin.Role.Edit";
        public const string Admin_Role_Delete = "Admin.Role.Delete";

        //Tenant
        public const string Admin_Tenant = "Admin.Tenant";
        public const string Admin_Tenant_View = "Admin.Tenant.View";
        public const string Admin_Tenant_Create = "Admin.Tenant.Create";
        public const string Admin_Tenant_Edit = "Admin.Tenant.Edit";
        public const string Admin_Tenant_Delete = "Admin.Tenant.Delete";

        //Configuration
        public const string Admin_Configuration = "Admin.Configuration";
        public const string Admin_Configuration_View = "Admin.Configuration.View";
        public const string Admin_Configuration_HRMSetting = "Admin.Configuration.HRMSetting";
        public const string Admin_Configuration_ProjectSetting = "Admin.Configuration.ProjectSetting";
        public const string Admin_Configuration_IMSSetting = "Admin.Configuration.IMSSetting";
        public const string Admin_Configuration_FinfastSetting = "Admin.Configuration.FinfastSetting";
        public const string Admin_Configuration_TimesheetSetting = "Admin.Configuration.TimesheetSetting";
        public const string Admin_Configuration_TalentSetting = "Admin.Configuration.TalentSetting";
        public const string Admin_Configuration_LoginSetting = "Admin.Configuration.LoginSetting";
        public const string Admin_Configuration_WokerAutoUpdateAllEmployeeInfo = "Admin.Configuration.WokerAutoUpdateAllEmployeeInfo";
        public const string Admin_Configuration_HRMSetting_View = "Admin.Configuration.HRMSetting.View";
        public const string Admin_Configuration_HRMSetting_Edit = "Admin.Configuration.HRMSetting.Edit";
        public const string Admin_Configuration_ProjectSetting_View = "Admin.Configuration.ProjectSetting.View";
        public const string Admin_Configuration_IMSSetting_View = "Admin.Configuration.IMSSetting.View";
        public const string Admin_Configuration_FinfastSetting_View = "Admin.Configuration.FinfastSetting.View";
        public const string Admin_Configuration_TimesheetSetting_View = "Admin.Configuration.TimesheetSetting.View";
        public const string Admin_Configuration_TalentSetting_View = "Admin.Configuration.TalentSetting.View";
        public const string Admin_Configuration_LoginSetting_View = "Admin.Configuration.LoginSetting.View";
        public const string Admin_Configuration_LoginSetting_Edit = "Admin.Configuration.LoginSetting.Edit";
        public const string Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View = "Admin.Configuration.WokerAutoUpdateAllEmployeeInfo.View";
        public const string Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit = "Admin.Configuration.WokerAutoUpdateAllEmployeeInfo.Edit";

        //Email Template
        public const string Admin_EmailTemplate = "Admin.EmailTemplate";
        public const string Admin_EmailTemplate_View = "Admin.EmailTemplate.View";
        public const string Admin_EmailTemplate_Edit = "Admin.EmailTemplate.Edit";
        public const string Admin_EmailTemplate_PreviewTemplate = "Admin.EmailTemplate.PreviewTemplate";
        public const string Admin_EmailTemplate_PreviewTemplate_SendMail = "Admin.EmailTemplate.PreviewTemplate.SendMail";
        //Bg Job
        public const string Admin_BackgroundJob = "Admin.BackgroundJob";
        public const string Admin_BackgroundJob_View = "Admin.BackgroundJob.View";
        public const string Admin_BackgroundJob_Delete = "Admin.BackgroundJob.Delete";
        public const string Admin_BackgroundJob_Retry = "Admin.BackgroundJob.Retry";
        //AuditLog
        public const string Admin_AuditLog = "Admin.AuditLog";
        public const string Admin_AuditLog_View = "Admin.AuditLog.View";
        //Category
        public const string Category = "Category";
        public const string Category_Branch = "Category.Branch";
        public const string Category_Usertype = "Category.Usertype";
        public const string Category_JobPosition = "Category.JobPosition";
        public const string Category_Level = "Category.Level";
        public const string Category_Skill = "Category.Skill";
        public const string Category_Team = "Category.Team";
        public const string Category_Bank = "Category.Bank";
        public const string Category_PunishmentType = "Category.PunishmentType";
        public const string Category_IssuedBy = "Category.IssuedBy";
        public const string Category_Branch_View = "Category.Branch.View";
        public const string Category_Branch_Create = "Category.Branch.Create";
        public const string Category_Branch_Edit = "Category.Branch.Edit";
        public const string Category_Branch_Delete = "Category.Branch.Delete";
        public const string Category_Usertype_View = "Category.Usertype.View";
        public const string Category_JobPosition_View = "Category.JobPosition.View";
        public const string Category_JobPosition_Create = "Category.JobPosition.Create";
        public const string Category_JobPosition_Edit = "Category.JobPosition.Edit";
        public const string Category_JobPosition_Delete = "Category.JobPosition.Delete";
        public const string Category_Level_View = "Category.Level.View";
        public const string Category_Level_Create = "Category.Level.Create";
        public const string Category_Level_Edit = "Category.Level.Edit";
        public const string Category_Level_Delete = "Category.Level.Delete";
        public const string Category_Skill_View = "Category.Skill.View";
        public const string Category_Skill_Create = "Category.Skill.Create";
        public const string Category_Skill_Edit = "Category.Skill.Edit";
        public const string Category_Skill_Delete = "Category.Skill.Delete";
        public const string Category_Team_View = "Category.Team.View";
        public const string Category_Team_Create = "Category.Team.Create";
        public const string Category_Team_Edit = "Category.Team.Edit";
        public const string Category_Team_Delete = "Category.Team.Delete";
        public const string Category_Bank_View = "Category.Bank.View";
        public const string Category_Bank_Create = "Category.Bank.Create";
        public const string Category_Bank_Edit = "Category.Bank.Edit";
        public const string Category_Bank_Delete = "Category.Bank.Delete";
        public const string Category_PunishmentType_View = "Category.PunishmentType.View";
        public const string Category_PunishmentType_Create = "Category.PunishmentType.Create";
        public const string Category_PunishmentType_Edit = "Category.PunishmentType.Edit";
        public const string Category_PunishmentType_Delete = "Category.PunishmentType.Delete";
        public const string Category_IssuedBy_View = "Category.IssuedBy.View";
        public const string Category_IssuedBy_Create = "Category.IssuedBy.Create";
        public const string Category_IssuedBy_Edit = "Category.IssuedBy.Edit";
        public const string Category_IssuedBy_Delete = "Category.IssuedBy.Delete";

        //Punishment
        public const string Punishment = "Punishment";
        public const string Punishment_View = "Punishment.View";
        public const string Punishment_Create = "Punishment.Create";
        public const string Punishment_Generate = "Punishment.Generate";
        public const string Punishment_Edit = "Punishment.Edit";
        public const string Punishment_Active = "Punishment.Active";
        public const string Punishment_Deactive = "Punishment.Deactive";
        public const string Punishment_Delete = "Punishment.Delete";
        public const string Punishment_PunishmentDetail = "Punishment.PunishmentDetail";
        public const string Punishment_PunishmentDetail_View = "Punishment.PunishmentDetail.View";
        public const string Punishment_PunishmentDetail_AddEmployee = "Punishment.PunishmentDetail.AddEmployee";
        public const string Punishment_PunishmentDetail_Import = "Punishment.PunishmentDetail.Import";
        public const string Punishment_PunishmentDetail_Edit = "Punishment.PunishmentDetail.Edit";
        public const string Punishment_PunishmentDetail_DownloadTemplate = "Punishment.PunishmentDetail.DownloadTemplate";
        public const string Punishment_PunishmentDetail_Delete = "Punishment.PunishmentDetail.Delete";

        //Punishment Fund
        public const string PunishmentFund = "PunishmentFund";
        public const string PunishmentFund_View = "PunishmentFund.View";
        public const string PunishmentFund_Add = "PunishmentFund.Add";
        public const string PunishmentFund_Disburse = "PunishmentFund.Disburse";
        public const string PunishmentFund_Edit = "PunishmentFund.Edit";
        public const string PunishmentFund_Delete = "PunishmentFund.Delete";

        //Employee
        public const string Employee = "Employee";
        public const string Employee_View = "Employee.View";
        public const string Employee_Create = "Employee.Create";
        public const string Employee_Edit= "Employee.Edit";
        public const string Employee_Export = "Employee.Export";
        public const string Employee_UploadAvatar = "Employee.UploadAvatar";
        public const string Employee_Delete = "Employee.Delete";
        public const string Employee_DownloadCreateTemplate = "Employee.DownloadCreateTemplate";
        public const string Employee_DownloadUpdateTemplate = "Employee.DownloadUpdateTemplate";
        public const string Employee_CreateEmployeeByFile = "Employee.CreateEmployeeByFile";
        public const string Employee_UpdateEmployeeByFile = "Employee.UpdateEmployeeByFile";
        public const string Employee_EmployeeDetail = "Employee.EmployeeDetail";
        public const string Employee_SyncUpdateEmployeesInforToOtherTools = "Employee.SyncUpdateEmployeesInforToOtherTools";

        //Tab Personal Info
        public const string Employee_EmployeeDetail_TabPersonalInfo = "Employee.EmployeeDetail.TabPersonalInfo";
        public const string Employee_EmployeeDetail_TabPersonalInfo_View = "Employee.EmployeeDetail.TabPersonalInfo.View";
        public const string Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar = "Employee.EmployeeDetail.TabPersonalInfo.UploadAvatar";
        public const string Employee_EmployeeDetail_TabPersonalInfo_Edit = "Employee.EmployeeDetail.TabPersonalInfo.Edit";
        public const string Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool = "Employee.EmployeeDetail.TabPersonalInfo.SyncToOtherTool";
        public const string Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool = "Employee.EmployeeDetail.TabPersonalInfo.SyncToOtherTool.ReCreateUserToOtherTool";
        public const string Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool = "Employee.EmployeeDetail.TabPersonalInfo.SyncToOtherTool.EditUserToOtherTool";
        public const string Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool = "Employee.EmployeeDetail.TabPersonalInfo.SyncToOtherTool.QuitJobUserToOtherTool";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch = "Employee.EmployeeDetail.TabPersonalInfo.ChangeBranch";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.BackToWork";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.MaternityLeave";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.Quit";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.Pause";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.ExtendMaternityLeave";
        public const string Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing = "Employee.EmployeeDetail.TabPersonalInfo.ChangeWorkingStatus.ExtendPausing";

        //Tab Contract
        public const string Employee_EmployeeDetail_TabContract = "Employee.EmployeeDetail.TabContract";
        public const string Employee_EmployeeDetail_TabContract_View = "Employee.EmployeeDetail.TabContract.View";
        public const string Employee_EmployeeDetail_TabContract_ImportContractFile = "Employee.EmployeeDetail.TabContract.ImportContractFile";
        public const string Employee_EmployeeDetail_TabContract_DeleteContractFile = "Employee.EmployeeDetail.TabContract.DeleteContractFile";
        public const string Employee_EmployeeDetail_TabContract_EditNote = "Employee.EmployeeDetail.TabContract.EditNote";
        public const string Employee_EmployeeDetail_TabContract_Edit = "Employee.EmployeeDetail.TabContract.Edit";
        public const string Employee_EmployeeDetail_TabContract_Delete = "Employee.EmployeeDetail.TabContract.Delete";
        public const string Employee_EmployeeDetail_TabContract_PrintLaborContract = "Employee.EmployeeDetail.TabContract.PrintLaborContract";
        public const string Employee_EmployeeDetail_TabContract_PrintTrainingContract = "Employee.EmployeeDetail.TabContract.PrintTrainingContract";
        public const string Employee_EmployeeDetail_TabContract_PrintConfidentialContract = "Employee.EmployeeDetail.TabContract.PrintConfidentialContract";
        public const string Employee_EmployeeDetail_TabContract_PrintProbationaryContract = "Employee.EmployeeDetail.TabContract.PrintProbationaryContract";
        public const string Employee_EmployeeDetail_TabContract_PrintCollaboratorContract = "Employee.EmployeeDetail.TabContract.PrintCollaboratorContract";
        //Tab Debt

        public const string Employee_EmployeeDetail_TabDebt = "Employee.EmployeeDetail.TabDebt";
        public const string Employee_EmployeeDetail_TabDebt_View = "Employee.EmployeeDetail.TabDebt.View";
        public const string Employee_EmployeeDetail_TabDebt_Add = "Employee.EmployeeDetail.TabDebt.Add";

        //Tab Benefit 

        public const string Employee_EmployeeDetail_TabBenefit = "Employee.EmployeeDetail.TabBenefit";
        public const string Employee_EmployeeDetail_TabBenefit_View = "Employee.EmployeeDetail.TabBenefit.View";
        public const string Employee_EmployeeDetail_TabBenefit_Add = "Employee.EmployeeDetail.TabBenefit.Add";
        public const string Employee_EmployeeDetail_TabBenefit_Edit = "Employee.EmployeeDetail.TabBenefit.Edit";
        public const string Employee_EmployeeDetail_TabBenefit_Delete = "Employee.EmployeeDetail.TabBenefit.Delete";

        //Tab Bonus

        public const string Employee_EmployeeDetail_TabBonus = "Employee.EmployeeDetail.TabBonus";
        public const string Employee_EmployeeDetail_TabBonus_View = "Employee.EmployeeDetail.TabBonus.View";

        //Tab Punishment 
        public const string Employee_EmployeeDetail_TabPunishment = "Employee.EmployeeDetail.TabPunishment";
        public const string Employee_EmployeeDetail_TabPunishment_View = "Employee.EmployeeDetail.TabPunishment.View";

        //Tab Salary History
        public const string Employee_EmployeeDetail_TabSalaryHistory = "Employee.EmployeeDetail.TabSalaryHistory";
        public const string Employee_EmployeeDetail_TabSalaryHistory_View = "Employee.EmployeeDetail.TabSalaryHistory.View";
        public const string Employee_EmployeeDetail_TabSalaryHistory_Edit = "Employee.EmployeeDetail.TabSalaryHistory.Edit";
        public const string Employee_EmployeeDetail_TabSalaryHistory_EditNote = "Employee.EmployeeDetail.TabSalaryHistory.EditNote";
        public const string Employee_EmployeeDetail_TabSalaryHistory_Delete = "Employee.EmployeeDetail.TabSalaryHistory.Delete";
        public const string Employee_EmployeeDetail_TabSalaryHistory_ForceDelete = "Employee.EmployeeDetail.TabSalaryHistory.ForceDelete";

        //Tab WorkingHistory
        public const string Employee_EmployeeDetail_TabWorkingHistory = "Employee.EmployeeDetail.TabWorkingHistory";
        public const string Employee_EmployeeDetail_TabWorkingHistory_View = "Employee.EmployeeDetail.TabWorkingHistory.View";
        public const string Employee_EmployeeDetail_TabWorkingHistory_EditNote = "Employee.EmployeeDetail.TabWorkingHistory.EditNote";
        public const string Employee_EmployeeDetail_TabWorkingHistory_EditDate = "Employee.EmployeeDetail.TabWorkingHistory.EditDate";
        public const string Employee_EmployeeDetail_TabWorkingHistory_Delete = "Employee.EmployeeDetail.TabWorkingHistory.Delete";

        //Tab Branch History
        public const string Employee_EmployeeDetail_TabBranchHistory = "Employee.EmployeeDetail.TabBranchHistory";
        public const string Employee_EmployeeDetail_TabBranchHistory_View = "Employee.EmployeeDetail.TabBranchHistory.View";
        public const string Employee_EmployeeDetail_TabBranchHistory_EditNote = "Employee.EmployeeDetail.TabBranchHistory.EditNote";
        public const string Employee_EmployeeDetail_TabBranchHistory_Delete = "Employee.EmployeeDetail.TabBranchHistory.Delete";

        //Tab Payslip History
        public const string Employee_EmployeeDetail_TabPayslipHistory = "Employee.EmployeeDetail.TabPayslipHistory";
        public const string Employee_EmployeeDetail_TabPayslipHistory_View = "Employee.EmployeeDetail.TabPayslipHistory.View";

        //WarningEmployee
        public const string WarningEmployee = "WarningEmployee";
        public const string WarningEmployee_BackToWork = "WarningEmployee.BackToWork";
        public const string WarningEmployee_BackToWork_View = "WarningEmployee.BackToWork.View";
        public const string WarningEmployee_BackToWork_UpdateEmployeeBackDate = "WarningEmployee.BackToWork.UpdateEmployeeBackDate";
        public const string WarningEmployee_BackToWork_BackToWork = "WarningEmployee.BackToWork.BackToWork";
        public const string WarningEmployee_ContractExpired = "WarningEmployee.ContractExpired";
        public const string WarningEmployee_ContractExpired_View = "WarningEmployee.ContractExpired.View";
        public const string WarningEmployee_RequestChangeInfo = "WarningEmployee.RequestChangeInfo";
        public const string WarningEmployee_RequestChangeInfo_View = "WarningEmployee.RequestChangeInfo.View";
        public const string WarningEmployee_RequestChangeInfo_DetailRequest = "WarningEmployee.RequestChangeInfo.DetailRequest";
        public const string WarningEmployee_RequestChangeInfo_DetailRequest_View = "WarningEmployee.RequestChangeInfo.DetailRequest.View";
        public const string WarningEmployee_RequestChangeInfo_DetailRequest_Approve = "WarningEmployee.RequestChangeInfo.DetailRequest.Approve";
        public const string WarningEmployee_RequestChangeInfo_DetailRequest_Reject = "WarningEmployee.RequestChangeInfo.DetailRequest.Reject";
        public const string WarningEmployee_PlanOnboard = "WarningEmployee.PlanOnboard";
        public const string WarningEmployee_PlanOnboard_View = "WarningEmployee.PlanOnboard.View";
        public const string WarningEmployee_PlanOnboard_ViewSalary = "WarningEmployee.PlanOnboard.ViewSalary";
        public const string WarningEmployee_PlanOnboard_Create = "WarningEmployee.PlanOnboard.Create";
        public const string WarningEmployee_PlanOnboard_Edit = "WarningEmployee.PlanOnboard.Edit";
        public const string WarningEmployee_PlanOnboard_Delete = "WarningEmployee.PlanOnboard.Delete";
        public const string WarningEmployee_PlanQuitEmployee = "WarningEmployee.PlanQuitEmployee";
        public const string WarningEmployee_PlanQuitEmployee_View = "WarningEmployee.PlanQuitEmployee.View";
        public const string WarningEmployee_PlanQuitEmployee_Edit = "WarningEmployee.PlanQuitEmployee.Edit";
        public const string WarningEmployee_PlanQuitEmployee_Detele = "WarningEmployee.PlanQuitEmployee.Detele";

        //Debt
        public const string Debt = "Debt";
        public const string Debt_View = "Debt.View";
        public const string Debt_Create = "Debt.Create";
        public const string Debt_Delete = "Debt.Delete";
        public const string Debt_DebtDetail = "Debt.DebtDetail";
        public const string Debt_DebtDetail_View = "Debt.DebtDetail.View";
        public const string Debt_DebtDetail_Edit = "Debt.DebtDetail.Edit";
        public const string Debt_DebtDetail_Delete = "Debt.DebtDetail.Delete";
        public const string Debt_DebtDetail_SetDone = "Debt.DebtDetail.SetDone";
        public const string Debt_DebtDetail_GeneratePaymentPlan = "Debt.DebtDetail.GeneratePaymentPlan";
        public const string Debt_DebtDetail_AddPaymentPlan = "Debt.DebtDetail.AddPaymentPlan";
        public const string Debt_DebtDetail_EditPaymentPlan = "Debt.DebtDetail.EditPaymentPlan";
        public const string Debt_DebtDetail_DeletePaymentPlan = "Debt.DebtDetail.DeletePaymentPlan";
        public const string Debt_DebtDetail_AddDebtPaid = "Debt.DebtDetail.AddDebtPaid";
        public const string Debt_DebtDetail_EditDebtPaid = "Debt.DebtDetail.EditDebtPaid";
        public const string Debt_DebtDetail_DeleteDebtPaid = "Debt.DebtDetail.DeleteDebtPaid";

        // Bonus
        public const string Bonus = "Bonus";
        public const string Bonus_View = "Bonus.View";
        public const string Bonus_Create = "Bonus.Create";
        public const string Bonus_Edit = "Bonus.Edit";
        public const string Bonus_Active = "Bonus.Active";
        public const string Bonus_Deactive = "Bonus.Deactive";
        public const string Bonus_Delete = "Bonus.Delete";
        public const string Bonus_BonusDetail = "Bonus.BonusDetail";

        //Tab Infomation
        public const string Bonus_BonusDetail_TabInformation = "Bonus.BonusDetail.TabInformation";
        public const string Bonus_BonusDetail_TabInformation_View = "Bonus.BonusDetail.TabInformation.View";
        public const string Bonus_BonusDetail_TabInformation_Edit = "Bonus.BonusDetail.TabInformation.Edit";
        //Tab Employee
        public const string Bonus_BonusDetail_TabEmployee = "Bonus.BonusDetail.TabEmployee";
        public const string Bonus_BonusDetail_TabEmployee_View = "Bonus.BonusDetail.TabEmployee.View";
        public const string Bonus_BonusDetail_TabEmployee_QuickAdd = "Bonus.BonusDetail.TabEmployee.QuickAdd";
        public const string Bonus_BonusDetail_TabEmployee_Add = "Bonus.BonusDetail.TabEmployee.Add";
        public const string Bonus_BonusDetail_TabEmployee_DownloadTemplate = "Bonus.BonusDetail.TabEmployee.DownloadTemplate";
        public const string Bonus_BonusDetail_TabEmployee_Import = "Bonus.BonusDetail.TabEmployee.Import";
        public const string Bonus_BonusDetail_TabEmployee_Edit = "Bonus.BonusDetail.TabEmployee.Edit";
        public const string Bonus_BonusDetail_TabEmployee_Delete = "Bonus.BonusDetail.TabEmployee.Delete";
        public const string Bonus_BonusDetail_TabEmployee_SendAllMail = "Bonus.BonusDetail.TabEmployee.SendAllMail";
        public const string Bonus_BonusDetail_TabEmployee_SendMail = "Bonus.BonusDetail.TabEmployee.SendMail";

        //Refund
        public const string Refund = "Refund";
        public const string Refund_View = "Refund.View";
        public const string Refund_Create = "Refund.Create";
        public const string Refund_Edit = "Refund.Edit";
        public const string Refund_Active = "Refund.Active";
        public const string Refund_Deactive = "Refund.Deactive";
        public const string Refund_Delete = "Refund.Delete";
        public const string Refund_RefundDetail = "Refund.RefundDetai";
        public const string Refund_RefundDetail_View = "Refund.RefundDetai.View";
        public const string Refund_RefundDetail_AddEmployee = "Refund.RefundDetai.AddEmployee";
        public const string Refund_RefundDetail_Edit = "Refund.RefundDetai.Edit";
        public const string Refund_RefundDetail_Delete = "Refund.RefundDetai.Delete";

        //Benefit
        public const string Benefit = "Benefit";
        public const string Benefit_View = "Benefit.View";
        public const string Benefit_Create = "Benefit.Create";
        public const string Benefit_Edit = "Benefit.Edit";
        public const string Benefit_Active = "Benefit.Active";
        public const string Benefit_Deactive = "Benefit.Deactive";
        public const string Benefit_Delete = "Benefit.Delete";
        public const string Benefit_BenefitDetail = "Benefit.BenefitDetail";
        //Tab Infomation
        public const string Benefit_BenefitDetail_TabInformation = "Benefit.BenefitDetail.TabInformation";
        public const string Benefit_BenefitDetail_TabInformation_View = "Benefit.BenefitDetail.TabInformation.View";
        public const string Benefit_BenefitDetail_TabInformation_Edit = "Benefit.BenefitDetail.TabInformation.Edit";
        public const string Benefit_BenefitDetail_TabInformation_Clone = "Benefit.BenefitDetail.TabInformation.Clone";
        public const string Benefit_BenefitDetail_TabInformation_Delete = "Benefit.BenefitDetail.TabInformation.Delete";
        //Tab Employee
        public const string Benefit_BenefitDetail_TabEmployee = "Benefit.BenefitDetail.TabEmployee";
        public const string Benefit_BenefitDetail_TabEmployee_View = "Benefit.BenefitDetail.TabEmployee.View";
        public const string Benefit_BenefitDetail_TabEmployee_QuickAdd = "Benefit.BenefitDetail.TabEmployee.QuickAdd";
        public const string Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate = "Benefit.BenefitDetail.TabEmployee.UpdateAllStartDate";
        public const string Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate = "Benefit.BenefitDetail.TabEmployee.UpdateAllEndDate";
        public const string Benefit_BenefitDetail_TabEmployee_Add = "Benefit.BenefitDetail.TabEmployee.Add";
        public const string Benefit_BenefitDetail_TabEmployee_Edit = "Benefit.BenefitDetail.TabEmployee.Edit";
        public const string Benefit_BenefitDetail_TabEmployee_Delete = "Benefit.BenefitDetail.TabEmployee.Delete";

        // Payroll
        public const string Payroll = "Payroll";
        public const string Payroll_View = "Payroll.View";
        public const string Payroll_Create = "Payroll.Create";
        public const string Payroll_Edit = "Payroll.Edit";
        public const string Payroll_Delete = "Payroll.Delete";
        public const string Payroll_SendToAccountant = "Payroll.SendToAccountant";
        public const string Payroll_ApproveAndSendtToCEO = "Payroll.ApproveAndSendtToCEO";
        public const string Payroll_RejectByKT = "Payroll.RejectByKT";
        public const string Payroll_ApproveByKT = "Payroll.ApproveByKT";
        public const string Payroll_RejectByCEO = "Payroll.RejectByCEO";
        public const string Payroll_ApproveByCEO = "Payroll.ApproveByCEO";
        public const string Payroll_Execute = "Payroll.Execute";
        public const string Payroll_Payslip = "Payroll.PayslipDetail";

        public const string Payroll_Payslip_View = "Payroll.Payslip.View";
        public const string Payroll_Payslip_CalculateSalary = "Payroll.Payslip.CalculateSalary";
        public const string Payroll_Payslip_SendMailAll = "Payroll.Payslip.SendMailAll";
        public const string Payroll_Payslip_Add = "Payroll.Payslip.Add";
        public const string Payroll_Payslip_Export = "Payroll.Payslip.Export";
        public const string Payroll_Payslip_Delete = "Payroll.Payslip.Delete";
        public const string Payroll_Payslip_SendMail = "Payroll.Payslip.SendMail";

        public const string Payroll_Payslip_SendToAccountant = "Payroll.Payslip.SendToAccountant";
        public const string Payroll_Payslip_ApproveAndSendtToCEO = "Payroll.Payslip.ApproveAndSendtToCEO";
        public const string Payroll_Payslip_RejectByKT = "Payroll.Payslip.RejectByKT";
        public const string Payroll_Payslip_ApproveByKT = "Payroll.Payslip.ApproveByKT";
        public const string Payroll_Payslip_RejectByCEO = "Payroll.Payslip.RejectByCEO";
        public const string Payroll_Payslip_ApproveByCEO = "Payroll.Payslip.ApproveByCEO";
        public const string Payroll_Payslip_Execute = "Payroll.Payslip.Execute";
        public const string Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter = "Payroll.Payslip.DownloadTemplateUpdateRemainLeaveDaysAfter";
        public const string Payroll_Payslip_UpdateRemainLeaveDaysAfter = "Payroll.Payslip.UpdateRemainLeaveDaysAfter";
        public const string Payroll_Payslip_ExportPayrollIncludeLastMonth = "Payroll.Payslip.ExportPayrollIncludeLastMonth";
        public const string Payroll_Payslip_ExportPayroll = "Payroll.Payslip.ExportPayroll";
        public const string Payroll_Payslip_ExportOutsideTech = "Payroll.Payslip.ExportOutsideTech";
        public const string Payroll_Payslip_ExportTechcombank = "Payroll.Payslip.ExportTechcombank";
        public const string Payroll_Payslip_UpdatePayslipDeadline = "Payroll.Payslip.UpdatePayslipDeadline";
        public const string Payroll_Payslip_PayslipDetail = "Payroll.Payslip.PayslipDetail";
        public const string Payroll_Payslip_UpdatePayslipDetail = "Payroll.Payslip.UpdatePayslipDetail";

        public const string Payroll_Payslip_PayslipDetail_TabSalary = "Payroll.Payslip.PayslipDetail.TabSalary";
        public const string Payroll_Payslip_PayslipDetail_TabSalary_View = "Payroll.Payslip.PayslipDetail.TabSalary.View";
        public const string Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary = "Payroll.Payslip.PayslipDetail.TabSalary.CollectAndReCalculateSalary";
        public const string Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary = "Payroll.Payslip.PayslipDetail.TabSalary.ReCalculateSalary";

        public const string Payroll_Payslip_PayslipDetail_TabDebt = "Payroll.Payslip.PayslipDetail.TabDebt";
        public const string Payroll_Payslip_PayslipDetail_TabDebt_View = "Payroll.Payslip.PayslipDetail.TabDebt.View";

        public const string Payroll_Payslip_PayslipDetail_TabBenefit = "Payroll.Payslip.PayslipDetail.TabBenefit";
        public const string Payroll_Payslip_PayslipDetail_TabBenefit_View = "Payroll.Payslip.PayslipDetail.TabBenefit.View";


        public const string Payroll_Payslip_PayslipDetail_TabBonus = "Payroll.Payslip.PayslipDetail.TabBonus";
        public const string Payroll_Payslip_PayslipDetail_TabBonus_View = "Payroll.Payslip.PayslipDetail.TabBonus.View";
        public const string Payroll_Payslip_PayslipDetail_TabBonus_Add = "Payroll.Payslip.PayslipDetail.TabBonus.Add";
        public const string Payroll_Payslip_PayslipDetail_TabBonus_Edit = "Payroll.Payslip.PayslipDetail.TabBonus.Edit";
        public const string Payroll_Payslip_PayslipDetail_TabBonus_Delete = "Payroll.Payslip.PayslipDetail.TabBonus.Delete";

        public const string Payroll_Payslip_PayslipDetail_TabPunishment = "Payroll.Payslip.PayslipDetail.TabPunishment";
        public const string Payroll_Payslip_PayslipDetail_TabPunishment_View = "Payroll.Payslip.PayslipDetail.TabPunishment.View";
        public const string Payroll_Payslip_PayslipDetail_TabPunishment_Add = "Payroll.Payslip.PayslipDetail.TabPunishment.Add";
        public const string Payroll_Payslip_PayslipDetail_TabPunishment_Edit = "Payroll.Payslip.PayslipDetail.TabPunishment.Edit";
        public const string Payroll_Payslip_PayslipDetail_TabPunishment_Delete = "Payroll.Payslip.PayslipDetail.TabPunishment.Delete";

        public const string Payroll_Payslip_PayslipDetail_TabPayslipPreview = "Payroll.Payslip.PayslipDetail.TabPayslipPreview";
        public const string Payroll_Payslip_PayslipDetail_TabPayslipPreview_View = "Payroll.Payslip.PayslipDetail.TabPayslipPreview.View";

        //SalaryChangeRequest
        public const string SalaryChangeRequest = "SalaryChangeRequest";
        public const string SalaryChangeRequest_View = "SalaryChangeRequest.View";
        public const string SalaryChangeRequest_Create = "SalaryChangeRequest.Create";
        public const string SalaryChangeRequest_Edit = "SalaryChangeRequest.Edit";
        public const string SalaryChangeRequest_Delete = "SalaryChangeRequest.Delete";
        public const string SalaryChangeRequest_Send = "SalaryChangeRequest.Send";
        public const string SalaryChangeRequest_Approve = "SalaryChangeRequest.Approve";
        public const string SalaryChangeRequest_Reject = "SalaryChangeRequest.Reject";
        public const string SalaryChangeRequest_Execute = "SalaryChangeRequest.Execute";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail = "SalaryChangeRequest.SalaryChangeRequestDetail";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_View = "SalaryChangeRequest.SalaryChangeRequestDetail.View";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Add = "SalaryChangeRequest.SalaryChangeRequestDetail.Add";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Delete = "SalaryChangeRequest.SalaryChangeRequestDetail.Delete";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Send = "SalaryChangeRequest.SalaryChangeRequestDetail.Send";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Approve = "SalaryChangeRequest.SalaryChangeRequestDetail.Approve";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Reject = "SalaryChangeRequest.SalaryChangeRequestDetail.Reject";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_Execute = "SalaryChangeRequest.SalaryChangeRequestDetail.Execute";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail = "SalaryChangeRequest.SalaryChangeRequestDetail.SenAllMail";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SendMail = "SalaryChangeRequest.SalaryChangeRequestDetail.SendMail";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate = "SalaryChangeRequest.SalaryChangeRequestDetail.DownloadTemplate";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint = "SalaryChangeRequest.SalaryChangeRequestDetail.ImportCheckpoint";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail = "SalaryChangeRequest.SalaryChangeRequestDetail.SalaryChangeRequestEmployeeDetail";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View = "SalaryChangeRequest.SalaryChangeRequestDetail.SalaryChangeRequestEmployeeDetail.View";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit = "SalaryChangeRequest.SalaryChangeRequestDetail.SalaryChangeRequestEmployeeDetail.Edit";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile = "SalaryChangeRequest.SalaryChangeRequestDetail.SalaryChangeRequestEmployeeDetail.UploadContractFile";
        public const string SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile = "SalaryChangeRequest.SalaryChangeRequestDetail.SalaryChangeRequestEmployeeDetail.DeleteContractFile";
        public class GrantPermissionRoles
        {
            public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>()
            {
                {
                    Host.Admin,
                    new List<string>()
                    {
                        PermissionNames.Home,
                        PermissionNames.Admin,

                        PermissionNames.Admin_User,
                        PermissionNames.Admin_User_View,
                        PermissionNames.Admin_User_Create,
                        PermissionNames.Admin_User_Edit,
                        PermissionNames.Admin_User_EditUserRole,
                        PermissionNames.Admin_User_Delete,
                        PermissionNames.Admin_User_ResetPassword,


                        PermissionNames.Admin_Role,
                        PermissionNames.Admin_Role_View,
                        PermissionNames.Admin_Role_Create,
                        PermissionNames.Admin_Role_Edit,
                        PermissionNames.Admin_Role_Delete,


                        PermissionNames.Admin_Tenant,
                        PermissionNames.Admin_Tenant_View,
                        PermissionNames.Admin_Tenant_Create,
                        PermissionNames.Admin_Tenant_Edit,
                        PermissionNames.Admin_Tenant_Delete,

                        PermissionNames.Admin_Configuration,
                        PermissionNames.Admin_Configuration_View,
                        PermissionNames.Admin_Configuration_HRMSetting,
                        PermissionNames.Admin_Configuration_ProjectSetting,
                        PermissionNames.Admin_Configuration_IMSSetting,
                        PermissionNames.Admin_Configuration_FinfastSetting,
                        PermissionNames.Admin_Configuration_TimesheetSetting,
                        PermissionNames.Admin_Configuration_TalentSetting,
                        PermissionNames.Admin_Configuration_LoginSetting,
                        PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo,
                        PermissionNames.Admin_Configuration_HRMSetting_View,
                        PermissionNames.Admin_Configuration_HRMSetting_Edit,
                        PermissionNames.Admin_Configuration_ProjectSetting_View,
                        PermissionNames.Admin_Configuration_IMSSetting_View,
                        PermissionNames.Admin_Configuration_FinfastSetting_View,
                        PermissionNames.Admin_Configuration_TimesheetSetting_View,
                        PermissionNames.Admin_Configuration_TalentSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_Edit,
                        PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View,
                        PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit,


                        PermissionNames.Admin_EmailTemplate,
                        PermissionNames.Admin_EmailTemplate_View,
                        PermissionNames.Admin_EmailTemplate_Edit,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail,

                        PermissionNames.Admin_BackgroundJob,
                        PermissionNames.Admin_BackgroundJob_View,
                        PermissionNames.Admin_BackgroundJob_Delete,
                        PermissionNames.Admin_BackgroundJob_Retry,

                        PermissionNames.Admin_AuditLog,
                        PermissionNames.Admin_AuditLog_View,

                        PermissionNames.Category,

                        PermissionNames.Category_Branch,
                        PermissionNames.Category_Branch_View,
                        PermissionNames.Category_Branch_Create,
                        PermissionNames.Category_Branch_Edit,
                        PermissionNames.Category_Branch_Delete,

                        PermissionNames.Category_Usertype,
                        PermissionNames.Category_Usertype_View,
                    

                        PermissionNames.Category_JobPosition,
                        PermissionNames.Category_JobPosition_View,
                        PermissionNames.Category_JobPosition_Create,
                        PermissionNames.Category_JobPosition_Edit,
                        PermissionNames.Category_JobPosition_Delete,

                        PermissionNames.Category_Level,
                        PermissionNames.Category_Level_View,
                        PermissionNames.Category_Level_Create,
                        PermissionNames.Category_Level_Edit,
                        PermissionNames.Category_Level_Delete,

                        PermissionNames.Category_Skill,
                        PermissionNames.Category_Skill_View,
                        PermissionNames.Category_Skill_Create,
                        PermissionNames.Category_Skill_Edit,
                        PermissionNames.Category_Skill_Delete,

                        PermissionNames.Category_Team,
                        PermissionNames.Category_Team_View,
                        PermissionNames.Category_Team_Create,
                        PermissionNames.Category_Team_Edit,
                        PermissionNames.Category_Team_Delete,


                        PermissionNames.Category_Bank,
                        PermissionNames.Category_Bank_View,
                        PermissionNames.Category_Bank_Create,
                        PermissionNames.Category_Bank_Edit,
                        PermissionNames.Category_Bank_Delete,

                        PermissionNames.Category_PunishmentType,
                        PermissionNames.Category_PunishmentType_View,
                        PermissionNames.Category_PunishmentType_Create,
                        PermissionNames.Category_PunishmentType_Edit,
                        PermissionNames.Category_PunishmentType_Delete,

                        PermissionNames.Category_IssuedBy,
                        PermissionNames.Category_IssuedBy_View,
                        PermissionNames.Category_IssuedBy_Create,
                        PermissionNames.Category_IssuedBy_Edit,
                        PermissionNames.Category_IssuedBy_Delete,

                        PermissionNames.Punishment,
                        PermissionNames.Punishment_View,
                        PermissionNames.Punishment_Create,
                        PermissionNames.Punishment_Generate,
                        PermissionNames.Punishment_Edit,
                        PermissionNames.Punishment_Active,
                        PermissionNames.Punishment_Deactive,
                        PermissionNames.Punishment_Delete,
                        PermissionNames.Punishment_PunishmentDetail,
                        PermissionNames.Punishment_PunishmentDetail_View,
                        PermissionNames.Punishment_PunishmentDetail_AddEmployee,
                        PermissionNames.Punishment_PunishmentDetail_Edit,
                        PermissionNames.Punishment_PunishmentDetail_Import,
                        PermissionNames.Punishment_PunishmentDetail_DownloadTemplate,
                        PermissionNames.Punishment_PunishmentDetail_Delete,


                        PermissionNames.PunishmentFund,
                        PermissionNames.PunishmentFund_View,
                        PermissionNames.PunishmentFund_Add,
                        PermissionNames.PunishmentFund_Disburse,
                        PermissionNames.PunishmentFund_Edit,
                        PermissionNames.PunishmentFund_Delete,


                        PermissionNames.Employee,
                        PermissionNames.Employee_View,
                        PermissionNames.Employee_Create,
                        PermissionNames.Employee_Edit,
                        PermissionNames.Employee_Export,
                        PermissionNames.Employee_Delete,
                        PermissionNames.Employee_UploadAvatar,
                        PermissionNames.Employee_DownloadCreateTemplate,
                        PermissionNames.Employee_DownloadUpdateTemplate,
                        PermissionNames.Employee_CreateEmployeeByFile,
                        PermissionNames.Employee_UpdateEmployeeByFile,
                        PermissionNames.Employee_EmployeeDetail,
                        PermissionNames.Employee_SyncUpdateEmployeesInforToOtherTools,

                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit,

                        PermissionNames.Employee_EmployeeDetail_TabContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_View,
                        PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Delete,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract,


                        PermissionNames.Employee_EmployeeDetail_TabDebt,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_View,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_Add,

                        PermissionNames.Employee_EmployeeDetail_TabBenefit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_View,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Add,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBonus,
                        PermissionNames.Employee_EmployeeDetail_TabBonus_View,

                        PermissionNames.Employee_EmployeeDetail_TabPunishment,
                        PermissionNames.Employee_EmployeeDetail_TabPunishment_View,

                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_ForceDelete,

                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory,
                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View,

                        PermissionNames.WarningEmployee,
                        PermissionNames.WarningEmployee_BackToWork,
                        PermissionNames.WarningEmployee_BackToWork_View,
                        PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate,
                        PermissionNames.WarningEmployee_BackToWork_BackToWork,
                        PermissionNames.WarningEmployee_ContractExpired,
                        PermissionNames.WarningEmployee_ContractExpired_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo,
                        PermissionNames.WarningEmployee_RequestChangeInfo_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject,
                        PermissionNames.WarningEmployee_PlanOnboard,
                        PermissionNames.WarningEmployee_PlanOnboard_View,
                        PermissionNames.WarningEmployee_PlanOnboard_ViewSalary,
                        PermissionNames.WarningEmployee_PlanOnboard_Create,
                        PermissionNames.WarningEmployee_PlanOnboard_Edit,
                        PermissionNames.WarningEmployee_PlanOnboard_Delete,
                        PermissionNames.WarningEmployee_PlanQuitEmployee,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_View,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Edit,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Detele,


                        PermissionNames.Debt,
                        PermissionNames.Debt_View,
                        PermissionNames.Debt_Create,
                        PermissionNames.Debt_Delete,
                        PermissionNames.Debt_DebtDetail,
                        PermissionNames.Debt_DebtDetail_View,
                        PermissionNames.Debt_DebtDetail_Edit,
                        PermissionNames.Debt_DebtDetail_Delete,
                        PermissionNames.Debt_DebtDetail_SetDone,
                        PermissionNames.Debt_DebtDetail_GeneratePaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddPaymentPlan,
                        PermissionNames.Debt_DebtDetail_DeletePaymentPlan,
                        PermissionNames.Debt_DebtDetail_EditPaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_EditDebtPaid,
                        PermissionNames.Debt_DebtDetail_DeleteDebtPaid,

                        PermissionNames.Refund,
                        PermissionNames.Refund_View,
                        PermissionNames.Refund_Create,
                        PermissionNames.Refund_Edit,
                        PermissionNames.Refund_Delete,
                        PermissionNames.Refund_Active,
                        PermissionNames.Refund_Deactive,
                        PermissionNames.Refund_RefundDetail,
                        PermissionNames.Refund_RefundDetail_View,
                        PermissionNames.Refund_RefundDetail_AddEmployee,
                        PermissionNames.Refund_RefundDetail_Edit,
                        PermissionNames.Refund_RefundDetail_Delete,

                        PermissionNames.Bonus,
                        PermissionNames.Bonus_View,
                        PermissionNames.Bonus_Create,
                        PermissionNames.Bonus_Edit,
                        PermissionNames.Bonus_Active,
                        PermissionNames.Bonus_Deactive,
                        PermissionNames.Bonus_Delete,
                        PermissionNames.Bonus_BonusDetail,

                        PermissionNames.Bonus_BonusDetail_TabInformation,
                        PermissionNames.Bonus_BonusDetail_TabInformation_View,
                        PermissionNames.Bonus_BonusDetail_TabInformation_Edit,

                        PermissionNames.Bonus_BonusDetail_TabEmployee,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_View,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Add,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Edit,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Import,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Delete,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Delete,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendAllMail,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendMail,

                        PermissionNames.Benefit,
                        PermissionNames.Benefit_View,
                        PermissionNames.Benefit_Create,
                        PermissionNames.Benefit_Edit,
                        PermissionNames.Benefit_Active,
                        PermissionNames.Benefit_Deactive,
                        PermissionNames.Benefit_Delete,
                        PermissionNames.Benefit_BenefitDetail,

                        PermissionNames.Benefit_BenefitDetail_TabInformation,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_View,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Clone,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Delete,

                        PermissionNames.Benefit_BenefitDetail_TabEmployee,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_View,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Add,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete,

                        PermissionNames.Payroll,
                        PermissionNames.Payroll_View,
                        PermissionNames.Payroll_Create,
                        PermissionNames.Payroll_Edit,
                        PermissionNames.Payroll_Delete,
                        PermissionNames.Payroll_SendToAccountant,
                        PermissionNames.Payroll_ApproveAndSendtToCEO,
                        PermissionNames.Payroll_RejectByKT,
                        PermissionNames.Payroll_ApproveByKT,
                        PermissionNames.Payroll_RejectByCEO,
                        PermissionNames.Payroll_ApproveByCEO,
                        PermissionNames.Payroll_Execute,
                        PermissionNames.Payroll_Payslip,


                        PermissionNames.Payroll_Payslip_View,
                        PermissionNames.Payroll_Payslip_CalculateSalary,
                        PermissionNames.Payroll_Payslip_SendMailAll,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_Export,
                        PermissionNames.Payroll_Payslip_Delete,
                        PermissionNames.Payroll_Payslip_SendMail,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_SendToAccountant,
                        PermissionNames.Payroll_Payslip_ApproveAndSendtToCEO,
                        PermissionNames.Payroll_Payslip_RejectByKT,
                        PermissionNames.Payroll_Payslip_ApproveByKT,
                        PermissionNames.Payroll_Payslip_RejectByCEO,
                        PermissionNames.Payroll_Payslip_ApproveByCEO,
                        PermissionNames.Payroll_Payslip_Execute,
                        PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth,
                        PermissionNames.Payroll_Payslip_ExportPayroll,
                        PermissionNames.Payroll_Payslip_ExportOutsideTech,
                        PermissionNames.Payroll_Payslip_ExportTechcombank,
                        PermissionNames.Payroll_Payslip_UpdatePayslipDeadline,
                        PermissionNames.Payroll_Payslip_PayslipDetail,
                        PermissionNames.Payroll_Payslip_UpdatePayslipDetail,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View,


                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View,

                        PermissionNames.SalaryChangeRequest,
                        PermissionNames.SalaryChangeRequest_View,
                        PermissionNames.SalaryChangeRequest_Create,
                        PermissionNames.SalaryChangeRequest_Edit,
                        PermissionNames.SalaryChangeRequest_Delete,
                        PermissionNames.SalaryChangeRequest_Send,
                        PermissionNames.SalaryChangeRequest_Approve,
                        PermissionNames.SalaryChangeRequest_Reject,
                        PermissionNames.SalaryChangeRequest_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute,
                         PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile,

                    }
                },
                {
                  Tenants.CEO,
                  new List<string>()
                  {
                      PermissionNames.Home,
                        PermissionNames.Admin,

                        PermissionNames.Admin_User,
                        PermissionNames.Admin_User_View,
                        PermissionNames.Admin_User_Create,
                        PermissionNames.Admin_User_Edit,
                        PermissionNames.Admin_User_EditUserRole,
                        PermissionNames.Admin_User_Delete,
                        PermissionNames.Admin_User_ResetPassword,


                        PermissionNames.Admin_Role,
                        PermissionNames.Admin_Role_View,
                        PermissionNames.Admin_Role_Create,
                        PermissionNames.Admin_Role_Edit,
                        PermissionNames.Admin_Role_Delete,


                        PermissionNames.Admin_Tenant,
                        PermissionNames.Admin_Tenant_View,
                        PermissionNames.Admin_Tenant_Create,
                        PermissionNames.Admin_Tenant_Edit,
                        PermissionNames.Admin_Tenant_Delete,

                        PermissionNames.Admin_Configuration,
                        PermissionNames.Admin_Configuration_View,
                        PermissionNames.Admin_Configuration_HRMSetting,
                        PermissionNames.Admin_Configuration_ProjectSetting,
                        PermissionNames.Admin_Configuration_IMSSetting,
                        PermissionNames.Admin_Configuration_FinfastSetting,
                        PermissionNames.Admin_Configuration_TimesheetSetting,
                        PermissionNames.Admin_Configuration_TalentSetting,
                        PermissionNames.Admin_Configuration_LoginSetting,
                        PermissionNames.Admin_Configuration_HRMSetting_View,
                        PermissionNames.Admin_Configuration_HRMSetting_Edit,
                        PermissionNames.Admin_Configuration_ProjectSetting_View,
                        PermissionNames.Admin_Configuration_IMSSetting_View,
                        PermissionNames.Admin_Configuration_FinfastSetting_View,
                        PermissionNames.Admin_Configuration_TimesheetSetting_View,
                        PermissionNames.Admin_Configuration_TalentSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_Edit,
                       


                        PermissionNames.Admin_EmailTemplate,
                        PermissionNames.Admin_EmailTemplate_View,
                        PermissionNames.Admin_EmailTemplate_Edit,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail,

                        PermissionNames.Admin_BackgroundJob,
                        PermissionNames.Admin_BackgroundJob_View,
                        PermissionNames.Admin_BackgroundJob_Delete,
                        PermissionNames.Admin_BackgroundJob_Retry,

                        PermissionNames.Admin_AuditLog,
                        PermissionNames.Admin_AuditLog_View,

                        PermissionNames.Category,

                        PermissionNames.Category_Branch,
                        PermissionNames.Category_Branch_View,
                        PermissionNames.Category_Branch_Create,
                        PermissionNames.Category_Branch_Edit,
                        PermissionNames.Category_Branch_Delete,

                        PermissionNames.Category_Usertype,
                        PermissionNames.Category_Usertype_View,


                        PermissionNames.Category_JobPosition,
                        PermissionNames.Category_JobPosition_View,
                        PermissionNames.Category_JobPosition_Create,
                        PermissionNames.Category_JobPosition_Edit,
                        PermissionNames.Category_JobPosition_Delete,

                        PermissionNames.Category_Level,
                        PermissionNames.Category_Level_View,
                        PermissionNames.Category_Level_Create,
                        PermissionNames.Category_Level_Edit,
                        PermissionNames.Category_Level_Delete,

                        PermissionNames.Category_Skill,
                        PermissionNames.Category_Skill_View,
                        PermissionNames.Category_Skill_Create,
                        PermissionNames.Category_Skill_Edit,
                        PermissionNames.Category_Skill_Delete,

                        PermissionNames.Category_Team,
                        PermissionNames.Category_Team_View,
                        PermissionNames.Category_Team_Create,
                        PermissionNames.Category_Team_Edit,
                        PermissionNames.Category_Team_Delete,


                        PermissionNames.Category_Bank,
                        PermissionNames.Category_Bank_View,
                        PermissionNames.Category_Bank_Create,
                        PermissionNames.Category_Bank_Edit,
                        PermissionNames.Category_Bank_Delete,

                        PermissionNames.Category_PunishmentType,
                        PermissionNames.Category_PunishmentType_View,
                        PermissionNames.Category_PunishmentType_Create,
                        PermissionNames.Category_PunishmentType_Edit,
                        PermissionNames.Category_PunishmentType_Delete,

                        PermissionNames.Punishment,
                        PermissionNames.Punishment_View,
                        PermissionNames.Punishment_Create,
                        PermissionNames.Punishment_Generate,
                        PermissionNames.Punishment_Edit,
                        PermissionNames.Punishment_Active,
                        PermissionNames.Punishment_Deactive,
                        PermissionNames.Punishment_Delete,
                        PermissionNames.Punishment_PunishmentDetail,
                        PermissionNames.Punishment_PunishmentDetail_View,
                        PermissionNames.Punishment_PunishmentDetail_AddEmployee,
                        PermissionNames.Punishment_PunishmentDetail_Edit,
                        PermissionNames.Punishment_PunishmentDetail_Import,
                        PermissionNames.Punishment_PunishmentDetail_DownloadTemplate,
                        PermissionNames.Punishment_PunishmentDetail_Delete,

                        PermissionNames.Employee,
                        PermissionNames.Employee_View,
                        PermissionNames.Employee_Create,
                        PermissionNames.Employee_Edit,
                        PermissionNames.Employee_Export,
                        PermissionNames.Employee_Delete,
                        PermissionNames.Employee_UploadAvatar,
                        PermissionNames.Employee_DownloadCreateTemplate,
                        PermissionNames.Employee_DownloadUpdateTemplate,
                        PermissionNames.Employee_CreateEmployeeByFile,
                        PermissionNames.Employee_UpdateEmployeeByFile,
                        PermissionNames.Employee_EmployeeDetail,

                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit,

                        PermissionNames.Employee_EmployeeDetail_TabContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_View,
                        PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Delete,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract,

                        PermissionNames.Employee_EmployeeDetail_TabDebt,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_View,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_Add,

                        PermissionNames.Employee_EmployeeDetail_TabBenefit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_View,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Add,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBonus,
                        PermissionNames.Employee_EmployeeDetail_TabBonus_View,

                        PermissionNames.Employee_EmployeeDetail_TabPunishment,
                        PermissionNames.Employee_EmployeeDetail_TabPunishment_View,

                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory,
                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View,

                        PermissionNames.WarningEmployee,
                        PermissionNames.WarningEmployee_BackToWork,
                        PermissionNames.WarningEmployee_BackToWork_View,
                        PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate,
                        PermissionNames.WarningEmployee_BackToWork_BackToWork,
                        PermissionNames.WarningEmployee_ContractExpired,
                        PermissionNames.WarningEmployee_ContractExpired_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo,
                        PermissionNames.WarningEmployee_RequestChangeInfo_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject,
                        PermissionNames.WarningEmployee_PlanOnboard,
                        PermissionNames.WarningEmployee_PlanOnboard_View,
                        PermissionNames.WarningEmployee_PlanOnboard_Create,
                        PermissionNames.WarningEmployee_PlanOnboard_Edit,
                        PermissionNames.WarningEmployee_PlanOnboard_Delete,
                        PermissionNames.WarningEmployee_PlanQuitEmployee,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_View,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Edit,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Detele,


                        PermissionNames.Debt,
                        PermissionNames.Debt_View,
                        PermissionNames.Debt_Create,
                        PermissionNames.Debt_Delete,
                        PermissionNames.Debt_DebtDetail,
                        PermissionNames.Debt_DebtDetail_View,
                        PermissionNames.Debt_DebtDetail_Edit,
                        PermissionNames.Debt_DebtDetail_Delete,
                        PermissionNames.Debt_DebtDetail_SetDone,
                        PermissionNames.Debt_DebtDetail_GeneratePaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddPaymentPlan,
                        PermissionNames.Debt_DebtDetail_DeletePaymentPlan,
                        PermissionNames.Debt_DebtDetail_EditPaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_EditDebtPaid,
                        PermissionNames.Debt_DebtDetail_DeleteDebtPaid,

                        PermissionNames.Refund,
                        PermissionNames.Refund_View,
                        PermissionNames.Refund_Create,
                        PermissionNames.Refund_Edit,
                        PermissionNames.Refund_Delete,
                        PermissionNames.Refund_Active,
                        PermissionNames.Refund_Deactive,
                        PermissionNames.Refund_RefundDetail,
                        PermissionNames.Refund_RefundDetail_View,
                        PermissionNames.Refund_RefundDetail_AddEmployee,
                        PermissionNames.Refund_RefundDetail_Edit,
                        PermissionNames.Refund_RefundDetail_Delete,

                        PermissionNames.Bonus,
                        PermissionNames.Bonus_View,
                        PermissionNames.Bonus_Create,
                        PermissionNames.Bonus_Edit,
                        PermissionNames.Bonus_Active,
                        PermissionNames.Bonus_Deactive,
                        PermissionNames.Bonus_Delete,
                        PermissionNames.Bonus_BonusDetail,

                        PermissionNames.Bonus_BonusDetail_TabInformation,
                        PermissionNames.Bonus_BonusDetail_TabInformation_View,
                        PermissionNames.Bonus_BonusDetail_TabInformation_Edit,

                        PermissionNames.Bonus_BonusDetail_TabEmployee,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_View,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Add,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Edit,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Import,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Delete,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate,

                        PermissionNames.Benefit,
                        PermissionNames.Benefit_View,
                        PermissionNames.Benefit_Create,
                        PermissionNames.Benefit_Edit,
                        PermissionNames.Benefit_Active,
                        PermissionNames.Benefit_Deactive,
                        PermissionNames.Benefit_Delete,
                        PermissionNames.Benefit_BenefitDetail,

                        PermissionNames.Benefit_BenefitDetail_TabInformation,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_View,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Clone,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Delete,

                        PermissionNames.Benefit_BenefitDetail_TabEmployee,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_View,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Add,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete,

                        PermissionNames.Payroll,
                        PermissionNames.Payroll_View,
                        PermissionNames.Payroll_Create,
                        PermissionNames.Payroll_Edit,
                        PermissionNames.Payroll_Delete,
                        PermissionNames.Payroll_RejectByCEO,
                        PermissionNames.Payroll_ApproveByCEO,
                        PermissionNames.Payroll_Execute,
                        PermissionNames.Payroll_Payslip,


                        PermissionNames.Payroll_Payslip_View,
                        PermissionNames.Payroll_Payslip_CalculateSalary,
                        PermissionNames.Payroll_Payslip_SendMailAll,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_Export,
                        PermissionNames.Payroll_Payslip_Delete,
                        PermissionNames.Payroll_Payslip_SendMail,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_RejectByCEO,
                        PermissionNames.Payroll_Payslip_ApproveByCEO,
                        PermissionNames.Payroll_Payslip_Execute,
                        PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth,
                        PermissionNames.Payroll_Payslip_ExportPayroll,
                        PermissionNames.Payroll_Payslip_ExportOutsideTech,
                        PermissionNames.Payroll_Payslip_ExportTechcombank,
                        PermissionNames.Payroll_Payslip_UpdatePayslipDeadline,
                        PermissionNames.Payroll_Payslip_PayslipDetail,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View,


                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View,

                        PermissionNames.SalaryChangeRequest,
                        PermissionNames.SalaryChangeRequest_View,
                        PermissionNames.SalaryChangeRequest_Create,
                        PermissionNames.SalaryChangeRequest_Edit,
                        PermissionNames.SalaryChangeRequest_Delete,
                        PermissionNames.SalaryChangeRequest_Send,
                        PermissionNames.SalaryChangeRequest_Approve,
                        PermissionNames.SalaryChangeRequest_Reject,
                        PermissionNames.SalaryChangeRequest_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile,
                  }
                },
                {
                  Tenants.KT,
                  new List<string>()
                  {
                      PermissionNames.Home,
                        PermissionNames.Admin,

                        PermissionNames.Admin_User,
                        PermissionNames.Admin_User_View,
                        PermissionNames.Admin_User_Create,
                        PermissionNames.Admin_User_Edit,
                        PermissionNames.Admin_User_EditUserRole,
                        PermissionNames.Admin_User_Delete,
                        PermissionNames.Admin_User_ResetPassword,


                        PermissionNames.Admin_Role,
                        PermissionNames.Admin_Role_View,
                        PermissionNames.Admin_Role_Create,
                        PermissionNames.Admin_Role_Edit,
                        PermissionNames.Admin_Role_Delete,


                        PermissionNames.Admin_Tenant,
                        PermissionNames.Admin_Tenant_View,
                        PermissionNames.Admin_Tenant_Create,
                        PermissionNames.Admin_Tenant_Edit,
                        PermissionNames.Admin_Tenant_Delete,

                        PermissionNames.Admin_Configuration,
                        PermissionNames.Admin_Configuration_View,
                        PermissionNames.Admin_Configuration_HRMSetting,
                        PermissionNames.Admin_Configuration_ProjectSetting,
                        PermissionNames.Admin_Configuration_IMSSetting,
                        PermissionNames.Admin_Configuration_FinfastSetting,
                        PermissionNames.Admin_Configuration_TimesheetSetting,
                        PermissionNames.Admin_Configuration_TalentSetting,
                        PermissionNames.Admin_Configuration_LoginSetting,
                        PermissionNames.Admin_Configuration_HRMSetting_View,
                        PermissionNames.Admin_Configuration_HRMSetting_Edit,
                        PermissionNames.Admin_Configuration_ProjectSetting_View,
                        PermissionNames.Admin_Configuration_IMSSetting_View,
                        PermissionNames.Admin_Configuration_FinfastSetting_View,
                        PermissionNames.Admin_Configuration_TimesheetSetting_View,
                        PermissionNames.Admin_Configuration_TalentSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_Edit,
                       


                        PermissionNames.Admin_EmailTemplate,
                        PermissionNames.Admin_EmailTemplate_View,
                        PermissionNames.Admin_EmailTemplate_Edit,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail,

                        PermissionNames.Admin_BackgroundJob,
                        PermissionNames.Admin_BackgroundJob_View,
                        PermissionNames.Admin_BackgroundJob_Delete,
                        PermissionNames.Admin_BackgroundJob_Retry,

                        PermissionNames.Admin_AuditLog,
                        PermissionNames.Admin_AuditLog_View,

                        PermissionNames.Category,

                        PermissionNames.Category_Branch,
                        PermissionNames.Category_Branch_View,
                        PermissionNames.Category_Branch_Create,
                        PermissionNames.Category_Branch_Edit,
                        PermissionNames.Category_Branch_Delete,

                        PermissionNames.Category_Usertype,
                        PermissionNames.Category_Usertype_View,


                        PermissionNames.Category_JobPosition,
                        PermissionNames.Category_JobPosition_View,
                        PermissionNames.Category_JobPosition_Create,
                        PermissionNames.Category_JobPosition_Edit,
                        PermissionNames.Category_JobPosition_Delete,

                        PermissionNames.Category_Level,
                        PermissionNames.Category_Level_View,
                        PermissionNames.Category_Level_Create,
                        PermissionNames.Category_Level_Edit,
                        PermissionNames.Category_Level_Delete,

                        PermissionNames.Category_Skill,
                        PermissionNames.Category_Skill_View,
                        PermissionNames.Category_Skill_Create,
                        PermissionNames.Category_Skill_Edit,
                        PermissionNames.Category_Skill_Delete,

                        PermissionNames.Category_Team,
                        PermissionNames.Category_Team_View,
                        PermissionNames.Category_Team_Create,
                        PermissionNames.Category_Team_Edit,
                        PermissionNames.Category_Team_Delete,


                        PermissionNames.Category_Bank,
                        PermissionNames.Category_Bank_View,
                        PermissionNames.Category_Bank_Create,
                        PermissionNames.Category_Bank_Edit,
                        PermissionNames.Category_Bank_Delete,

                        PermissionNames.Category_PunishmentType,
                        PermissionNames.Category_PunishmentType_View,
                        PermissionNames.Category_PunishmentType_Create,
                        PermissionNames.Category_PunishmentType_Edit,
                        PermissionNames.Category_PunishmentType_Delete,

                        PermissionNames.Punishment,
                        PermissionNames.Punishment_View,
                        PermissionNames.Punishment_Create,
                        PermissionNames.Punishment_Generate,
                        PermissionNames.Punishment_Edit,
                        PermissionNames.Punishment_Active,
                        PermissionNames.Punishment_Deactive,
                        PermissionNames.Punishment_Delete,
                        PermissionNames.Punishment_PunishmentDetail,
                        PermissionNames.Punishment_PunishmentDetail_View,
                        PermissionNames.Punishment_PunishmentDetail_AddEmployee,
                        PermissionNames.Punishment_PunishmentDetail_Edit,
                        PermissionNames.Punishment_PunishmentDetail_Import,
                        PermissionNames.Punishment_PunishmentDetail_DownloadTemplate,
                        PermissionNames.Punishment_PunishmentDetail_Delete,

                        PermissionNames.PunishmentFund,
                        PermissionNames.PunishmentFund_View,
                        PermissionNames.PunishmentFund_Add,
                        PermissionNames.PunishmentFund_Disburse,
                        PermissionNames.PunishmentFund_Edit,
                        PermissionNames.PunishmentFund_Delete,

                        PermissionNames.Employee,
                        PermissionNames.Employee_View,
                        PermissionNames.Employee_Create,
                        PermissionNames.Employee_Edit,
                        PermissionNames.Employee_Export,
                        PermissionNames.Employee_Delete,
                        PermissionNames.Employee_UploadAvatar,
                        PermissionNames.Employee_DownloadCreateTemplate,
                        PermissionNames.Employee_DownloadUpdateTemplate,
                        PermissionNames.Employee_CreateEmployeeByFile,
                        PermissionNames.Employee_UpdateEmployeeByFile,
                        PermissionNames.Employee_EmployeeDetail,

                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit,

                        PermissionNames.Employee_EmployeeDetail_TabContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_View,
                        PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Delete,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract,

                        PermissionNames.Employee_EmployeeDetail_TabDebt,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_View,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_Add,

                        PermissionNames.Employee_EmployeeDetail_TabBenefit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_View,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Add,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBonus,
                        PermissionNames.Employee_EmployeeDetail_TabBonus_View,

                        PermissionNames.Employee_EmployeeDetail_TabPunishment,
                        PermissionNames.Employee_EmployeeDetail_TabPunishment_View,

                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory,
                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View,

                        PermissionNames.WarningEmployee,
                        PermissionNames.WarningEmployee_BackToWork,
                        PermissionNames.WarningEmployee_BackToWork_View,
                        PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate,
                        PermissionNames.WarningEmployee_BackToWork_BackToWork,
                        PermissionNames.WarningEmployee_ContractExpired,
                        PermissionNames.WarningEmployee_ContractExpired_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo,
                        PermissionNames.WarningEmployee_RequestChangeInfo_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject,
                        PermissionNames.WarningEmployee_PlanOnboard,
                        PermissionNames.WarningEmployee_PlanOnboard_View,
                        PermissionNames.WarningEmployee_PlanOnboard_Create,
                        PermissionNames.WarningEmployee_PlanOnboard_Edit,
                        PermissionNames.WarningEmployee_PlanOnboard_Delete,
                        PermissionNames.WarningEmployee_PlanQuitEmployee,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_View,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Edit,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Detele,


                        PermissionNames.Debt,
                        PermissionNames.Debt_View,
                        PermissionNames.Debt_Create,
                        PermissionNames.Debt_Delete,
                        PermissionNames.Debt_DebtDetail,
                        PermissionNames.Debt_DebtDetail_View,
                        PermissionNames.Debt_DebtDetail_Edit,
                        PermissionNames.Debt_DebtDetail_Delete,
                        PermissionNames.Debt_DebtDetail_SetDone,
                        PermissionNames.Debt_DebtDetail_GeneratePaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddPaymentPlan,
                        PermissionNames.Debt_DebtDetail_DeletePaymentPlan,
                        PermissionNames.Debt_DebtDetail_EditPaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_EditDebtPaid,
                        PermissionNames.Debt_DebtDetail_DeleteDebtPaid,

                        PermissionNames.Bonus,
                        PermissionNames.Bonus_View,
                        PermissionNames.Bonus_Create,
                        PermissionNames.Bonus_Edit,
                        PermissionNames.Bonus_Active,
                        PermissionNames.Bonus_Deactive,
                        PermissionNames.Bonus_Delete,
                        PermissionNames.Bonus_BonusDetail,

                        PermissionNames.Bonus_BonusDetail_TabInformation,
                        PermissionNames.Bonus_BonusDetail_TabInformation_View,
                        PermissionNames.Bonus_BonusDetail_TabInformation_Edit,

                        PermissionNames.Bonus_BonusDetail_TabEmployee,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_View,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Add,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Edit,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Import,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Delete,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendAllMail,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendMail,


                        PermissionNames.Refund,
                        PermissionNames.Refund_View,
                        PermissionNames.Refund_Create,
                        PermissionNames.Refund_Edit,
                        PermissionNames.Refund_Delete,
                        PermissionNames.Refund_Active,
                        PermissionNames.Refund_Deactive,
                        PermissionNames.Refund_RefundDetail,
                        PermissionNames.Refund_RefundDetail_View,
                        PermissionNames.Refund_RefundDetail_AddEmployee,
                        PermissionNames.Refund_RefundDetail_Edit,
                        PermissionNames.Refund_RefundDetail_Delete,

                        PermissionNames.Benefit,
                        PermissionNames.Benefit_View,
                        PermissionNames.Benefit_Create,
                        PermissionNames.Benefit_Edit,
                        PermissionNames.Benefit_Active,
                        PermissionNames.Benefit_Deactive,
                        PermissionNames.Benefit_Delete,
                        PermissionNames.Benefit_BenefitDetail,

                        PermissionNames.Benefit_BenefitDetail_TabInformation,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_View,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Clone,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Delete,

                        PermissionNames.Benefit_BenefitDetail_TabEmployee,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_View,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Add,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete,

                        PermissionNames.Payroll,
                        PermissionNames.Payroll_View,
                        PermissionNames.Payroll_Create,
                        PermissionNames.Payroll_Edit,
                        PermissionNames.Payroll_Delete,
                        PermissionNames.Payroll_ApproveAndSendtToCEO,
                        PermissionNames.Payroll_RejectByKT,
                        PermissionNames.Payroll_ApproveByKT,
                        PermissionNames.Payroll_Payslip,


                        PermissionNames.Payroll_Payslip_View,
                        PermissionNames.Payroll_Payslip_CalculateSalary,
                        PermissionNames.Payroll_Payslip_SendMailAll,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_Export,
                        PermissionNames.Payroll_Payslip_Delete,
                        PermissionNames.Payroll_Payslip_SendMail,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_ApproveAndSendtToCEO,
                        PermissionNames.Payroll_Payslip_RejectByKT,
                        PermissionNames.Payroll_Payslip_ApproveByKT,
                        PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth,
                        PermissionNames.Payroll_Payslip_ExportPayroll,
                        PermissionNames.Payroll_Payslip_ExportOutsideTech,
                        PermissionNames.Payroll_Payslip_ExportTechcombank,
                        PermissionNames.Payroll_Payslip_UpdatePayslipDeadline,
                        PermissionNames.Payroll_Payslip_PayslipDetail,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View,


                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View,

                        PermissionNames.SalaryChangeRequest,
                        PermissionNames.SalaryChangeRequest_View,
                        PermissionNames.SalaryChangeRequest_Create,
                        PermissionNames.SalaryChangeRequest_Edit,
                        PermissionNames.SalaryChangeRequest_Delete,
                        PermissionNames.SalaryChangeRequest_Send,
                        PermissionNames.SalaryChangeRequest_Approve,
                        PermissionNames.SalaryChangeRequest_Reject,
                        PermissionNames.SalaryChangeRequest_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile,
                  }
                },
                {
                  Tenants.SubKT,
                  new List<string>()
                  {
                      PermissionNames.Home,
                        PermissionNames.Admin,

                        PermissionNames.Admin_User,
                        PermissionNames.Admin_User_View,
                        PermissionNames.Admin_User_Create,
                        PermissionNames.Admin_User_Edit,
                        PermissionNames.Admin_User_EditUserRole,
                        PermissionNames.Admin_User_Delete,
                        PermissionNames.Admin_User_ResetPassword,


                        PermissionNames.Admin_Role,
                        PermissionNames.Admin_Role_View,
                        PermissionNames.Admin_Role_Create,
                        PermissionNames.Admin_Role_Edit,
                        PermissionNames.Admin_Role_Delete,


                        PermissionNames.Admin_Tenant,
                        PermissionNames.Admin_Tenant_View,
                        PermissionNames.Admin_Tenant_Create,
                        PermissionNames.Admin_Tenant_Edit,
                        PermissionNames.Admin_Tenant_Delete,

                        PermissionNames.Admin_Configuration,
                        PermissionNames.Admin_Configuration_View,
                        PermissionNames.Admin_Configuration_HRMSetting,
                        PermissionNames.Admin_Configuration_ProjectSetting,
                        PermissionNames.Admin_Configuration_IMSSetting,
                        PermissionNames.Admin_Configuration_FinfastSetting,
                        PermissionNames.Admin_Configuration_TimesheetSetting,
                        PermissionNames.Admin_Configuration_TalentSetting,
                        PermissionNames.Admin_Configuration_LoginSetting,
                        PermissionNames.Admin_Configuration_HRMSetting_View,
                        PermissionNames.Admin_Configuration_HRMSetting_Edit,
                        PermissionNames.Admin_Configuration_ProjectSetting_View,
                        PermissionNames.Admin_Configuration_IMSSetting_View,
                        PermissionNames.Admin_Configuration_FinfastSetting_View,
                        PermissionNames.Admin_Configuration_TimesheetSetting_View,
                        PermissionNames.Admin_Configuration_TalentSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_View,
                        PermissionNames.Admin_Configuration_LoginSetting_Edit,
                        


                        PermissionNames.Admin_EmailTemplate,
                        PermissionNames.Admin_EmailTemplate_View,
                        PermissionNames.Admin_EmailTemplate_Edit,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate,
                        PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail,

                        PermissionNames.Admin_BackgroundJob,
                        PermissionNames.Admin_BackgroundJob_View,
                        PermissionNames.Admin_BackgroundJob_Delete,
                        PermissionNames.Admin_BackgroundJob_Retry,

                        PermissionNames.Admin_AuditLog,
                        PermissionNames.Admin_AuditLog_View,

                        PermissionNames.Category,

                        PermissionNames.Category_Branch,
                        PermissionNames.Category_Branch_View,
                        PermissionNames.Category_Branch_Create,
                        PermissionNames.Category_Branch_Edit,
                        PermissionNames.Category_Branch_Delete,

                        PermissionNames.Category_Usertype,
                        PermissionNames.Category_Usertype_View,


                        PermissionNames.Category_JobPosition,
                        PermissionNames.Category_JobPosition_View,
                        PermissionNames.Category_JobPosition_Create,
                        PermissionNames.Category_JobPosition_Edit,
                        PermissionNames.Category_JobPosition_Delete,

                        PermissionNames.Category_Level,
                        PermissionNames.Category_Level_View,
                        PermissionNames.Category_Level_Create,
                        PermissionNames.Category_Level_Edit,
                        PermissionNames.Category_Level_Delete,

                        PermissionNames.Category_Skill,
                        PermissionNames.Category_Skill_View,
                        PermissionNames.Category_Skill_Create,
                        PermissionNames.Category_Skill_Edit,
                        PermissionNames.Category_Skill_Delete,

                        PermissionNames.Category_Team,
                        PermissionNames.Category_Team_View,
                        PermissionNames.Category_Team_Create,
                        PermissionNames.Category_Team_Edit,
                        PermissionNames.Category_Team_Delete,


                        PermissionNames.Category_Bank,
                        PermissionNames.Category_Bank_View,
                        PermissionNames.Category_Bank_Create,
                        PermissionNames.Category_Bank_Edit,
                        PermissionNames.Category_Bank_Delete,

                        PermissionNames.Category_PunishmentType,
                        PermissionNames.Category_PunishmentType_View,
                        PermissionNames.Category_PunishmentType_Create,
                        PermissionNames.Category_PunishmentType_Edit,
                        PermissionNames.Category_PunishmentType_Delete,

                        PermissionNames.Punishment,
                        PermissionNames.Punishment_View,
                        PermissionNames.Punishment_Create,
                        PermissionNames.Punishment_Generate,
                        PermissionNames.Punishment_Edit,
                        PermissionNames.Punishment_Active,
                        PermissionNames.Punishment_Deactive,
                        PermissionNames.Punishment_Delete,
                        PermissionNames.Punishment_PunishmentDetail,
                        PermissionNames.Punishment_PunishmentDetail_View,
                        PermissionNames.Punishment_PunishmentDetail_AddEmployee,
                        PermissionNames.Punishment_PunishmentDetail_Edit,
                        PermissionNames.Punishment_PunishmentDetail_Import,
                        PermissionNames.Punishment_PunishmentDetail_DownloadTemplate,
                        PermissionNames.Punishment_PunishmentDetail_Delete,

                        PermissionNames.PunishmentFund,
                        PermissionNames.PunishmentFund_View,
                        PermissionNames.PunishmentFund_Add,
                        PermissionNames.PunishmentFund_Disburse,
                        PermissionNames.PunishmentFund_Edit,
                        PermissionNames.PunishmentFund_Delete,

                        PermissionNames.Employee,
                        PermissionNames.Employee_View,
                        PermissionNames.Employee_Create,
                        PermissionNames.Employee_Edit,
                        PermissionNames.Employee_Export,
                        PermissionNames.Employee_Delete,
                        PermissionNames.Employee_UploadAvatar,
                        PermissionNames.Employee_DownloadCreateTemplate,
                        PermissionNames.Employee_DownloadUpdateTemplate,
                        PermissionNames.Employee_CreateEmployeeByFile,
                        PermissionNames.Employee_UpdateEmployeeByFile,
                        PermissionNames.Employee_EmployeeDetail,

                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,
                        PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit,

                        PermissionNames.Employee_EmployeeDetail_TabContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_View,
                        PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile,
                        PermissionNames.Employee_EmployeeDetail_TabContract_Delete,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract,
                        PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract,

                        PermissionNames.Employee_EmployeeDetail_TabDebt,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_View,
                        PermissionNames.Employee_EmployeeDetail_TabDebt_Add,

                        PermissionNames.Employee_EmployeeDetail_TabBenefit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_View,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Add,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit,
                        PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBonus,
                        PermissionNames.Employee_EmployeeDetail_TabBonus_View,

                        PermissionNames.Employee_EmployeeDetail_TabPunishment,
                        PermissionNames.Employee_EmployeeDetail_TabPunishment_View,

                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate,
                        PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete,


                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote,
                        PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete,

                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory,
                        PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View,

                        PermissionNames.WarningEmployee,
                        PermissionNames.WarningEmployee_BackToWork,
                        PermissionNames.WarningEmployee_BackToWork_View,
                        PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate,
                        PermissionNames.WarningEmployee_BackToWork_BackToWork,
                        PermissionNames.WarningEmployee_ContractExpired,
                        PermissionNames.WarningEmployee_ContractExpired_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo,
                        PermissionNames.WarningEmployee_RequestChangeInfo_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve,
                        PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject,
                        PermissionNames.WarningEmployee_PlanOnboard,
                        PermissionNames.WarningEmployee_PlanOnboard_View,
                        PermissionNames.WarningEmployee_PlanOnboard_Create,
                        PermissionNames.WarningEmployee_PlanOnboard_Edit,
                        PermissionNames.WarningEmployee_PlanOnboard_Delete,
                        PermissionNames.WarningEmployee_PlanQuitEmployee,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_View,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Edit,
                        PermissionNames.WarningEmployee_PlanQuitEmployee_Detele,


                        PermissionNames.Debt,
                        PermissionNames.Debt_View,
                        PermissionNames.Debt_Create,
                        PermissionNames.Debt_Delete,
                        PermissionNames.Debt_DebtDetail,
                        PermissionNames.Debt_DebtDetail_View,
                        PermissionNames.Debt_DebtDetail_Edit,
                        PermissionNames.Debt_DebtDetail_Delete,
                        PermissionNames.Debt_DebtDetail_SetDone,
                        PermissionNames.Debt_DebtDetail_GeneratePaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddPaymentPlan,
                        PermissionNames.Debt_DebtDetail_DeletePaymentPlan,
                        PermissionNames.Debt_DebtDetail_EditPaymentPlan,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_AddDebtPaid,
                        PermissionNames.Debt_DebtDetail_EditDebtPaid,
                        PermissionNames.Debt_DebtDetail_DeleteDebtPaid,

                        PermissionNames.Bonus,
                        PermissionNames.Bonus_View,
                        PermissionNames.Bonus_Create,
                        PermissionNames.Bonus_Edit,
                        PermissionNames.Bonus_Active,
                        PermissionNames.Bonus_Deactive,
                        PermissionNames.Bonus_Delete,
                        PermissionNames.Bonus_BonusDetail,

                        PermissionNames.Bonus_BonusDetail_TabInformation,
                        PermissionNames.Bonus_BonusDetail_TabInformation_View,
                        PermissionNames.Bonus_BonusDetail_TabInformation_Edit,

                        PermissionNames.Bonus_BonusDetail_TabEmployee,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_View,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Add,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Edit,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Import,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_Delete,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendAllMail,
                        PermissionNames.Bonus_BonusDetail_TabEmployee_SendMail,

                        PermissionNames.Refund,
                        PermissionNames.Refund_View,
                        PermissionNames.Refund_Create,
                        PermissionNames.Refund_Edit,
                        PermissionNames.Refund_Delete,
                        PermissionNames.Refund_Active,
                        PermissionNames.Refund_Deactive,
                        PermissionNames.Refund_RefundDetail,
                        PermissionNames.Refund_RefundDetail_View,
                        PermissionNames.Refund_RefundDetail_AddEmployee,
                        PermissionNames.Refund_RefundDetail_Edit,
                        PermissionNames.Refund_RefundDetail_Delete,

                        PermissionNames.Benefit,
                        PermissionNames.Benefit_View,
                        PermissionNames.Benefit_Create,
                        PermissionNames.Benefit_Edit,
                        PermissionNames.Benefit_Active,
                        PermissionNames.Benefit_Deactive,
                        PermissionNames.Benefit_Delete,
                        PermissionNames.Benefit_BenefitDetail,

                        PermissionNames.Benefit_BenefitDetail_TabInformation,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_View,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Clone,
                        PermissionNames.Benefit_BenefitDetail_TabInformation_Delete,

                        PermissionNames.Benefit_BenefitDetail_TabEmployee,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_View,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Add,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate,
                        PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete,

                        PermissionNames.Payroll,
                        PermissionNames.Payroll_View,
                        PermissionNames.Payroll_Create,
                        PermissionNames.Payroll_Edit,
                        PermissionNames.Payroll_Delete,
                        PermissionNames.Payroll_SendToAccountant,
                        PermissionNames.Payroll_Payslip,


                        PermissionNames.Payroll_Payslip_View,
                        PermissionNames.Payroll_Payslip_CalculateSalary,
                        PermissionNames.Payroll_Payslip_SendMailAll,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_Export,
                        PermissionNames.Payroll_Payslip_Delete,
                        PermissionNames.Payroll_Payslip_SendMail,
                        PermissionNames.Payroll_Payslip_Add,
                        PermissionNames.Payroll_Payslip_SendToAccountant,
                        PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter,
                        PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth,
                        PermissionNames.Payroll_Payslip_ExportPayroll,
                        PermissionNames.Payroll_Payslip_ExportOutsideTech,
                        PermissionNames.Payroll_Payslip_ExportTechcombank,
                        PermissionNames.Payroll_Payslip_UpdatePayslipDeadline,
                        PermissionNames.Payroll_Payslip_PayslipDetail,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View,


                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete,

                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview,
                        PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View,

                        PermissionNames.SalaryChangeRequest,
                        PermissionNames.SalaryChangeRequest_View,
                        PermissionNames.SalaryChangeRequest_Create,
                        PermissionNames.SalaryChangeRequest_Edit,
                        PermissionNames.SalaryChangeRequest_Delete,
                        PermissionNames.SalaryChangeRequest_Send,
                        PermissionNames.SalaryChangeRequest_Approve,
                        PermissionNames.SalaryChangeRequest_Reject,
                        PermissionNames.SalaryChangeRequest_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile,
                        PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile,
                  }
                }
            };
        }
        public class SystemPermission
        {
            public string Name { get; set; }
            public MultiTenancySides MultiTenancySides { get; set; }
            public string DisplayName { get; set; }
            public bool IsConfiguration { get; set; }
            public List<SystemPermission> Childrens { get; set; }
            public static List<SystemPermission> ListPermissions = new List<SystemPermission>()
            {
                  new SystemPermission{ Name =  PermissionNames.Home, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Home" },
                  new SystemPermission{ Name =  PermissionNames.Admin ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "User"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit User Info"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_EditUserRole ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit User Role"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Admin_User_ResetPassword ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reset Password"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Role ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Role"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Role_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Role_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Role_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Role_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Tenant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tanent"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Tenant_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Delete,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Configuration"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "HRM Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ProjectSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Project Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_IMSSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "IMS Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_FinfastSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Finfast Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TimesheetSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Timesheet Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TalentSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Talent Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Login Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Worker Update All Employee Info To Other Tool Setting"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ProjectSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_IMSSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_FinfastSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TimesheetSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TalentSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},

                  new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EmailTemplate"},
                  new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Template"},
                  new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_PreviewTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Preview Template"},
                  new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                  new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Background Job"},
                  new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_Retry ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Retry"},
                  new SystemPermission{ Name =  PermissionNames.Admin_AuditLog ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AuditLog"},
                  new SystemPermission{ Name =  PermissionNames.Admin_AuditLog_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Category"},
                  new SystemPermission{ Name =  PermissionNames.Category_Branch ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Branch"},
                  new SystemPermission{ Name =  PermissionNames.Category_Branch_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_Branch_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_Branch_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_Branch_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_Usertype ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Usertype"},
                  new SystemPermission{ Name =  PermissionNames.Category_Usertype_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  

                  new SystemPermission{ Name =  PermissionNames.Category_JobPosition ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "JobPosition"},
                  new SystemPermission{ Name =  PermissionNames.Category_JobPosition_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_Level ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Level"},
                  new SystemPermission{ Name =  PermissionNames.Category_Level_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_Level_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_Level_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_Level_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_Skill ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Skill"},
                  new SystemPermission{ Name =  PermissionNames.Category_Skill_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_Skill_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_Skill_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_Skill_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_Team ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Team"},
                  new SystemPermission{ Name =  PermissionNames.Category_Team_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_Team_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_Team_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_Team_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_Bank ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bank"},
                  new SystemPermission{ Name =  PermissionNames.Category_Bank_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_Bank_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_Bank_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_Bank_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_PunishmentType ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Type"},
                  new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Category_IssuedBy ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "IssuedBy"},
                  new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Punishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Generate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Generate"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Detail"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_AddEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Employee"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Import ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},

                  new SystemPermission{ Name =  PermissionNames.PunishmentFund ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Fund"},
                  new SystemPermission{ Name =  PermissionNames.PunishmentFund_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.PunishmentFund_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.PunishmentFund_Disburse ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Disburse"},
                  new SystemPermission{ Name =  PermissionNames.PunishmentFund_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.PunishmentFund_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Employee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee"},
                  new SystemPermission{ Name =  PermissionNames.Employee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Employee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Employee_Export ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export"},
                  new SystemPermission{ Name =  PermissionNames.Employee_UploadAvatar ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Avatar"},
                  new SystemPermission{ Name =  PermissionNames.Employee_DownloadCreateTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Create Template"},
                  new SystemPermission{ Name =  PermissionNames.Employee_DownloadUpdateTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Down load Update Template"},
                  new SystemPermission{ Name =  PermissionNames.Employee_CreateEmployeeByFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Employee By File"},
                  new SystemPermission{ Name =  PermissionNames.Employee_UpdateEmployeeByFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Employee By File"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee Detail"},
                  new SystemPermission{ Name =  PermissionNames.Employee_SyncUpdateEmployeesInforToOtherTools ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sync all employees to other tools"},


                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Personal Info"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sync to other tool"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ReCreate"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quit job"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Avatar"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Change Branch"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Change Working Status"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Extend Maternity Leave"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Extend Pausing"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Maternity Leave"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Pause"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quit"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Contract"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import Contract File"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditNote"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Contract File"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Training Contract"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Collaborator Contract"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Confidential Contract"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Labor Contract"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Probationary Contract"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Debt"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Benefit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Bonus"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPunishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Punishment"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPunishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Salary History"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_ForceDelete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Force Delete"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Working History"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Date"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Branch History"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPayslipHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Payslip"},
                  new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.WarningEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Warning Employee"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Employee Back Date"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_ContractExpired ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contract Expired"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_ContractExpired_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Temp Employee TS"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Detail request"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Plan Onboard"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_View,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_ViewSalary,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewSalary"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Create,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Edit,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Delete,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Plan Quit Employee"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_View,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_Edit,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_Detele,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},


                  new SystemPermission{ Name =  PermissionNames.Debt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Debt"},
                  new SystemPermission{ Name =  PermissionNames.Debt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Debt_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Debt_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Debt Detail"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_SetDone ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Set Done"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_GeneratePaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Generate Payment Plan"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_AddPaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Payment Plan"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_EditPaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Payment Plan"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_DeletePaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Payment Plan"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_AddDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Debt Paid"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_EditDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Debt Paid"},
                  new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_DeleteDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Debt Paid"},


                  new SystemPermission{ Name =  PermissionNames.Bonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bonus"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bonus Detail"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Information"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},

                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Employee"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quick Add"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Import ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_SendAllMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send All Mail"},

                  new SystemPermission{ Name =  PermissionNames.Benefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Benefit"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Benefit Detail"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Information"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Clone ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Clone"},

                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Employee"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quick Add"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update All Start Date"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update All End Date"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},


                  new SystemPermission{ Name =  PermissionNames.Refund ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Refund"},
                  new SystemPermission{ Name =  PermissionNames.Refund_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Refund_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Refund_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Refund_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Refund_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                  new SystemPermission{ Name =  PermissionNames.Refund_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "DeActive"},
                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Refun Detail"},
                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_AddEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Payroll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payroll"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_SendToAccountant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send To Accountant"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_ApproveAndSendtToCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve And Sendt To CEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_RejectByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By KT"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_ApproveByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By KT"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_RejectByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By CEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_ApproveByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By CEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payslip"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_CalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Calculate Salary"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendMailAll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail All "},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Export ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendToAccountant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send To Accountant"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveAndSendtToCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve And Sendt ToCEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_RejectByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By KT"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By KT"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_RejectByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By CEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By CEO"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template Update Remain Leave Days After"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Remain Leave Days After"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Payroll Include Last Month"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportPayroll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Payroll"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportOutsideTech ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Outside Tech"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportTechcombank ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Techcombank"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdatePayslipDeadline ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Payslip Deadline"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdatePayslipDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Payslip Detail"},


                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payslip Detail"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Salary"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Collect And Re-Calculate Salary"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ReCalculate Salary"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Debt"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Benefit"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Bonus"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Punishment"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Payslip Preview"},
                  new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Send ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request Detail"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send All Mail"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import Checkpoint"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request Employee Detail"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Contract File"},
                  new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Contract File"},
            };

            public static List<SystemPermission> TreePermissions = new List<SystemPermission>()
            {
                new SystemPermission{ Name =  PermissionNames.Home, MultiTenancySides = MultiTenancySides.Host , DisplayName = "Home" },
                new SystemPermission { Name =  PermissionNames.Admin, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin",
                        Childrens = new List<SystemPermission>()
                        {
                            new SystemPermission{ Name =  PermissionNames.Admin_User, MultiTenancySides = MultiTenancySides.Host , DisplayName = "Users",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit User Info"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_EditUserRole ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit User Role"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_User_ResetPassword ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reset Password"},
                                }
                            },
                           new SystemPermission{ Name =  PermissionNames.Admin_Role, MultiTenancySides = MultiTenancySides.Host , DisplayName = "Roles",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Admin_Role_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Role_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Role_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Role_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                           },
                           new SystemPermission{ Name =  PermissionNames.Admin_Tenant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tanent",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Admin_Tenant_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Tenant_Delete,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }


                           },
                           new SystemPermission{ Name =  PermissionNames.Admin_Configuration ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Configuration",
                                Childrens = new List<SystemPermission>()
                               {
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "HRMSetting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_HRMSetting_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                        }
                                    
                                    },
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ProjectSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Project Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_ProjectSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        }

                                    },

                                    
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_IMSSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "IMS Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_IMSSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        }

                                    },
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_FinfastSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Finfast Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_FinfastSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        }

                                    },
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TimesheetSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Timesheet Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TimesheetSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        }

                                    },
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TalentSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Talent Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_TalentSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        }

                                    },
                                    new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Login Setting",
                                        Childrens = new List<SystemPermission>()
                                        {
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                             new SystemPermission{ Name =  PermissionNames.Admin_Configuration_LoginSetting_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                        }

                                    },
                                    new SystemPermission{ Name = PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Worker Update All Employee Info To Other Tool Setting",
                                    
                                        Childrens = new List<SystemPermission>()
                                        {
                                            new SystemPermission{ Name = PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName ="View" },
                                            new SystemPermission{ Name = PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit , MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName ="Edit"}
                                            
                                        }
                                    },
                           
                                }
                           },
                           new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Email Template",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_PreviewTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Preview Template",
                                        Childrens = new List<SystemPermission>()
                                        {
                                            new SystemPermission{ Name =  PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                                        }
                                    
                                    
                                    },
                  
                                }
                           },
                           new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Background Job" ,
                                 Childrens = new List<SystemPermission>()
                                 {
                                    new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    new SystemPermission{ Name =  PermissionNames.Admin_BackgroundJob_Retry ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Retry"},
                                 }
                           },
                           new SystemPermission{ Name =  PermissionNames.Admin_AuditLog ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AuditLog" ,
                                 Childrens = new List<SystemPermission>()
                                 {
                                    new SystemPermission{ Name =  PermissionNames.Admin_AuditLog_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                 }
                           },
                        }
                   },
                new SystemPermission{ Name =  PermissionNames.Category,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Category",
                        Childrens = new List<SystemPermission>()
                        {

                            new SystemPermission{ Name =  PermissionNames.Category_Branch ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Branch",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Branch_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Branch_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Branch_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Branch_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }

                            },
                            new SystemPermission{ Name =  PermissionNames.Category_Usertype ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Usertype",
                                Childrens=new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Usertype_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"}
                                }
                            },
                            new SystemPermission{ Name =  PermissionNames.Category_JobPosition ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "JobPosition",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_JobPosition_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_JobPosition_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },
                            new SystemPermission{ Name =  PermissionNames.Category_Level ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Level" ,
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Level_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Level_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Level_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Level_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },

                            new SystemPermission{ Name =  PermissionNames.Category_Skill ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Skill",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Skill_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Skill_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Skill_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Skill_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },

                            new SystemPermission{ Name =  PermissionNames.Category_Team ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Team",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Team_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Team_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Team_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Team_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }



                            },

                            new SystemPermission{ Name =  PermissionNames.Category_Bank ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bank",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_Bank_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Bank_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Bank_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_Bank_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },

                            new SystemPermission{ Name =  PermissionNames.Category_PunishmentType ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Type",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_PunishmentType_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },

                            new SystemPermission{ Name =  PermissionNames.Category_IssuedBy ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "IssuedBy",
                                Childrens =new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                    new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                    new SystemPermission{ Name =  PermissionNames.Category_IssuedBy_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                }
                            },
                        }






                   },
                new SystemPermission{ Name =  PermissionNames.Punishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment",
                        Childrens = new List<SystemPermission>()
                        {
                            new SystemPermission{ Name =  PermissionNames.Punishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Generate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Generate"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                            new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Detail",
                                Childrens = new List<SystemPermission>()
                                {
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_AddEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Employee"},
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_Import ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                                  new SystemPermission{ Name =  PermissionNames.Punishment_PunishmentDetail_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},
                                }

                            },
                        }
                },
                new SystemPermission{ Name =  PermissionNames.PunishmentFund ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Punishment Fund",
                    Childrens = new List<SystemPermission>()
                    {
                        new SystemPermission{ Name =  PermissionNames.PunishmentFund_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                        new SystemPermission{ Name =  PermissionNames.PunishmentFund_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                        new SystemPermission{ Name =  PermissionNames.PunishmentFund_Disburse ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Disburse"},
                        new SystemPermission{ Name =  PermissionNames.PunishmentFund_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                        new SystemPermission{ Name =  PermissionNames.PunishmentFund_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                    }
                },
                  
                new SystemPermission{ Name =  PermissionNames.Employee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee",
                        Childrens = new List<SystemPermission>()
                        {
                          new SystemPermission{ Name =  PermissionNames.Employee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                          new SystemPermission{ Name =  PermissionNames.Employee_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                          new SystemPermission{ Name =  PermissionNames.Employee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                          new SystemPermission{ Name =  PermissionNames.Employee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                          new SystemPermission{ Name =  PermissionNames.Employee_Export ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export"},
                          new SystemPermission{ Name =  PermissionNames.Employee_UploadAvatar ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Avatar"},
                          new SystemPermission{ Name =  PermissionNames.Employee_DownloadCreateTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Create Template"},
                          new SystemPermission{ Name =  PermissionNames.Employee_DownloadUpdateTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Down load Update Template"},
                          new SystemPermission{ Name =  PermissionNames.Employee_CreateEmployeeByFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create Employee By File"},
                          new SystemPermission{ Name =  PermissionNames.Employee_UpdateEmployeeByFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Employee By File"},
                          new SystemPermission{ Name =  PermissionNames.Employee_SyncUpdateEmployeesInforToOtherTools ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sync employees to other tools"},
                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Employee Detail",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Personal Info",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sync to other tool",
                                          Childrens = new List<SystemPermission>(){
                                            new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ReCreate"},
                                            new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                            new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quit job"},
                                          } 
                                      },

                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Avatar"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Change Branch"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Change Working Status",
                                        Childrens=new List<SystemPermission>()
                                        {
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work"},
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Extend Maternity Leave"},
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Extend Pausing"},
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Maternity Leave"},
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Pause"},
                                          new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quit"},
                                        }
                                      },
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Contract",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import Contract File"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Contract File"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintTrainingContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Training Contract"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Collaborator Contract"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintConfidentialContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Confidential Contract"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintLaborContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Labor Contract"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabContract_PrintProbationaryContract ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print Probationary Contract"},
                                    }

                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Debt",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabDebt_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                    }

                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Benefit",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Bonus",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    }

                                },

                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPunishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Punishment",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPunishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    }

                                },

                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Salary History",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_ForceDelete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Force Delete"},
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Working History",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Date"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                                },

                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Branch History",
                                    Childrens = new List<SystemPermission>()
                                    {
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Note"},
                                      new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPayslipHistory ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Payslip",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    }
                                },
                            }

                          },



                        }



                   },
                new SystemPermission{ Name =  PermissionNames.WarningEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Warning Employee",
                    Childrens = new List<SystemPermission>()
                    {
                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_UpdateEmployeeBackDate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Employee Back Date"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_BackToWork_BackToWork ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Back To Work"},
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_ContractExpired ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contract Expired",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_ContractExpired_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Plan Onboard",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_ViewSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewSalary"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanOnboard_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                            }
                        },

                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Temp Employee TS",
                            Childrens = new List<SystemPermission>()
                            {
                                
                                 new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                 new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Detail request",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_RequestChangeInfo_DetailRequest_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                                    }
                                 
                                 
                                 
                                 },
                                  
                            }
                        },
                        new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Plan Quit Employee",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                new SystemPermission{ Name =  PermissionNames.WarningEmployee_PlanQuitEmployee_Detele ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                            }
                        },

                    }
                },
                new SystemPermission{ Name =  PermissionNames.Debt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Debt",
                    Childrens = new List<SystemPermission>()
                    {
                      new SystemPermission{ Name =  PermissionNames.Debt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                      new SystemPermission{ Name =  PermissionNames.Debt_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                      new SystemPermission{ Name =  PermissionNames.Debt_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                      new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Debt Detail",
                        Childrens =new List<SystemPermission>()
                        {

                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_SetDone ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Set Done"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_GeneratePaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Generate Payment Plan"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_AddPaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Payment Plan"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_EditPaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Payment Plan"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_DeletePaymentPlan ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Payment Plan"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_AddDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add Debt Paid"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_EditDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit Debt Paid"},
                          new SystemPermission{ Name =  PermissionNames.Debt_DebtDetail_DeleteDebtPaid ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Debt Paid"},

                        }
                      },
                    }
                },
                new SystemPermission{ Name =  PermissionNames.Bonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bonus",
                    Childrens = new List<SystemPermission>()
                    {
                      new SystemPermission{ Name =  PermissionNames.Bonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                      new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Bonus Detail",
                        Childrens = new List<SystemPermission>()
                        {
                            new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Information",
                                Childrens = new List<SystemPermission>()
                                {
                                    new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                    new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabInformation_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                }
                            },
                            new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Employee",
                                Childrens = new List<SystemPermission>()
                                {
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quick Add"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Import ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                                  new SystemPermission{ Name =  PermissionNames.Bonus_BonusDetail_TabEmployee_SendAllMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send All Mail"},
                                }

                            },
                        }

                      },
                      

                    }
                },

                new SystemPermission{ Name =  PermissionNames.Refund ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Refund",
                    Childrens = new List<SystemPermission>()
                    {
                      new SystemPermission{ Name =  PermissionNames.Refund_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                      new SystemPermission{ Name =  PermissionNames.Refund_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                      new SystemPermission{ Name =  PermissionNames.Refund_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                      new SystemPermission{ Name =  PermissionNames.Refund_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                      new SystemPermission{ Name =  PermissionNames.Refund_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                      new SystemPermission{ Name =  PermissionNames.Refund_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                      new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Refund Detail",
                        Childrens = new List<SystemPermission>()
                        {
                                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_AddEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                  new SystemPermission{ Name =  PermissionNames.Refund_RefundDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                        }
                      },
                    }
                },


                new SystemPermission{ Name =  PermissionNames.Benefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Benefit",
                        Childrens = new List<SystemPermission>()
                        {
                          new SystemPermission{ Name =  PermissionNames.Benefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_Active ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Active"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_Deactive ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Deactive"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                          new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Benefit Detail",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Information",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabInformation_Clone ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Clone"},
                                    }
                                },
                                new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Employee",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Quick Add"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                        new SystemPermission{ Name =  PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                                },

                            }



                          },


                        }

                      },
                new SystemPermission{ Name =  PermissionNames.Payroll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payroll",
                    Childrens = new List<SystemPermission>()
                    {
                        new SystemPermission{ Name =  PermissionNames.Payroll_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_SendToAccountant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send To Accountant"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_ApproveAndSendtToCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve And Send To CEO"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_RejectByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By KT"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_ApproveByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By KT"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_RejectByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By CEO"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_ApproveByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By CEO"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payslip",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_CalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Calculate Salary"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendMailAll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail All"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Export ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_SendToAccountant ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send To Accountant"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveAndSendtToCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve And Send To CEO"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_RejectByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By KT"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveByKT ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By KT"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_RejectByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject By CEO"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ApproveByCEO ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve By CEO"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template Update Remain Leave Days After"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Remain Leave Days After"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Payroll Include Last Month"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportPayroll ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Payroll"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportOutsideTech ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Outside Tech"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_ExportTechcombank ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Export Techcombank"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdatePayslipDeadline ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Payslip Deadline"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_UpdatePayslipDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Update Payslip Detail"},
                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Payslip Detail",

                                    Childrens = new List<SystemPermission>()
                                    {

                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Salary",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Collect And Re-Calculate Salary"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ReCalculate Salary"},
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Debt",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Benefit",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"}
                                            }
                                        
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Bonus",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Punishment",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Add"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                            }
                                        },
                                        new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Tab Payslip Preview",
                                            Childrens = new List<SystemPermission>()
                                            {
                                                new SystemPermission{ Name =  PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                            }
                                        
                                        },




                                    }   
                                
                                
                                
                                },


                            }
                        }
                    },




                },
                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request",
                    Childrens = new List<SystemPermission>()
                    {
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Create ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Send ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request Detail",
                            Childrens = new List<SystemPermission>()
                            {
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Create"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Send ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Approve ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Approve"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Reject ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Reject"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Execute ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Execute"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send All Mail"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Send Mail"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download Template"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Import Checkpoint"},
                                new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Salary Change Request Employee Detail",
                                    Childrens = new List<SystemPermission>()
                                    {
                                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Edit"},
                                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Upload Contract File"},
                                        new SystemPermission{ Name =  PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile ,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete Contract File"},
                                    }

                                },
                            }


                        },
                    }

                },



              






            };

        }
    }
}

