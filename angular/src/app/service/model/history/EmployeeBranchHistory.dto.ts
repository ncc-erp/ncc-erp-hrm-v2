export interface EmployeeBranchHistory{
    id: number,
    employeeId: number,
    branchId: number,
    note: string,
    dateAt: string,
    branchInfo: {
      name: string,
      color: string
    },
    updatedTime: string,
    updatedUser: string,
    isNotAllowToDelete: boolean
}