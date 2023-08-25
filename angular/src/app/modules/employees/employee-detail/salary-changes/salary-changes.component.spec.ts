import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryChangesComponent } from './salary-changes.component';

describe('SalaryChangesComponent', () => {
  let component: SalaryChangesComponent;
  let fixture: ComponentFixture<SalaryChangesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryChangesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryChangesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
