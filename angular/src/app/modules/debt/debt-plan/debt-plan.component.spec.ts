import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DebtPlanComponent } from './debt-plan.component';

describe('DebtPlanComponent', () => {
  let component: DebtPlanComponent;
  let fixture: ComponentFixture<DebtPlanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DebtPlanComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DebtPlanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
