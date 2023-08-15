import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DebtPaidComponent } from './debt-paid.component';

describe('DebtPaidComponent', () => {
  let component: DebtPaidComponent;
  let fixture: ComponentFixture<DebtPaidComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DebtPaidComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DebtPaidComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
