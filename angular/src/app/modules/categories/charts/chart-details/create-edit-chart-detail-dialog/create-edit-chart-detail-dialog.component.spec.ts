import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditChartDetailDialogComponent } from './create-edit-chart-detail-dialog.component';

describe('CreateEditChartDetailDialogComponent', () => {
  let component: CreateEditChartDetailDialogComponent;
  let fixture: ComponentFixture<CreateEditChartDetailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditChartDetailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditChartDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
