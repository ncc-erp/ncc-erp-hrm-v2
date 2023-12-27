import { Component, EventEmitter, Injector, OnInit, Output } from "@angular/core";
import { ChartDetailSettingService } from "@app/service/api/chart-settings/chart-detail-settings/chart-detail-setting.service";
import { ChartDetailSettingDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { APP_ENUMS } from "@shared/AppEnums";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { startWithTap } from "@shared/helpers/observerHelper";
import { finalize } from "rxjs/operators";

@Component({
  selector: "app-create-edit-chart-detail-dialog",
  templateUrl: "./create-edit-chart-detail-dialog.component.html",
  styleUrls: ["./create-edit-chart-detail-dialog.component.css"],
})
export class CreateEditChartDetailDialogComponent
  extends DialogComponentBase<ChartDetailSettingDto>
  implements OnInit
{
  @Output() onMultiFilterWithCondition? = new EventEmitter()
  @Output() onMultiFilter? = new EventEmitter()
  
  constructor(
    injector: Injector,
    private chartDetailService: ChartDetailSettingService
  ) {
    super(injector);
  }

  public chartDetail = {} as ChartDetailSettingDto;
  public listJobPositions;
  public listBranches;
  public listLevels;
  public listTeams;
  public listUserTypes = this.APP_ENUM.UserType;
  public listPayslipDetailTypes = this.APP_ENUM.ESalaryType;
  public listWorkingStatuses = this.APP_ENUM.UserStatus;
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.chartDetail = this.dialogData;
      this.title = `Edit chart detail <strong>${this.dialogData.name}</strong>`;
    } else {
      this.title = "Create new chart detail";
    }
  }

  saveAndClose() {
    this.trimData(this.chartDetail);
    if (this.dialogData?.id) {
      this.subscription.push(
        this.chartDetailService
          .update(this.chartDetail)
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
            abp.notify.success(`Update chart successfull`);
            this.dialogRef.close(true);
          })
      );
    } else {
      this.subscription.push(
        this.chartDetailService
          .create(this.chartDetail)
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
            abp.notify.success(`Created new chart ${this.chartDetail.name}`);
            this.dialogRef.close(true);
          })
      );
    }
  }
  
 
  onTableMultiSelectWithConditionFilter(filterInput:any){
    this.onMultiFilterWithCondition.emit(filterInput);
  }

  onTableMultiSelectFilter(listData: any, property: number){
    let filterParam = {
      value : listData,
      property: property
    }
    this.onMultiFilter.emit(filterParam);
  }
}
