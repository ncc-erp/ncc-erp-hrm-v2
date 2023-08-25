import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkingHistoryEditNote } from './working-history-edit-note.component';

describe('WorkingHistoryEditNote', () => {
  let component: WorkingHistoryEditNote;
  let fixture: ComponentFixture<WorkingHistoryEditNote>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkingHistoryEditNote ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkingHistoryEditNote);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
