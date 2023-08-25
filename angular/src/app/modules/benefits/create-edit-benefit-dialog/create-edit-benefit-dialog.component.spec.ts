import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBenefitDialogComponent } from './create-edit-benefit-dialog.component';

describe('CreateEditBenefitDialogComponent', () => {
  let component: CreateEditBenefitDialogComponent;
  let fixture: ComponentFixture<CreateEditBenefitDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditBenefitDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBenefitDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
