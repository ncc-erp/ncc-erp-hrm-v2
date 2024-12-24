import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewAddBenefitEmployeeDialogComponent } from './review-add-benefit-employee-dialog.component';

describe('ReviewAddBenefitEmployeeDialogComponent', () => {
  let component: ReviewAddBenefitEmployeeDialogComponent;
  let fixture: ComponentFixture<ReviewAddBenefitEmployeeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReviewAddBenefitEmployeeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReviewAddBenefitEmployeeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
