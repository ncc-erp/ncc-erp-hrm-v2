import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeContractComponent } from './employee-contract.component';

describe('EmployeeContractComponent', () => {
  let component: EmployeeContractComponent;
  let fixture: ComponentFixture<EmployeeContractComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeContractComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
