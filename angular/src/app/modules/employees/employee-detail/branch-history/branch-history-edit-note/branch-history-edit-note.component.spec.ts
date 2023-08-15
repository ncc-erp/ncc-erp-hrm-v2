import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BranchHistoryEditNoteComponent } from './branch-history-edit-note.component';

describe('BranchHistoryEditNoteComponent', () => {
  let component: BranchHistoryEditNoteComponent;
  let fixture: ComponentFixture<BranchHistoryEditNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BranchHistoryEditNoteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BranchHistoryEditNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
