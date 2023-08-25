import { MailPreviewInfo } from "./mail.dto"

export interface SendMailAllEmployeeDto {
    payrollId: number,
    deadline: string
}

export interface SendMailOneemployeeDto {
    mailContent: MailPreviewInfo,
    deadline: string,
    payslipId: number
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