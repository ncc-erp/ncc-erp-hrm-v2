import { TestBed } from '@angular/core/testing';

import { PayrollTokenServiceService } from './payroll-token-service.service';

describe('PayrollTokenServiceService', () => {
  let service: PayrollTokenServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PayrollTokenServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
