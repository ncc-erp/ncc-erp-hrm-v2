import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayRollComponent } from './pay-roll.component';

describe('PayRollComponent', () => {
  let component: PayRollComponent;
  let fixture: ComponentFixture<PayRollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayRollComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayRollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
