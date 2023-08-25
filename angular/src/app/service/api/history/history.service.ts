import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ApiResponseDto } from "../../model/common.dto";
import { EmployeeBranchHistory } from "../../model/history/EmployeeBranchHistory.dto";
import { EmployeePaySlipHistory } from "../../model/history/EmployeePayslipHistory.dto";
import { EmployeeSalaryHistoryDto } from "../../model/history/EmployeeSalaryHistory.dto";
import { EmployeeWorkingHistoryDto } from "../../model/history/EmployeeWorkingHistory.dto";
import { BaseApiService } from "../base-api.service";
import { UpdateNoteInHistoryDto } from '../../model/history/UpdateNoteInHistory.dto'
import { UpdateDateInHistoryDto } from "@app/modules/employees/employee-detail/working-history/working-history-edit-date/working-history-edit-date.component";
@Injectable({
    providedIn: 'root'
})
export class HistoryService extends BaseApiService {
    changeUrl() {
        return 'History'
    }

    getAllEmployeeBranchHistory(employeeId: number): Observable<ApiResponseDto<EmployeeBranchHistory[]>> {
        return this.processGet(`GetAllEmployeeBranchHistory?employeeId=${employeeId}`)
    }

    getAllEmployeeWorkingHistory(employeeId: number): Observable<ApiResponseDto<EmployeeWorkingHistoryDto[]>> {
        return this.processGet(`GetAllEmployeeWorkingHistory?employeeId=${employeeId}`)
    }

    getAllEmployeeSalaryHistory(employeeId: number): Observable<ApiResponseDto<EmployeeSalaryHistoryDto[]>> {
        return this.processGet(`GetAllEmployeeSalaryHistory?employeeId=${employeeId}`)
    }

    getAllEmployeePayslipHistory(employeeId: number): Observable<ApiResponseDto<EmployeePaySlipHistory[]>> {
        return this.processGet(`GetAllEmployeePayslipHistory?employeeId=${employeeId}`)
    }

    updateBranchHistoryNote(payload: UpdateNoteInHistoryDto) {
        return this.processPut(`UpdateNoteInBranchHistory`, payload)
    }

    updateWorkingHistoryNote(payload: UpdateNoteInHistoryDto) {
        return this.processPut(`UpdateNoteInWorkingHistory`, payload)
    }

    updateSalaryHistoryNote(payload: UpdateNoteInHistoryDto) {
        return this.processPut(`UpdateNoteInSalaryHistory`, payload)
    }

    deleteBranchHistory(branchHistoryId: number): Observable<ApiResponseDto<void>> {
        return this.processDelete(`DeleteBranchHistory?id=${branchHistoryId}`)
    }

    deleteWorkingHistory(workingHistoryId: number, employeeId: number): Observable<ApiResponseDto<void>> {
        return this.processDelete(`DeleteWorkingHistory?id=${workingHistoryId}&employeeId=${employeeId}`)
    }

    deleteSalaryHistory(salaryHistoryId: number): Observable<ApiResponseDto<void>> {
        return this.processDelete(`DeleteSalaryHistory?id=${salaryHistoryId}`)
    }
    updateWorkingHistoryDate(payload: UpdateDateInHistoryDto) {
        return this.processPut(`UpdateDateInWorkingHistory`, payload)
    }
}