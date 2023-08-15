import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportCheckpointComponent } from './import-checkpoint.component';

describe('ImportCheckpointComponent', () => {
  let component: ImportCheckpointComponent;
  let fixture: ComponentFixture<ImportCheckpointComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportCheckpointComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportCheckpointComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
