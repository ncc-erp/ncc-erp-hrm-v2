import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportEmployeeDialogComponent } from './export-employee-dialog.component';

describe('ExportEmployeeDialogComponent', () => {
  let component: ExportEmployeeDialogComponent;
  let fixture: ComponentFixture<ExportEmployeeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExportEmployeeDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExportEmployeeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
