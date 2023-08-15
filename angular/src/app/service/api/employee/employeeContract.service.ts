import { Injectable, Injector } from '@angular/core'
import { ApiResponseDto } from '@app/service/model/common.dto'
import { ContractFileDto } from '@app/service/model/employee/employee.dto'
import { Observable } from 'rxjs'
import { BaseApiService } from '../base-api.service'
import { UpdateContractNoteDto } from '@app/service/model/employee/UpdateContracNoteDto'
import { EmployeeContractDto } from '@app/modules/employees/employee-detail/employee-contract/employee-contract.component'
@Injectable({
    providedIn: 'root'
})
export class EmployeeContractService extends BaseApiService {
    changeUrl() {
        return 'EmployeeContract'
    }
    constructor(injector: Injector) {
        super(injector)
    }
    getContractBySalaryRequest(requestEmployeeId: number) {
        return this.processGet(`GetContractBySalaryRequest?requestEmployeeId=${requestEmployeeId}`)
    }
    updateNote(payload: UpdateContractNoteDto) {
        return this.processPut(`UpdateNote`,payload)
    }
    public uploadContractFile(file: File, id: string): Observable<ApiResponseDto<string>> {
        const input = new FormData();
        input.append('file', file);
        input.append('contractId', id);
        return this.processPost('UploadContractFile', input)
    }
    deleteContract(id: number) {
        return this.processDelete(`DeleteContract?id=${id}`)
    }
    public deleleContractFile(id: number): Observable<ApiResponseDto<number>> {
        return this.processPost(`DeleteContractFile?id=${id}`, {})
    }
    public updateEmployeeContract(data:EmployeeContractDto): Observable<ApiResponseDto<any>>{
        return this.processPut(`UpdateEmployeeContract`, data);
    }

    public GetContractTemplate(contractId: number , type : number): Observable<ApiResponseDto<any>>{
        return this.processGet(`GetContractTemplate?contractId=${contractId}&type=${type}`,);
    }

}
