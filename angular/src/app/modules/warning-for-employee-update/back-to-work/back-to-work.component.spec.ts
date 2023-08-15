import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BackToWorkComponent } from './back-to-work.component';

describe('BackToWorkComponent', () => {
  let component: BackToWorkComponent;
  let fixture: ComponentFixture<BackToWorkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BackToWorkComponent]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BackToWorkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
