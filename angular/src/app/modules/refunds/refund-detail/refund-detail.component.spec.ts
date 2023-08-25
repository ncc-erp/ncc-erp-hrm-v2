import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RefundDetailComponent } from './refund-detail.component';

describe('RefundDetailComponent', () => {
  let component: RefundDetailComponent;
  let fixture: ComponentFixture<RefundDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RefundDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RefundDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
