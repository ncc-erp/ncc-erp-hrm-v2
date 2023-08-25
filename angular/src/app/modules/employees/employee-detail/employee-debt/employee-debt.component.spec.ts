import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeDebtComponent } from './employee-debt.component';

describe('EmployeeDebtComponent', () => {
  let component: EmployeeDebtComponent;
  let fixture: ComponentFixture<EmployeeDebtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeDebtComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeDebtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
