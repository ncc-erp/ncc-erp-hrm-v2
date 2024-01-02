import { ChartSettingDto } from "./chart-setting.dto";
import { ChartDetailFullDto } from "./chart-detail-settings/chart-detail-full.dto";

export class ChartFullDto extends ChartSettingDto {
  chartDetails: ChartDetailFullDto[];
}
