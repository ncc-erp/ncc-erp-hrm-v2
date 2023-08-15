import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeePunishmentComponent } from './employee-punishment.component';

describe('EmployeePunishmentComponent', () => {
  let component: EmployeePunishmentComponent;
  let fixture: ComponentFixture<EmployeePunishmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeePunishmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeePunishmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
