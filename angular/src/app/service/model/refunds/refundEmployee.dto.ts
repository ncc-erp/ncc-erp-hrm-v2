export interface RefundEmployeeDto {
    id: number,
    employeeId: number,
    refundId: number,
    money: number,
    note: string,
    createMode?: boolean,
    updateMode?: boolean,
    fullName: string,
    email: string,
    sex: number,
    status: number,
    userType: number,
    levelId: number,
    branchId: number,
    jobPositionId: number,
    avatar: string,
    skills: any,
    teams: any,
    avatarFullPath: string,
    userTypeName: string
}

export interface AddEmployeeToRefundDto {
    employeeId: number,
    refundId: number,
    money: number,
    note: string
}

export interface UpdateRefundEmployeeDto {
    id: number,
    refundId: number,
    employeeId: number,
    money: number,
    note: string
}
