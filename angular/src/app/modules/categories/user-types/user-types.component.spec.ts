import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserTypesComponent } from './user-types.component';

describe('UserTypesComponent', () => {
  let component: UserTypesComponent;
  let fixture: ComponentFixture<UserTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserTypesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
