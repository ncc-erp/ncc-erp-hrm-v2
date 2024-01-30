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
  ViewChild,
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
import {
  ChartSelectComponent,
  IChartDataType,
} from "@shared/components/chart-select/chart-select.component";
import * as moment from "moment";
import { FormControl } from "@angular/forms";
import { DateSelectorHomeEnum } from "../../shared/AppConsts";
import { DateTimeSelectorHome } from "./date-selector-homepage/date-selector-homepage.component";
import {
  DisplayCircleChartDto,
  DisplayLineChartDto,
} from "../service/model/chart-settings/chart.dto";
import { ListInfoComponent } from "./listinfo/list-info/list-info.component";

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

  listCircleEmployeeChartSelectBox: IChartDataType[] = []; // list Circle chart for select box for dropdata
  listCirclePayslipChartSelectBox: IChartDataType[] = []; // list Circle chart for select box for dropdata
  listLineEmployeeChartSelectBox: IChartDataType[] = []; // list Line chart for select box for dropdata
  listLinePayslipChartSelectBox: IChartDataType[] = []; // list Line chart for select box for dropdata

  listAllLineChartSelectBox: IChartDataType[] = []; // list Line chart for select box for dropdata
  listAllCircleChartSelectBox: IChartDataType[] = []; // list Line chart for select box for dropdata

  lineEmployeeChartDataDisplay: DisplayLineChartDto[] = []; // line Employee Chart for Display
  linePayslipChartDataDisplay: DisplayLineChartDto[] = []; // line Payslip Chart for Display
  circleEmployeeChartDataDisplay: DisplayCircleChartDto[] = []; // Circle Employee Chart for Display
  circlePayslipChartDataDisplay: DisplayCircleChartDto[] = []; // Circle Payslip Chart for Display

  listAllLineChartDataDisplay: DisplayLineChartDto[] = []; // All line chart for Display
  listAllCircleChartDataDisplay: DisplayCircleChartDto[] = []; // All Circle chart for Display

  allLineChartIds: number[]; // list line chart when There are no options selected
  allCircleChartIds: number[]; //list line chart when There are no options selected

  listEmployeeChartIds: number[]; // list Employee chart use filter
  listPayslipChartIds: number[]; // list Payslip chart use filter

  tableData = {} as any;
  typeDate: any;
  // dateTime
  fromDateDefault: any;
  toDateDefault: any;
  fromDateLineChart: any;
  toDateLineChart: any;
  fromDateCircleChart: any;
  toDateCircleChart: any;

  isDefault: boolean = true;
  isLineEmptyDisplay: boolean = false;
  isCircleEmptyDisplay: boolean = false;

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
    this.fromDateDefault = moment(new Date(date.getFullYear(), 0, 1)).format(
      "YYYY-MM-DD"
    );
    this.toDateDefault = moment(new Date(date.getFullYear(), 11, 31)).format(
      "YYYY-MM-DD"
    );
    this.fromDateLineChart = this.fromDateDefault;
    this.toDateLineChart = this.toDateDefault;
    this.fromDateCircleChart = this.fromDateDefault;
    this.toDateCircleChart = this.toDateDefault;
    this.getAllChartActive(this.fromDateDefault, this.toDateDefault);
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
    let payslipIds = ids.filter((id) => this.listPayslipChartIds.includes(id));
    if (ids.length == 0) {
      this.isCircleEmptyDisplay = true;
    }
    if (employeeIds.length != 0) {
      this.getDataForCircleEmployeeChart(
        this.fromDateCircleChart,
        this.toDateCircleChart,
        this.listCircleChartId
      );
      this.isCircleEmptyDisplay = false;
    } else {
      this.circleEmployeeChartDataDisplay = [];
      this.mapListAllCharts();
    }
    if (payslipIds.length != 0) {
      this.getDataForCirclePayslipChart(
        this.fromDateCircleChart,
        this.toDateCircleChart,
        this.listCircleChartId
      );
      this.isCircleEmptyDisplay = false;
    } else {
      this.circlePayslipChartDataDisplay = [];
      this.mapListAllCharts();
    }
  }

  onDateChangeCircleChart(event: DateTimeSelectorHome) {
    let data = event;
    this.searchWithDateTimeCircleChart = data;
    this.defaultDateFilterTypeCircleChart = data.dateType;
    this.searchWithDateTimeCircleChart.dateType = data.dateType;
    this.fromDateCircleChart = moment(
      this.searchWithDateTimeCircleChart.fromDate
    ).format("YYYY-MM-DD");
    this.toDateCircleChart = moment(
      this.searchWithDateTimeCircleChart.toDate
    ).format("YYYY-MM-DD");

    this.getDataForCircleEmployeeChart(
      this.fromDateCircleChart,
      this.toDateCircleChart,
      this.listCircleChartId
    );
    this.getDataForCirclePayslipChart(
      this.fromDateCircleChart,
      this.toDateCircleChart,
      this.listCircleChartId
    );
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
          this.circleEmployeeChartDataDisplay = rs.result.circleCharts.map(
            (item) => ({
              id: item.id,
              name: item.chartName,
              chartDataType: rs.result.chartDataType,
              chartDetails: item.pies.map(
                (pie) =>
                  new DisplayCircleChartDetailDto(
                    pie.id,
                    pie.data,
                    pie.pieName,
                    {
                      color: pie.color,
                    }
                  )
              ),
            })
          );
          this.mapListAllCharts();
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
          this.circlePayslipChartDataDisplay = rs.result.circleCharts.map(
            (item) => ({
              id: item.id,
              name: item.chartName,
              chartDataType: rs.result.chartDataType,
              chartDetails: item.pies.map(
                (pie) =>
                  new DisplayCircleChartDetailDto(
                    pie.id,
                    pie.data,
                    pie.pieName,
                    {
                      color: pie.color,
                    }
                  )
              ),
            })
          );
          this.mapListAllCharts();
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
        // Select Box - Line Employee
        this.listLineEmployeeChartSelectBox = result.lineCharts.map((line) => ({
          name: line.chartName,
          value: line.id,
          dataType: result.chartDataType,
          hidden: false,
        }));
        // Select Box - Circle Employee
        this.listCircleEmployeeChartSelectBox = result.circleCharts.map(
          (circle) => ({
            name: circle.chartName,
            value: circle.id,
            dataType: result.chartDataType,
            hidden: false,
          })
        );

        // Display line Employee
        this.lineEmployeeChartDataDisplay = result.lineCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDataType: result.chartDataType,
          chartDetails: item.lines.map(
            (line) =>
              new DisplayLineChartDetailDto(line.id, line.data, line.lineName, {
                color: line.color,
              })
          ),
        }));
        // Display Circle Employee
        this.circleEmployeeChartDataDisplay = result.circleCharts.map(
          (item) => ({
            id: item.id,
            name: item.chartName,
            chartDataType: result.chartDataType,
            chartDetails: item.pies.map((pie) => ({
              id: pie.id,
              data: pie.data,
              pieName: pie.pieName,
              itemStyle: { color: pie.color },
            })),
          })
        );
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
        // Select Box - Line Payslip
        this.listLinePayslipChartSelectBox = result.lineCharts.map((line) => ({
          name: line.chartName,
          value: line.id,
          dataType: result.chartDataType,
          hidden: false,
        }));

        // Select Box - Circle Payslip
        this.listCirclePayslipChartSelectBox = result.circleCharts.map(
          (line) => ({
            name: line.chartName,
            value: line.id,
            dataType: result.chartDataType,
            hidden: false,
          })
        );
        // Display line Payslip
        this.linePayslipChartDataDisplay = result.lineCharts.map((item) => ({
          id: item.id,
          name: item.chartName,
          chartDataType: result.chartDataType,
          chartDetails: item.lines.map(
            (line) =>
              new DisplayLineChartDetailDto(line.id, line.data, line.lineName, {
                color: line.color,
              })
          ),
        }));

        // Display Circle Payslip
        this.circlePayslipChartDataDisplay = result.circleCharts.map(
          (item) => ({
            id: item.id,
            name: item.chartName,
            chartDataType: result.chartDataType,
            chartDetails: item.pies.map((pie) => ({
              id: pie.id,
              data: pie.data,
              pieName: pie.pieName,
              itemStyle: { color: pie.color },
            })),
          })
        );
        this.allLineChartIds = this.allLineChartIds.concat(
          result.lineCharts.map((item) => item.id)
        );
        this.allCircleChartIds = this.allCircleChartIds.concat(
          result.circleCharts.map((item) => item.id)
        );

        this.listPayslipChartIds = result.circleCharts
          .map((e) => e.id)
          .concat(result.lineCharts.map((e) => e.id));
        this.mapListAllCharts();
        this.listLineChartId = [
          ...this.listLineEmployeeChartSelectBox.map((item1) => item1.value),
          ...this.listLinePayslipChartSelectBox.map((item) => item.value),
        ];
        this.listCircleChartId = [
          ...this.listCircleEmployeeChartSelectBox.map((item1) => item1.value),
          ...this.listCirclePayslipChartSelectBox.map((item) => item.value),
        ];
      });
  }

  onLineChartSelect(ids: number[]) {
    this.listLineChartId = ids;
    let employeeIds = ids.filter((id) =>
      this.listEmployeeChartIds.includes(id)
    );
    let payslipIds = ids.filter((id) => this.listPayslipChartIds.includes(id));
    if (!ids.length) {
      this.isLineEmptyDisplay = true;
    }
    if (employeeIds.length) {
      this.getDataForLineEmployeeChart(
        this.fromDateLineChart,
        this.toDateLineChart,
        this.listLineChartId
      );
      this.isLineEmptyDisplay = false;
    } else {
      this.lineEmployeeChartDataDisplay = [];
      this.mapListAllCharts();
    }
    if (payslipIds.length) {
      this.getDataForLinePayslipChart(
        this.fromDateLineChart,
        this.toDateLineChart,
        this.listLineChartId
      );
      this.isLineEmptyDisplay = false;
    } else {
      this.linePayslipChartDataDisplay = [];
      this.mapListAllCharts();
    }
  }

  onDateChangeLineChart(event: DateTimeSelectorHome) {
    let data = event;
    this.searchWithDateTimeLineChart = data;
    this.defaultDateFilterTypeLineChart = data.dateType;
    this.searchWithDateTimeLineChart.dateType = data.dateType;
    this.fromDateLineChart = moment(
      this.searchWithDateTimeLineChart.fromDate
    ).format("YYYY-MM");
    this.toDateLineChart = moment(
      this.searchWithDateTimeLineChart.toDate
    ).format("YYYY-MM");
    this.getDataForLineEmployeeChart(
      this.fromDateLineChart,
      this.toDateLineChart,
      this.listLineChartId
    );
    this.getDataForLinePayslipChart(
      this.fromDateLineChart,
      this.toDateLineChart,
      this.listLineChartId
    );
  }
  mapListAllCharts() {
    this.listAllLineChartDataDisplay = [
      ...this.lineEmployeeChartDataDisplay,
      ...this.linePayslipChartDataDisplay,
    ];
    this.listAllCircleChartDataDisplay = [
      ...this.circleEmployeeChartDataDisplay,
      ...this.circlePayslipChartDataDisplay,
    ];
    this.listAllCircleChartSelectBox = [
      ...this.listCircleEmployeeChartSelectBox,
      ...this.listCirclePayslipChartSelectBox,
    ];
    this.listAllLineChartSelectBox = [
      ...this.listLineEmployeeChartSelectBox,
      ...this.listLinePayslipChartSelectBox,
    ];
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
          this.lineEmployeeChartDataDisplay = rs.result.lineCharts.map(
            (item) => ({
              id: item.id,
              name: item.chartName,
              chartDataType: rs.result.chartDataType,
              chartDetails: item.lines.map(
                (line) =>
                  new DisplayLineChartDetailDto(
                    line.id,
                    line.data,
                    line.lineName,
                    {
                      color: line.color,
                    }
                  )
              ),
            })
          );
          this.mapListAllCharts();
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
          this.linePayslipChartDataDisplay = rs.result.lineCharts.map(
            (item) => ({
              id: item.id,
              name: item.chartName,
              chartDataType: rs.result.chartDataType,
              chartDetails: item.lines.map(
                (line) =>
                  new DisplayLineChartDetailDto(
                    line.id,
                    line.data,
                    line.lineName,
                    {
                      color: line.color,
                    }
                  )
              ),
            })
          );
          this.mapListAllCharts();
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
}
