import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BonusInformationComponent } from './bonus-information.component';

describe('BonusInformationComponent', () => {
  let component: BonusInformationComponent;
  let fixture: ComponentFixture<BonusInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BonusInformationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BonusInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
