import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEmployeeBonusDialogComponent } from './add-employee-bonus-dialog.component';

describe('AddEmployeeBonusDialogComponent', () => {
  let component: AddEmployeeBonusDialogComponent;
  let fixture: ComponentFixture<AddEmployeeBonusDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEmployeeBonusDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEmployeeBonusDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
