import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSalaryChangeRequestComponent } from './create-salary-change-request.component';

describe('CreateSalaryChangeRequestComponent', () => {
  let component: CreateSalaryChangeRequestComponent;
  let fixture: ComponentFixture<CreateSalaryChangeRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateSalaryChangeRequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateSalaryChangeRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
