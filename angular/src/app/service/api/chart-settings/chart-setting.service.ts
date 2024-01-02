import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { ChartSettingDto } from '@app/service/model/chart-settings/chart-setting.dto';

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

   public active(id: number): Observable<ApiResponseDto<ChartSettingDto>> {
    return this.processPut('Active', id)
   }
   
   public deActive(id: number): Observable<ApiResponseDto<ChartSettingDto>> {
    return this.processPut('DeActive', id);
   }
}
