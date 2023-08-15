import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEmployeeContractComponent } from './edit-employee-contract.component';

describe('EditEmployeeContractComponent', () => {
  let component: EditEmployeeContractComponent;
  let fixture: ComponentFixture<EditEmployeeContractComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditEmployeeContractComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEmployeeContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
