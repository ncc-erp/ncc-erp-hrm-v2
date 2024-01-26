import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CreateEditChartDetailDialogComponent } from '@app/modules/categories/charts/chart-details/create-edit-chart-detail-dialog/create-edit-chart-detail-dialog.component';
import { ChartDetailService } from '@app/service/api/categories/charts/chart-details/chart-detail.service';
import { HomePageService } from '@app/service/api/homepage/homepage.service';
import { ChartDetailFullDto } from '@app/service/model/chart-settings/chart-detail-settings/chart-detail-full.dto';
import { DisplayLineChartDto } from '@app/service/model/chart-settings/chart.dto';
import { InputChartDetailDto, PayslipDataChartDto } from '@app/service/model/homepage/HomepageEmployeeStatistic.dto';
import { APP_ENUMS, ChartDataType, EmployeeMonthlyStatus } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-chart-detail-data',
  templateUrl: './chart-detail-data.component.html',
  styleUrls: ['./chart-detail-data.component.css']
})
export class ChartDetailDataComponent extends AppComponentBase implements OnInit {
  public chartDetailId: number
  public chartDataType: ChartDataType;
  public isEmployeeChart: boolean;

  public chartDetail: ChartDetailFullDto;
  public chartDetailName: string;

  public startDate: Date;
  public endDate: Date;
  
  public currentPage: number = 1
  public itemPerPage:number = 10
  public pageSizeType: number = 10;
  sortColumn: string;
  sortDirect: number;
  iconSort: string;
  listPayslipData: PayslipDataChartDto[] = []
  sortedDetail: PayslipDataChartDto[] = []
  
  constructor(
    private homePageService: HomePageService,
    private chartDetailService: ChartDetailService,
    public dialogRef: MatDialogRef<ChartDetailDataComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data,
    injector: Injector,
    private router: Router) {
    super(injector);
  }

  ngOnInit(): void {
    this.chartDataType = this.data.chartData.chartDataType;
    this.isEmployeeChart = this.chartDataType == APP_ENUMS.ChartDataType.Employee;
    this.chartDetailId = this.data.chartDetailId;
    this.startDate = this.data.startDate;
    this.endDate = this.data.endDate;
    this.refresh();
  }

  refresh(){
    this.getChartDetail(this.chartDetailId)
    this.getDetailDataChart()
  }

  getChartDetail(id: number){
    this.chartDetailService.get(id).subscribe(data =>{
      this.chartDetail = data.result;
      this.chartDetailName = this.chartDetail.name;
    })
    this.chartDetailService.getChartDetailSelectionData();
  }
  getDetailDataChart(){
    let payload = {
      chartDetailId: this.chartDetailId,
      chartDataType: this.chartDataType,
      startDate: this.startDate,
      endDate: this.endDate
    } as InputChartDetailDto;
    this.homePageService.GetDetailDataChart(payload).subscribe(rs =>{
      this.listPayslipData = rs.result;
      this.sortedDetail = this.listPayslipData.slice();
    })
  }

  public onViewDetail(id: number) {
    let ref = this.dialog.open(CreateEditChartDetailDialogComponent, {
      width: "700px",
      data: {
        ...this.chartDetail,
        ChartDataType: APP_ENUMS.ChartDataType.Employee
      },
      disableClose: true,
    });
    // ref.componentInstance.onSaveChange.subscribe((data) => {
      //   this.refreshChart()
      // });
  }

  getMonthlyStatus(value: number) {
    return EmployeeMonthlyStatus[value] || EmployeeMonthlyStatus.Working; // Default to Working if undefined
  }

  sortData(data) {
    if (this.sortColumn !== data) {
      this.sortDirect = -1;
    }
    this.sortColumn = data;
    this.sortDirect++;
    if (this.sortDirect > 1) {
      this.iconSort = "";
      this.sortDirect = -1;
    }
    if (this.sortDirect == 1) {
      this.iconSort = "fas fa-sort-amount-down";  // Descending sort
      this.sortDesc(this.sortColumn);
    } else if (this.sortDirect == 0) {
      this.iconSort = "fas fa-sort-amount-up";    // Ascending sort
      this.sortAsc(this.sortColumn);
    } else {
      this.iconSort = "fas fa-sort";              // Default
      this.sortedDetail = this.listPayslipData.slice();
    }
  }

  sortAsc(sortColumn: string){
    this.sortedDetail.sort((a,b) => (typeof a[sortColumn] === "number") ? a[sortColumn]-b[sortColumn] : (a[sortColumn] ?? "").localeCompare(b[sortColumn] ?? ""));
  }
  sortDesc(sortColumn: string){
    this.sortedDetail.sort((a,b) => (typeof a[sortColumn] === "number") ? b[sortColumn]-a[sortColumn] : (b[sortColumn] ?? "").localeCompare(a[sortColumn] ?? ""));
  }

  changePageSize() {
    this.currentPage = 1;
    this.itemPerPage = this.pageSizeType;
    this.refresh();
  }

  onClose() {
    this.dialogRef.close()
  }
}
