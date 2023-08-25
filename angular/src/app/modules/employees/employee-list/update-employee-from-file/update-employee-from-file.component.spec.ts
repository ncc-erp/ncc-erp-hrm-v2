import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateEmployeeFromFileComponent } from './update-employee-from-file.component';

describe('UpdateEmployeeFromFileComponent', () => {
  let component: UpdateEmployeeFromFileComponent;
  let fixture: ComponentFixture<UpdateEmployeeFromFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateEmployeeFromFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateEmployeeFromFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
