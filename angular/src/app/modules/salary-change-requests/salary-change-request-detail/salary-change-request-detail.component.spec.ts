import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryChangeRequestDetailComponent } from './salary-change-request-detail.component';

describe('SalaryChangeRequestDetailComponent', () => {
  let component: SalaryChangeRequestDetailComponent;
  let fixture: ComponentFixture<SalaryChangeRequestDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryChangeRequestDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryChangeRequestDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
