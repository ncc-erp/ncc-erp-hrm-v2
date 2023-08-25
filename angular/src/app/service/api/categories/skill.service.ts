import {Injectable} from '@angular/core'
import { BaseApiService } from "../base-api.service";

@Injectable({
    providedIn:'root'
})
export class SkillService extends BaseApiService{
    changeUrl() {
        return "Skill"
    }

}