import { Injectable, Injector } from "@angular/core";
import { AddUpdateEmployeeToSalaryChangeRequestDto, ImportCheckpointDto } from "@app/service/model/salary-change-request/AddEmployeeToSalaryChangeRequestDto";
import { BaseApiService } from '../base-api.service'
import { UpdateChangeRequestDto } from '../../model/salary-change-request/UpdateChangeRequestDto'
import { ApiResponseDto, MessageResponse } from "@app/service/model/common.dto";
import { GetRequestEmployeeDto } from "@app/service/model/salary-change-request/GetRequestEmployee";
import { Observable } from "rxjs";
import { UpdateRequestEmployeeInfoDto } from "@app/service/model/history/UpdateRequestEmployeeInfoDto";
import { PagedResultDto } from "@shared/paged-listing-component-base";
import { InputGetEmployeeInSalaryRequestDto } from "@app/service/model/salary-change-request/InputGetEmployeeInSalaryRequestDto";
import { SendCheckpointMailToOneEmployeeDto } from "@app/service/model/mail/sendMail.dto";
@Injectable({
    providedIn: 'root'
})
export class SalaryChangeRequestService extends BaseApiService {
    constructor(injector: Injector) {
        super(injector)
    }
    changeUrl() {
        return "SalaryChangeRequest"
    }
    getListDateFromSalaryRequest() {
        return this.processGet("GetListDateFromSalaryRequest")
    }
    getEmployeesNotInRequest(requestId: number) {
        return this.processGet(`GetEmployeeNotInRequest?requestId=${requestId}`)
    }
    addEmployeeToSalaryRequest(payload: AddUpdateEmployeeToSalaryChangeRequestDto) {
        return this.processPost(`AddEmployeeTosalaryRequest`, payload)
    }
    deleteSalaryRequestEmployee(id: number) {
        return this.processDelete(`DeleteSalaryRequestEmployee?id=${id}`)
    }
    getEmployeesInSalaryRequest(id: number, payload: InputGetEmployeeInSalaryRequestDto): Observable<ApiResponseDto<PagedResultDto>> {
        return this.processPost(`GetEmployeesInSalaryRequest?requestId=${id}`, payload)
    }
    getRequestEmployeeById(id: number): Observable<ApiResponseDto<GetRequestEmployeeDto>> {
        return this.processGet(`GetRequestEmployeeById?id=${id}`)
    }
    getEmployeeSalaryHistoryByEmployeeId(id: number, payload) {
        return this.processGetAllPaging(`GetEmployeeSalaryHistoryByEmployeeId?employeeId=${id}`, payload)
    }
    updateRquestEmployee(payload: AddUpdateEmployeeToSalaryChangeRequestDto) {
        return this.processPut(`UpdateSalaryRequestemployee`, payload)
    }
    updateRequestStatus(payload: UpdateChangeRequestDto) {
        return this.processPut(`UpdateRequestStatus`, payload)
    }
    updateRequestEmployeeInfo(payload: UpdateRequestEmployeeInfoDto) {
        return this.processPut(`UpdateRequestEmployeeInfo`, payload)
    }
    ImportCheckpoint(payload: FormData): Observable<ApiResponseDto<MessageResponse>> {
        return this.processPost(`ImportCheckpoint`, payload)
    }
    public sendMail(input : SendCheckpointMailToOneEmployeeDto): Observable<ApiResponseDto<void>>{
        return  this.processPost(`SendMailToOneEmployee`, input);
    }
    
    public sendAllMail(id: number , input: InputGetEmployeeInSalaryRequestDto): Observable<ApiResponseDto<string>> {
        return this.processPost("SendMailToAllEmployee?id="+id, input);
    }
    
    public getCheckpointTemplate(requestId: number):Observable<ApiResponseDto<any>>{
        return this.processGet(`GetCheckpointTemplate?requestId=${requestId}`);
    }

    public getTemplateToImportCheckpoint(){
        return this.processGet(`GetTemplateToImportCheckpoint`);
    }

}
