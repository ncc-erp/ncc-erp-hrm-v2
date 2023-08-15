import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BonusEmployeeComponent } from './bonus-employee.component';

describe('BonusEmployeeComponent', () => {
  let component: BonusEmployeeComponent;
  let fixture: ComponentFixture<BonusEmployeeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BonusEmployeeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BonusEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
