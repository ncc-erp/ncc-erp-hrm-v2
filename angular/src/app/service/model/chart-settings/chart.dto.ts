import { ChartDetailFullDto } from "./chart-detail-settings/chart-detail-full.dto";

export class ChartDto {
  id: number;
  name: string;
  chartType: number;
  chartDataType: number;
  timePeriodType: number;
  chartDataTypeName: string;
  chartTypeName: string;
  timePeriodTypeName: string;
  isActive: number;
}


export class ChartFullDto extends ChartDto {
  chartDetails: ChartDetailFullDto[];
}