import { BaseEmployeeDto } from './../../../../shared/dto/user-infoDto';
export interface PunishmentsDto{
    id:number;
    name:string;
    date: string;
    isActive: boolean;
    totalMoney: number;
    employeeCount: number;
    isAbleUpdateNote : boolean;
}

export interface CreateEditPunishmentDto{
    id:number;
    name:string;
    date: string;
    isActive: boolean;
    isAbleUpdateNote : boolean;
    
}
export interface PunishmentEmployeeDto extends  BaseEmployeeDto{
    id:number;
    money: number;
    note: string;
    punishmentId : number;
    employeeId: number;
    createMode?: boolean;
    updateMode?: boolean    
}

export interface UpdateEmployeeInPunishmentDto{
    id:number;
    punishmentId : number;
    money: number;
    note: string;
    employeeId: number;
    createMode?: boolean;
}

export interface DateDto{
    date: string;  
}

export interface DefaulEmployeeFilterPunishmentDto{
    userType: any;
    userLevel: any;
    status: any;
    moneyFrom: any;
    moneyTo: any;
    team: any;
    branch: any;
    gender: any;
    jobPosition: any;

}

export interface GetPunishmentOfEmployeeDto{
    id: number;
    punishmentId: number;
    punishmentName: string;
    money: number;
    note: string;
    applyMonth: string;
    isActive: boolean;
    createMode?: boolean;
    updateMode?: boolean;

}
export interface GeneratePunishmentDto{
    punishmentTypeId: number;
    date: string;
}
