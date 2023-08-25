import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailSalaryComponent } from './payslip-detail-salary.component';

describe('PayslipDetailSalaryComponent', () => {
  let component: PayslipDetailSalaryComponent;
  let fixture: ComponentFixture<PayslipDetailSalaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailSalaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailSalaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
