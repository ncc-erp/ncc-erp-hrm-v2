import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeStatusToQuitComponent } from './change-status-to-quit.component';

describe('ChangeStatusToQuitComponent', () => {
  let component: ChangeStatusToQuitComponent;
  let fixture: ComponentFixture<ChangeStatusToQuitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeStatusToQuitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeStatusToQuitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
