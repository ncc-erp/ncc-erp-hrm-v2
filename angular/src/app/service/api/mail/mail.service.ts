import { Injectable, Injector } from '@angular/core';
import { from, Observable } from 'rxjs';
import { ApiResponseDto } from '../../model/common.dto';
import { BaseApiService } from '../base-api.service'
import { EmailDto, MailPreviewInfo } from '../../model/mail/mail.dto'
import { EmailFunc } from '@shared/AppEnums'
@Injectable({
    providedIn: 'root'
})
export class MailService extends BaseApiService {
    constructor(injector: Injector) {
        super(injector)
    }

    changeUrl() {
        return 'Mail'
    }

    sendMail(data: MailPreviewInfo): Observable<ApiResponseDto<string>> {
        return this.processPost('SendMail', data)
    }
}