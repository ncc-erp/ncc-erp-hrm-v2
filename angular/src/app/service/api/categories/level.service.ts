import {Injectable} from '@angular/core'
import { BaseApiService } from "../base-api.service";

@Injectable({
    providedIn:'root'
})
export class LevelService extends BaseApiService{
    changeUrl() {
        return "Level"
    }

}