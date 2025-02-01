import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendDirectMessageToUserComponent } from './send-direct-message-touser.component';

describe('ConfirmMailDialogComponent', () => {
  let component: SendDirectMessageToUserComponent;
  let fixture: ComponentFixture<SendDirectMessageToUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SendDirectMessageToUserComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SendDirectMessageToUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
