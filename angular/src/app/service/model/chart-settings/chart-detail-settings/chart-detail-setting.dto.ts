export class ChartDetailSettingDto {
  id: number;
  chartId: number;
  name: string;
  color: string;
  isActive: number;
  branchIds: number[];
  jobPositionIds: number[];
  levelIds: number[];
  teamIds: number[];
  payslipDetailTypes: number[]; // app enum ESalaryType
  userTypes: number[]; // app enum usertype
  workingStatuses: number[]; // app enum UserStatus
  gender: number[];
}
