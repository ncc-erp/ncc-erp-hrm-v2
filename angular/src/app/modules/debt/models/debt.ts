import { EDebtStatus } from "@app/service/model/debt/debt.dto"
import { EPaymentType } from "./paymentType"

export interface DebtPlan {
    id: number,
    debtId: number,
    money: number,
    updatedAt: {
        by: string,
        at: string,
    },
    note: string,
    date: string,
    isAllowEdit: boolean,
    isEdit: boolean,
    displayDate: string,
    creatorUser: string,
    creationTime:string,
    updatedUser: string,
    updatedTime: string,
    paymentType: EPaymentType
}

export interface DebtPaid{
    id: number,
    debtId: number,
    money: number,
    paymentType: number,
    updatedAt: {
        by: string,
        at: string,
    },
    note: string,
    date: string,
    isAllowEdit: boolean,
    isEdit: boolean,
    creatorUser: string,
    creationTime:string,
    updatedUser: string,
    updatedTime: string,
    userSalaryId: number,
}

export interface DebtCreateDto{
    id: number,
    employeeId: number,
    interestRate: number,
    money: number,
    startDate: string,
    endDate: string,
    note: string,
    paymentType: EPaymentType,
    debtStatus: EDebtStatus,
    interest: number
    fullName?: string,
}
