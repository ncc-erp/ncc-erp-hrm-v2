import { Injectable } from '@angular/core'
import { ApiResponseDto } from '@app/service/model/common.dto'
import { GetEmployeeWorkingHistoryDto } from '@app/service/model/working-history/GetEmployeeWorkingHistoryDto'
import { Observable } from 'rxjs'
import { BaseApiService } from '../base-api.service'

@Injectable({
    providedIn: 'root'
})
export class WorkingHistoryService extends BaseApiService {
    changeUrl() {
        return 'WorkingHistory'
    }

    getWorkingHistoriesOfEmployee(employeeId: number){
        return this.processGet(`GetWorkingHistoriesOfEmployee?employeeId=${employeeId}`)
    }
    
    update(payload: GetEmployeeWorkingHistoryDto): Observable<ApiResponseDto<any>> {
        return this.processPut(`Update`,payload)
    }
}