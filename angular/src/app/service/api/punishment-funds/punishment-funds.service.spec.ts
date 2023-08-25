import { TestBed } from '@angular/core/testing';

import { PunishmentFundsService } from './punishment-funds.service';

describe('PunishmentFundsService', () => {
  let service: PunishmentFundsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PunishmentFundsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
