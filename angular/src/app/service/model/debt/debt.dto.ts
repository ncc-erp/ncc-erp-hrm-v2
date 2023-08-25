import { BadgeInfoDto, BaseEmployeeDto } from "@shared/dto/user-infoDto";
import { GetEmployeeDto } from "../employee/employee.dto"
import { GetInputFilterDto } from "../employee/GetEmployeeExcept.dto";

export interface DebtDto {
    id: number,
    fullName: string,
    email: string,
    sex: number,
    jobPosition: string,
    jobPositionId: number,
    userType: string,
    userTypeId: number,
    team: string,
    teamId: number,
    status: EDebtStatus,
    userTypeInfo: BadgeInfoDto,
    levelId: number,
    levelInfo: BadgeInfoDto,
    branchId: number,
    branchInfo: BadgeInfoDto,
    avatar: string,
    employeeId: number,
    interestRate: number,
    money: number,
    startDate: string,
    endDate: string,
    note: string,
    paymentType: EPaymentType,
    debtStatus: EDebtStatus
    interest: number
    totalPaid: number,
    creationTime: string,
    creatorUser:string,
    updatedUser:string,
    updatedTime:string,
}

export enum EPaymentType {
    All = 0,
    Salary = 1,
    Cash = 2
}

export enum EDebtStatus {
    Inprogress = 1,
    Done = 2,
}

export enum EUserStatus{
    DangLam,
    Tamnghi,
    Danghi,
    Nghisinh
}

export interface DebtInputFilterDto extends GetInputFilterDto{
    debtstatusIds: number[],
    paymentTypeIds: number[]
}