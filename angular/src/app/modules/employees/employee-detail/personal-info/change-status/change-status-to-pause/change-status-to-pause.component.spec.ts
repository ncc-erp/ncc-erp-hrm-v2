import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeStatusToPauseComponent } from './change-status-to-pause.component';

describe('ChangeStatusToPauseComponent', () => {
  let component: ChangeStatusToPauseComponent;
  let fixture: ComponentFixture<ChangeStatusToPauseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeStatusToPauseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeStatusToPauseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
