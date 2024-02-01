import { ChartDetailSelectionDto } from "./chart-detail-selection.dto";

export class ChartDetailFullDto extends ChartDetailSelectionDto {
  id: number;
  chartId: number;
  name: string;
  color: string;
  isActive: number;
  chartDataType?: number;
  isViewOnly: boolean = false;
}
