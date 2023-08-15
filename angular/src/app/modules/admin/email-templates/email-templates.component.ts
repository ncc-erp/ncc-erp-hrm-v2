import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { EmailDto, MailPreviewInfo } from '@app/service/model/mail/mail.dto'
import { BreadCrumbDto } from '@shared/components/common/bread-crumb/bread-crumb.component';
import { MatDialog } from '@angular/material/dialog';
import { MailDialogComponent, MailDialogData } from './mail-dialog/mail-dialog.component';
import { EditEmailDialogComponent } from './edit-email-dialog/edit-email-dialog.component';
import { EmailTemplateService } from '@app/service/api/email-template/email-template.service'
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { EmailFunc } from '@shared/AppEnums';
import { AppConsts } from '@shared/AppConsts';
@Component({
  selector: 'app-email-templates',
  templateUrl: './email-templates.component.html',
  styleUrls: ['./email-templates.component.css']
})
export class EmailTemplatesComponent extends AppComponentBase implements OnInit {

  public listBreadCrumb: BreadCrumbDto[] = []
  public mails: EmailDto[] = []
  public showSendMailButton: boolean = true;
  public showSendMailHeader: boolean = true;
  public TemplateTypes = AppConsts.TemplateType;
  constructor(injector: Injector, private matDialog: MatDialog, private emailTemplateService: EmailTemplateService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ', url: null },
      { name: 'Email', url: null }]
      this.getAllTemplate()
  }
  getAllTemplate(){
   this.subscription.push(
    this.emailTemplateService.getAll().subscribe(rs => {
      this.mails = rs.result
    })
   )
  }
  preview(mailData: EmailDto) {
    this.isShowSendMail(mailData.type)
    const previewDialogData: MailDialogData = {
      templateId: mailData.id,
      title: `Preview ${mailData.name}`,
    }
    const dialogRef = this.matDialog.open(MailDialogComponent, {
      data: previewDialogData,
      width: '1600px',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe((mailInfo: MailPreviewInfo) => {
      if (!mailInfo) return;

      this.emailTemplateService.sendMail(mailInfo).subscribe(res => {
        if (res?.success) {
            abp.message.success("Send mail successful!");
        }
      });
    });
  }

  edit(mailData: EmailDto) {
    this.isShowSendMail(mailData.type)
    const dialogRef = this.matDialog.open(EditEmailDialogComponent, {
      data: {
        templateId: mailData.id,
        title: `Edit ${mailData.name}`,
        showSendMailHeader: this.showSendMailHeader

      },
      width: '85%',
      maxWidth: '85%',
      panelClass: 'email-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
      this.getAllTemplate();
    })
  }

  isShowSendMail(type){
    switch(type){
      case EmailFunc.ContractBM :; case EmailFunc.ContractCTV: ; case EmailFunc.ContractDT:;
      case EmailFunc.ContractLD:; case EmailFunc.ContractTV:{
        this.showSendMailButton = false;
        this.showSendMailHeader = false;
      }
    }
  }

  isShowPreviewBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_PreviewTemplate);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_EmailTemplate_Edit);
  }
}