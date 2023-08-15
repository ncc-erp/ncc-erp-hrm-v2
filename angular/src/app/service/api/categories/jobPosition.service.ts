import { Injectable, Injector } from "@angular/core";
import { BaseApiService } from "../base-api.service";

@Injectable({
    providedIn:'root'
})
export class JobPositionService extends BaseApiService{
    changeUrl() {
        return "JobPosition"
    }
    constructor(injector: Injector){
        super(injector)
    }
}