import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailPunishmentComponent } from './payslip-detail-punishment.component';

describe('PayslipDetailPunishmentComponent', () => {
  let component: PayslipDetailPunishmentComponent;
  let fixture: ComponentFixture<PayslipDetailPunishmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailPunishmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailPunishmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
