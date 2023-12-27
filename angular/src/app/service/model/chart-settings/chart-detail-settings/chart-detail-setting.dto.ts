export class ChartDetailSettingDto {
  id: number;
  chartId: number;
  name: string;
  color: string;
  isActive: number;
  jobPositionIds: number[];
  levelIds: number[];
  branchIds: number[];
  teamIds: number[];
  userTypes: number[]; // app enum usertype
  payslipDetailTypes: number[]; // app enum ESalaryType
  workingStatuses: number[]; // app enum UserStatus
}
