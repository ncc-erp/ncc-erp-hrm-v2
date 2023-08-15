import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableToggleColumnComponent } from './table-toggle-column.component';

describe('TableToggleColumnComponent', () => {
  let component: TableToggleColumnComponent;
  let fixture: ComponentFixture<TableToggleColumnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableToggleColumnComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableToggleColumnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
