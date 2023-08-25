import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateEmployeeBackDateComponent } from './update-employee-back-date.component';

describe('UpdateEmployeeBackDateComponent', () => {
  let component: UpdateEmployeeBackDateComponent;
  let fixture: ComponentFixture<UpdateEmployeeBackDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateEmployeeBackDateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateEmployeeBackDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
