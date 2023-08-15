import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkingHistoryEditDateComponent } from './working-history-edit-date.component';

describe('WorkingHistoryEditDateComponent', () => {
  let component: WorkingHistoryEditDateComponent;
  let fixture: ComponentFixture<WorkingHistoryEditDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkingHistoryEditDateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkingHistoryEditDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
