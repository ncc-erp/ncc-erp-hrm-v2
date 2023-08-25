export interface UpdateRequestEmployeeInfoDto {
    id: number,
    salaryChangeRequestId? : number,
    employeeId: number,
    levelId: number,
    toLevelId: number,
    fromUserType: number,
    toUserType: number,
    jobPositionId: number,
    toJobPositionId: number,
    salary: number,
    toSalary: number,
    applyDate: string,
    note: string,
    type: number,
    hasContract: boolean
}