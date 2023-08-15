import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdatePayslipDeadlineDialogComponent } from './update-payslip-deadline-dialog.component';

describe('UpdatePayslipDeadlineDialogComponent', () => {
  let component: UpdatePayslipDeadlineDialogComponent;
  let fixture: ComponentFixture<UpdatePayslipDeadlineDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdatePayslipDeadlineDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdatePayslipDeadlineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
