import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMezonTokenComponent } from './add-mezon-token.component';

describe('AddMezonTokenComponent', () => {
  let component: AddMezonTokenComponent;
  let fixture: ComponentFixture<AddMezonTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddMezonTokenComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMezonTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
