import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditLevelDialogComponent } from './create-edit-level-dialog.component';

describe('CreateEditLevelDialogComponent', () => {
  let component: CreateEditLevelDialogComponent;
  let fixture: ComponentFixture<CreateEditLevelDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditLevelDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditLevelDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
