import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyPayrollComponent } from './apply-payroll.component';

describe('ApplyPayrollComponent', () => {
  let component: ApplyPayrollComponent;
  let fixture: ComponentFixture<ApplyPayrollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApplyPayrollComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplyPayrollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
