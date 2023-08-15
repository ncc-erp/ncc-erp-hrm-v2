import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeBonusComponent } from './employee-bonus.component';

describe('EmployeeBonusComponent', () => {
  let component: EmployeeBonusComponent;
  let fixture: ComponentFixture<EmployeeBonusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeeBonusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeBonusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
