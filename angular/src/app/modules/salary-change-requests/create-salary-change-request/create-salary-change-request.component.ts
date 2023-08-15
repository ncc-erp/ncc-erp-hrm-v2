import { Component, Injector, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';
import { SalaryChangeRequestService } from '../../../service/api/salary-change-request/salary-change-request.service';
import { CreateSalaryChangeRequestDto } from '../../../service/model/salary-change-request/CreateSalaryChangeRequestDto';
import { GetSalaryChangeRequestDto } from '../../../service/model/salary-change-request/GetSalaryChangeRequestDto';
const MONTH_YEAR = {
  parse: {
    dateInput: 'MM/YYYY',
  },
  display: {
    dateInput: 'MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
}
@Component({
  selector: 'app-create-salary-change-request',
  templateUrl: './create-salary-change-request.component.html',
  styleUrls: ['./create-salary-change-request.component.css'],
  providers: [{
    provide: DateAdapter,
    useClass: MomentDateAdapter,
    deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
  },
  { provide: MAT_DATE_FORMATS, useValue: MONTH_YEAR },
  ],
})
export class CreateSalaryChangeRequestComponent extends DialogComponentBase<GetSalaryChangeRequestDto> implements OnInit {
  public salaryChangeRequest: GetSalaryChangeRequestDto = {} as GetSalaryChangeRequestDto
  constructor(injector: Injector, private salaryChangeRequestService: SalaryChangeRequestService) {
    super(injector)
  }
  public date:FormControl = new FormControl(moment(),Validators.required);
  ngOnInit(): void {
    if (this.dialogData) {
      this.salaryChangeRequest = this.dialogData
      this.title = "Edit request " + this.salaryChangeRequest.name;
      this.date.setValue(moment(this.dialogData.applyMonth))
    } else {
      this.title = "Create new salary change request"
    }
  }
  chosenYearHandler(normalizedYear: moment.Moment) {
    const ctrlValue = this.date.value;
    ctrlValue.year(normalizedYear.year());
    this.date.setValue(ctrlValue);
  }

  chosenMonthHandler(normalizedMonth: moment.Moment, datepicker: MatDatepicker<moment.Moment>) {
    const ctrlValue = this.date.value;
    ctrlValue.month(normalizedMonth.month());
    this.date.setValue(ctrlValue);
    datepicker.close();
  }
  saveAndClose() {
    this.salaryChangeRequest.applyMonth = this.formatDateYMD(moment(this.date.value).startOf('month'));
    if (this.salaryChangeRequest.id) {
      this.subscription.push(
        this.salaryChangeRequestService.update(this.salaryChangeRequest).subscribe(rs => {
          this.notify.success("Salary change request updated")
          this.dialogRef.close()
        })
      )
    } else {
      this.subscription.push(
        this.salaryChangeRequestService.create(this.salaryChangeRequest as CreateSalaryChangeRequestDto).subscribe(rs => {
          this.notify.success("Salary change request created")
          this.dialogRef.close()
        })
      )
    }
  }
}
