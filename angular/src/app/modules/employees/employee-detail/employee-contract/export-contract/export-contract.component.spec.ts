import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportContractComponent } from './export-contract.component';

describe('ExportContractComponent', () => {
  let component: ExportContractComponent;
  let fixture: ComponentFixture<ExportContractComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExportContractComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExportContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
