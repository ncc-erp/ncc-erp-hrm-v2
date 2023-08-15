export interface EmployeeWorkingHistoryDto{
    id: number,
    employeeId: number,
    status: number,
    note: string,
    dateAt: string,
    updatedTime: string,
    updatedUser: string,
    isNotAllowToDelete: boolean
}