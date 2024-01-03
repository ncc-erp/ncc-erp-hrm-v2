import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditChartDialogComponent } from './create-edit-chart-dialog.component';

describe('CreateEditChartDialogComponent', () => {
  let component: CreateEditChartDialogComponent;
  let fixture: ComponentFixture<CreateEditChartDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditChartDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditChartDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
