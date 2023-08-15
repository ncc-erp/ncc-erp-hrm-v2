import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryChangeEditNoteComponent } from './salary-change-edit-note.component';

describe('SalaryChangeEditNoteComponent', () => {
  let component: SalaryChangeEditNoteComponent;
  let fixture: ComponentFixture<SalaryChangeEditNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryChangeEditNoteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryChangeEditNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
