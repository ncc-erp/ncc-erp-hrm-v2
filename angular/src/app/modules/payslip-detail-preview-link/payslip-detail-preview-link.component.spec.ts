import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayslipDetailPreviewLinkComponent } from './payslip-detail-preview-link.component';

describe('PayslipDetailPreviewLinkComponent', () => {
  let component: PayslipDetailPreviewLinkComponent;
  let fixture: ComponentFixture<PayslipDetailPreviewLinkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayslipDetailPreviewLinkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayslipDetailPreviewLinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
