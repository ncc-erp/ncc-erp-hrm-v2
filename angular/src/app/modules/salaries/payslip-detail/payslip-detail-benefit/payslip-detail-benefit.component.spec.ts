import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailBenefitComponent } from './payslip-detail-benefit.component';

describe('PayslipDetailBenefitComponent', () => {
  let component: PayslipDetailBenefitComponent;
  let fixture: ComponentFixture<PayslipDetailBenefitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailBenefitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailBenefitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
