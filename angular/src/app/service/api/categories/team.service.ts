import { Injectable } from '@angular/core'
import { AddEmployeesToTeamDto } from '@app/service/model/categories/team.dto';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { BaseApiService } from "../base-api.service";

@Injectable({
    providedIn: 'root'
})
export class TeamService extends BaseApiService {
    changeUrl() {
        return "Team"
    }

    public AddEmployeeToTeam(input: AddEmployeesToTeamDto): Observable<ApiResponseDto<AddEmployeesToTeamDto>> {
        return this.processPost("AddEmployeeToTeam", input);
    }
}
