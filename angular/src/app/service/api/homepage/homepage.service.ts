import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { ApiResponseDto } from '../../model/common.dto';
import { BaseApiService } from '../base-api.service';
import { HomepageEmployeeStatisticDto, EmployeeDataFromChartDetailDto } from '../../model/homepage/HomepageEmployeeStatistic.dto'
import { DisplayCircleChartDto,ResultCircleChartDto,ResultChartDto,DisplayLineChartDto } from '@app/service/model/chart-settings/chart.dto';

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
  GetDataEmployeeCharts(startDate:string, endDate:string, chartIds:number[]):Observable<ApiResponseDto<ResultChartDto>>{
    return this.processPost(`GetDataEmployeeCharts`, { startDate ,endDate,chartIds})
  }
  GetDataPayslipCharts(startDate:string, endDate:string, chartIds :number[]):Observable<ApiResponseDto<ResultChartDto>>{
    return this.processPost(`GetDataPayslipCharts`, { startDate ,endDate,chartIds})
  }
  GetAllDataEmployeeCharts(startDate:string, endDate:string,):Observable<ApiResponseDto<ResultChartDto>>{
    return this.processPost(`GetAllDataEmployeeCharts`, { startDate ,endDate})
  }
  GetAllDataPayslipCharts(startDate:string, endDate:string,):Observable<ApiResponseDto<ResultChartDto>>{
    return this.processPost(`GetAllDataPayslipCharts`, { startDate ,endDate})
  }
  GetDetailDataChart(payload: any):Observable<ApiResponseDto<EmployeeDataFromChartDetailDto[]>>{
    return this.processPost(`GetDetailDataChart`, payload)
  }
  ExportOnboardQuitEmployees(payload: any):Observable<ApiResponseDto<any>> {
    return this.processPost(`ExportOnboardQuitEmployees`, payload)
  }
  // getAllActive(startDate:string, endDate:string):Observable<ApiResponseDto<ResultChartDto[]>>{
  //   return this.processPost(`GetAllActiveCharts`, { startDate ,endDate})
  // }
}
