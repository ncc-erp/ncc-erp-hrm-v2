import { Component, Injector, OnInit } from "@angular/core";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { ChartSettingDto } from "../../../service/model/chart-settings/chart-setting.dto";
import { CreateEditChartDialogComponent } from "./create-edit-chart-dialog/create-edit-chart-dialog.component";
import { ChartSettingService } from "@app/service/api/chart-settings/chart-setting.service";
import { finalize } from "rxjs/operators";
import { MatMenuTrigger } from "@angular/material/menu";
import { APP_ENUMS } from "@shared/AppEnums";
import { ListBenefitDefaultFilter } from "@app/modules/benefits/list-benefit/list-benefit.component";
import { FILTER_VALUE } from "@app/modules/categories/punishment-types/punishment-types.component";
import { startWithTap } from "@shared/helpers/observerHelper";

@Component({
  selector: "app-chart-settings",
  templateUrl: "./chart-settings.component.html",
  styleUrls: ["./chart-settings.component.css"],
})
export class ChartSettingsComponent
  extends PagedListingComponentBase<ChartSettingDto>
  implements OnInit
{
  constructor(
    injector: Injector,
    private chartSettingService: ChartSettingService
  ) {
    super(injector);
  }

  public chartList = [] as ChartSettingDto[];
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: "0px", y: "0px" };
  public statusList = this.getListFormEnum(APP_ENUMS.ActiveStatus);
  public defaultValue = APP_ENUMS.ActiveStatus.Active;
  public readonly filterList = [
    {
      key: "All",
      value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    },
    {
      key: "Active",
      value: FILTER_VALUE.ACTIVE,
    },
    {
      key: "Inactive",
      value: FILTER_VALUE.INACTIVE,
    },
  ];

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    // if(request.filterItems.length == 0) {
    //   request.filterItems = [{
    //     propertyName: "isActive",
    //     value: 1,
    //     comparision: 0
    //   }]
    // }

    this.subscription.push(
      this.chartSettingService
        .getAllPagging(request)
        .pipe(
          finalize(() => {
            finishedCallback();
          })
        )
        .subscribe((rs) => {
          this.chartList = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        })
    );
  }

  ngOnInit() {
    this.refresh();
  }

  onCreate() {
    this.openDialog(CreateEditChartDialogComponent);
  }

  isShowCreateBtn() {
    return true;
  }

  isShowEditBtn() {
    return true;
  }

  isShowDeleteBtn() {
    return true;
  }

  isShowActiveBtn() {
    return true;
  }

  goToChartDetailPage(id: number) {
    this.router.navigate(["/app/admin/chart-details"], {
      queryParams: {
        id: id
      }
    })
  }

  onUpdate(chart: ChartSettingDto) {
    this.openDialog(CreateEditChartDialogComponent, { ...chart });
  }

  onActive(chart: ChartSettingDto) {
    const data: ChartSettingDto = {
      ...chart,
      isActive:
        chart.isActive == this.APP_ENUM.ActiveStatus.Active
          ? this.APP_ENUM.ActiveStatus.InActive
          : this.APP_ENUM.ActiveStatus.Active,
    };
    
    this.subscription.push(this.chartSettingService.update(data)
      .pipe(startWithTap(() => { this.isLoading = true }))
      .pipe(finalize(() => { this.isLoading = false }))
      .subscribe(rs => {
        abp.notify.success(`Active chart successfull`)
        this.refresh();
      }))
  }

  onDelete(chart: ChartSettingDto) {
    this.confirmDelete(`Delete chart <strong>${chart.name}</strong>`, () =>
      this.chartSettingService
        .delete(chart.id)
        .toPromise()
        .then((rs) =>
          abp.notify.success(`Delete chart ${chart.name} completed`)
        )
    );
  }
}
