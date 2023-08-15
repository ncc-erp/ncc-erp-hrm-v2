import { BadgeInfoDto } from "../history/EmployeePayslipHistory.dto";

export class HomepageEmployeeStatisticDto {
    branchName: string;
    employeeTotal: number;
    internCount: number;
    staffCount: number;
    ctvCount: number;
    tViecCount: number;
    onboardTotal: number;
    onboardInternCount: number;
    onboardStaffCount: number;
    quitJobTotal: number;
    quitJobInternCount: number;
    quitJobStaffCount: number;
    pausingCount: number;
    maternityLeaveCount: number;
    onboardEmployees: LastEmployeeWorkingHistoryDto[];
    quitEmployees: LastEmployeeWorkingHistoryDto[];
    pausingEmployees: LastEmployeeWorkingHistoryDto[];
    matenityLeaveEmployees: LastEmployeeWorkingHistoryDto[];
}
export class LastEmployeeWorkingHistoryDto {
    dateAt: string;
    lastStatus: string;
    employeeId: string;
    branchId: string;
    fullName: string;
    email: string;
    avatar: string;
    sex: number;
    branchInfo: BadgeInfoDto;
    userTypeInfo: BadgeInfoDto;
    jobPositionInfo: BadgeInfoDto;
    levelInfo: BadgeInfoDto;
    userType: number;
}
