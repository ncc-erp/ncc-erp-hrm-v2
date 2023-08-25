import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryChangeRequestListComponent } from './salary-change-request-list.component';

describe('SalaryChangeRequestListComponent', () => {
  let component: SalaryChangeRequestListComponent;
  let fixture: ComponentFixture<SalaryChangeRequestListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryChangeRequestListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryChangeRequestListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
