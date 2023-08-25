import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PunishmentDetailComponent } from './punishment-detail.component';

describe('PunishmentDetailComponent', () => {
  let component: PunishmentDetailComponent;
  let fixture: ComponentFixture<PunishmentDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PunishmentDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PunishmentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
