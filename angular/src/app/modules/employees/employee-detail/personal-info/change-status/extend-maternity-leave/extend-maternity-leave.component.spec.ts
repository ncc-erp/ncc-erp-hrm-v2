import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtendMaternityLeaveComponent } from './extend-maternity-leave.component';

describe('ExtendMaternityLeaveComponent', () => {
  let component: ExtendMaternityLeaveComponent;
  let fixture: ComponentFixture<ExtendMaternityLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExtendMaternityLeaveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExtendMaternityLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
