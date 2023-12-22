import { Component, Injector, OnInit } from "@angular/core";
import { ChartDetailSettingDto } from "@app/service/model/chart-detail-settings/chart-detail-setting.dto";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { ChartDetailSettingService } from "@app/service/api/chart-detail-settings/chart-detail-setting.service";
import { MatMenuTrigger } from "@angular/material/menu";
import { APP_ENUMS } from "@shared/AppEnums";
import { finalize } from "rxjs/operators";
import { CreateEditChartDetailDialogComponent } from './create-edit-chart-detail-dialog/create-edit-chart-detail-dialog.component';

@Component({
  selector: "app-chart-detail-settings",
  templateUrl: "./chart-detail-settings.component.html",
  styleUrls: ["./chart-detail-settings.component.css"],
})
export class ChartDetailSettingsComponent
  extends PagedListingComponentBase<ChartDetailSettingDto>
  implements OnInit
{
  constructor(
    injector: Injector,
    private chartDetailSettingService: ChartDetailSettingService
  ) {
    super(injector);
  }

  public chartDetailList = [] as ChartDetailSettingDto[];
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: "0px", y: "0px" };
  public statusList = this.getListFormEnum(APP_ENUMS.ActiveStatus, true);
  public defaultValue = APP_ENUMS.ActiveStatus.Active;

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    this.subscription.push(
      this.chartSettingService
        .getAllPagging(request)
        .pipe(
          finalize(() => {
            finishedCallback();
          })
        )
        .subscribe((rs) => {
          this.chartDetailList = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        })
    );
  }

  ngOnInit() {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: "" },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: "Chart setting" },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: "Chart detail setting" },
    ];
    this.refresh();
  }

  onCreate() {
    this.openDialog(CreateEditChartDetailDialogComponent);
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

  goToChartPage() {
    this.router.navigate(['/app/admin/charts']);
  }

  onUpdate(chartDetail: ChartDetailSettingDto) {
    this.openDialog(CreateEditChartDetailDialogComponent, { ...chartDetail });

  }

  onActive(chartDetail: ChartDetailSettingDto) {
    
  }

  onDelete(chartDetail: ChartDetailSettingDto) {
    this.confirmDelete(`Delete chart detail <strong>${chartDetail.name}</strong>`, () =>
      this.chartDetailSettingService
        .delete(chartDetail.id)
        .toPromise()
        .then((rs) =>
          abp.notify.success(`Delete chart detail ${chartDetail.name} completed`)
        )
    );
  }
}
