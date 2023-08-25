import { Injectable} from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service'
@Injectable({
    providedIn: 'root'
})
export class AuditLogsService extends BaseApiService{
    changeUrl() {
        return "AuditLog";
    }
    public getAllAuditLogs(input): Observable<any> {
        return this.processGetAllPaging("GetAllPagging", input)
      }

    public getAllEmailAddressInAuditLog(): Observable<any> {
      return this.processGet("GetAllEmailAddressInAuditLog");
    }
}
