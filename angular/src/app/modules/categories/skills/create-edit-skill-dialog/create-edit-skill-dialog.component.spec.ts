import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditSkillDialogComponent } from './create-edit-skill-dialog.component';

describe('CreateEditSkillDialogComponent', () => {
  let component: CreateEditSkillDialogComponent;
  let fixture: ComponentFixture<CreateEditSkillDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditSkillDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditSkillDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
