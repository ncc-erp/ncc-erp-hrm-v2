import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailBonusComponent } from './payslip-detail-bonus.component';

describe('PayslipDetailBonusComponent', () => {
  let component: PayslipDetailBonusComponent;
  let fixture: ComponentFixture<PayslipDetailBonusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailBonusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailBonusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
