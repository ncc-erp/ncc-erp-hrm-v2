import { BaseEmployeeDto } from '../../../../shared/dto/user-infoDto';
export interface IBonusDto {
    id:number
    name: string;
}

export interface BonusDto extends IBonusDto{
    applyMonth: string;
    isActive: boolean;
    totalMoney: number;
    employeeCouunt: number;
}

export interface EditBonusDto  extends IBonusDto{
    isApply: boolean;
    isActive: boolean;
    applyMonth?: string;
}
export interface GetBonusDetailDto  extends BonusDto{
    creationTime: string;
    lastModificationTime: string;
    fullNameCreation: string;
    fullNameModification: string;
}


export interface BonusEmployeeDto extends  BaseEmployeeDto{
    id:number;
    money: number;
    note: string;
    bonusId : number;
    employeeId: number;
    createMode?: boolean;
    lastModificationTime: string;
    fullNameModification: string;
}

export interface AddBonusEmployeeDto{
    id?:number;
    money: number;
    note: string;
    bonusId : number;
    employeeIds: any;
}

export interface AddBonusForEmployeeDto{
    money: number;
    note: string;
    bonusId : number;
    employeeId: number;
}

export interface EditBonusEmployeeDto{
    id:number;
    money: number;
    note: string;
    bonusId : number;
}

export interface EmployeeBonusDetailDto{
    id:number;
    bonusId:number;
    money: number;
    note: string;
    bonusName: string;
    applyMonth: string;
    isActive: boolean;
    createMode?: boolean;
}
