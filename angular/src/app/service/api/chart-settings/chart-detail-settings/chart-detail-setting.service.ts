import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "../../base-api.service";
import { Observable } from "rxjs";
import { ChartDetailSettingDto } from "../../../model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { ChartDetailSelectionBaseInfo } from "../../../model/chart-settings/chart-detail-settings/chart-detail-selection-base-info.dto";
import { ApiResponseDto } from "../../../model/common.dto";
import {
  PagedRequestDto,
  PagedResultDto,
} from "@shared/paged-listing-component-base";
import { ChartFullDeTailDto } from "@app/service/model/chart-settings/chart-full-detail.dto";

@Injectable({
  providedIn: "root",
})
export class ChartDetailSettingService extends BaseApiService {
  changeUrl() {
    return "ChartDetail";
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public getAllDetailsByChartId(
    chartId: number
  ): Observable<ApiResponseDto<ChartFullDeTailDto>> {
    const chartFullDetail = this.processGet(`GetAllDetailsByChartId?id=${chartId}`);
    return chartFullDetail;
  }

  getChartDetailSelectionData(): Observable<ApiResponseDto<ChartDetailSelectionBaseInfo>> {
    const selectionData = this.processGet('GetChartDetailSelectionData')
    return selectionData
  }
}
