import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateBenefitDateDialogComponent } from './update-benefit-date-dialog.component';

describe('UpdateBenefitDateDialogComponent', () => {
  let component: UpdateBenefitDateDialogComponent;
  let fixture: ComponentFixture<UpdateBenefitDateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateBenefitDateDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateBenefitDateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
