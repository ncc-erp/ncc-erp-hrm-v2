import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PunishmentsComponent } from './punishments.component';

describe('PunishmentsComponent', () => {
  let component: PunishmentsComponent;
  let fixture: ComponentFixture<PunishmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PunishmentsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PunishmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
