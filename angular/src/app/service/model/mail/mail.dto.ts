export class EmailDto {
    id: number;
    type: number;
    name: string;
    description: string;
    cCs: string;
    arrCCs: string[];
    sendToEmail: string;
    templateType: number;
}
export class MailPreviewInfo {
    templateId: string;
    name: string;
    description: string;
    type?: number;
    bodyMessage: string;
    subject: string;
    cCs: string[];
    arrCCs?: string[];
    mailFuncType?: number;
    propertiesSupport: string[];
    sendToEmail: string;
    templateType: number;
}

export class MailDialogConfig {
    templateId?: number;
    mailInfo?: MailPreviewInfo;
    showEditBtn?: boolean;
    isAllowSendMail?: boolean;
}

export interface UpdateEmailTemplate {
    id: number;
    name: string;
    bodyMessage: string;
    subject: string;
    description: string;
    type: number;
    listCC: string[];
    sendToEmail: string;
}