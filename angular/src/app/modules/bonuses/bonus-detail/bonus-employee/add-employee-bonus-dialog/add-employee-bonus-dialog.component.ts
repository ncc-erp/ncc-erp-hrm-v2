import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { BonusDto, AddBonusEmployeeDto } from '@app/service/model/bonuses/bonus.dto';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-add-employee-bonus-dialog',
  templateUrl: './add-employee-bonus-dialog.component.html',
  styleUrls: ['./add-employee-bonus-dialog.component.css']
})

export class AddEmployeeBonusDialogComponent extends DialogComponentBase<BonusDto> implements OnInit {
  constructor(injector: Injector, 
    private bonusService: BonusService,
    private dialog: MatDialog) {
    super(injector)
  }
  public bonusEmployee = {} as AddBonusEmployeeDto;
  public bonus = {} as BonusDto;
  public addedEmployeeIds : number[] = [];
  ngOnInit(): void {
    this.title = `Add employee to bonus <strong>${this.dialogData.name}</strong>`
    this.bonusEmployee.bonusId = this.dialogData.id;
    this.bonusEmployee.note = this.dialogData.name;
  }

  stepUpdateNoteMoney() {
    this.addEmployee();
  }

  private async getAllEmployeeInBonus() {
    await this.bonusService.getAllEmployeeInBonus(this.bonusEmployee.bonusId).toPromise().then(rs => {
      this.addedEmployeeIds = rs.result
    })
  }

  private async addEmployee(){
    await this.getAllEmployeeInBonus();
    this.getAllEmployeeInBonus();
    var ref = this.dialog.open(AddEmployeeComponent,{
      width: "92vw",
      height: "95vh",
      maxWidth: "100vw",
      data: {
        title: `Add employee to bonus <strong>${this.dialogData.name}</strong>`,
        addedEmployeeIds:  this.addedEmployeeIds
      }
    })
    ref.afterClosed().subscribe((res) => {
      if(res && res.length){
        this.bonusEmployee.employeeIds = res;
        this.bonusService.multipleAddEmployeeToBonus( this.bonusEmployee).subscribe(res => {
          abp.notify.success("Add employee to bonus successful");
          this.dialogRef.close(true)
        })
      }
    })
  }
}
