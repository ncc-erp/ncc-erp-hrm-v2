import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { BonusDto, BonusEmployeeDto} from '@app/service/model/bonuses/bonus.dto';
import { CreatePayslipDetailDto, CreatePayslipBonusDto, PayslipDetailByTypeDto, UpdatePayslipDetailDto } from '@app/service/model/payslip/payslip.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'app-payslip-detail-bonus',
  templateUrl: './payslip-detail-bonus.component.html',
  styleUrls: ['./payslip-detail-bonus.component.css']
})
export class PayslipDetailBonusComponent extends AppComponentBase implements OnInit {

  public listPayslipDetailBonuses: PayslipDetailByTypeDto[] = [];
  public listBonuses:BonusDto[] = [];
  public bonusEmp = {} as BonusDto;
  public searchBonus:string = "";
  public isEditing: boolean = false;
  public isAdding: boolean = false;
  public employeeId: number = 0;
  public payslipId: number = 0;
  private payrollStatus: number;

  constructor(
    injector: Injector,
    private bonusService: BonusService,
    public route: ActivatedRoute,
    public payslipService: PayslipService
    ) 
    { 
      super(injector)
    }

  ngOnInit(): void {
    this.employeeId = Number(this.route.snapshot.queryParamMap.get('employeeId'));
    this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.payrollStatus = Number(this.route.snapshot.queryParamMap.get('status'));

    this.getAllBonuses();
    this.getAllPayslipBonuses();
  }

  public getAllPayslipBonuses() {
    this.isLoading = true;
    this.subscription.push(
      this.payslipService.GetPayslipDetailByType(this.payslipId , APP_ENUMS.ESalaryType.Bonus).subscribe((rs) => {
        this.listPayslipDetailBonuses = rs.result;
        this.isLoading = false;
      },()=> this.isLoading = false)
    )
  }

  public onAdd(){
    let ps = {} as PayslipDetailByTypeDto;
    ps.createMode = true;
    this.isAdding = true;
    this.listPayslipDetailBonuses.unshift(ps);
  }

  public onUpdate(ps: PayslipDetailByTypeDto){
    ps.createMode = true;
    this.isEditing = true
  }

  public getAllBonuses(){
    this.subscription.push(
      this.payslipService.getAvailableBonuses(this.payslipId).subscribe((rs)=>{
        this.listBonuses = rs.result;
      })
    )
  }

  public onSave(ps: PayslipDetailByTypeDto) {
    if (this.isEditing) {
      this.edit(ps);
    } else {
        this.create(ps);
    }

  }
  
  public create(ps: PayslipDetailByTypeDto){
    var input = { } as CreatePayslipBonusDto;
    input.payslipId = this.payslipId
    input.bonusId = this.bonusEmp.id
    input.note = this.bonusEmp.name
    input.money = ps.money

    this.subscription.push(
      this.payslipService.CreatePayslipDetailBonus(input).subscribe((rs)=>{
        abp.notify.success(rs.result);
        ps.createMode = false;
        this.isAdding = false;
        this.getAllBonuses();
        this.getAllPayslipBonuses();
      },()=> ps.createMode = true)
    )
  }

  public edit(ps: PayslipDetailByTypeDto){
    var input = {} as UpdatePayslipDetailDto;
    input.note = ps.note;
    input.money = ps.money;
    input.id = ps.id;

    this.subscription.push(
      this.payslipService.UpdatePayslipDetailBonus(input).subscribe((rs)=>{
        abp.notify.success(rs.result);
        ps.createMode = false;
        this.isEditing = false;
        this.getAllPayslipBonuses();

      },()=> ps.createMode = true)
    )
    
  }

  public onDelete(ps: PayslipDetailByTypeDto) {
    abp.message.confirm(`Delete payslip detail with Id = ${ps.id}?`,"", (result)=>{
      if(result){
        this.payslipService.DeletePayslipDetail(ps.id).subscribe(rs => {
          abp.notify.success(`Deleted payslip detail with Id = ${ps.id}`);
          this.getAllBonuses();
          this.getAllPayslipBonuses();
        })
      }
    }
   )
  }


  public onCancel(ps: PayslipDetailByTypeDto){
    ps.createMode = false;
    this.isEditing = false;
    this.isAdding = false;
    this.getAllPayslipBonuses();;
  }
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_Add);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_Delete);
  }
  isViewTabBonus(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_View);
  } 
  isShowActions(){
    return this.payrollStatus == APP_ENUMS.PayrollStatus.New  || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT;
  }
}
