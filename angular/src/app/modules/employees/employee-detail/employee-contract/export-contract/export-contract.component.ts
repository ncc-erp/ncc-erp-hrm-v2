import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { EditEmailDialogComponent, EditEmailDialogData } from '@app/modules/admin/email-templates/edit-email-dialog/edit-email-dialog.component';
import { EmailTemplateService } from '@app/service/api/email-template/email-template.service';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { MailPreviewInfo, UpdateEmailTemplate } from '@app/service/model/mail/mail.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { cloneDeep } from 'lodash-es';

@Component({
  selector: 'app-export-contract',
  templateUrl: './export-contract.component.html',
  styleUrls: ['./export-contract.component.css']
})
export class ExportContractComponent extends DialogComponentBase<any> implements OnInit {

  constructor(injector: Injector,
    public sanitizer: DomSanitizer,
    private employeeContractService : EmployeeContractService,
    private emailTemplateService: EmailTemplateService,
    public dialog: MatDialog) {
    super(injector);
  }

  public contractId:number = 0;
  public type:number;
  public contractInfo: MailPreviewInfo = {} as MailPreviewInfo;
  ngOnInit(): void {
    this.contractId = this.dialogData.contractId;
    this.type = this.dialogData.tempType;
    this.title = this.dialogData.title;
    this.getContractTemplate();
  }

  public getContractTemplate(){
    this.subscription.push(
      this.employeeContractService.GetContractTemplate(this.contractId, this.type).subscribe((rs)=>{
        this.contractInfo = rs.result;
      })
    )
  }

  public printContractToPDF(){
    const wnd = window.open("","","_blank");
    wnd.document.write(this.contractInfo.bodyMessage);
    wnd.document.close();
    wnd.print()
    wnd.close()
  }

  onEdit() {
    const editMailDialogData: EditEmailDialogData = {
      mailInfo: cloneDeep(this.contractInfo),
      showDialogHeader: false,
      temporarySave: true,
    }
    const dialogRef = this.dialog.open(EditEmailDialogComponent, {
      data: editMailDialogData,
      width: '1600px',
      panelClass: 'email-dialog'
    })
    dialogRef.afterClosed().subscribe(rs => {
      if (rs) {
        this.contractInfo = cloneDeep(rs)
      }
    })
  }
}
