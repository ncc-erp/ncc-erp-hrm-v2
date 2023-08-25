import { Component, OnInit, Injector, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EmailDto, MailPreviewInfo } from '@app/service/model/mail/mail.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { MatDialog } from '@angular/material/dialog';
import { EditEmailDialogComponent, EditEmailDialogData } from '../edit-email-dialog/edit-email-dialog.component';
import { EmailTemplateService } from '@app/service/api/email-template/email-template.service';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { EmailFunc, TemplateType } from '@shared/AppEnums';
@Component({
  selector: 'app-mail-dialog',
  templateUrl: './mail-dialog.component.html',
  styleUrls: ['./mail-dialog.component.css'],
})
export class MailDialogComponent extends DialogComponentBase<any> implements OnInit {
  public mailInfo = new MailPreviewInfo();
  public saving: boolean = false;
  public content: SafeHtml = '';
  public cancelDisabled: boolean = false;
  public saveDisabled: boolean = false;
  public showEditButton: boolean = false;
  public showDialogHeader: boolean = true;
  public showSendMailButton: boolean = true;
  public showSendMailHeader: boolean = true;
  public templateId: number;
  public EmailTypes = EmailFunc;
  constructor(injector: Injector, public sanitizer: DomSanitizer, private dialog: MatDialog, private emailTemplateService: EmailTemplateService) {
    super(injector)
  }

  ngOnInit(): void {
    this.showSendMailButton = this.dialogData.showSendMailButton;
    this.showSendMailHeader = this.dialogData.showSendMailHeader;
    Object.assign(this, this.dialogData)
    if (this.templateId) {
      this.getFakeData()
    }
  }


  editTemplate() {
    const dialogData: EditEmailDialogData = {
      mailInfo: { ...this.mailInfo },
      showDialogHeader: false,
      temporarySave: true,
    }
    const editDialog = this.dialog.open(EditEmailDialogComponent, {
      data: dialogData,
      width: '1600px',
      panelClass: 'email-dialog'
    })

    editDialog.afterClosed().subscribe(rs => {
      if (rs) {
        this.mailInfo = { ...rs }
      }
    })
  }

  getFakeData() {
    this.subscription.push(
      this.emailTemplateService.previewTemplate(this.templateId).subscribe(rs => {
        this.mailInfo = rs.result
      })
    )
  }

  sendMail() {
    if (!this.mailInfo.sendToEmail) {
      abp.message.error("Invalid email address!")
      return;
    }
    this.dialogRef.close(this.mailInfo)
  }

  isShowSendMailBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_PreviewTemplate_SendMail) 
    && this.mailInfo.templateType == TemplateType.Mail;
  }
  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_Edit);
  }
  isShowHeaderSendMail(){
    return this.mailInfo.templateType == TemplateType.Mail;
  }
}

export interface MailDialogData {
  mailInfo?: MailPreviewInfo,
  showEditButton?: boolean,
  templateId?: number,
  isAllowSendMail?: boolean,
  showDialogHeader?: boolean,
  title?: string,
}

