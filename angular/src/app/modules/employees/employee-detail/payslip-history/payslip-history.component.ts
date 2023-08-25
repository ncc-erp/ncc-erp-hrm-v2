import { Component, Injector, OnInit } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { HistoryService } from '@app/service/api/history/history.service';
import { EmployeePaySlipHistory } from '@app/service/model/history/EmployeePayslipHistory.dto';
import { PayslipHistoryDto } from '@app/service/model/payslip-history/PayslipHistoryDto'
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-payslip-history',
  templateUrl: './payslip-history.component.html',
  styleUrls: ['./payslip-history.component.css']
})
export class PayslipHistoryComponent extends PagedListingComponentBase<unknown> implements OnInit {
  public employeeId: number
  public listPayslipHistory: EmployeePaySlipHistory[] = []
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {

  }
  constructor(injector: Injector, private historyService: HistoryService) {
    super(injector)
  }

  ngOnInit(): void {
    this.employeeId = Number(this.activatedRoute.snapshot.queryParamMap.get("id"))
    this.getEmployeePayslipHistory()
  }

  getEmployeePayslipHistory() {
    if(this.isAllowViewTabPayslipHistory()){
      this.isLoading = true;
      this.historyService.getAllEmployeePayslipHistory(this.employeeId)
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe(rs => {
        this.listPayslipHistory = rs.result
      })
    }
  }
  
  isAllowViewTabPayslipHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPayslipHistory_View);
  }


}
