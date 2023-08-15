import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssuedByComponent } from './issued-by.component';

describe('IssuedByComponent', () => {
  let component: IssuedByComponent;
  let fixture: ComponentFixture<IssuedByComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IssuedByComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IssuedByComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
