import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailPreviewComponent } from './payslip-detail-preview.component';

describe('PayslipDetailPreviewComponent', () => {
  let component: PayslipDetailPreviewComponent;
  let fixture: ComponentFixture<PayslipDetailPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailPreviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
