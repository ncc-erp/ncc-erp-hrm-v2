import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayrollTokenComponent } from '../payroll-token/payroll-token.component';

describe('PayrollTokenComponent', () => {
  let component: PayrollTokenComponent;
  let fixture: ComponentFixture<PayrollTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayrollTokenComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayrollTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
