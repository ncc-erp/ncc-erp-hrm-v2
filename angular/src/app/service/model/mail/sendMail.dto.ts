import { PayslipDetailDebtComponent } from './../../../modules/salaries/payslip-detail/payslip-detail-debt/payslip-detail-debt.component';
import { MailPreviewInfo, MezonPreviewInfo } from "./mail.dto"

export interface SendMailAllEmployeeDto {
    payrollId: number,
    deadline: string
}

export interface SendMailOneemployeeDto {
    mailContent: MailPreviewInfo,
    deadline: string,
    payslipId: number
}
export interface SendDirectMessageToUserDto{
    mezonContent : MezonPreviewInfo,
    deadline: string,
    payslipId: number,
}
export interface SendDebtMailToOneEmployeeDto{
    mailContent: MailPreviewInfo,
    debtId: number

}
export interface SendBonusMailToOneEmployeeDto{
    mailContent: MailPreviewInfo,
    bonusEmployeeId: number

}
export interface SendCheckpointMailToOneEmployeeDto{
    mailContent: MailPreviewInfo,
    requestId: number
}