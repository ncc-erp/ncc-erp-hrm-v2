import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEmployeeToSalaryChangeRequestComponent } from './add-employee-to-salary-change-request.component';

describe('AddEmployeeToSalaryChangeRequestComponent', () => {
  let component: AddEmployeeToSalaryChangeRequestComponent;
  let fixture: ComponentFixture<AddEmployeeToSalaryChangeRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEmployeeToSalaryChangeRequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEmployeeToSalaryChangeRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
