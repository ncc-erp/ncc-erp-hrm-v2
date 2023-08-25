import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailDebtComponent } from './payslip-detail-debt.component';

describe('PayslipDetailDebtComponent', () => {
  let component: PayslipDetailDebtComponent;
  let fixture: ComponentFixture<PayslipDetailDebtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailDebtComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailDebtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
