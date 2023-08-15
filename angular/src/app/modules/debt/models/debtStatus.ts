import { EDebtStatus } from "@app/service/model/debt/debt.dto"
export interface DebtStatus {
    key: string,
    value: number
}
export const DEBT_STATUS_LIST: DebtStatus[] = [
    {
        key: 'Inprogress',
        value: 1
    },
    {
        key: 'Done',
        value: 2
    }
]

export const DEBT_STATUS_FILTER = [
    ...DEBT_STATUS_LIST,
    {
        key: 'All',
        value: 'all'
    }
]

export const DEBT_STATUS = {
    [EDebtStatus.Inprogress]:{
        key: 'Inprogress',
        value: EDebtStatus.Inprogress,
        color: '#28a745'
    },
    [EDebtStatus.Done]:{
        key: 'Done',
        value: EDebtStatus.Done,
        color: '#dc3545'
    }
}
