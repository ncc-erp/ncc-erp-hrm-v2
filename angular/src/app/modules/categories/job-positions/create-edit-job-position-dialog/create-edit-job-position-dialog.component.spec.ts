import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditJobPositionDialogComponent } from './create-edit-job-position-dialog.component';

describe('CreateEditJobPositionDialogComponent', () => {
  let component: CreateEditJobPositionDialogComponent;
  let fixture: ComponentFixture<CreateEditJobPositionDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditJobPositionDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditJobPositionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
