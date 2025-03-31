import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipTokenComponent } from './payslip-token.component';

describe('PayslipTokenComponent', () => {
  let component: PayslipTokenComponent;
  let fixture: ComponentFixture<PayslipTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipTokenComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
