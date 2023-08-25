import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListBenefitComponent } from './list-benefit.component';

describe('ListBenefitComponent', () => {
  let component: ListBenefitComponent;
  let fixture: ComponentFixture<ListBenefitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListBenefitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListBenefitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
