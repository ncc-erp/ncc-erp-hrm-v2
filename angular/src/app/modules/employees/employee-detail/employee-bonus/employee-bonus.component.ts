import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { EmployeeBonusDetailDto } from '@app/service/model/bonuses/bonus.dto';
import { finalize } from 'rxjs/operators';
import { APP_ENUMS } from '@shared/AppEnums';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-employee-bonus',
  templateUrl: './employee-bonus.component.html',
  styleUrls: ['./employee-bonus.component.css']
})
export class EmployeeBonusComponent extends PagedListingComponentBase<EmployeeBonusDetailDto> implements OnInit {
  public employeeBonusDetailList: EmployeeBonusDetailDto[] = []
  public employeeId: number

  public listDate: any = [];
  public bonusStatusList = [];
  public bonusStatus = APP_ENUMS.IsActiveHaveAll.Active;
  public applyMonthDefault = "-1";
  public listStatus = Object.keys(APP_ENUMS.IsActiveHaveAll).forEach((key) => {
    this.bonusStatusList.push({
      key: key,
      value: APP_ENUMS.IsActiveHaveAll[key]
    })
  })

  constructor(injector: Injector, private bonusService: BonusService) {
    super(injector)
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowViewTabBonus()){
      this.subscription.push(
        this.bonusService.getAllPagingBonusesByEmployeeId(this.employeeId, request)
          .pipe(finalize(() => {
            finishedCallback();
          }))
          .subscribe(rs => {
            this.employeeBonusDetailList = rs.result.items;
            this.showPaging(rs.result, pageNumber)
          })
      );
    }
  }

  ngOnInit(): void {
    this.employeeId = this.activatedRoute.snapshot.queryParams["id"];
    this.setDefaultFilter({ propertyName: 'isActive', value: this.bonusStatus, comparision: 0 })
    this.getListMonthFilterOfEmployee();
    this.refresh()
  }

  getListMonthFilterOfEmployee() {
    this.bonusService.getListMonthFilterOfEmployee(this.employeeId).subscribe((res) => {
      this.listDate = res.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("YYYY/MM/DD"),
        }
      });
      this.listDate.unshift({ key: "All", value: "-1" })
    })
  }

  
  viewDetail(bonus) { 
    this.router.navigate(["/app/bonuses/list-bonus/bonus-detail/bonus-employee"], {
      queryParams: {
        id: bonus.bonusId,
        name: bonus.bonusName,
        active: bonus.isActive,
        applyMonth: bonus.applyMonth
      }
      
    })

  }

  isAllowViewTabBonus(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBonus_View);
  } 

}
