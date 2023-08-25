import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateDateComponent } from './update-date.component';

describe('UpdateDateComponent', () => {
  let component: UpdateDateComponent;
  let fixture: ComponentFixture<UpdateDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateDateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
