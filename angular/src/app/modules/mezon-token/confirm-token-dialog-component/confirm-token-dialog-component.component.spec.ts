import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmTokenDialogComponentComponent } from './confirm-token-dialog-component.component';

describe('ConfirmTokenDialogComponentComponent', () => {
  let component: ConfirmTokenDialogComponentComponent;
  let fixture: ComponentFixture<ConfirmTokenDialogComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConfirmTokenDialogComponentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmTokenDialogComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
