import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RetryBackgroundJobComponent } from './retry-background-job.component';

describe('RetryBackgroundJobComponent', () => {
  let component: RetryBackgroundJobComponent;
  let fixture: ComponentFixture<RetryBackgroundJobComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RetryBackgroundJobComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RetryBackgroundJobComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
