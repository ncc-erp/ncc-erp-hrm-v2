import { TestBed } from '@angular/core/testing';

import { PayslipService } from './payslip.service';

describe('PayslipService', () => {
  let service: PayslipService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PayslipService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
