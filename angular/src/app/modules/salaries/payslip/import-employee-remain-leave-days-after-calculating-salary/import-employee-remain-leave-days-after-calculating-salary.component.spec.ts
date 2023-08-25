import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent } from './import-employee-remain-leave-days-after-calculating-salary.component';

describe('ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent', () => {
  let component: ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent;
  let fixture: ComponentFixture<ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
