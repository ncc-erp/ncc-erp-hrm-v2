import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenefitInfomationComponent } from './benefit-infomation.component';

describe('BenefitInfomationComponent', () => {
  let component: BenefitInfomationComponent;
  let fixture: ComponentFixture<BenefitInfomationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BenefitInfomationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BenefitInfomationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
