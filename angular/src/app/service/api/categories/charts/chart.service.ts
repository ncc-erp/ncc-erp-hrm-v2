import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../../base-api.service';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { ChartDto } from '@app/service/model/chart-settings/chart.dto';

@Injectable({
    providedIn: "root"
})
export class ChartSettingService extends BaseApiService {

  changeUrl() {
    return "Chart"
  }

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
}
