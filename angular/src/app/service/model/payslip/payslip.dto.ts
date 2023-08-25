import { NumberInput } from '@angular/cdk/coercion';
import { BankDto } from '../categories/bank.dto';
import { BranchDto } from '../categories/branch.dto';
import { BadgeInfoDto } from '../history/EmployeePayslipHistory.dto';
import { BaseEmployeeDto } from './../../../../shared/dto/user-infoDto';
export interface  PaySlipDto extends  BaseEmployeeDto{
    realSalary: number;
    remainLeaveDays: number;
    toBranch: string;
    toUserType: string;
    toLevel: string;
    toJobPosition: string;
    id: number;
    employeeId: number;
}

export class PayslipDetailDto{
  inputSalary: [
    {
      fromDate: string,
      salary: number,
      note: string
    }
  ];

  calculateResult: {
    normalSalary: number,
    otSalary: number,
    maternityLeaveSalary:number,
    totalBenefit: number,
    totalBonus: number,
    totalPunishment: number,
    totalDebt: number,
    totalRealSalary: number,
    totalRefund:number,
    remainingLeaveHour: number
  };
  parollMonth: string;
  employeeFullName: string;
  standardWorkingDay: number;
  standardOpenTalk: number;
  employeeTotalNoramlDay: number
  leaveDayBefore: number;
  monthlyAddedLeaveDay: number;
  leaveDayAfter: number;
  normalworkingDay: number;
  openTalkCount: number;
  totalDay: number;
  offDay: number;
  otHour: number;
  nonSalaryOffDay: number;
  workAtOfficeOrOnsiteDay: number
  totalStandardDay:number
  employeeTotalDay:number
  totalRealSalary:number
  refundLeaveDay:number
  ConfirmStatus: number
  complainDeadline: string
  complainNote: string
}
export class PayslipDetailByTypeDto{
  id: number;
  note: string;
  payslipId: number;
  money: number;
  type: number;
  updatedUser: string;
  updatedTime: string;
  createMode: boolean
}

export class CreatePayslipDetailDto{
  note: string;
  payslipId: number;
  money: number;
  type: number;
  isProjectCost?: boolean
}

export class CreatePayslipDetailPunishementDto extends CreatePayslipDetailDto
{
  PunishmentId?:number
}

export class CreatePayslipBonusDto extends CreatePayslipDetailDto{
  bonusId: number;
}

export class UpdatePayslipDetailDto{
  note: string;
  money: number;
  id: number;
}

export class SummaryInfomationDto{
  totalSalary:number;
  quantity: number;
  avgSalary: number;
  name: string;
}

export class ReCalculateDto {
  payslipId: number;
}

export interface UpdatePayslipDeadLineDto{
  payslipId:number,
  deadline:string
}

export interface UpdatePayslipInfo{
  id:number,
  remainLeaveDayBefore: number,
  addedLeaveDay: number,
  normalDay: number,
  opentalkCount: number,
  offDay: number,
  otHour: number,
  refundLeaveDay: number,
  remainLeaveDayAfter: number,
  normalSalary: number,
  otSalary: number,
  Salary:number
}


export interface GetPayslipEmployeeDto{
  Id:number;
  FullName: string;
  Email: string;
  Avatar: string;
  Sex: Number;
  BranchInfo: BadgeInfoDto;
  LevelInfo: BadgeInfoDto;
  JobPositionInfo: BadgeInfoDto;
  userTypeInfo: BadgeInfoDto;
  realSalary:number;
  employeeId: number;
}



