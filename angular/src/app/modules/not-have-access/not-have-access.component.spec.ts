import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotHaveAccessComponent } from './not-have-access.component';

describe('NotHaveAccessComponent', () => {
  let component: NotHaveAccessComponent;
  let fixture: ComponentFixture<NotHaveAccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotHaveAccessComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NotHaveAccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
