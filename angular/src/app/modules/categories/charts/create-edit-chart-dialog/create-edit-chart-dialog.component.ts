import { Component, Injector, OnInit } from "@angular/core";
import {
  ChartDto,
  ChartSelectionDto,
  CreateChartDto,
  UpdateChartDto,
} from "@app/service/model/chart-settings/chart.dto";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { ChartSettingService } from "@app/service/api/categories/charts/chart.service";
import { startWithTap } from "@shared/helpers/observerHelper";
import { finalize } from "rxjs/operators";
import { APP_ENUMS } from "@shared/AppEnums";
import { KeyValueDto } from "@app/service/model/common.dto";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
  selector: "app-create-edit-chart-dialog",
  templateUrl: "./create-edit-chart-dialog.component.html",
  styleUrls: ["./create-edit-chart-dialog.component.css"],
})
export class CreateEditChartDialogComponent
  extends DialogComponentBase<ChartDto>
  implements OnInit
{
  public chart = {} as ChartDto;
  public chartType = this.getListFormEnum(APP_ENUMS.ChartType, true);
  public timePeriodType = this.getListFormEnum(APP_ENUMS.TimePeriodType, true);
  public chartDataType = this.getListFormEnum(APP_ENUMS.ChartDataType, true);

  public isEdit: boolean;
  public formGroupChart: FormGroup;
  public listUsers: KeyValueDto[];
  public listRoles: KeyValueDto[];

  constructor(
    injector: Injector,
    private chartService: ChartSettingService,
    private formBuilder: FormBuilder
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getAllFilterData();
    this.initForm();
    this.formGroupChart.enable();
    if (this.dialogData?.id) {
      this.isEdit = true;
      this.getChart(this.dialogData.id);
    } else {
      this.title = "Create new chart";
    }
  }

  getChart(id: number) {
    this.subscription.push(
      this.chartService.get(id).subscribe((rs) => {
        this.chart = rs.result;
        this.title = `Edit chart <strong>${this.chart.name}</strong>`;
        this.setValueToUpdate();
      })
    );
  }

  getAllFilterData() {
    const selectionData: ChartSelectionDto = this.chartService.selectionData;

    // Render data to list key value to create multiple select
    this.listUsers = selectionData.shareToUsers;
    this.listRoles = selectionData.shareToRoles;
  }

  initForm() {
    this.formGroupChart = this.formBuilder.group({
      id: 0,
      name: ["", [Validators.required]],
      chartType: APP_ENUMS.ChartType.Line,
      chartDataType: APP_ENUMS.ChartDataType.Employee,
      timePeriodType: APP_ENUMS.TimePeriodType.Month,
      isActive: [],
      listUsers: [],
      listRoles: [],
    });
  }

  setValueToCreate() {
    this.formGroupChart.patchValue({
      name: this.chart.name,
      chartType: this.chart.chartType,
      chartDataType: this.chart.chartDataType,
      timePeriodType: this.chart.timePeriodType,
    });
  }
  // render value to form from database
  setValueToUpdate() {
    this.formGroupChart.patchValue({
      id: this.chart.id,
      name: this.chart.name,
      chartType: this.chart.chartType,
      chartDataType: this.chart.chartDataType,
      timePeriodType: this.chart.timePeriodType,
      isActive: this.chart.isActive,
      listUsers: this.listUsers.filter((element) =>
        this.chart.shareToUsers.find(
          (selected) => selected.value == element.value
        )
      ),
      listRoles: this.listRoles.filter((element) =>
        this.chart.shareToRoles.find(
          (selected) => selected.value == element.value
        )
      ),
    });
  }

  onClearAllFilter(key) {
    const control = this.formGroupChart.get(key);

    if (control) {
      if (Array.isArray(control.value)) {
        control.setValue([]); // Set value to an empty array
      } else {
        control.setValue(""); // Set value to an empty string (for non-array controls)
      }
    }
  }

  saveAndClose() {
    const createChart: CreateChartDto = {
      name: this.formGroupChart.value.name,
      chartType: this.formGroupChart.value.chartType,
      chartDataType: this.formGroupChart.value.chartDataType,
      timePeriodType: this.formGroupChart.value.timePeriodType,
    };
    this.trimData(createChart);
    const updateChart: UpdateChartDto = {
      id: this.formGroupChart.value.id,
      ...createChart,
      shareToUserIds: this.formGroupChart.value.listUsers?.map((e) => e.value),
      shareToRoleIds: this.formGroupChart.value.listRoles?.map((e) => e.value),
      isActive: this.formGroupChart.value.isActive,
    };
    this.trimData(updateChart);
    if (
      this.formGroupChart.value.name &&
      this.formGroupChart.value.name.length !== 0
    ) {
      if (this.isEdit) {
        this.subscription.push(
          this.chartService
            .update(updateChart)
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
          this.chartService
            .create(createChart)
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
              abp.notify.success(`Created new chart ${createChart.name}`);
              this.dialogRef.close(true);
            })
        );
      }
    }
  }
}
