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
  public contextMenuPosition = { x: '0px', y: '0px' };
  public statusList = this.getListFormEnum(APP_ENUMS.ActiveStatus)
  public defaultValue = APP_ENUMS.ActiveStatus.Active

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
          this.chartList = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        })
    );
  }

  ngOnInit() {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: "" },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: "Chart setting" },
    ];
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
    this.router.navigate(['/app/admin/charts-detail', id]);
  }

  onUpdate(chart: ChartSettingDto) {
    this.openDialog(CreateEditChartDialogComponent, { ...chart });

  }

  onActive(chart: ChartSettingDto) {
    
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
