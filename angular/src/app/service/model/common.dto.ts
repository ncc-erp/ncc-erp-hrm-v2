import { HttpErrorResponse } from "@angular/common/http"

export class ApiResponseDto<T>{
    result?: T;
    targetUrl?: string;
    success: boolean;
    error: HttpErrorResponse;
    unAuthorizedRequest?: boolean;
    loading: boolean;
}
export class MessageResponse {
    failedList: any[];
    successList: any[]
}

export interface FileBase64Dto {
    fileName: string
    fileType: string
    base64: string
}

export interface BreadCrumbDto {
    name: string,
    url?: string
}

export interface filterDataDto {
    key: string,
    value: any
}
