import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkingHistoryComponent } from './working-history.component';

describe('WorkingHistoryComponent', () => {
  let component: WorkingHistoryComponent;
  let fixture: ComponentFixture<WorkingHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkingHistoryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkingHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
