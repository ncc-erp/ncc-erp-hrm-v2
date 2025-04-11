import { BaseEmployeeDto } from './../../../../shared/dto/user-infoDto';

export interface MezonTokenDto extends  BaseEmployeeDto{
        id: number;
        employeeId: number;
        statusToken: number;
        note : string;
        amount: number;
        sentToEmployeeAt: string;
        referenceId: number;
}