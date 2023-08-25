import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PunishmentTypesComponent } from './punishment-types.component';

describe('PunishmentTypesComponent', () => {
  let component: PunishmentTypesComponent;
  let fixture: ComponentFixture<PunishmentTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PunishmentTypesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PunishmentTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
