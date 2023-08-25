import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneratePunishmentsComponent } from './generate-punishments.component';

describe('GeneratePunishmentsComponent', () => {
  let component: GeneratePunishmentsComponent;
  let fixture: ComponentFixture<GeneratePunishmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GeneratePunishmentsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GeneratePunishmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
