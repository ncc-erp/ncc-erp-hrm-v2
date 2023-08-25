import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddBenefitEmployeeDialogComponent } from './add-benefit-employee-dialog.component';

describe('AddBenefitEmployeeDialogComponent', () => {
  let component: AddBenefitEmployeeDialogComponent;
  let fixture: ComponentFixture<AddBenefitEmployeeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddBenefitEmployeeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddBenefitEmployeeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
