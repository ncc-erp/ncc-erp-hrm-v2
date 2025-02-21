
export interface InfoEmployeeDto{
    employeeId: number; 
    fullName: string;
    avatarFullPath: string;
    branchInfo: BadgeInfoDto;
    userTypeInfo: BadgeInfoDto;
    jobPositionInfo: BadgeInfoDto;
    sex : number;
    email: string;
}
export interface BadgeInfoDto{
    name: string;
    color:string;
}
export interface ResultReport{
    applyDate: string;
    salary: number;
}
export interface ReportSalaryDto{
    infoEmployee: InfoEmployeeDto;
    resultReport: ResultReport[];
    totalSalary: number;
}