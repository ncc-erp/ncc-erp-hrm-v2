export interface TeamDto {
    id: number,
    name: string,
    employeeCount:number
}

export interface AddEmployeesToTeamDto{
    teamId:number,
    employeeIds:number[]
}
