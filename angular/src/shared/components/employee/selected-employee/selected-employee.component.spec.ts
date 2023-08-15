import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectedEmployeeComponent } from './selected-employee.component';

describe('SelectedEmployeeComponent', () => {
  let component: SelectedEmployeeComponent;
  let fixture: ComponentFixture<SelectedEmployeeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectedEmployeeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectedEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
