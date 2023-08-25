import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListPunishmentRefundsComponent } from './list-punishment-refunds.component';

describe('ListPunishmentRefundsComponent', () => {
  let component: ListPunishmentRefundsComponent;
  let fixture: ComponentFixture<ListPunishmentRefundsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListPunishmentRefundsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListPunishmentRefundsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
