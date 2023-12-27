import { Component, Injector, OnInit } from "@angular/core";
import { ChartDetailSettingDto } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-setting.dto";
import { ChartFullDeTailDto } from "@app/service/model/chart-settings/chart-full-detail.dto";
import {
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { ChartDetailSettingService } from "@app/service/api/chart-settings/chart-detail-settings/chart-detail-setting.service";
import { MatMenuTrigger } from "@angular/material/menu";
import { APP_ENUMS } from "@shared/AppEnums";
import { finalize } from "rxjs/operators";
import { CreateEditChartDetailDialogComponent } from "./create-edit-chart-detail-dialog/create-edit-chart-detail-dialog.component";
import { FILTER_VALUE } from "@app/modules/categories/punishment-types/punishment-types.component";
import { AppConsts } from "@shared/AppConsts";
import { BranchService } from "@app/service/api/categories/branch.service";
import { JobPositionService } from "@app/service/api/categories/jobPosition.service";
import { LevelService } from "@app/service/api/categories/level.service";
import { TeamService } from "@app/service/api/categories/team.service";
import { ChartDetailSelectionBaseInfo } from "@app/service/model/chart-settings/chart-detail-settings/chart-detail-selection-base-info.dto";

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
    throw new Error("Method not implemented.");
  }
  constructor(
    injector: Injector,
    private chartDetailSettingService: ChartDetailSettingService,
    private branchService: BranchService,
    private jobPositionService: JobPositionService,
    private levelService: LevelService,
    private teamService: TeamService
  ) {
    super(injector);
  }

  public chartFullDetail = {} as ChartFullDeTailDto;
  public chartDetailList = [] as ChartDetailSettingDto[];
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: "0px", y: "0px" };
  public chartTypeTemplate = AppConsts.ChartType;
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

  // Relation data
  public listJobPositions;
  public listBranches;
  public listLevels;
  public listTeams;
  public listUserTypes;
  public listPayslipDetailTypes;
  public listWorkingStatuses;

  ngOnInit() {
    const id: number = this.activatedRoute.snapshot.queryParams["id"];
    this.getAllChartDetail(id);
    this.getMultipleList()
  }

  getAllChartDetail(id: number) {
    this.subscription.push(
      this.chartDetailSettingService
        .getAllDetailsByChartId(id)
        .subscribe((rs) => {
          this.chartFullDetail = rs.result;
          this.chartDetailList = this.chartFullDetail.chartDetails;
        })
    );

  }

  getMultipleList() {
    this.subscription.push(
      this.chartDetailSettingService
        .getChartDetailSelectionData()
        .subscribe((rs) => {
          const selectionData: ChartDetailSelectionBaseInfo = rs.result
          this.listJobPositions = selectionData.jobPositions
          this.listBranches = selectionData.branches
          this.listLevels = selectionData.levels
          this.listTeams = selectionData.teams
          this.listUserTypes = selectionData.userTypes
          this.listPayslipDetailTypes = selectionData.payslipDetailTypes
          this.listWorkingStatuses = selectionData.workingStatuses
        })
    );
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
    this.router.navigate(["/app/admin/charts"]);
  }

  onUpdate(chartDetail: ChartDetailSettingDto) {
    this.openDialog(CreateEditChartDetailDialogComponent, { ...chartDetail });
  }

  onActive(chartDetail: ChartDetailSettingDto) {}

  onDelete(chartDetail: ChartDetailSettingDto) {
    this.confirmDelete(
      `Delete chart detail <strong>${chartDetail.name}</strong>`,
      () =>
        this.chartDetailSettingService
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
