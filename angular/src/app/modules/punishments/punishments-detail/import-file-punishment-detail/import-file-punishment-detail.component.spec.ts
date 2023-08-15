import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportFilePunishmentDetailComponent } from './import-file-punishment-detail.component';

describe('ImportFilePunishmentDetailComponent', () => {
  let component: ImportFilePunishmentDetailComponent;
  let fixture: ComponentFixture<ImportFilePunishmentDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportFilePunishmentDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportFilePunishmentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
