import { Component, Injector, OnInit } from "@angular/core";
import { ChartDetailSettingDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { ChartFullDto } from "@app/service/model/chart-settings/chart-full-detail.dto";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { ChartDetailSettingService } from "@app/service/api/chart-settings/chart-detail-settings/chart-detail-setting.service";
import { MatMenuTrigger } from "@angular/material/menu";
import { APP_ENUMS } from "@shared/AppEnums";
import { CreateEditChartDetailDialogComponent } from "./create-edit-chart-detail-dialog/create-edit-chart-detail-dialog.component";
import { FILTER_VALUE } from "@app/modules/categories/punishment-types/punishment-types.component";
import { AppConsts } from "@shared/AppConsts";
import { ChartDetailFullDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-full.dto";
import { startWithTap } from "@shared/helpers/observerHelper";
import { finalize } from "rxjs/operators";

@Component({
  selector: "app-chart-detail-settings",
  templateUrl: "./chart-detail-settings.component.html",
  styleUrls: ["./chart-detail-settings.component.css"],
})
export class ChartDetailSettingsComponent
  extends PagedListingComponentBase<any>
  implements OnInit
{
  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const id: number = this.activatedRoute.snapshot.queryParams["id"];
    this.getAllChartDetail(id);
    this.chartDetailService.getChartDetailSelectionData();
  }

  constructor(
    injector: Injector,
    private chartDetailService: ChartDetailSettingService
  ) {
    super(injector);
  }

  public chartFull = {} as ChartFullDto;
  public chartFullDetailList = [] as ChartDetailFullDto[];
  public chartUpdateData = {} as ChartDetailSettingDto;
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: "0px", y: "0px" };
  // public chartTypeTemplate = AppConsts.ChartType;
  // public chartDataTypeTemplate = AppConsts.ChartDataType;
  // public chartIsActiveTemplate = AppConsts.Status;

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

  ngOnInit() {
    this.refresh();
  }

  getAllChartDetail(id: number) {
    this.subscription.push(
      this.chartDetailService
        .getAllDetailsByChartId(id)
        .pipe(
          startWithTap(() => {
            this.isLoading = true;
          })
        )
        .pipe(
          finalize(() => {
            this.isLoading = false;
          })
        )
        .subscribe((rs) => {
          this.chartFull = rs.result;
          this.chartFullDetailList = this.chartFull.chartDetails;
        })
    );
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
    this.router.navigate(["/app/admin/charts"]);
  }

  onCreate() {
    this.openDialog(CreateEditChartDetailDialogComponent);
  }

  onUpdate(chartDetail: ChartDetailFullDto) {
    this.openDialog(CreateEditChartDetailDialogComponent, { ...chartDetail });
  }

  onActive(chartDetail: ChartDetailSettingDto) {}

  onDelete(chartDetail: ChartDetailFullDto) {
    this.confirmDelete(
      `Delete chart detail <strong>${chartDetail.name}</strong>`,
      () =>
        this.chartDetailService
          .delete(chartDetail.id)
          .toPromise()
          .then((rs) =>
            abp.notify.success(
              `Delete chart detail ${chartDetail.name} completed`
            )
          )
    );
  }
}
