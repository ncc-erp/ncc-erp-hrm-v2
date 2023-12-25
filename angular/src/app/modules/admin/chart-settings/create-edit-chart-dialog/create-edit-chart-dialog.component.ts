import { Component, Injector, OnInit } from '@angular/core';
import { ChartSettingDto } from '@app/service/model/chart-settings/chart-setting.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { ChartSettingService } from '@app/service/api/chart-settings/chart-setting.service';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'app-create-edit-chart-dialog',
  templateUrl: './create-edit-chart-dialog.component.html',
  styleUrls: ['./create-edit-chart-dialog.component.css']
})
export class CreateEditChartDialogComponent extends DialogComponentBase<ChartSettingDto> implements OnInit {
  public chart = {} as ChartSettingDto;
  chartType = this.getListFormEnum(APP_ENUMS.ChartType, true);
  timePeriodType = this.getListFormEnum(APP_ENUMS.TimePeriodType, true);
  
  constructor(injector: Injector,
    private chartService: ChartSettingService) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.chart = this.dialogData
      this.title = `Edit chart <strong>${this.dialogData.name}</strong>`
    }
    else {
      this.title = "Create new chart"
    }
  }

  saveAndClose() {
    this.trimData(this.chart)
    if (this.dialogData?.id) {
      this.subscription.push(this.chartService.update(this.chart)
      .pipe(startWithTap(() => { this.isLoading = true }))
      .pipe(finalize(() => { this.isLoading = false }))
      .subscribe(rs => {
        abp.notify.success(`Update chart successfull`)
        this.dialogRef.close(true)
      }))
    }
    else {
      this.subscription.push(
        this.chartService.create(this.chart)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Created new chart ${this.chart.name}`)
          this.dialogRef.close(true)
        })
      )
    }
  }

  
}
