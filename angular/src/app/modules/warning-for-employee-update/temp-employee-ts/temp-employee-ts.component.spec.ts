import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TempEmployeeTsComponent } from './temp-employee-ts.component';

describe('TempEmployeeTsComponent', () => {
  let component: TempEmployeeTsComponent;
  let fixture: ComponentFixture<TempEmployeeTsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TempEmployeeTsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TempEmployeeTsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
