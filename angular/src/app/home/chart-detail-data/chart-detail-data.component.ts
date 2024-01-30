import { Component, Inject, Injector, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CreateEditChartDetailDialogComponent } from '@app/modules/categories/charts/chart-details/create-edit-chart-detail-dialog/create-edit-chart-detail-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { ChartDetailService } from '@app/service/api/categories/charts/chart-details/chart-detail.service';
import { HomePageService } from '@app/service/api/homepage/homepage.service';
import { ChartDetailFullDto } from '@app/service/model/chart-settings/chart-detail-settings/chart-detail-full.dto';
import { DisplayLineChartDto } from '@app/service/model/chart-settings/chart.dto';
import { InputChartDetailDto, PayslipDataChartDto } from '@app/service/model/homepage/HomepageEmployeeStatistic.dto';
import { AppConsts } from '@shared/AppConsts';
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
  public sortColumn: string;
  public sortDirect: number;
  public iconSort: string;
  public listPayslipData: PayslipDataChartDto[] = []
  public sortedDetail: PayslipDataChartDto[] = []
  public groupedData: any[] = [];

  public totalMoney: number = 0;
  
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
      if (!this.isEmployeeChart){
        this.totalMoney = this.listPayslipData.reduce((sum, val) => sum + val.money, 0);
      }
      this.sortedDetail = this.listPayslipData.slice();
      this.groupedData = this.groupByEmployees(this.sortedDetail);
      this.groupedData.forEach(item => {
        item.hideMonthlyStatus = true;
        item.hideMonthlyMoney = true;
      })
    })
  }

  public onViewDetail() {
    let ref = this.dialog.open(CreateEditChartDetailDialogComponent, {
      width: "700px",
      data: {
        ...this.chartDetail,
        chartDataType: this.chartDataType,
        isViewOnly: true
      },
      disableClose: true,
    });
    // ref.componentInstance.onSaveChange.subscribe((data) => {
      //   this.refreshChart()
      // });
  }

  getMonthlyStatusStyle(userMonthlyStatusEnum: number, isStyle: boolean) {
    return isStyle ? AppConsts.userMonthlyStatus[userMonthlyStatusEnum]?.class : AppConsts.userMonthlyStatus[userMonthlyStatusEnum]?.name
  }

  getMonthlyStatus(value: number) {
    return EmployeeMonthlyStatus[value] || EmployeeMonthlyStatus.Working; // Default to Working if undefined
  }

  toggleMonthlyStatus(item){
    item.hideMonthlyStatus = !item.hideMonthlyStatus;
  }

  toggleMonthlyMoney(item){
    item.hideMonthlyMoney = !item.hideMonthlyMoney;
  }

  changePageSize() {
    this.currentPage = 1;
    this.itemPerPage = this.pageSizeType;
    this.refresh();
  }

  onClose() {
    this.dialogRef.close()
  }
  
  isAllowRoutingDetail() {
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail);
  }

  groupByEmployees(sortedDetail) {
    let grouped = sortedDetail.reduce((accumulator, current) => {
        accumulator[current.employeeId] = accumulator[current.employeeId] || [];
        accumulator[current.employeeId].push(current);
        return accumulator;
    }, {});

    return Object.values(grouped);
  }
}

