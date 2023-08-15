import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditViewDebtComponent } from './create-edit-view-debt.component';

describe('CreateEditViewDebtComponent', () => {
  let component: CreateEditViewDebtComponent;
  let fixture: ComponentFixture<CreateEditViewDebtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditViewDebtComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditViewDebtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
