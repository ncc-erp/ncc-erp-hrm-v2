import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditBankDialogComponent } from './create-edit-bank-dialog.component';

describe('CreateEditBankDialogComponent', () => {
  let component: CreateEditBankDialogComponent;
  let fixture: ComponentFixture<CreateEditBankDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditBankDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditBankDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
