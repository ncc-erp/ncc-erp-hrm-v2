import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryChangesEditDialogComponent } from './salary-changes-edit-dialog.component';

describe('SalaryChangesEditDialogComponent', () => {
  let component: SalaryChangesEditDialogComponent;
  let fixture: ComponentFixture<SalaryChangesEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryChangesEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryChangesEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
