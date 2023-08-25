export interface EmployeePaySlipHistory{
    id: number,
    employeeId: number,
    realSalary: number,
    normalSalary: number,
    remainLeaveHour: number,
    branchId: number,
    branchInfo:BadgeInfoDto,
    payslipBranchInfo : BadgeInfoDto,
    payslipLevelInfo: BadgeInfoDto,
    payslipJobPositionInfo: BadgeInfoDto,
    userTypeInfo: BadgeInfoDto,
    jobPositionId: number,
    jobPositionInfo: BadgeInfoDto,
    levelId: number,
    levelInfo: BadgeInfoDto,
    otSalary: number,
    benefit: number,
    bonus: number,
    punishment: number,
    debt: number,
    updatedTime: string,
    updatedUser: string,
    applyMonth: string,
    remainLeaveDayBefore: number,
    remainLeaveDayAfter: number,
    standardSalary: StandardSalaryDto,
    payrollInfo: PayrollInfoDto
}

export interface StandardSalaryDto{
  salary : number,
  date: string
}
export interface BadgeInfoDto{
  name: string,
  color: string
}

export interface PayrollInfoDto {
  payrollId: number,
  status: number
}