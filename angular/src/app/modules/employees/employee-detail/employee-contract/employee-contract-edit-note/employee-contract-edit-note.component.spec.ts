import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeContractEditNoteComponent } from './employee-contract-edit-note.component';

describe('EmployeeContractEditNoteComponent', () => {
  let component: EmployeeContractEditNoteComponent;
  let fixture: ComponentFixture<EmployeeContractEditNoteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeContractEditNoteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeContractEditNoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
