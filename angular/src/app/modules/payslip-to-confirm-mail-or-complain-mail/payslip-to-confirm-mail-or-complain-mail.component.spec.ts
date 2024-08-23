import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipToConfirmMailOrComplainMailComponent } from './payslip-to-confirm-mail-or-complain-mail.component';

describe('PayslipToConfirmMailOrComplainMailComponent', () => {
  let component: PayslipToConfirmMailOrComplainMailComponent;
  let fixture: ComponentFixture<PayslipToConfirmMailOrComplainMailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipToConfirmMailOrComplainMailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipToConfirmMailOrComplainMailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
