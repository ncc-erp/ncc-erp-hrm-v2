import { BaseEmployeeDto } from './../../../../shared/dto/user-infoDto';
export interface UpdateEmployeeBackDateDto {
    employeeId: number,
    backDate: string
}

export interface TempEmployeeTalentDto {
    id:number,
    nccEmail: string,
    email: string,
    fullName: string,
    jobPositionId: number,
    phone: string,
    dateOfBirth: string,
    onboardDate: string,
    salary: number,
    probationPercentage: number,
    skills: string,
    levelName: string,
    positionName: string,
    userTypeName: string,
    statusName: string,
    genderName: string,
    branchName: string,
    userType: number,
    branchId: number,
    levelId: number,
    sex: number,
    skillStr: string,
    onboardStatus: number
}
export interface RejectChangeInfoDto {
    id: number;
}
export interface UpdateRequestDetailDto {
    id: number;
    employeeId: number;
    phone: string;
    birthday: string;
    bankId: number;
    bankAccountNumber: number;
    taxCode: string;
    address: string;
    idCard: string;
    issuedOn: string;
    issuedBy: string;
    placeOfPermanent: string;
}
export interface GetRequestDetailDto {
    id: number;
    employeeId: number;
    requestStatus: number;
    phone: string;
    birthday: string;
    bankId: number;
    bankAccountNumber: number;
    taxCode: string;
    address: string;
    idCard: string;
    issuedOn: string;
    issuedBy: string;
    placeOfPermanent: string;
    isChangePhone: Boolean,
    isChangeBirthday: Boolean,
    isChangeBankId: Boolean,
    isChangeBankAccountNumber: Boolean,
    isChangeTaxCode: Boolean,
    isChangeAddress: Boolean,
    isChangeIdCard: Boolean,
    isChangeIssuedOn: Boolean,
    isChangeIssuedBy: Boolean,
    isChangePlaceOfPermanent: Boolean
}

export interface PlanQuitEmployeeDto extends BaseEmployeeDto {
    name: string
    workingStatus: string
    dateAt: string
    creationTime: string
    jobId:number
    workingHistoryId: number
    isAbandoned?: boolean
}

export interface UpdateTempEmployeeTalentDto {
    id: number,
    fullName: string,
    nccEmail: string,
    phone: string,
    branchId: number,
    dateOfBirth: string,
    levelId: number,
    userType: number,
    jobPositionId: number,
    skills: string,
    onboardDate: string,
    salary: number,
    probationPercentage: number,
    personalEmail: string
}
