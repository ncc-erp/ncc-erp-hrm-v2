import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportEmployeeToBonusComponent } from './import-employee-to-bonus.component';

describe('ImportEmployeeToBonusComponent', () => {
  let component: ImportEmployeeToBonusComponent;
  let fixture: ComponentFixture<ImportEmployeeToBonusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportEmployeeToBonusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportEmployeeToBonusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
