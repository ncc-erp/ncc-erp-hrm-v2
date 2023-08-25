import { ExportEmployeeDialogComponent } from './export-employee-dialog/export-employee-dialog.component';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { FilterDto, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { Router } from '@angular/router';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { UploadAvatarComponent } from '../upload-avatar/upload-avatar.component';
import { ListEmployeeComponent } from '@shared/components/employee/list-employee/list-employee.component';
import { SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { MatMenuTrigger } from '@angular/material/menu';
import { EmployeeFilterComponent } from '@shared/components/employee/employee-filter/employee-filter.component';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { error } from 'console';
@Component({
  selector: 'app-employees',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  providers: [{ provide: MAT_DIALOG_DATA, useValue: {} },]

})
export class EmployeeListComponent extends PagedListingComponentBase<GetEmployeeDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    throw new Error('Method not implemented.');
  }
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  @ViewChild('listEmployee') listEmployeeComp: ListEmployeeComponent;
  @ViewChild('employeeFilter') employeeFilterComp: EmployeeFilterComponent;
  constructor(injector: Injector,private employeeService:EmployeeService) {
    super(injector);
  }
  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:'Employee'}];
  }
  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_Create);
  }
  isShowExportBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_Export);
  }
  isShowDownCreateTemp(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_DownloadCreateTemplate);
  }
  isShowCreateEmpByFile(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_CreateEmployeeByFile);
  }
  isShowDownUpdateTemp(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_DownloadUpdateTemplate);
  }
  isShowUpdateEmpByFile(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_UpdateEmployeeByFile);
  }
  isShowImportEmployee(){
    return this.isShowDownCreateTemp() || this.isShowDownUpdateTemp()
    || this.isShowCreateEmpByFile() || this.isShowUpdateEmpByFile();
  }
  public onFilter(filterItem: FilterDto): void {
    this.listEmployeeComp.onFilter(filterItem)
  }

  public onMultiFilterWithCondition(teamsFilterInput: TeamsFilterInputDto): void {
    this.listEmployeeComp?.onMultiFilterWithCondition(teamsFilterInput);
  }

  public onMultiFilter(listData): void {
    this.listEmployeeComp.onMultiFilter(listData);
  }

  public onFilterBySeniority(listData: SeniorityFilterDto): void {
    this.listEmployeeComp.onFilterBySeniority(listData);
  }

  public onDateSelectorChange(filterValue){
    this.listEmployeeComp?.onDateSelectorChange(filterValue);
  }

  public onUpdateAvatar(employee) {
    const dialogRef = this.dialog.open(UploadAvatarComponent, {
      width: '600px',
      data: employee.id
    })
    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.employeeService.uploadAvatar(res , employee.id).subscribe(data => {
          if (data) {
            this.notify.success('Upload Avatar Successfully!');
            this.refresh();
            employee.avatar = data.result;
          } else { this.notify.error('Upload Avatar Failed!'); }
        });
      }
    });
  }

  onSearchEnter(searchText: string) {
    this.listEmployeeComp.onSearchEnter(searchText)
  }
  public navigateHome() {
    this.router.navigate(['/app/home'])
  }
  public onCreate(){
    this.router.navigate(['/app/employees/create'])
  }
  public OnSyncToAllEmployees() {
    this.isLoading = true;

    this.employeeService.SyncAllEmployees().subscribe(() => {
      abp.notify.success(this.l('Synced successfully'));
      this.isLoading = false;      
    }, (error) => this.isLoading = false);
  }

  public onExport(){
    this.listEmployeeComp.onExport();
  }
  public exportStatistic(){

    let ref = this.dialog.open(ExportEmployeeDialogComponent, {
      width: "500px"
    })

    ref.afterClosed().subscribe(rs => {
      if(rs){
        this.listEmployeeComp.exportStatistic(rs.startDate, rs.endDate);
      }
    })


  }

  public onCreateEmployeeFromFile(){
    this.listEmployeeComp.onCreateEmployeeFromFile();
  }
  public onUpdateEmployeeFromFile(){
    this.listEmployeeComp.onUpdateEmployeeFromFile();
  }
  public getCreateTemp(){
    this.listEmployeeComp.getMetaDataToCreate();
  }
  public getUpdateTemp(){
    this.listEmployeeComp.getMetaDataToUpdate();
  }
}
