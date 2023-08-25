import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditIssuedByDialogComponent } from './create-edit-issued-by-dialog.component';

describe('CreateEditIssuedByDialogComponent', () => {
  let component: CreateEditIssuedByDialogComponent;
  let fixture: ComponentFixture<CreateEditIssuedByDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditIssuedByDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditIssuedByDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});