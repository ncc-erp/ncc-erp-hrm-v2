import { Component, OnInit, Injector } from '@angular/core';
import { DialogComponentBase } from '../../../../../shared/dialog-component-base';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { GetEmployeeBasicInfo } from '@app/service/model/employee/employee.dto';
import { property, zip } from 'lodash-es';
import { MatDialogRef } from '@angular/material/dialog';
import { GetPayslipEmployeeDto } from '@app/service/model/payslip/payslip.dto';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-penalty-user-dialog',
  templateUrl: './penalty-user-dialog.component.html',
  styleUrls: ['./penalty-user-dialog.component.css']
})
export class PenaltyUserDialogComponent extends DialogComponentBase<any>  implements OnInit {
public ListPenaltyEmployees:GetPayslipEmployeeDto[];
private pagedlisting: PagedListingComponentBase<any>
id:number;
sortProperty: string;
sortDirection: number;
  constructor(injector: Injector,
    public dialogref: MatDialogRef<PenaltyUserDialogComponent>,
    private payslipService:PayslipService
    ) {
    super(injector);
    
    }

  ngOnInit(): void {
    Object.assign(this,this.dialogData)
    this.title = "Phạt không thu được";
    this.GetPenalty()
  }

  public GetPenalty()
  {
    this.payslipService.GetAllPenaltyNotCollected(this.id).subscribe((res)=> {
    this.ListPenaltyEmployees = res.result
    })
  }

  public onSortChange(columnName: string) { 
      // Kiểm tra tên cột và xác định hướng sắp xếp
      let sortDirection: number = 0;
      if (columnName === this.sortProperty && this.sortDirection === 0) {
        sortDirection = 1;
      }
  
      // Cập nhật thuộc tính sortProperty và sortDirection
      this.sortProperty = columnName;
      this.sortDirection = sortDirection;
  
      // Sắp xếp dữ liệu theo tên cột và hướng sắp xếp
      this.ListPenaltyEmployees.sort((a, b) => {
        const valueA = a[columnName];
        const valueB = b[columnName];
  
        if (valueA < valueB) {
          return sortDirection === 0 ? -1 : 1;
        } else if (valueA > valueB) {
          return sortDirection === 0 ? 1 : -1;
        } else {
          return 0;
        }
      });
;
}
  
onClose(){
  this.dialogRef.close();
}
  
  

  isAllowRoutingDetail() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }

  public columnList = [
    {
      name: "no",
      displayName: "#",
      sortable: false,
      className: "",
      width: 50
    },
    {
      name: "email",
      displayName: "Employee",
      sortable: true,
      className: "",
      width: 400
    },
    {
      name: "realSalary",
      displayName: "Money",
      sortable: true,
      className: "",
      width: 100
    }
  ]
}
