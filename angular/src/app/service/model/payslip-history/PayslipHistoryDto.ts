import {BaseEmployeeDto} from '@shared/dto/user-infoDto'
export interface PayslipHistoryDto extends BaseEmployeeDto{
    id: number,
    basicSalary: number,
    realSalary: number,
    remainHourBefore: number,
    remainHourAfter: number,
    realSalaryThisMonth: number;
    normalSalary: number,
    OTsalary: number,
    benefit: number,
    bonus: number,
    punishment: number,
    debt: number,
    employeeId: number
}
