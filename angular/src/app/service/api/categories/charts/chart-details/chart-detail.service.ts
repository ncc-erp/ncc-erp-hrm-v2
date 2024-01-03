import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "@app/service/api/base-api.service";
import { Observable } from "rxjs";
import { ChartDetailSettingDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { ChartDetailSelectionDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-selection.dto";
import { ApiResponseDto } from "@app/service/model/common.dto";
import {
  PagedRequestDto,
  PagedResultDto,
} from "@shared/paged-listing-component-base";
import { ChartFullDto } from "@app/service/model/chart-settings/chart.dto";

@Injectable({
  providedIn: "root",
})
export class ChartDetailService extends BaseApiService {
  changeUrl() {
    return "ChartDetail";
  }

  public selectionData: ChartDetailSelectionDto

  constructor(injector: Injector) {
    super(injector);
  }

  public getAllDetailsByChartId(
    chartId: number
  ): Observable<ApiResponseDto<ChartFullDto>> {
    const chartFullDetail = this.processGet(`GetAllDetailsByChartId?id=${chartId}`);
    return chartFullDetail;
  }

  public getChartDetailSelectionData(): Observable<ApiResponseDto<ChartDetailSelectionDto>> {
    const selectionData = this.processGet('GetChartDetailSelectionData')
      
    selectionData.subscribe((rs) => {
      this.selectionData = rs.result
    })

    return selectionData
  }

  public active(id: number): Observable<ApiResponseDto<ChartDetailSettingDto>> {
    return this.processPut('Active', id)
   }
   
   public deActive(id: number): Observable<ApiResponseDto<ChartDetailSettingDto>> {
    return this.processPut('DeActive', id);
   }

}
