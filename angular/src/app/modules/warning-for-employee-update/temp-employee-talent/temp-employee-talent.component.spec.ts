import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TempEmployeeTalentComponent } from './temp-employee-talent.component';

describe('TempEmployeeTalentComponent', () => {
  let component: TempEmployeeTalentComponent;
  let fixture: ComponentFixture<TempEmployeeTalentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TempEmployeeTalentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TempEmployeeTalentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
