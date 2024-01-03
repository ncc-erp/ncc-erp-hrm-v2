import {
  AfterContentChecked,
  AfterContentInit,
  AfterViewInit,
  Component,
  Injector,
  OnInit,
} from "@angular/core";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { ChartDto } from "../../../service/model/chart-settings/chart.dto";
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
  extends PagedListingComponentBase<ChartDto>
  implements OnInit
{
  constructor(
    injector: Injector,
    private chartSettingService: ChartSettingService
  ) {
    super(injector);
  }

  public chartList = [] as ChartDto[];
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: "100px", y: "100px" };
  public statusList = this.getListFormEnum(APP_ENUMS.ActiveStatus);
  public defaultValue = FILTER_VALUE.ACTIVE;
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
    if (this.filterItems.length == 0) {
      this.filterItems.push({
        propertyName: "isActive",
        value: 1,
        comparision: 0,
      });
    }

    this.refresh();
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

  onCreate() {
    this.openDialog(CreateEditChartDialogComponent);
  }

  onUpdate(chart: ChartDto) {
    this.openDialog(CreateEditChartDialogComponent, { ...chart });
  }

  onActive(id: number) {
    this.subscription.push(
      this.chartSettingService
        .active(id)
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
          abp.notify.success(`Active chart successfull`);
          this.refresh();
        })
    );
  }

  onDeActive(id: number) {
    this.subscription.push(
      this.chartSettingService
        .deActive(id)
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
          abp.notify.success(`Deactive chart successfull`);
          this.refresh();
        })
    );
  }

  onDelete(chart: ChartDto) {
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
