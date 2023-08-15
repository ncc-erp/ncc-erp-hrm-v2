import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveCancelButtonComponent } from './save-cancel-button.component';

describe('SaveCancelButtonComponent', () => {
  let component: SaveCancelButtonComponent;
  let fixture: ComponentFixture<SaveCancelButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaveCancelButtonComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SaveCancelButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
