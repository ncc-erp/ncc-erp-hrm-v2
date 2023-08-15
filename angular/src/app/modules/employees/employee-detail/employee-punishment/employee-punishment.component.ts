import { AppConsts } from './../../../../../shared/AppConsts';
import { APP_ENUMS } from './../../../../../shared/AppEnums';
import { PunishmentService } from './../../../../service/api/punishment/punishment.service';
import { GetPunishmentOfEmployeeDto, PunishmentsDto } from './../../../../service/model/punishments/punishments.dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { Route, ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-employee-punishment',
  templateUrl: './employee-punishment.component.html',
  styleUrls: ['./employee-punishment.component.css']
})
export class EmployeePunishmentComponent extends PagedListingComponentBase<any> implements OnInit {
  public listPunishmentOfEmployee: GetPunishmentOfEmployeeDto[] = [];
  private employeeId:number = 0;

  public DEFAULT_FILTER = {
    isActive: APP_ENUMS.PunishmentStatus.Active,
    applyMonth: AppConsts.DEFAULT_ALL_FILTER_VALUE
  }
  public punishmentStatusList = [];
  public listDate: any = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowViewTabPunishment()){
      this.subscription.push(
        this.punishmentService.getPunishmentByEmployeeId(this.employeeId, request).pipe(finalize(()=> finishedCallback())).subscribe((data)=>{
          this.listPunishmentOfEmployee = data.result.items;
          this.showPaging(data.result, pageNumber);
        })
    )
    }
  }
  

  constructor(injector: Injector, public punishmentService: PunishmentService , private route: ActivatedRoute) {
    super(injector);
  }

  ngOnInit(): void {
    this.employeeId = Number(this.route.snapshot.queryParamMap.get("id"));
    this.refresh();
    this.AddFilterAll(this.punishmentStatusList);
    this.punishmentStatusList = this.getListFormEnum(APP_ENUMS.PunishmentStatus);
    this.setDefaultFilter({ propertyName: 'isActive', value: this.DEFAULT_FILTER.isActive, comparision: 0 })
    this.getDateFromPunishmentsOfEmployee();
  }
  public getDateFromPunishmentsOfEmployee() {
    this.subscription.push(
    this.punishmentService.getDateFromPunishmentsOfEmployee(this.employeeId).subscribe((res) => {
      this.listDate = res.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("MM/YYYY"),
        }
      });
      this.AddFilterAll(this.listDate);
    }))
  }

  private AddFilterAll(listData: any[]) {
    listData.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
  }

  public showPunishDetail(punishment: GetPunishmentOfEmployeeDto) {
    this.router.navigate(["/app/punishments/list-punishments/punishment-detail"], {
      queryParams: {
        id: punishment.punishmentId
      }
    })
  } 

  isAllowViewTabPunishment(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPunishment_View);
  }

}
