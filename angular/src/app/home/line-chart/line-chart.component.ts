import {
  Component,
  Input,
  OnInit,
  ElementRef,
  ViewChild,
  Injector,
  Output,
  EventEmitter,
} from "@angular/core";
import { inject } from "@angular/core/testing";
import { MatDialog } from "@angular/material/dialog";
import { ActivatedRoute, Router } from "@angular/router";
import { AppComponentBase } from "@shared/app-component-base";
import * as echarts from "echarts";
import * as moment from "moment";
import {
  ResultLineChartDetailDto,
  DisplayLineChartDto,
} from "../../service/model/chart-settings/chart.dto";
import { APP_ENUMS } from "@shared/AppEnums";

@Component({
  selector: "app-line-chart",
  templateUrl: "./line-chart.component.html",
  styleUrls: ["./line-chart.component.css"],
})
export class LineChartComponent extends AppComponentBase implements OnInit {
  @Input() lineChartData: DisplayLineChartDto;
  @Input() fromDate: any;
  @Input() toDate: any;
  @ViewChild("chartContainer") chartContainer: ElementRef;
  @Output() refreshData = new EventEmitter<any>();

  constructor(
    injector: Injector,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {
    super(injector);
  }

  ngOnInit() {}
  ngAfterViewInit() {
    if (this.chartContainer) {
    var myChart = echarts.init(this.chartContainer.nativeElement);
    let dataLineChartDetail = [];
      this.lineChartData.chartDetails.forEach((item) => {
        let chartDetail = {
          id: item.id,
          data: item.data,
          name: item.lineName,
          itemStyle: {color : item.itemStyle.color},
          type: "line",
          smooth: true,
        };
        dataLineChartDetail.push(chartDetail);
      });

      const option = {
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "cross",
            crossStyle: {
              color: "#999",
            },
          },
        },

       title: {
          text: this.lineChartData.name,
          left: 'center',
          textStyle: {
            fontFamily: 'Source Sans Pro',
            fontSize: 20,
          },
        },
        toolbox: {
          feature: {
            type: "line",
            saveAsImage: { show: true },
          },
        },
        xAxis: [
          {
            type: "category",
            data: this.convertMonth(this.fromDate , this.toDate),
            name : "Month",
            axisPointer: {
              type: "shadow",
            },
          },
        ],
        yAxis: [
          {
            name: 'Total',
            type: "value",
          },
        ],
        series: dataLineChartDetail
      };

      option && myChart.setOption(option);

      myChart.on("click", (params: any) => {
        this.viewDataEmployeeLineChartDetail(params.seriesId, params.name);
      });
    }
  }

  onRefreshData() {
    this.refreshData.emit();
  }
  
  viewDataEmployeeLineChartDetail(chartDetailId: number, monthYear: string){
    let { fromDate, toDate } = this.convertToDateRange(monthYear);
    console.log(chartDetailId);
    console.log(fromDate + " " + toDate);
    if(this.lineChartData.chartDataType == APP_ENUMS.ChartDataType.Employee){
      console.log("employee");
    }else{
      console.log("salary");
    }
  }
  
  convertMonth(fromDate : Date, toDate : Date){
    let result: string[] = [];
    let currentMonth = new Date(fromDate);
    currentMonth = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), 1);
    let endDate = new Date(toDate);
    while (currentMonth <= endDate) {
      let month = currentMonth.getMonth() + 1;
      let year = currentMonth.getFullYear();
      result.push(`${month}-${year}`);
      currentMonth.setMonth(currentMonth.getMonth() + 1);
    }
    return result;
  }
  
  convertToDateRange(str) {
    const parts = str.split("-");
    const month = parseInt(parts[0], 10);
    const year = parseInt(parts[1], 10);
    const fromDate = new Date(year, month - 1, 1);
    const toDate = new Date(year, month, 0);

    return { fromDate, toDate };
}
}