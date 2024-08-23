import { Component, Injector } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { PayslipService } from "@app/service/api/payslip/payslip.service";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
  selector: "app-not-have-access",
  templateUrl: "./not-have-access.component.html",
  styleUrls: ["./not-have-access.component.css"],
})
export class NotHaveAccessComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
  ngOnInit(): void {}
}
