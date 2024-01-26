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
import { DisplayCircleChartDto } from "../../service/model/chart-settings/chart.dto";
import { ChartDetailDataComponent } from "../chart-detail-data/chart-detail-data.component";

@Component({
  selector: "app-circle-chart",
  templateUrl: "./circle-chart.component.html",
  styleUrls: ["./circle-chart.component.css"],
})
export class CircleChartComponent extends AppComponentBase implements OnInit {
  @Input() circlechartData: DisplayCircleChartDto;
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
      const option = {
        title: {
          text: this.circlechartData.name,
          left: "center",
          textStyle: {
            fontFamily: "Source Sans Pro",
            fontSize: 20,
          },
        },
        tooltip: {
          trigger: "item",
        },
        legend: {
          orient: "horizontal",
          bottom: 0,
          data: this.circlechartData.chartDetails?.map((item) => ({
            name: item.pieName,
          })),
        },

        toolbox: {
          feature: {
            saveAsImage: { show: true },
          },
        },
        grid: {
          top: "20%", // Adjust the top padding (space for the title)
          bottom: "20%", // Adjust the bottom padding (space for the legend)
        },
        series: [
          {
            type: "pie",
            data: this.circlechartData.chartDetails.map((item) => ({
              name: item.pieName,
              value: item.data,
              detail: item,
              itemStyle: {
                color: item.itemStyle.color,
              },
              label: {
                show: true,
                formatter: "{b} ({d}%)", // Display name and percentage
              },
            })),
            emphasis: {
              itemStyle: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: "rgba(0, 0, 0, 0.5)",
              },
            },
            events: {
              click: (event: any) => {},
            },
          },
        ],
      };

      option && myChart.setOption(option);
      myChart.on('click', (params: any) => {
        this.viewDataEmployeeCircleChartDetail(params.data.detail.id);
      });
    }
  }

  onRefreshData() {
    this.refreshData.emit();
  }

  viewDataEmployeeCircleChartDetail(chartDetailId: number){
    let ref = this.dialog.open(ChartDetailDataComponent, {
      minWidth: "50%",
      data: {
        startDate: this.fromDate,
        endDate: this.toDate,
        chartData : this.circlechartData,
        chartDetailId : chartDetailId
      },
      disableClose: true
    });
    // ref.componentInstance.refreshDataEvent.subscribe((data) => {
    //   this.onRefreshData();
    // });
  }
}
