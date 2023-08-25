import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobPositionsComponent } from './job-positions.component';

describe('JobPositionsComponent', () => {
  let component: JobPositionsComponent;
  let fixture: ComponentFixture<JobPositionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JobPositionsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JobPositionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
