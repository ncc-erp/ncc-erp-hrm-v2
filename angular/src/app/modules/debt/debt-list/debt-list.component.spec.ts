import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DebtListComponent } from './debt-list.component';

describe('DebtListComponent', () => {
  let component: DebtListComponent;
  let fixture: ComponentFixture<DebtListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DebtListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DebtListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
