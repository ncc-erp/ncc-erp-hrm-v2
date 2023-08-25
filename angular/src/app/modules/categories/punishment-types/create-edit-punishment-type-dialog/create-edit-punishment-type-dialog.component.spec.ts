import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditPunishmentTypeDialogComponent } from './create-edit-punishment-type-dialog.component';

describe('CreateEditPunishmentTypeDialogComponent', () => {
  let component: CreateEditPunishmentTypeDialogComponent;
  let fixture: ComponentFixture<CreateEditPunishmentTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditPunishmentTypeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditPunishmentTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
