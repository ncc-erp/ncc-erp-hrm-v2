import { PagedRequestDto, PagedResultDto } from './../../../../shared/paged-listing-component-base';
import { ApiResponseDto } from './../../model/common.dto';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';


@Injectable({
  providedIn: 'root'
})
export class ReportService extends BaseApiService {
  changeUrl() {
    return "Report";
  }

  constructor(injector: Injector) {
    super(injector);    
   }              
   
  public GetAllReport(input: any): Observable<ApiResponseDto<any>> {
     return this.processPost(`GetAllReport`,input);
   }

   public ExportReportSalary(input: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`ExportReportSalary`,input);  
   }
}