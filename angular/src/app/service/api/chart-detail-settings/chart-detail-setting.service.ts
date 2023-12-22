import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { Observable } from 'rxjs';
import { ChartDetailSettingDto } from "../../model/chart-detail-settings/chart-detail-setting.dto"
import { ApiResponseDto } from '../../model/common.dto';

@Injectable({
    providedIn: "root"
})
export class ChartDetailSettingService extends BaseApiService {

  changeUrl() {
    return "Chart-detail"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  public getAllChartDetail(): Observable<ApiResponseDto<ChartDetailSettingDto[]>>{
    const any = this.processGet(`GetAll`);
    return any;
  }
}
