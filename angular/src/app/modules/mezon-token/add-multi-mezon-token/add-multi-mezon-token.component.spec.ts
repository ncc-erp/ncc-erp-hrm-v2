import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMultiMezonTokenComponent } from './add-multi-mezon-token.component';

describe('AddMultiMezonTokenComponent', () => {
  let component: AddMultiMezonTokenComponent;
  let fixture: ComponentFixture<AddMultiMezonTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddMultiMezonTokenComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMultiMezonTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
