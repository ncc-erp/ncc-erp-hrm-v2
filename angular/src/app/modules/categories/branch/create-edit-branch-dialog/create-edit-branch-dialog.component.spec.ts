import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBranchDialogComponent } from './create-edit-branch-dialog.component';

describe('CreateEditBranchDialogComponent', () => {
  let component: CreateEditBranchDialogComponent;
  let fixture: ComponentFixture<CreateEditBranchDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditBranchDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBranchDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
