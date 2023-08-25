import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponseDto } from '../../model/common.dto';
import { BaseApiService } from '../base-api.service'
import { EmailDto, MailPreviewInfo, UpdateEmailTemplate } from '../../model/mail/mail.dto'
@Injectable({
    providedIn: 'root'
})
export class EmailTemplateService extends BaseApiService {
    constructor(injector: Injector) {
        super(injector)
    }

    changeUrl() {
        return 'EmailTemplate'
    }

    getAll(): Observable<ApiResponseDto<EmailDto[]>> {
        return this.processGet('GetAll')
    }

    getTemplateById(templateId: number): Observable<ApiResponseDto<MailPreviewInfo>> {
        return this.processGet(`GetTemplateById?id=${templateId}`)
    }
    previewTemplate(templateId: number): Observable<ApiResponseDto<MailPreviewInfo>> {
        return this.processGet(`PreviewTemplate?id=${templateId}`)
    } 

    updateTemplate(payload: UpdateEmailTemplate):Observable<ApiResponseDto<UpdateEmailTemplate>> {
        return this.processPost(`UpdateTemplate`, payload)
    }

    public sendMail(input : MailPreviewInfo):Observable<ApiResponseDto<any>> {
        return this.processPost(`SendMail`,input);
    }
}