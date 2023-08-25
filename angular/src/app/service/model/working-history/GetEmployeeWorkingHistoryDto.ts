export interface GetEmployeeWorkingHistoryDto{
    id: number,
    employeeId: number,
    status: number,
    note: string,
    dateAt: string,
    workingStatusName: string,
    lastModifyTime: string,
    lastModifyUser: string,
}