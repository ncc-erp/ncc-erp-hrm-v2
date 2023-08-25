import { TestBed } from '@angular/core/testing';

import { SalariesService } from './salaries.service';

describe('SalariesService', () => {
  let service: SalariesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SalariesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
