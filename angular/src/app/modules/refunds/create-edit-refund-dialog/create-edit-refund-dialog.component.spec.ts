import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditRefundDialogComponent } from './create-edit-refund-dialog.component';

describe('CreateEditRefundDialogComponent', () => {
  let component: CreateEditRefundDialogComponent;
  let fixture: ComponentFixture<CreateEditRefundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditRefundDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditRefundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
