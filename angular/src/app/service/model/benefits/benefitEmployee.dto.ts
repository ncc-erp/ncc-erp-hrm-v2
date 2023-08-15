import { BaseEmployeeDto } from '../../../../shared/dto/user-infoDto';
import { GetInputFilterDto } from '../employee/GetEmployeeExcept.dto';
export interface BenefitEmployeeDto extends BaseEmployeeDto {
    id: number
    employeeId: number
    benefitId: number
    startDate: string
    endDate: string
    createMode: boolean
    updatedUserName: string
    updatedtime: string
    workingStatus: {
        status: number,
        dateAt: string
    }
}

export interface BEmployeeDefaultFilterDto{
    branch: number
    userLevel: number
    status: number
    team: number
    gender: number
    userType: number
    jobPosition: number
    applyMonth: number}

export interface BenefitOfEmployeeDto{
    id: number,
    benefitName: string,
    benefitId: number,
    benefitType: number,
    money: number,
    startDate: string,
    endDate: string,
    status: boolean,
    isEdit: boolean,
    isSelect?: boolean
  }
export enum updateDateType {
    startDate = 1,
    endDate = 2
}