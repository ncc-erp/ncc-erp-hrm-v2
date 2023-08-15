export class PayRollDto{
    id: number;
    applyMonth: string;
    status: number;
    standardOpentalk: number;
    standardWorkingDay: number;
    statusName: string
}

export class CreatePayRollDto{
    year: number;
    month: number;
    standardOpenTalk: number;
}

export interface UpdatePayrollDto{
    id: number;
    applyMonth: string;
    openTalk: number;
    normalWorkingDay: number;
    status: number
}

export interface CalculateResultDto{
    errorList: ErroResultDto[]
    payslipIds:number[]
}

export interface ErroResultDto{
    message : string
    referenceId: number
}
