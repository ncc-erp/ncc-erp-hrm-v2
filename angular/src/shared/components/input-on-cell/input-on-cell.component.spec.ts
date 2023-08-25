import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputOnCellComponent } from './input-on-cell.component';

describe('InputOnCellComponent', () => {
  let component: InputOnCellComponent;
  let fixture: ComponentFixture<InputOnCellComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputOnCellComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputOnCellComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
