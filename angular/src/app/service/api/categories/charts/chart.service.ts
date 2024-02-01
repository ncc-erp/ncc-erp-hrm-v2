import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../../base-api.service';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { ChartDto, ChartSelectionDto } from '@app/service/model/chart-settings/chart.dto';

@Injectable({
    providedIn: "root"
})
export class ChartSettingService extends BaseApiService {

  changeUrl() {
    return "Chart"
  }

  public selectionData: ChartSelectionDto

  constructor(injector: Injector) {
    super(injector)
  }

   public active(id: number): Observable<ApiResponseDto<ChartDto>> {
    return this.processPut('Active', id)
   }
   
   public deActive(id: number): Observable<ApiResponseDto<ChartDto>> {
    return this.processPut('DeActive', id);
   }

   public clone(chartId: number): Observable<ApiResponseDto<ChartDto>> {
    return this.processPost('Clone', chartId)
   }

   public getChartSelectionData(): Observable<ApiResponseDto<ChartSelectionDto>> {
    const selectionData = this.processGet('GetChartSelectionData')
      
    selectionData.subscribe((rs) => {
      this.selectionData = rs.result
    })

    return selectionData
  }
}
