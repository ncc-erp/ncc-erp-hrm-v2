import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculateResultDialogComponent } from './calculate-result-dialog.component';

describe('CalculateResultDialogComponent', () => {
  let component: CalculateResultDialogComponent;
  let fixture: ComponentFixture<CalculateResultDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CalculateResultDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CalculateResultDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
