import { TestBed } from '@angular/core/testing';

import { PayRollService } from './pay-roll.service';

describe('PayRollService', () => {
  let service: PayRollService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PayRollService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
