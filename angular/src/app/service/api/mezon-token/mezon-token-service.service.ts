import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { ApiResponseDto } from '../../model/common.dto';
import { Observable } from 'rxjs';




import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class MezonTokenServiceService extends BaseApiService {
  changeUrl() { 
    return "MezonToken"
  }
  constructor(injector: Injector) { 
    super(injector)
  }


  public deleteMezonToken(id: any): Observable<ApiResponseDto<any>> {
    return this.processDelete(`DeleteMezonToken?id=${id}`);
  }

  public deleteAllPending(): Observable<ApiResponseDto<any>> {
    return this.processDelete(`DeleteAllPending`);
  }

  public editMezonToken(input: any): Observable<ApiResponseDto<any>> {
    return this.processPut(`EditMezonToken`, input);
  }

  public exportMezonToken(request : any): Observable<ApiResponseDto<any>>{
    return this.processPost(`ExportMezonToken`, request);
  }

  public sentToken(input: any): Observable<ApiResponseDto<any>> { 
    return this.processPost(`SentToken`, input);
  }

  public sentAllMezonTokenPending(): Observable<ApiResponseDto<any>> {  
    return this.processPost(`SentAllToken`, null);
  }
}
