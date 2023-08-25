import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "../base-api.service";

@Injectable({
    providedIn:'root'
})
export class PunishmentTypeService extends BaseApiService{
    changeUrl() {
        return "PunishmentType"
    }
    constructor(injector: Injector){
        super(injector)
    }
}