import { ChartSettingDto } from "./chart-setting.dto";
import { ChartDetailSettingDto } from "./chart-detail-settings/chart-detail-setting.dto";

export class ChartFullDeTailDto extends ChartSettingDto {
  chartDetails: ChartDetailSettingDto[];
}
