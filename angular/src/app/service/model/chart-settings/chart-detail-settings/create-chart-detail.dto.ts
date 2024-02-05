export class CreateChartDetailDto {
    chartId: number;
    name: string;
    color: string;
    branchIds: number[];
    jobPositionIds: number[];
    levelIds: number[];
    teamIds: number[];
    payslipDetailTypes: number[]; // app enum ESalaryType
    userTypes: number[]; // app enum usertype
    workingStatuses: number[]; // app enum UserStatus
    gender: number[];
  }
  