import { HttpClient } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../../../service/api/base-api.service';
import { PagedRequestDto } from '../../../../shared/paged-listing-component-base';

@Injectable({
  providedIn: 'root'
})
export class TestService extends BaseApiService {
  changeUrl() {
    return "User"
  }

  constructor(injector:Injector) {
    super(injector);
  }

  public getAllUserPaging(payload: PagedRequestDto):Observable<any>{
    let apiString = "GetAllPaging"
    return this.processGetAllPaging(apiString, payload)
  }
}
