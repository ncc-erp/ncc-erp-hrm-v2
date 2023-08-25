import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenefitEmployeeComponent } from './benefit-employee.component';

describe('BenefitEmployeeComponent', () => {
  let component: BenefitEmployeeComponent;
  let fixture: ComponentFixture<BenefitEmployeeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BenefitEmployeeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BenefitEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
