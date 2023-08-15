import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmMailDialogComponent } from './confirm-mail-dialog.component';

describe('ConfirmMailDialogComponent', () => {
  let component: ConfirmMailDialogComponent;
  let fixture: ComponentFixture<ConfirmMailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmMailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmMailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
