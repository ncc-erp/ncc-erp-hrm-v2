import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditDisburseComponent } from './create-edit-disburse.component';

describe('CreateEditDisburseComponent', () => {
  let component: CreateEditDisburseComponent;
  let fixture: ComponentFixture<CreateEditDisburseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditDisburseComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditDisburseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
