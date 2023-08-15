import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { PagedResultDto } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service'
@Injectable({
    providedIn: 'root'
}) 
export class BackgroundJobsService extends BaseApiService{
    changeUrl() {
        return "BackgroundJob";
    }
    public getAllBackgroundJobs(input): Observable<ApiResponseDto<PagedResultDto>> {
        return this.processGetAllPaging("GetAllPaging", input)
    }
    public retryBackgroundJob(input):Observable<ApiResponseDto<any>>{
        return this.processPut(`RetryBackgroundJob`, input);
    }


}
