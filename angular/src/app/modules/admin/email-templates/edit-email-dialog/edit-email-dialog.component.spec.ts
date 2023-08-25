import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEmailDialogComponent } from './edit-email-dialog.component';

describe('EditEmailDialogComponent', () => {
  let component: EditEmailDialogComponent;
  let fixture: ComponentFixture<EditEmailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditEmailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEmailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
