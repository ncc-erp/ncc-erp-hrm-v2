import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeStatusToMaternityLeaveComponent } from './change-status-to-maternity-leave.component';

describe('ChangeStatusToMaternityLeaveComponent', () => {
  let component: ChangeStatusToMaternityLeaveComponent;
  let fixture: ComponentFixture<ChangeStatusToMaternityLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeStatusToMaternityLeaveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeStatusToMaternityLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
