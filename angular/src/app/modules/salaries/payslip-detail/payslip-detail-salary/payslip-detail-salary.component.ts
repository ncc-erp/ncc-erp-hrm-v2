import { CalculateResultComponent } from './../calculate-result/calculate-result.component';
import { MatDialog } from '@angular/material/dialog';
import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { PayslipDetailDto, ReCalculateDto, UpdatePayslipInfo } from '@app/service/model/payslip/payslip.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { GetSalaryDetailDto } from '@app/service/model/payslip/GetSalaryDetailDto'
import { CollectPayslipDto } from '@app/service/model/payslip/CollectPayslipDto';
import { APP_ENUMS } from '@shared/AppEnums';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators'
import { EditPayslipDetailDialogComponent } from './edit-payslip-detail-dialog/edit-payslip-detail-dialog.component';
@Component({
  selector: 'app-payslip-detail-salary',
  templateUrl: './payslip-detail-salary.component.html',
  styleUrls: ['./payslip-detail-salary.component.css']
})
export class PayslipDetailSalaryComponent extends AppComponentBase implements OnInit {
  public payslipDetailSalary = {} as PayslipDetailDto;
  public payslipResults: GetSalaryDetailDto[] = []
  public payrollId: number = 0;
  public employeeId: number = 0;
  public payslipId: number = 0;
  public input:UpdatePayslipInfo
  private payrollStatus:number
  public salaryResult: SalaryResult = {
    benefit: 0,
    bonus: 0,
    debt: 0,
    maternityLeaveSalary: 0,
    normalSalary: 0,
    OTSalary: 0,
    punishment: 0
  }

  constructor(
    injector: Injector,
    private payslipService: PayslipService,
    private route: ActivatedRoute,
    private router: Router,
    private dialog:MatDialog
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.isViewTabSalary()) {
      this.subscription.push(
        this.route.queryParams.subscribe(rs => {
          this.payrollId = Number(rs['payrollId'] ?? this.payrollId);
          this.employeeId = Number(rs['employeeId'] ?? this.employeeId)
          this.payslipId = Number(rs['id'] ?? this.payslipId);
          this.payrollStatus = Number(rs['status'] ?? this.payrollStatus);
        }
        )
      )
      this.getPayslipDetail();
      this.getPayslipResult();
      
    }
  }

  public getPayslipDetail() {
    this.subscription.push(
      this.payslipService.getPayslipDetail(this.payslipId).subscribe((rs) => {
        this.payslipDetailSalary = rs.result;
      })
    )
  }

  public reCalculateSalary() {
    let payload: ReCalculateDto = {
      payslipId: this.payslipId
    }
    abp.message.confirm(`Are you sure want to re calculate salary?`, "", (result) => {
      if (result) {
        this.subscription.push(
          this.payslipService.ReCalculatePayslip(payload)
          .pipe(startWithTap(() => {
            this.isLoading = true
          }))
          .pipe(finalize(() => {
            this.isLoading = false
          }))
          .subscribe((rs) => {
            this.payslipDetailSalary.totalRealSalary = rs.result
            abp.message.success(`Re-Calculate successful!`);
          })
        )
      }
    })
  }

  collectEmployeePayslip() {
    const payload = {
      payrollId: this.payrollId,
      employeeIds: [this.employeeId]
    } as CollectPayslipDto

    abp.message.confirm(`Are you sure want to collect and re-calculate salary?`, "", (result) => {
      if (result) {
        this.subscription.push(
          this.payslipService.ReGeneratePayslip(this.payslipId)
          .pipe(startWithTap(() => {
            this.isLoading = true
          }))
          .pipe(finalize(() => {
            this.isLoading = false
          }))
          .subscribe((rs) => {
            if(rs.result.payslipIds){
              this.payslipId = rs.result.payslipIds[0]
              this.router.navigate([], {
                queryParams: {
                  id: this.payslipId,
                  employeeId: this.employeeId,
                  payrollId: this.payrollId,
                  status: this.payrollStatus
                },
              }).then(() => {
                this.getPayslipDetail();
                this.getPayslipResult();
              })
              abp.message.success(`Re-Calculate successful!`);

            }
            else if(rs.result.errorList != null && rs.result.errorList.length > 0){
              this.dialog.open(CalculateResultComponent, {
                width: "800px",
                data: rs.result.errorList
              })
               this.APP_CONST.calSalaryProcess.next({})
            }

          })
        )
      }
    })
  }

  public UpdatePayslipInfo(){
    const dialogRef =this.dialog.open(EditPayslipDetailDialogComponent,{
      disableClose:true,
      width:"auto",
      height:"auto",
      data:{
        id:this.payslipId
      }
      
    });
    dialogRef.afterClosed().subscribe(res=>{
      if(res){
        this.getPayslipDetail();
        this.getPayslipResult();
      }
    })
  }

  getPayslipResult() {
    this.subscription.push(
      this.payslipService.getPayslipResult(this.payslipId).subscribe((rs) => {
        this.payslipResults = rs.result
        rs.result.forEach(detail => {
          switch (detail.type) {
            case APP_ENUMS.ESalaryType.SalaryNormal:
              this.salaryResult.normalSalary += detail.money
              break;
            case APP_ENUMS.ESalaryType.SalaryOT:
              this.salaryResult.OTSalary += detail.money;
              break;
            case APP_ENUMS.ESalaryType.SalaryMaternityLeave:
              this.salaryResult.maternityLeaveSalary += detail.money;
            case APP_ENUMS.ESalaryType.Bonus:
              this.salaryResult.bonus += detail.money;
              break;
            case APP_ENUMS.ESalaryType.Benefit:
              this.salaryResult.benefit += detail.money;
              break;
            case APP_ENUMS.ESalaryType.Punishment:
              this.salaryResult.punishment += detail.money;
              break;
            case APP_ENUMS.ESalaryType.Debt:
              this.salaryResult.debt += detail.money;
              break;
            default:
              break;
          }
        })
      })
    )
  }
  get isShowCollectAndReCalculateSalaryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabSalary_CollectAndReCalculateSalary)
    && (this.payrollStatus == APP_ENUMS.PayrollStatus.New
      || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT)
  }

  isShowReCalculateSalaryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary)
     && (this.payrollStatus == APP_ENUMS.PayrollStatus.New
     || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT)
  }

  isViewTabSalary() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabSalary_View);
  }

  isShowActions(){
    return this.payrollStatus != APP_ENUMS.PayrollStatus.Executed
  }
}

export interface SalaryResult {
  normalSalary: number
  OTSalary: number
  benefit: number
  bonus: number
  punishment: number
  debt: number
  maternityLeaveSalary: number
}
