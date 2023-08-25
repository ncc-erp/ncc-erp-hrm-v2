import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmChangeStatusDialogComponent } from './confirm-change-status-dialog.component';

describe('ConfirmChangeStatusDialogComponent', () => {
  let component: ConfirmChangeStatusDialogComponent;
  let fixture: ComponentFixture<ConfirmChangeStatusDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmChangeStatusDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmChangeStatusDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
