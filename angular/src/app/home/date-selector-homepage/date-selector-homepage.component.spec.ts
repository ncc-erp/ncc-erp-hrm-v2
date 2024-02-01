import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DateSelectorHomepageComponent } from './date-selector-homepage.component';

describe('DateSelectorHomepageComponent', () => {
  let component: DateSelectorHomepageComponent;
  let fixture: ComponentFixture<DateSelectorHomepageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DateSelectorHomepageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DateSelectorHomepageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
