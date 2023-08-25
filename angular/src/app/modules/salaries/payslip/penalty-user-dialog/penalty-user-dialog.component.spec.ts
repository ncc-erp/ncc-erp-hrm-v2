import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PenaltyUserDialogComponent } from './penalty-user-dialog.component';

describe('PenaltyUserDialogComponent', () => {
  let component: PenaltyUserDialogComponent;
  let fixture: ComponentFixture<PenaltyUserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PenaltyUserDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PenaltyUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
