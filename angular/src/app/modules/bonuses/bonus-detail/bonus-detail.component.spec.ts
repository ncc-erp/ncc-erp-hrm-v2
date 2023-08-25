import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BonusDetailComponent } from './bonus-detail.component';

describe('BonusDetailComponent', () => {
  let component: BonusDetailComponent;
  let fixture: ComponentFixture<BonusDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BonusDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BonusDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
