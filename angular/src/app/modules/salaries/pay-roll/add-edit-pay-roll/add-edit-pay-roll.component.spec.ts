import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditPayRollComponent } from './add-edit-pay-roll.component';

describe('AddEditPayRollComponent', () => {
  let component: AddEditPayRollComponent;
  let fixture: ComponentFixture<AddEditPayRollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditPayRollComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditPayRollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
