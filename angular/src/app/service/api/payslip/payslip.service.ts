import { Injectable, Injector, Input } from '@angular/core';
import { ApiResponseDto, FileBase64Dto } from '@app/service/model/common.dto';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { from, Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service'
import { AddEmployeeToPayroll } from '@app/service/model/payslip/AddEmployeeToPayroll';
import { GetSalaryDetailDto } from '@app/service/model/payslip/GetSalaryDetailDto';
import { CollectPayslipDto } from '@app/service/model/payslip/CollectPayslipDto'
import { CreatePayslipDetailDto, CreatePayslipBonusDto, GetPayslipEmployeeDto, ReCalculateDto, UpdatePayslipDeadLineDto, UpdatePayslipDetailDto, UpdatePayslipInfo } from '@app/service/model/payslip/payslip.dto';
import { MailPreviewInfo } from '@app/service/model/mail/mail.dto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { SendMailAllEmployeeDto, SendMailOneemployeeDto } from '@app/service/model/mail/sendMail.dto';
@Injectable({
  providedIn: 'root'
})
export class PayslipService extends BaseApiService {
  changeUrl() {
    return "Payslip"
  }

  constructor(injector: Injector) {
    super(injector);
  }

  public collectPayslip(payload: CollectPayslipDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`GenerateAllPayslip`, payload);
  }

  public getPayslipEmployeePaging(payrollId: number, input: GetInputFilterDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`GetPayslipEmployeePaging?payrollId=${payrollId}`, input);
  }


  public getPayslipDetail(payslipId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetPayslipDetail?id=${payslipId}`);
  }

  public getPayslipResult(payslipId: number): Observable<ApiResponseDto<GetSalaryDetailDto[]>> {
    return this.processGet(`GetPayslipResult?payslippId=${payslipId}`)
  }

  public AddEmployeeToPayroll(payload: AddEmployeeToPayroll): Observable<ApiResponseDto<any>> {
    return this.processPost(`AddEmployeesToPayRoll`, payload)
  }

  public CalculateSalaryForAllPayslip(payload: CollectPayslipDto): Observable<ApiResponseDto<void>> {
    return this.processPost(`GenerateAllPayslip`, payload)
  }

  public CalculateSalaryForOnePayslip(payload: CollectPayslipDto): Observable<ApiResponseDto<void>> {
    return this.processPost(`GenerateAllPayslip`, payload)
  }

  public GetEmployeeIdsInPayroll(id: number): Observable<ApiResponseDto<number[]>> {
    return this.processGet(`GetEmployeeIdsInPayroll?id=${id}`)
  }

  public GetPayslipDetailByType(payslipId: number, type: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetPayslipDetailByType?payslipId=${payslipId}&type=${type}`);
  }
  
  public getAvailableBonuses(payslipId: number): Observable<ApiResponseDto<any>>{
    return this.processGet(`GetAvailableBonuses?payslipId=${payslipId}`)
  }

  public CreatePayslipDetailPunishment(input: CreatePayslipDetailDto): Observable<ApiResponseDto<CreatePayslipDetailDto>> {
    return this.processPost(`CreatePayslipDetailPunisment`, input)
  }
//////////////////
  public CreatePayslipDetailPunishmentAndCreateEmployeePunishment(input: CreatePayslipDetailDto): Observable<ApiResponseDto<string>> {
    return this.processPost(`CreatePayslipDetailPunishmentAndCreateEmployeePunishment`, input)
  }

  public CreatePayslipDetailBonus(input: CreatePayslipBonusDto): Observable<ApiResponseDto<string>> {
    return this.processPost(`CreatePayslipDetailBonus`, input)
  }

  public UpdatePayslipDetailPunishment(input: UpdatePayslipDetailDto): Observable<ApiResponseDto<string>> {
    return this.processPut(`UpdatePayslipDetailPunishment`, input)
  }

  public UpdatePayslipDetailBonus(input: UpdatePayslipDetailDto): Observable<ApiResponseDto<string>> {
    return this.processPut(`UpdatePayslipDetailBonus`, input)
  }

  public DeletePayslipDetail(id: number): Observable<ApiResponseDto<number>> {
    return this.processDelete(`DeletePayslipDetail?id=${id}`);
  }
  public GetSumaryInfomation(payrollId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetSumaryInfomation?payrollId=${payrollId}`);
  }

  public ReCalculatePayslip(recalculatePayslipDto: ReCalculateDto): Observable<ApiResponseDto<any>> {
    return this.processPut(`ReCalculate`, recalculatePayslipDto);
  }

  public ReGeneratePayslip(payslipId: number): Observable<ApiResponseDto<any>> {
    return this.processPost(`ReGeneratePayslip?payslipId=${payslipId}`, null)
  }
  public getEmailTemplate(payslipId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetEmailTemplate?payslippId=${payslipId}`)
  }

  public sendMailToOneEmployee(input: SendMailOneemployeeDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendMailToOneEmployee`, input)
  }

  public sendMailToAllEmployee(input: SendMailAllEmployeeDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendMailToAllEmployee`, input)
  }

  public updatePayslipDeadline(input: UpdatePayslipDeadLineDto): Observable<ApiResponseDto<UpdatePayslipDeadLineDto>> {
    return this.processPut(`UpdatePayslipDeadline`, input)
  }

  public exportTechcombank(payrollId:number): Observable<ApiResponseDto<FileBase64Dto>> {
    return this.processGet(`ExportTechcombank?payrollId=${payrollId}`)
  }

  public exportOutsideTech(payrollId:number): Observable<ApiResponseDto<FileBase64Dto>> {
    return this.processGet(`exportOutsideTech?payrollId=${payrollId}`)
  }

  public exportPayroll(payrollId: number, input: GetInputFilterDto): Observable<ApiResponseDto<FileBase64Dto>> {
    return this.processPost(`ExportPayroll?payrollId=${payrollId}`, input);

  }

  public ExportPayrollIncludeLastMonth(payrollId: number): Observable<ApiResponseDto<FileBase64Dto>> {
    return this.processGet(`ExportPayrollIncludeLastMonth?payrollId=${payrollId}`);
  }

  public UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary(input: FormData): Observable<any>{
    return this.processPost(`UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary`, input);
  }

  public UpdatePayslipDetail(input:UpdatePayslipInfo):Observable<ApiResponseDto<UpdatePayslipInfo>>{
    return this.processPut(`UpdatePayslipDetail`,input)
  }

  public GetPayslipBeforeUpdateInfo(payslipId: number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetPayslipBeforeUpdateInfo?payslipId=${payslipId}`);
  }

  /// <summary>
        /// Hàm này lấy ra thông tin chi tiết danh sách các nhân viên chưa trả tiền phạt/ lương âm
        /// </summary>
        /// <param name="payrollId"></param>
        /// <returns>
        ///     Return các thông tin sau:
        ///     {
        ///         Id: Id của payslip;
        ///         FullName: Tên nhân viên;
        ///         Email: Email nhân viên;
        ///         Avatar: string;
        ///         Sex: Giới tính;
        ///         BranchInfo: {Tên; Màu}
        ///         LevelInfo: {Tên; Màu}
        ///         JobPositionInfo: {Tên; Màu}
        ///         UserTypeInfo: {Tên; màu}
        ///         RealSalary: Lương thực lĩnh (điều kiện < 0)
        ///         EmployeeId: Id nhân viên
        ///     }
        /// </returns>
  public GetAllPenaltyNotCollected(id:number):Observable<ApiResponseDto<GetPayslipEmployeeDto[]>>{
    return this.processGet(`GetAllPenaltyNotCollected?payrollId=${id}`)
  }

  public GetAvailablePunishmentsInMonth(payslipId:number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetAvailablePunishmentsInMonth?payslipId=${payslipId}`)
  }
}
