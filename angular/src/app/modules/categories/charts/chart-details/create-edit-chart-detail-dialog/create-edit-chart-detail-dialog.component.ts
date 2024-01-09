import {
  Component,
  EventEmitter,
  Injector,
  OnInit,
  Output,
} from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ChartDetailService } from "@app/service/api/categories/charts/chart-details/chart-detail.service";
import { ChartDetailFullDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-full.dto";
import { ChartDetailSelectionDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-selection.dto";
import { CreateChartDetailDto } from "@app/service/model/chart-settings/chart-detail-settings/create-chart-detail.dto";
import { UpdateChartDetailDto } from "@app/service/model/chart-settings/chart-detail-settings/update-chart-detail.dto";
import { KeyValueDto } from "@app/service/model/common.dto";
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
  extends DialogComponentBase<ChartDetailFullDto>
  implements OnInit
{
  @Output() onMultiFilterWithCondition? = new EventEmitter();
  @Output() onMultiFilter? = new EventEmitter();

  constructor(
    injector: Injector,
    private chartDetailService: ChartDetailService,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder
  ) {
    super(injector);
  }

  public chartDetail = {} as ChartDetailFullDto;
  public formGroup: FormGroup;
  public listBranches: KeyValueDto[];
  public listJobPositions: KeyValueDto[];
  public listLevels: KeyValueDto[];
  public listTeams: KeyValueDto[];
  public listUserTypes: KeyValueDto[];
  public listPayslipDetailTypes: KeyValueDto[];
  public listWorkingStatuses: KeyValueDto[];
  public listGender: KeyValueDto[];

  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum;
  public isChartSalaryDataType: boolean;

  ngOnInit(): void {
    this.getAllFilterData();
    this.initForm();
    this.formGroup.enable();

    if (this.dialogData?.id) {
      this.getChartDetail(this.dialogData.id);

      this.title = `Edit chart detail <strong>${this.dialogData.name}</strong>`;
    } else {
      this.title = "Create new chart detail";
    }
    this.isChartSalaryDataType =
      this.APP_ENUM.ChartDataType.Salary === this.dialogData.chartDataType;

    const id: number = this.activatedRoute.snapshot.queryParams["id"];
    this.chartDetail.chartId = id;
  }

  getChartDetail(id: number) {
    this.subscription.push(
      this.chartDetailService.get(id).subscribe((rs) => {
        this.chartDetail = rs.result;
        this.setValueToUpdate();
      })
    );
  }

  getAllFilterData() {
    const selectionData: ChartDetailSelectionDto =
      this.chartDetailService.selectionData;

    // Render data to list key value to create multiple select
    this.listBranches = selectionData.branches;
    this.listJobPositions = selectionData.jobPositions;
    this.listLevels = selectionData.levels;
    this.listTeams = selectionData.teams;
    this.listUserTypes = selectionData.userTypes;
    this.listPayslipDetailTypes = selectionData.payslipDetailTypes;
    this.listWorkingStatuses = selectionData.workingStatuses;
    this.listGender = selectionData.gender;
  }

  initForm() {
    this.formGroup = this.formBuilder.group({
      id: 0,
      name: ["", [Validators.required]],
      color: [""],
      gender: [],
      branches: [],
      jobPositions: [],
      levels: [],
      teams: [],
      userTypes: [],
      workingStatuses: [],
      payslipDetailTypes: [],
    });
  }

  // render value to form from database
  setValueToUpdate() {
    this.formGroup.patchValue({
      id: this.chartDetail.id,
      name: this.chartDetail.name,
      color: this.chartDetail.color,
      branches: this.listBranches.filter((element) =>
        this.chartDetail.branches.find(
          (selected) => selected.value == element.value
        )
      ),
      jobPositions: this.listJobPositions.filter((element) =>
        this.chartDetail.jobPositions.find(
          (selected) => selected.value == element.value
        )
      ),
      levels: this.listLevels.filter((element) =>
        this.chartDetail.levels.find(
          (selected) => selected.value == element.value
        )
      ),
      teams: this.listTeams.filter((element) =>
        this.chartDetail.teams.find(
          (selected) => selected.value == element.value
        )
      ),
      payslipDetailTypes: this.listPayslipDetailTypes.filter((element) =>
        this.chartDetail.payslipDetailTypes.find(
          (selected) => selected.value == element.value
        )
      ),
      userTypes: this.listUserTypes.filter((element) =>
        this.chartDetail.userTypes.find(
          (selected) => selected.value == element.value
        )
      ),
      gender: this.listGender.filter((element) =>
        this.chartDetail.gender.find(
          (selected) => selected.value == element.value
        )
      ),
      workingStatuses: this.listWorkingStatuses.filter((element) =>
        this.chartDetail.workingStatuses.find(
          (selected) => selected.value == element.value
        )
      ),
    });
  }

  saveAndClose() {
    // create entity by formGroup data
    const createChart: CreateChartDetailDto = {
      chartId: this.chartDetail.chartId,
      name: this.formGroup.value.name,
      color: this.formGroup.value.color,
      branchIds: this.formGroup.value.branches?.map((e) => e.value),
      jobPositionIds: this.formGroup.value.jobPositions?.map((e) => e.value),
      levelIds: this.formGroup.value.levels?.map((e) => e.value),
      payslipDetailTypes: this.formGroup.value.payslipDetailTypes?.map(
        (e) => e.value
      ),
      teamIds: this.formGroup.value.teams?.map((e) => e.value),
      userTypes: this.formGroup.value.userTypes?.map((e) => e.value),
      workingStatuses: this.formGroup.value.workingStatuses?.map(
        (e) => e.value
      ),
      gender: this.formGroup.value.gender?.map((e) => e.value),
    };

    this.trimData(createChart);

    if (this.dialogData?.id) {
      // UPDATE
      const updateChart: UpdateChartDetailDto = {
        ...createChart,
        id: this.dialogData.id,
      };

      this.subscription.push(
        this.chartDetailService
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
            abp.notify.success(`Update chart detail successfull`);
            this.dialogRef.close(true);
          })
      );
    } // INSERT
    else {
      this.subscription.push(
        this.chartDetailService
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
            abp.notify.success(
              `Created new chart detail ${this.chartDetail.name}`
            );
            this.dialogRef.close(true);
          })
      );
    }
  }

  onTableMultiSelect(listData: any, property: string) {
    this.chartDetail[property] = listData;
  }
}
