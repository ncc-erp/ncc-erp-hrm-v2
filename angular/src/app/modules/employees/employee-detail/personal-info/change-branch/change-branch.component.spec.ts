import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeBranchComponent } from './change-branch.component';

describe('ChangeBranchComponent', () => {
  let component: ChangeBranchComponent;
  let fixture: ComponentFixture<ChangeBranchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChangeBranchComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeBranchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
