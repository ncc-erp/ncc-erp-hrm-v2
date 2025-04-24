import { ResultReport } from './../../../service/model/report/reportSalary.dto';
import { ReportService } from './../../../service/api/report/report.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { GetEmployeeBasicInfo } from './../../../service/model/employee/employee.dto';
import { Component, OnInit, Output, EventEmitter} from '@angular/core';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { PagedListingComponentBase,PagedRequestDto } from '@shared/paged-listing-component-base';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { Injector } from '@angular/core';
import { BreadCrumbComponent } from '@shared/components/common/bread-crumb/bread-crumb.component';
import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
import { BranchService } from '@app/service/api/categories/branch.service';
import { validEvents } from '@node_modules/@tinymce/tinymce-angular/editor/Events';
import { APP_ENUMS } from '@shared/AppEnums';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { TeamsFilterInputDto} from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { ReportSalaryDto } from '@app/service/model/report/reportSalary.dto';
import * as FileSaver from 'file-saver';
import { LevelService } from '@app/service/api/categories/level.service';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { startWithTap } from '@shared/helpers/observerHelper';
@Component({
  selector: 'app-report-salary',
  templateUrl: './report-salary.component.html',
  styleUrls: ['./report-salary.component.css']
})
export class ReportSalaryComponent extends PagedListingComponentBase<GetEmployeeDto> implements OnInit {
   

    constructor(injector: Injector,private employeeService:EmployeeService,
      private payrollService:PayRollService,
      private branchService:BranchService,
      private positionService:JobPositionService,
      private teamService: TeamService,
      private reportService: ReportService,
      private levelService: LevelService,
      ) { 
      super(injector);

    }
  public listPayrollWihStatusExecute: any;
  public listEmailEmployee: GetEmployeeBasicInfo[];
  public listUserType: any = [];
  public listBranch: any = [];
  public listPosition: any = [];
  public userLevelList: any = [];
  // public branchIds: number[];
   public userTypeIds: number[] = [];
   public jobPositionsId: number[];
  public payrollIds: number[];
  public employeeIds: number[];
  public requestItem: any;
  public listTeam: any = [];
  public levelIds: number[]=[];
  public levelPayslipIds :number[]= [];
  public jobPositionPayslipIds : number[]=[];
  public branchPayslipIds : number[] =[];
  public teamPayslipIds: number[] = [];
  public userTypePayslipIds: number[] = [];

  public defaultValue = {} as DefaulEmployeeFilterDto
  public resultList: ReportSalaryDto []= [];
  public dropdownFilterValue: number
  public dropdownMultiValueFilter: number[] = []
  public applyDates : any;
  public listApplyDate: any[] = [];
  public salaryByApplyDate: any;

  public allTotalSalary : number;

  @Output() onMultiFilterWithCondition? = new EventEmitter()
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum;
  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:'Report Salary'}];
      
      this.getPayrollWithStatusExecute();
      this.getAllEmployeeToSelect();
      this.getAllBranch();
      this.getAllPosition();
      this.getAllUserType();
      this.getAllTeam();
      this.getAllLevel();
  }
  
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.isLoading = true;
  
    let input = {
      teamIds: this.teamIds,
      teamPayslipIds: this.teamPayslipIds,
      branchIds: this.branchIds,
      userTypes: this.userTypeIds,
      userTypePayslips: this.userTypePayslipIds,
      jobPositionIds: this.jobPositionsId,
      payrollIds: this.payrollIds,
      employeeIds: this.employeeIds,
      levelIds: this.levelIds,
      levelPayslipIds: this.levelPayslipIds,
      jobPositionPayslipIds: this.jobPositionPayslipIds,
      branchPayslipIds: this.branchPayslipIds,
      gridParam: request
    } as any;
  
    this.requestItem = input;

    this.subscription.push(
      this.reportService.GetListReportSalary(input)
        .pipe(
          finalize(() => {
            this.isLoading = false; ;
            finishedCallback();
          })
        )
        .subscribe({
          next: (rs) => {
            this.resultList = rs.result.result.items;
            this.allTotalSalary = rs.result.allTotalSalary;         
            this.applyDates = rs.result.payroll;

         
          this.salaryByApplyDate = rs.result.result.items.reduce((acc, employee) => {
              employee.resultReports.forEach(report => {
                acc[report.payrollName] = acc[report.payrollName] || {};
                acc[report.payrollName][employee.infoEmployee.employeeId] = report.salary;
              });
              return acc;
            }, {});

            this.salaryByApplyDateWithoutPaging = rs.result.resultReport.reduce((acc, report) => {
              acc[report.payrollName] = (acc[report.payrollName] || 0) + report.salary;
              return acc;
            }, {});
            
            this.allTotalSalary = rs.result.resultReport.reduce((sum , report) => sum + report.salary,0);
            
            this.showPaging(rs.result.result, pageNumber)
          },
          error: (err) => {
            console.error('Error:', err);
          }
        })
    );
  }
    isAllowRoutingDetail(){
      return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail);
    }




    
  public getPayrollWithStatusExecute(){
    this.payrollService.GetPayrollWithStatusExecute().subscribe(res=>{
      this.listPayrollWihStatusExecute = res.result;
    })
  }

  public getAllEmployeeToSelect(){
    this.employeeService.getAllEmployeeToSelect().subscribe(res=>{
      this.listEmailEmployee = res.result;
    })
  }
 public getAllLevel() {
    this.subscription.push(this.levelService.getAll().subscribe(rs => {
      this.userLevelList = this.mapToFilter(rs.result, true)
    }))
  }
  private getAllBranch(){
    this.subscription.push(this.branchService.getAll().subscribe(res=>{
      this.listBranch = this.mapToFilter(res.result,true).map(item =>({
        name :  item.key,
        value: item.value,
        hidden: false
      }));
    }));
  }

  private getAllPosition(){
    this.subscription.push(this.positionService.getAll().subscribe(res=>{
      this.listPosition = this.mapToFilter(res.result,true).map(item =>({
        name :  item.key,
        value: item.value,
        hidden: false
      }));
    }));
  }

  private getAllUserType(){
    const listUserType = this.getListFormEnum(APP_ENUMS.UserType).filter(item => item.key != 'All');
    this.listUserType = listUserType.map(item =>({  
      name :  item.key,
      value: item.value,
      hidden: false
    }));
  }

  private getAllTeam(){
    this.subscription.push(this.teamService.getAll().subscribe(rs => {
      this.listTeam = this.mapToFilter(rs.result,true).map(item =>({  
        name :  item.key,
        value: item.value,

      }));  
    }))
  }
  onPayrollIdSelect(ids: number[]) {
    this.payrollIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onBranchSelect(ids: number[]) {
    this.branchIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onBranchPayslipEmplyeeSelect(ids: number[]) {
    this.branchPayslipIds = ids;
    this.onSearchEnter(this.searchText)
  } 


  onEmployeeSelect(ids: number[]) {
    this.employeeIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onUserTypeSelect(ids: number[]) {
    this.userTypeIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onUserTypeEmployeePayslipSelect(ids: number[]) {  
   this.userTypePayslipIds = ids;
   this.onSearchEnter(this.searchText)
  }

 onJobPositionSelect(ids: number[]) {
    this.jobPositionsId = ids;
    this.onSearchEnter(this.searchText)
  }
  onJobPositionPayslipEmplyeeSelect(ids: number[]) {
    this.jobPositionPayslipIds = ids;
    this.onSearchEnter(this.searchText)
  }

  onTableMultiSelectLevelIdFilter(ids: number[]) {
    this.levelIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onMultiSelectPayslipEmployeeLevelFilter(ids: number[]) {
    this.levelPayslipIds = ids;
    this.onSearchEnter(this.searchText)
  }
 
  onPayrollSelect(ids: number[]) {
    this.payrollIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onTeamSelect(ids: number[]) {
    this.teamIds = ids;
    this.onSearchEnter(this.searchText)
  }
  onTeamSelectForPaySlipEmployee(ids: number[]){
    this.teamPayslipIds = ids;
    this.onSearchEnter(this.searchText)
  }
   public onExport() {
      this.requestItem.gridParam.maxResultCount = 50000;
      this.subscription.push(
        this.reportService.ExportReportSalary(this.requestItem).subscribe((rs) => {
          console.log(rs.result)
          const file = new Blob([this.convertFile(atob(rs.result.base64))], {
            type: "application/vnd.ms-excel;charset=utf-8"
          });
          FileSaver.saveAs(file, `Export-ReportSalary.xlsx`)
        })
  
      )
    }
}

export interface DefaulEmployeeFilterDto {
  userType: any;
  userLevel: any;
  status: any;
  jobPosition: any;
  team: any;
  branch: any;
  gender: any;
  birthday: any;
}
