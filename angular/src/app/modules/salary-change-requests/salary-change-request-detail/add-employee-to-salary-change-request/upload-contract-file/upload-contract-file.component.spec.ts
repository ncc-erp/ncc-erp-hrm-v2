import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadContractFileComponent } from './upload-contract-file.component';

describe('UploadContractFileComponent', () => {
  let component: UploadContractFileComponent;
  let fixture: ComponentFixture<UploadContractFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadContractFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadContractFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
