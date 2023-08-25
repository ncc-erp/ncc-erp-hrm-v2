import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditUserTypeDialogComponent } from './create-edit-user-type-dialog.component';

describe('CreateEditUserTypeDialogComponent', () => {
  let component: CreateEditUserTypeDialogComponent;
  let fixture: ComponentFixture<CreateEditUserTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditUserTypeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditUserTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
