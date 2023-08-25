import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { PunishmentService } from '@app/service/api/punishment/punishment.service';
import { CreatePayslipDetailDto, CreatePayslipDetailPunishementDto, PayslipDetailByTypeDto, UpdatePayslipDetailDto } from '@app/service/model/payslip/payslip.dto';
import {  PunishmentEmployeeDto, PunishmentsDto } from '@app/service/model/punishments/punishments.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'app-payslip-detail-punishment',
  templateUrl: './payslip-detail-punishment.component.html',
  styleUrls: ['./payslip-detail-punishment.component.css']
})
export class PayslipDetailPunishmentComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private punishmentService: PunishmentService,
    public router: Router,
    public route: ActivatedRoute,
    public payslipService : PayslipService
  ) {
    super(injector)
  }
  public listPayslipDetailPunishments: PayslipDetailByTypeDto[] = [];
  public listPunishments: PunishmentsDto[] = [];
  public listPunishmentsIsActiveInMonth: PunishmentsDto[] = [];
  public punishment = {} as PunishmentsDto;
  public searchPunishment: string = "";
  public isEditing: boolean = false;
  public isAdding: boolean = false;
  public employeeId: number = 0;
  public payslipId: number = 0;
  public payrollId:number = 0;
  private payrollStatus:number

  ngOnInit(): void {
    this.employeeId = Number(this.route.snapshot.queryParamMap.get('employeeId'));
    this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.payrollStatus = Number(this.route.snapshot.queryParamMap.get('status'));
    this.payrollId = Number(this.route.snapshot.queryParamMap.get('payrollId'));
    this.getAllPayslipPunishment();
    this.GetAllActivePunishmentInMonth();
  }

  public GetAllActivePunishmentInMonth()
  {
    this.isLoading = true;AppComponentBase
    this.subscription.push(
      this.payslipService.GetAvailablePunishmentsInMonth(this.payslipId).subscribe((rs) => {
        this.listPunishments = rs.result;
        this.isLoading = false;
      }, ()=> this.isLoading = false)
    )
  }

  public onAdd() {
    let ps = {} as PayslipDetailByTypeDto;
    ps.createMode = true;
    this.isAdding = true;
    this.listPayslipDetailPunishments.unshift(ps);
  }

  public onUpdate(ps: PunishmentEmployeeDto) {
    ps.createMode = true;
    this.isEditing = true
  }

  public getAllPayslipPunishment() {
    this.isLoading = true;
    this.subscription.push(
      this.payslipService.GetPayslipDetailByType(this.payslipId , APP_ENUMS.ESalaryType.Punishment).subscribe((rs) => {
        this.listPayslipDetailPunishments = rs.result;
        this.isLoading = false;
      },()=> this.isLoading = false)
    )
  }

  public onSave(ps: PayslipDetailByTypeDto) {

    delete ps['createMode']
    if (this.isEditing) {
      this.edit(ps);
    } else {
      this.create(ps);
    }
  }

  public create(ps: PayslipDetailByTypeDto){
    var input = { } as CreatePayslipDetailPunishementDto;
    input.payslipId = this.payslipId
    input.note = this.punishment.name
    input.money = ps.money
    input.PunishmentId = this.punishment.id;
    this.subscription.push(     
      this.payslipService.CreatePayslipDetailPunishmentAndCreateEmployeePunishment(input).subscribe((rs)=>{
        ps.createMode = false;
        this.isAdding = false;
        this.getAllPayslipPunishment();
        this.GetAllActivePunishmentInMonth();
        this.notify.success(rs.result)
      },()=> ps.createMode = true)
    )
  }

  public edit(ps: PayslipDetailByTypeDto){
    var input = {} as UpdatePayslipDetailDto;
    input.note = ps.note;
    input.money = ps.money;
    input.id = ps.id;
    this.subscription.push(
      this.payslipService.UpdatePayslipDetailPunishment(input).subscribe((rs)=>{
        ps.createMode = false;
        this.isEditing = false;
        this.getAllPayslipPunishment();
        this.GetAllActivePunishmentInMonth();
        abp.notify.success(rs.result)
      },()=> ps.createMode = true)
    )
    
  }

  public onDelete(input: PayslipDetailByTypeDto) {
    abp.message.confirm(`Delete payslip detail with Id = ${input.id}?`, "", (result) => {
      if (result) {
        this.payslipService.DeletePayslipDetail(input.id).subscribe(rs => {
          abp.notify.success(`Deleted  payslip detail with Id = ${input.id}`);
          this.getAllPayslipPunishment();
          this.GetAllActivePunishmentInMonth();
        })
      }
    })
  }


  public onCancel(ps: PayslipDetailByTypeDto) {
    ps.createMode = false;
    this.isEditing = false;
    this.isAdding = false;
    this.getAllPayslipPunishment();
  }

  
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_Add);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_Delete);
  }
  
  isViewTabPunishment(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_View);
  } 

  isShowActions(){
    return this.payrollStatus == APP_ENUMS.PayrollStatus.New || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT;
  }

}
