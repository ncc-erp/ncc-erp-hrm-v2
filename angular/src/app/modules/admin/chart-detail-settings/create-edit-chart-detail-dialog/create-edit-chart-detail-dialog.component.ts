import {
  Component,
  EventEmitter,
  Injector,
  OnInit,
  Output,
} from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { ChartDetailSettingService } from "@app/service/api/chart-settings/chart-detail-settings/chart-detail-setting.service";
import { ChartDetailFullDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-full.dto";
import { ChartDetailSelectionDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-selection.dto";
import { ChartDetailSettingDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { FilterKeyValueDto } from "@app/service/model/chart-settings/chart-detail-settings/filterKeyValue.dto";
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
    private chartDetailService: ChartDetailSettingService,
    private activatedRoute: ActivatedRoute,
    private formBuilder: FormBuilder,
  ) {
    super(injector);
  }

  public chartDetail = {} as ChartDetailFullDto;
  public formGroup: FormGroup;

  public listBranches: FilterKeyValueDto[];
  public listJobPositions: FilterKeyValueDto[];
  public listLevels: FilterKeyValueDto[];
  public listTeams: FilterKeyValueDto[];
  public listUserTypes: FilterKeyValueDto[];
  public listPayslipDetailTypes: FilterKeyValueDto[];
  public listWorkingStatuses: FilterKeyValueDto[];
  public listSexes: FilterKeyValueDto[];

  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum;

  ngOnInit(): void {
    this.getAllFilterData()
    this.initForm()
    this.formGroup.enable()

    if (this.dialogData?.id) {
      this.getChartDetail(this.dialogData.id)
      // this.chartDetail = this.dialogData;
      this.title = `Edit chart detail <strong>${this.dialogData.name}</strong>`;
    } else {
      this.title = "Create new chart detail";
    }

    const id: number = this.activatedRoute.snapshot.queryParams["id"];
    this.chartDetail.chartId = id;
  }

  mapNameIdToFilterKeyValue(data: any[]): FilterKeyValueDto[] {
    return data.map((e) => ({ key: e.name, value: e.id } as FilterKeyValueDto));
  }
  mapEnumToKeyValue(data: any[]): FilterKeyValueDto[] {
    return data.map(
      (e) => ({ key: e.key, value: e.value } as FilterKeyValueDto)
    );
  }

  getChartDetail(id: number) {
     this.subscription.push(
      this.chartDetailService
        .get(id)
        .subscribe((rs) => {
          this.chartDetail = rs.result;
          this.formGroup.controls['sex'].setValue(this.chartDetail.sexes)
        })
    );
  }

  getAllFilterData() {
    const selectionData: ChartDetailSelectionDto =
    this.chartDetailService.selectionData;

  // Render data to list key value to create multiple select
  this.listBranches = this.mapNameIdToFilterKeyValue(selectionData.branches);
  this.listJobPositions = this.mapNameIdToFilterKeyValue(
    selectionData.jobPositions
  );
  this.listLevels = this.mapNameIdToFilterKeyValue(selectionData.levels);
  this.listTeams = this.mapNameIdToFilterKeyValue(selectionData.teams);
  this.listUserTypes = this.mapEnumToKeyValue(selectionData.userTypes);
  this.listPayslipDetailTypes = this.mapEnumToKeyValue(
    selectionData.payslipDetailTypes
  );
  this.listWorkingStatuses = this.mapEnumToKeyValue(
    selectionData.workingStatuses
  );
  this.listSexes = this.mapEnumToKeyValue(selectionData.sexes)
  }

  initForm() {
    this.formGroup = this.formBuilder.group({
      id: 0,
      name: ["", [Validators.required]],
      sex: [""]
    })

    this.formGroup.controls.sex.valueChanges.subscribe((selectedSexes) => {
      const selectedSexesValues: any= Object.values(selectedSexes);
      // this.chartDetail.sexes = selectedSexesValues
    })
  }

  setFormValue(chartDetail: ChartDetailSettingDto)
  {
    this.formGroup.patchValue({
      name: chartDetail.name
    })
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
            abp.notify.success(`Update chart detail successfull`);
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
