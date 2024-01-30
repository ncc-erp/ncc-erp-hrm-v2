import { KeyValueDto } from "../common.dto";
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
  shareToUsers: KeyValueDto[];
  shareToRoles: KeyValueDto[];
  showPreView: boolean;
}

export class ResultChartDto {
  chartDataType: number;
  circleCharts: ResultCircleChartDto[];
  lineCharts: ResultLineChartDto[];
}
export class ChartFullDto extends ChartDto {
  chartDetails: ChartDetailFullDto[];
}

export class ResultCircleChartDto {
  id: number;
  chartName: string;
  pies: ResultCircleChartDetailDto[];
}

export class ResultCircleChartDetailDto {
  id: number;
  pieName: string;
  color: string;
  data: number;
}

export class ResultLineChartDto {
  id: number;
  chartName: string;
  lines: ResultLineChartDetailDto[];
}

export class ResultLineChartDetailDto {
  id: number;
  lineName: string;
  color: string;
  data: any;
}
export class DisplayLineChartDto {
  id: number;
  name: string;
  chartDataType: number;
  chartDetails: DisplayLineChartDetailDto[];
}

export class DisplayLineChartDetailDto {
  id: number;
  data: number[];
  lineName: string;
  itemStyle: ItemStyleColor;
  type: string;
  smooth: boolean;

  constructor(id: number, data: number[], name: string, itemStyle: ItemStyleColor) {
    this.id = id;
    this.data = data;
    this.lineName = name;
    this.itemStyle = itemStyle;
    this.type = "line";
    this.smooth = true;
  }
}

export class ItemStyleColor {
  color: string;
}

export class DisplayCircleChartDto {
  id: number;
  name: string;
  chartDataType: number;
  chartDetails: DisplayCircleChartDetailDto[];
}

export class DisplayCircleChartDetailDto {
  id: number;
  data: number;
  pieName: string;
  itemStyle: ItemStyleColor;
  constructor(id: number, data: number, name: string, itemStyle: ItemStyleColor) {
    this.id = id;
    this.data = data;
    this.pieName = name;
    this.itemStyle = itemStyle;
  }
}

export class ChartSelectionDto {
  shareToUsers: KeyValueDto[] 
  shareToRoles: KeyValueDto[] 
}

export class CreateChartDto
{
  name: string;
  chartType: number;
  chartDataType: number;
  timePeriodType: number;
}

export class UpdateChartDto extends CreateChartDto
{
  id: number;
  shareToUserIds: number[];
  shareToRoleIds: number[];
  isActive: number;
}
