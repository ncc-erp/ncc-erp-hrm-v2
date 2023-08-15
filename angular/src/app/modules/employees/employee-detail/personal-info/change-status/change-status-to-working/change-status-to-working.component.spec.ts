import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeStatusToWorkingComponent } from './change-status-to-working.component';

describe('ChangeStatusToWorkingComponent', () => {
  let component: ChangeStatusToWorkingComponent;
  let fixture: ComponentFixture<ChangeStatusToWorkingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeStatusToWorkingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeStatusToWorkingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
