import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChartSettingsComponent } from './chart-settings.component';

describe('ChartSettingsComponent', () => {
  let component: ChartSettingsComponent;
  let fixture: ComponentFixture<ChartSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChartSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChartSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
