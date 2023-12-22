import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChartDetailSettingsComponent } from './chart-detail-settings.component';

describe('ChartDetailSettingsComponent', () => {
  let component: ChartDetailSettingsComponent;
  let fixture: ComponentFixture<ChartDetailSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChartDetailSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChartDetailSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
