
export interface BaseEmployeeDto {
    fullName: string;
    email: string;
    sex: number;
    jobPositionId:number;
    team:string;
    teams: EmployeeTeamDto[];
    status:number;
    userTypeInfo: BadgeInfoDto;
    levelInfo: BadgeInfoDto;
    branchInfo: BadgeInfoDto;
    avatar: string;
    jobPositionInfo:BadgeInfoDto;
    avatarFullPath: string;
    userTypeName: string,
    userType: number
}
export interface BadgeInfoDto{
    name: string;
    color:string;
}

export interface EmployeeTeamDto{
    teamId: number,
    teamName: string,
}

export interface EmployeeBreadcrumbInfoDto{
    id: number;
    fullName: string;
    email: string;
    avatar: string;
}
