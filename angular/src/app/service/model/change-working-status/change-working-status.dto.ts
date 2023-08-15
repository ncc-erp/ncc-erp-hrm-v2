import {BenefitOfEmployeeDto} from "../benefits/benefitEmployee.dto"
export interface ChangeStatusToQuitDto{
    employeeId: number;
    toStatus:number;
    applyDate : string;
    note: string;
    listCurrentBenefits: BenefitOfEmployeeDto[];
    isConfirmed: boolean;
}

export interface ChangeStatusToPauseDto{
    employeeId: number;
    toStatus:number;
    applyDate : string;
    backDate : string;
    note: string;
    listCurrentBenefits: BenefitOfEmployeeDto[];
    isConfirmed: boolean;
}
export interface ChangeStatusToMaternityLeaveDto{
    employeeId: number;
    toStatus:number;
    applyDate : string;
    backDate : string;
    note: string;
    toSalary: number;
    basicSalary: number;
    listCurrentBenefits: BenefitOfEmployeeDto[];
    isConfirmed: boolean;
}

export interface ExtendWorkingStatusDto{
    employeeId: number;
    toStatus:number;
    backDate : string;
    note: string;
    listCurrentBenefits: BenefitOfEmployeeDto[];
    isConfirmed: boolean;
}

export interface ChangeStatusWorkingDto{
    employeeId: number;
    toStatus:number;
    toUserType: number;
    toLevelId: number;
    toJobPositionId: number;
    probationPercentage: number;
    hasContract: boolean;
    realSalary: number;
    basicSalary: number;
    applyDate : string;
    note: string;
    contractEndDate: string;
    listCurrentBenefits: BenefitOfEmployeeDto[];
    isConfirmed: boolean;

}

export interface listCurrentBenefitsOfEmployee{
    employeeId : number;
    benefitId: number;
    endDate?: string;
    startDate: string;
}

export interface lastestEmployeeInfoDto{
    employeeId: number;
    toStatus:number;
    toUserType: number;
    toLevelId: number;
    toJobPositionId: number;
    probationPercentage: number;
    hasContract: boolean;
    realSalary: number;
    basicSalary: number;
    applyDate : string;
    note: string;
    contractEndDate: string;
}
  