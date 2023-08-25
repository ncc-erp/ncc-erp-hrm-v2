import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditTeamDialogComponent } from './create-edit-team-dialog.component';

describe('CreateEditTeamDialogComponent', () => {
  let component: CreateEditTeamDialogComponent;
  let fixture: ComponentFixture<CreateEditTeamDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditTeamDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditTeamDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
