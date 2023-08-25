import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogHeaderComponent } from './dialog-header.component';

describe('DialogHeaderComponent', () => {
  let component: DialogHeaderComponent;
  let fixture: ComponentFixture<DialogHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
