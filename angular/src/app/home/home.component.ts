import { map } from "lodash-es";
import { forkJoin } from "rxjs";
import { filter } from "rxjs/operators";
import {
  DisplayCircleChartDetailDto,
  DisplayLineChartDetailDto,
} from "./../service/model/chart-settings/chart.dto";
import {
  Component,
  Injector,
  ChangeDetectionStrategy,
  OnInit,
} from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { HomePageService } from "../service/api/homepage/homepage.service";
import {
  HomepageEmployeeStatisticDto,
  LastEmployeeWorkingHistoryDto,
} from "../../app/service/model/homepage/HomepageEmployeeStatistic.dto";
import { MatDialog } from "@angular/material/dialog";
import { APP_CONSTANT } from "../permission/api.constant";
import { IChartDataType } from "@shared/components/chart-select/chart-select.component";
import * as moment from "moment";
import { FormControl } from "@angular/forms";
import { DateSelectorHomeEnum } from "../../shared/AppConsts";
import { DateTimeSelectorHome } from "./date-selector-homepage/date-selector-homepage.component";
import {
  DisplayCircleChartDto,
  DisplayLineChartDto,
} from "../service/model/chart-settings/chart.dto";
import { ListInfoComponent } from "./listinfo/list-info/list-info.component";
import { ChartSettingService } from "../service/api/categories/charts/chart.service";

@Component({
  templateUrl: "./home.component.html",
  animations: [appModuleAnimation()],
  styleUrls: ["./home.component.css"],
})
export class HomeComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private homePageService: HomePageService,
    private dialog: MatDialog
  ) {
    super(injector);
  }
  APP_CONSTANT = APP_CONSTANT;
  public listCty: HomepageEmployeeStatisticDto[] = [];
  public filterFromDate: string;
  public filterToDate: string;
  public isLoadingChart: boolean = false;
  listLineChartId: number[] = [];
  listCircleChartId: number[] = [];
  public isByPeriod: boolean = true;
  viewChange = new FormControl(this.APP_CONSTANT.TypeViewHomePage.Month);
  activeView: number = 0;

  listCircleChart: IChartDataType[] = []; // list Circle chart for select box
  listLineChart: IChartDataType[] = []; // list Line chart for select box
  lineChartDataDisplay: DisplayLineChartDto[];
  circleChartDataDisplay: DisplayCircleChartDto[];
  allLineChartIds: number[];
  allCircleChartIds: number[];
  listEmployeeChartIds: number[];
  listPayslipChartIds: number[];

  tableData = {} as any;
  typeDate: any;
  fromDate: any;
  toDate: any;
  chartIds: any;
  distanceFromAndToDate = "";
  defaultDateFilterTypeCircleChart: DateSelectorHomeEnum =
    DateSelectorHomeEnum.YEAR;
  searchWithDateTimeCircleChart = {} as DateTimeSelectorHome;
  defaultDateFilterTypeLineChart: DateSelectorHomeEnum =
    DateSelectorHomeEnum.YEAR;
  searchWithDateTimeLineChart = {} as DateTimeSelectorHome;

  ngOnInit(): void {
    var date = new Date();
    this.filterFromDate = new Date(
      date.getFullYear(),
      date.getMonth(),
      1
    ).toDateString();
    this.filterToDate = new Date(
      date.getFullYear(),
      date.getMonth() + 1,
      0
    ).toDateString();
    this.fromDate = moment(new Date(date.getFullYear(), 0, 1)).format(
      "YYYY-MM-DD"
    );
    this.toDate = moment(new Date(date.getFullYear(), 11, 31)).format(
      "YYYY-MM-DD"
    );
    this.getAllChartActive(this.fromDate, this.toDate);
  }

  getData(startDate: string, endDate: string) {
    this.homePageService
      .GetAllWorkingHistory(startDate, endDate)
      .subscribe((rs) => {
        if (rs.success) {
          this.listCty = rs.result;
        }
      });
  }
  getValueOrNone(value: number) {
    if (value == 0) return "";
    return value;
  }
  getTextBold(branchName: string) {
    if (branchName == "Toàn công ty") {
      return "text-bold";
    }
  }
  showList(
    list: LastEmployeeWorkingHistoryDto[],
    action: string,
    branchName: string,
    title: string,
    userType?: number,
    isOnboardAndQuit?: boolean
  ) {
    if (list.length > 0) {
      switch (userType) {
        case this.APP_ENUM.UserType.Internship: {
          list = list.filter(
            (x) => x.userType == this.APP_ENUM.UserType.Internship
          );
          break;
        }
        case this.APP_ENUM.UserType.Staff: {
          list = list.filter(
            (x) => x.userType != this.APP_ENUM.UserType.Internship
          );
          break;
        }
        default: {
          list = list;
        }
      }

      this.dialog.open(ListInfoComponent, {
        data: {
          listInfo: list,
          action: action,
          branchName: branchName,
          title: title,
          isOnboardAndQuit: isOnboardAndQuit,
        },
        minWidth: "50%",
        autoFocus: false,
        restoreFocus: false,
      });
    }
  }
  onCircleChartSelect(ids: number[]) {
    this.listCircleChartId = ids;
    let employeeIds = ids.filter((id) =>
      this.listEmployeeChartIds.includes(id)
    );

    if (employeeIds) {
      if (ids.length == 0) {
        this.getDataForCircleEmployeeChart(
          this.fromDate,
          this.toDate,
          this.allCircleChartIds
        );
      } else {
        this.getDataForCircleEmployeeChart(
          this.fromDate,
          this.toDate,
          this.listCircleChartId
        );
      }
    } else {
      if (ids.length == 0) {
        this.getDataForCirclePayslipChart(
          this.fromDate,
          this.toDate,
          this.allCircleChartIds
        );
      } else {
        this.getDataForCirclePayslipChart(
          this.fromDate,
          this.toDate,
          this.listCircleChartId
        );
      }
    }
  }

  onDateChangeCircleChart(event: DateTimeSelectorHome) {
    let data = event;
    this.searchWithDateTimeCircleChart = data;
    this.defaultDateFilterTypeCircleChart = data.dateType;
    this.searchWithDateTimeCircleChart.dateType = data.dateType;
    this.fromDate = moment(this.searchWithDateTimeCircleChart.fromDate).format(
      "YYYY-MM-DD"
    );
    this.toDate = moment(this.searchWithDateTimeCircleChart.toDate).format(
      "YYYY-MM-DD"
    );

    if (this.listCircleChartId.length == 0) {
      this.getDataForCircleEmployeeChart(
        this.fromDate,
        this.toDate,
        this.allCircleChartIds
      );
      this.getDataForCirclePayslipChart(
        this.fromDate,
        this.toDate,
        this.allCircleChartIds
      );
    } else {
      this.getDataForCircleEmployeeChart(
        this.fromDate,
        this.toDate,
        this.listCircleChartId
      );
      this.getDataForCirclePayslipChart(
        this.fromDate,
        this.toDate,
        this.listCircleChartId
      );
    }
  }
  getDataForCircleEmployeeChart(fromDate, toDate, chartIds) {
    this.isLoadingChart = true;
    if (chartIds == null) {
      chartIds = [];
    }
    this.homePageService
      .GetDataEmployeeCharts(fromDate, toDate, chartIds)
      .subscribe(
        (rs) => {
          this.circleChartDataDisplay = rs.result.circleCharts.map((item) => ({
            id: item.id,
            name: item.chartName,
            chartDetails: item.pies.map(
              (pie) =>
                new DisplayCircleChartDetailDto(pie.data, pie.pieName, {
                  color: pie.color,
                })
            ),
          }));
          this.isLoadingChart = false;
        },
        () => (this.isLoadingChart = false)
      );
  }

  getDataForCirclePayslipChart(fromDate, toDate, chartIds) {
    this.isLoadingChart = true;
    if (chartIds == null) {
      chartIds = [];
    }
    this.homePageService
      .GetDataPayslipCharts(fromDate, toDate, chartIds)
      .subscribe(
        (rs) => {
          this.circleChartDataDisplay = rs.result.circleCharts.map((item) => ({
            id: item.id,
            name: item.chartName,
            chartDetails: item.pies.map(
              (pie) =>
                new DisplayCircleChartDetailDto(pie.data, pie.pieName, {
                  color: pie.color,
                })
            ),
          }));
          this.isLoadingChart = false;
        },
        () => (this.isLoadingChart = false)
      );
  }

  async getAllChartActive(startDate: string, endDate: string) {
    await this.homePageService
      .GetAllDataEmployeeCharts(startDate, endDate)
      .subscribe((data) => {
        let result = data.result;
        result.chartDataType = this.APP_ENUM.ChartDataType.Employee;
        this.listLineChart = result.lineCharts.map((line) => ({
          name: line.chartName,
          value: line.id,
          dataType: result.chartDataType,
          hidden: false,
        }));

        this.listCircleChart = result.circleCharts.map((circle) => ({
          name: circle.chartName,
          value: circle.id,
          dataType: result.chartDataType,
          hidden: false,
        }));
        this.lineChartDataDisplay = result.lineCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDetails: item.lines.map(
            (line) =>
              new DisplayLineChartDetailDto(line.data, line.lineName, {
                color: line.color,
              })
          ),
        }));
        this.circleChartDataDisplay = result.circleCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDetails: item.pies.map((pie) => ({
            data: pie.data,
            pieName: pie.pieName,
            itemStyle: { color: pie.color },
          })),
        }));
        this.allLineChartIds = result.lineCharts.map((item) => item.id);
        this.allCircleChartIds = result.circleCharts.map((item) => item.id);
        this.listEmployeeChartIds = result.circleCharts
          .map((e) => e.id)
          .concat(result.lineCharts.map((e) => e.id));
      });
    await this.homePageService
      .GetAllDataPayslipCharts(startDate, endDate)
      .subscribe((data) => {
        let result = data.result;
        result.chartDataType = this.APP_ENUM.ChartDataType.Salary;
        let listLinePayslipChart = result.lineCharts.map((line) => ({
          name: line.chartName,
          value: line.id,
          dataType: result.chartDataType,
          hidden: false,
        }));
        this.listLineChart = this.listLineChart.concat(listLinePayslipChart);

        let listCircleChart = result.circleCharts.map((line) => ({
          name: line.chartName,
          value: line.id,
          dataType: result.chartDataType,
          hidden: false,
        }));
        this.listCircleChart = this.listCircleChart.concat(listCircleChart);

        let listLineChartDataDisplay = result.lineCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDetails: item.lines.map(
            (line) =>
              new DisplayLineChartDetailDto(line.data, line.lineName, {
                color: line.color,
              })
          ),
        }));
        this.lineChartDataDisplay = this.lineChartDataDisplay.concat(
          listLineChartDataDisplay
        );

        let listDisplayLinePayslipChart = result.circleCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDetails: item.pies.map((pie) => ({
            data: pie.data,
            pieName: pie.pieName,
            itemStyle: { color: pie.color },
          })),
        }));
        this.circleChartDataDisplay = this.circleChartDataDisplay.concat(
          listDisplayLinePayslipChart
        );

        (this.allLineChartIds = this.allLineChartIds.concat(
          result.lineCharts.map((item) => item.id)
        )),
          (this.allCircleChartIds = this.allCircleChartIds.concat(
            result.circleCharts.map((item) => item.id)
          ));
        this.listPayslipChartIds = result.circleCharts
          .map((e) => e.id)
          .concat(result.lineCharts.map((e) => e.id));
      });

    setTimeout(() => {
      this.allLineChartIds;
      this.allCircleChartIds;
      this.listEmployeeChartIds;
      this.listPayslipChartIds;
      console.log(1);
    }, 2000);
  }
  onLineChartSelect(ids: number[]) {
    this.listLineChartId = ids;
    let employeeIds = ids.filter((id) =>
      this.listEmployeeChartIds.includes(id)
    );
    if (employeeIds) {
      if (ids.length == 0) {
        this.getDataForLineEmployeeChart(
          this.fromDate,
          this.toDate,
          this.allLineChartIds
        );
      } else {
        this.getDataForLineEmployeeChart(
          this.fromDate,
          this.toDate,
          this.listLineChartId
        );
      }
    } else {
      if (ids.length == 0) {
        this.getDataForLinePayslipChart(
          this.fromDate,
          this.toDate,
          this.allCircleChartIds
        );
      } else {
        this.getDataForLinePayslipChart(
          this.fromDate,
          this.toDate,
          this.listCircleChartId
        );
      }
    }
  }
  onDateChangeLineChart(event: DateTimeSelectorHome) {
    let data = event;
    this.searchWithDateTimeLineChart = data;
    this.defaultDateFilterTypeLineChart = data.dateType;
    this.searchWithDateTimeLineChart.dateType = data.dateType;
    this.fromDate = moment(this.searchWithDateTimeLineChart.fromDate).format(
      "YYYY-MM-DD"
    );
    this.toDate = moment(this.searchWithDateTimeLineChart.toDate).format(
      "YYYY-MM-DD"
    );
    if (this.listLineChartId.length == 0) {
      this.getDataForLineEmployeeChart(
        this.fromDate,
        this.toDate,
        this.allLineChartIds
      );
      this.getDataForLinePayslipChart(
        this.fromDate,
        this.toDate,
        this.allLineChartIds
      );
    } else {
      this.getDataForLineEmployeeChart(
        this.fromDate,
        this.toDate,
        this.listLineChartId
      );
      this.getDataForLinePayslipChart(
        this.fromDate,
        this.toDate,
        this.listLineChartId
      );
    }
  }

  getDataForLineEmployeeChart(startDate, endDate, chartIds: number[]) {
    this.isLoadingChart = true;
    if (chartIds == null) {
      chartIds = [];
    }

    this.homePageService
      .GetDataEmployeeCharts(startDate, endDate, chartIds)
      .subscribe(
        (rs) => {
          this.lineChartDataDisplay = rs.result.lineCharts.map((item) => ({
            id: item.id,
            name: item.chartName,
            chartDetails: item.lines.map(
              (line) =>
                new DisplayLineChartDetailDto(line.data, line.lineName, {
                  color: line.color,
                })
            ),
          }));
          this.isLoadingChart = false;
        },
        () => (this.isLoadingChart = false)
      );
  }

  getDataForLinePayslipChart(startDate, endDate, chartIds: number[]) {
    this.isLoadingChart = true;
    if (chartIds == null) {
      chartIds = [];
    }

    this.homePageService
      .GetDataPayslipCharts(startDate, endDate, chartIds)
      .subscribe(
        (rs) => {
          this.lineChartDataDisplay = rs.result.lineCharts.map((item) => ({
            id: item.id,
            name: item.chartName,
            chartDetails: item.lines.map(
              (line) =>
                new DisplayLineChartDetailDto(line.data, line.lineName, {
                  color: line.color,
                })
            ),
          }));
          this.isLoadingChart = false;
        },
        () => (this.isLoadingChart = false)
      );
  }
  public onDateSelectorChange(data) {
    this.filterFromDate = data?.fromDate;
    this.filterToDate = data?.toDate;
    this.getData(this.filterFromDate, this.filterToDate);
  }
  changeView(reset?: boolean, fDate?: any, tDate?: any) {
    if (reset) {
      this.activeView = 0;
    }
    let fromDate, toDate;
    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Month) {
      fromDate = moment().startOf("M").add(this.activeView, "M");
      toDate = moment(fromDate).endOf("M");
      this.typeDate = "Month";
    }

    if (this.viewChange.value === this.APP_CONSTANT.TypeViewHomePage.Year) {
      fromDate = moment().startOf("y").add(this.activeView, "y");
      toDate = moment(fromDate).endOf("y");
      this.typeDate = "Years";
    }

    if (
      this.viewChange.value == this.APP_CONSTANT.TypeViewHomePage.CustomTime
    ) {
      fromDate = "";
      toDate = "";
      if (!reset && fDate && tDate) {
        if (fDate && tDate) {
          fromDate = fDate.format("DD MMM YYYY");
          toDate = tDate.format("DD MMM YYYY");
        }
        this.setFromAndToDate(fromDate, toDate);
        this.distanceFromAndToDate = fromDate + "  -  " + toDate;
      } else {
        this.distanceFromAndToDate = "Custom Time";
      }
    }

    if (fromDate != "" && toDate != "") {
      let fDate = "",
        tDate = "";
      let list = [];
      list[0] = { value: fromDate.isSame(toDate, "year"), type: "YYYY" };
      list[1] = { value: fromDate.isSame(toDate, "month"), type: "MM" };
      list[2] = { value: fromDate.isSame(toDate, "day"), type: "DD" };
      list.map((value) => {
        if (value.value) {
          tDate = toDate.format(value.type) + " " + tDate;
        } else {
          fDate = fromDate.format(value.type) + " " + fDate;
          tDate = toDate.format(value.type) + " " + tDate;
        }
      });
      this.distanceFromAndToDate = fDate + " - " + tDate;
    }
    if (
      this.viewChange.value != this.APP_CONSTANT.TypeViewHomePage.CustomTime
    ) {
      fromDate = fromDate == "" ? "" : fromDate.format("YYYY-MM-DD");
      toDate = toDate == "" ? "" : toDate.format("YYYY-MM-DD");
      this.setFromAndToDate(fromDate, toDate);
    }
  }
  setFromAndToDate(fromDate, toDate) {
    this.fromDate = fromDate;
    this.toDate = toDate;
  }
}
