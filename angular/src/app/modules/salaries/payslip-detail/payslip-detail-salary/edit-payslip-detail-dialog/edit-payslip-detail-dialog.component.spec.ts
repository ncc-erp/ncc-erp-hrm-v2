import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPayslipDetailDialogComponent } from './edit-payslip-detail-dialog.component';

describe('EditPayslipDetailDialogComponent', () => {
  let component: EditPayslipDetailDialogComponent;
  let fixture: ComponentFixture<EditPayslipDetailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditPayslipDetailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditPayslipDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
