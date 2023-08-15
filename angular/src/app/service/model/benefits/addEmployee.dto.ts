export interface AddEmployeeToBenefitDto {
    benefitId: number
    startDate: string
    endDate: string
    listEmployeeId: number[]
}
export interface QuickAddEmployeeDto {
    benefitId: number
    startDate: string
    endDate: string
    employeeId: number
}