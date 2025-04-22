import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MezonTokenComponent } from './mezon-token.component';

describe('PayslipTokenComponent', () => {
  let component: MezonTokenComponent;
  let fixture: ComponentFixture<MezonTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MezonTokenComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MezonTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
