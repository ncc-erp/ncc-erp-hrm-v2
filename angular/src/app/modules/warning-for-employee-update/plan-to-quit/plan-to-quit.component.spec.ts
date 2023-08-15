import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlanToQuitComponent } from './plan-to-quit.component';

describe('PlanToQuitComponent', () => {
  let component: PlanToQuitComponent;
  let fixture: ComponentFixture<PlanToQuitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlanToQuitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanToQuitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
