import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateEditPunishmentsComponent } from './create-edit-punishment.component';

describe('CreatePunishmentsComponent', () => {
  let component: CreateEditPunishmentsComponent;
  let fixture: ComponentFixture<CreateEditPunishmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateEditPunishmentsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateEditPunishmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
