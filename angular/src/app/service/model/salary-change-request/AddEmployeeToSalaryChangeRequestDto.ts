export interface AddUpdateEmployeeToSalaryChangeRequestDto {
    id: number,
    salaryChangeRequestId: number,
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
    contractEndDate?: string,
    probationPercentage: number,
    basicSalary: number,
    status: number,
    note: string,
    contractCode: string,
    file: string,
    hasContract: boolean
}

export interface ImportCheckpointDto {
    file: FormData,
    salaryChangeRequestId: number
}
