import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MultiCreateEmployeeFromTempComponent } from './multi-create-employee-from-temp.component';

describe('MultiCreateEmployeeFromTempComponent', () => {
  let component: MultiCreateEmployeeFromTempComponent;
  let fixture: ComponentFixture<MultiCreateEmployeeFromTempComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MultiCreateEmployeeFromTempComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MultiCreateEmployeeFromTempComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
