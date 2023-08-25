import { BadgeInfoDto, BaseEmployeeDto } from "@shared/dto/user-infoDto";

export interface GetEmployeeDto extends BaseEmployeeDto {
    id: number;
    phone: number;
    birthday: string;
    idCard: string;
    issuedOn: string;
    issuedBy: string;
    placeOfPermanent: string
    address: string
    bank: string
    bankAccountNumber: number;
    remainLeaveDay: number;
    salary: number;
    realSalary: number;
    probationPercentage: number;
    taxCode: string;
    insuranceStatus: number;
    selected: boolean;
    updatedTime: string;
    updatedUserId:number;
    seniority: StartWorkingDto;
    startWorkingDate: string;
    countSeniority: string;
    bankId: number;
    levelId: number;
    userType: number,
    teamsName?: string,
    personalEmail:string
}

export interface ContractEmployeeExpiredDto extends GetEmployeeDto{
    contractDayLeft : number;
    contractEndDate: string;
    contractStartDate: string;
}
export interface CreateUpdateEmployeeDto {
    id: number;
    email: string;
    phone: number;
    birthday: string;
    fullName: string;
    idCard: string;
    issuedOn: string;
    issuedBy: string;
    placeOfPermanent: string
    address: string;
    bankAccountNumber: string;
    remainLeaveDay: number;
    salary: number;
    realSalary: number;
    taxCode: string;
    insuranceStatus: number;
    startWorkingDate: string;
    userType: number,
    jobPositionId: number,
    branchId: number,
    teams: number[],
    levelId: number,
    status: number,
    probationPercentage: number,
    avatar: string,
    sex: number,
    bankId: number,
    skills: number[],
    contractStartDate: string,
    contractEndDate:string,
    contractCode: string,
    personalEmail:string
}
export interface StartWorkingDto{
    years: number;
    months: number;
    days: number;
}

export interface AvatarDto{
    file : File,
    employeeId :number
}

export interface ContractFileDto{
    file: File,
    contractId: number
}
export  interface TempEmployeeTsDto{
    id: number;
    employeeId : number;
    email: string;
    phone: number;
    fullName: string;
    sex: number;
    requestStatus: number;
    userTypeInfo: BadgeInfoDto,
    jobPositionInfo: BadgeInfoDto,
    levelInfo: BadgeInfoDto,
    branchInfo : BadgeInfoDto,
    RequestStatusInfo: BadgeInfoDto,
    avatarFullPath: string,
    updatedTime: string,
    updatedUser: string

}
export class GetEmployeeBasicInfo{
    id: number;
    fullName: string;
    email: string;
}
