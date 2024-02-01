import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChartDetailDataComponent } from './chart-detail-data.component';

describe('ChartDetailDataComponent', () => {
  let component: ChartDetailDataComponent;
  let fixture: ComponentFixture<ChartDetailDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChartDetailDataComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChartDetailDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
