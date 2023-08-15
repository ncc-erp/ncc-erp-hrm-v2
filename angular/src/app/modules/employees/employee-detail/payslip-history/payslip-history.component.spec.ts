import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipHistoryComponent } from './payslip-history.component';

describe('PayslipHistoryComponent', () => {
  let component: PayslipHistoryComponent;
  let fixture: ComponentFixture<PayslipHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipHistoryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
