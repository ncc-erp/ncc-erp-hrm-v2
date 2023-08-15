import { ESalaryChangeRequestStatus } from "../salary-change-request/GetSalaryChangeRequestDto"

export interface EmployeeSalaryHistoryDto{
    id: number,
    employeeId: number,
    fromSalary: number,
    toSalary: number,
    fromUserType: number,
    toUserType: number,
    fromLevelId: number,
    fromLevelInfo: {
      name: string,
      color: string
    },
    toLevelId: number,
    toLevelInfo: {
      name: string,
      color: string
    },
    fromUserTypeInfo: {
      name: string,
      color: string
    },
    toUserTypeInfo: {
      name: string,
      color: string
    },
    applyDate: string,
    contractCode: string,
    request: ChangeRequestInfoDto,
    note: string,
    updatedTime: string,
    updatedUser: string,
    isNotAllowToDelete: boolean,
    fromJobPositionId: number,
    toJobPositionId: number,
    type: number,
    typeName: string,
    hasContract: boolean
}

export interface ChangeRequestInfoDto{
  id: number,
  name: string,
  status: ESalaryChangeRequestStatus
}

export type UpdateSalaryHistoryDto = Omit<EmployeeSalaryHistoryDto, "fromUserTypeInfo" | "toUserTypeInfo" | "toLevelInfo" | "request">