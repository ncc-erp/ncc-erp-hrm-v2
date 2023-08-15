import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BenefitDetailComponent } from './benefit-detail.component';

describe('BenefitDetailComponent', () => {
  let component: BenefitDetailComponent;
  let fixture: ComponentFixture<BenefitDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BenefitDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BenefitDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
