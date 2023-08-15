import { DEFAULT_FILTER_VALUE } from './paged-listing-component-base';
import { Injector, ElementRef, OnDestroy, Component } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import {
    LocalizationService,
    PermissionCheckerService,
    FeatureCheckerService,
    NotifyService,
    SettingService,
    MessageService,
    AbpMultiTenancyService
} from 'abp-ng2-module';

import { AppSessionService } from '@shared/session/app-session.service';
import { APP_ENUMS } from './AppEnums';
import * as moment from 'moment';
import { Subscription } from 'rxjs';
import { isString } from 'lodash-es';
@Component({
    template: ''
})
export abstract class AppComponentBase implements OnDestroy {

    APP_ENUM = APP_ENUMS;
    APP_CONST = AppConsts;
    localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

    localization: LocalizationService;
    permission: PermissionCheckerService;
    feature: FeatureCheckerService;
    notify: NotifyService;
    setting: SettingService;
    message: MessageService;
    multiTenancy: AbpMultiTenancyService;
    appSession: AppSessionService;
    elementRef: ElementRef;
    isLoading: boolean = false;
    checkNull: boolean = false;
    subscription: Subscription[] = [];
    public listBreadCrumb:Object = []
    constructor(injector: Injector) {
        this.localization = injector.get(LocalizationService);
        this.permission = injector.get(PermissionCheckerService);
        this.feature = injector.get(FeatureCheckerService);
        this.notify = injector.get(NotifyService);
        this.setting = injector.get(SettingService);
        this.message = injector.get(MessageService);
        this.multiTenancy = injector.get(AbpMultiTenancyService);
        this.appSession = injector.get(AppSessionService);
        this.elementRef = injector.get(ElementRef);
    }
    ngOnDestroy(): void {
        if(this.subscription && this.subscription.length){
            this.subscription.forEach(sub => {
                sub.unsubscribe()
            })
        }
    }

    l(key: string, ...args: any[]): string {
        let localizedText = this.localization.localize(key, this.localizationSourceName);

        if (!localizedText) {
            localizedText = key;
        }

        if (!args || !args.length) {
            return localizedText;
        }

        args.unshift(localizedText);
        return abp.utils.formatString.apply(this, args);
    }

    isGranted(permissionName: string): boolean {
        return this.permission.isGranted(permissionName);
    }

    public formatDateYMD(date: any) {
        return moment(date).format("YYYY-MM-DD")
    }

    public formatDateMY(date: any) {
        return moment(date).format("MM/YYYY")
    }

    public formatDateYMDHm(date){
        return moment(date).format("YYYY-MM-DD HH:mm")
    }

    trimData(data: Object) {
        for (const propName in data) {
            if (isString(data[propName])) {
                data[propName] = data[propName].trim();
            }
        }
    }
    mapToFilter(listData: any[], noAllOption?:boolean) {
        listData = listData.map(x => {
            return {
                key: x.name,
                value: x.id
            }
        })
        if(!noAllOption){
            listData.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
        }
        return listData
    }

    getListFormEnum(fromEnum: any, noAllOption?:boolean) {
        let result = Object.entries(fromEnum).map(item => {
            return {
                key: item[0],
                value: item[1]
            }
        })
        if(!noAllOption){
            result.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
        }
        return result
    }

    convertFile(fileData) {
        var buf = new ArrayBuffer(fileData.length);
        var view = new Uint8Array(buf);
        for (var i = 0; i != fileData.length; ++i) view[i] = fileData.charCodeAt(i) & 0xFF;
        return buf;
    }
}
