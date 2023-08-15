import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtendPausingComponent } from './extend-pausing.component';

describe('ExtendPausingComponent', () => {
  let component: ExtendPausingComponent;
  let fixture: ComponentFixture<ExtendPausingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExtendPausingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExtendPausingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
