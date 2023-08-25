import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBonusDialogComponent } from './create-edit-bonus-dialog.component';

describe('CreateEditBonusDialogComponent', () => {
  let component: CreateEditBonusDialogComponent;
  let fixture: ComponentFixture<CreateEditBonusDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditBonusDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBonusDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
