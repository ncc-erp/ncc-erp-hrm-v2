import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { ApiResponseDto } from '../../model/common.dto';
import { BaseApiService } from '../base-api.service';
import { HomepageEmployeeStatisticDto } from '../../model/homepage/HomepageEmployeeStatistic.dto'
@Injectable({
  providedIn: 'root'
})
export class HomePageService extends BaseApiService {
  changeUrl() {
    return 'Home';
  }
  GetAllWorkingHistory(startDate: string, endDate: string): Observable<ApiResponseDto<HomepageEmployeeStatisticDto[]> > {
    return this.processGet(`GetAllWorkingHistory?startDate=${startDate}&endDate=${endDate}`)
  }
}
