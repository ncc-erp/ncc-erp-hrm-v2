import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDataForChartDetailComponent } from './user-data-for-chart-detail.component';

describe('UserDataForChartDetailComponent', () => {
  let component: UserDataForChartDetailComponent;
  let fixture: ComponentFixture<UserDataForChartDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserDataForChartDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserDataForChartDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
