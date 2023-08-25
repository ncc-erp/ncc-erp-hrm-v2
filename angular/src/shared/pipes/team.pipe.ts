import {Pipe, PipeTransform} from '@angular/core'
import {EmployeeTeamDto} from '@shared/dto/user-infoDto'
@Pipe({
    name: 'team'
})
export class TeamPipe implements PipeTransform{
    transform(value: EmployeeTeamDto[], args: []) {
        return value.map(team => `<span class="badge badge-team mr-2 text-white">${team.teamName}</span>`).join("")
    }
}

export const TEAM_BADGE_COLOR = "#4361ee"