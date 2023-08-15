import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEmployeeFromFileComponent } from './create-employee-from-file.component';

describe('CreateEmployeeFromFileComponent', () => {
  let component: CreateEmployeeFromFileComponent;
  let fixture: ComponentFixture<CreateEmployeeFromFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEmployeeFromFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEmployeeFromFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
