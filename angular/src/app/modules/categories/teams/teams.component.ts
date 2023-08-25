import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { AddEmployeesToTeamDto, TeamDto } from '@app/service/model/categories/team.dto'
import { TeamService } from '@app/service/api/categories/team.service'
import { finalize } from 'rxjs/operators';
import { CreateEditTeamDialogComponent } from './create-edit-team-dialog/create-edit-team-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';
@Component({
  selector: 'app-teams',
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.css']
})
export class TeamsComponent extends PagedListingComponentBase<TeamDto> implements OnInit {
  public teamList: TeamDto[] = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.teamService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.teamList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector, private teamService: TeamService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'Teams' }],
      this.refresh()
  }

  isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Team_Create);
  }
  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Team_Edit);
  }
  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Team_Delete);
  }


  onCreate() {
    this.openDialog(CreateEditTeamDialogComponent);
  }

  onUpdate(team: TeamDto) {
    this.openDialog(CreateEditTeamDialogComponent, { ...team })
  }

  onDelete(team: TeamDto) {
    this.confirmDelete(`Delete team <strong>${team.name}</strong>`, () => {
      this.teamService.delete(team.id).subscribe(() => {
        abp.notify.success(`Deleted team ${team.name}`)
        this.refresh();
      })
    })
  }


  onAddEmployees(team: TeamDto) {
    var ref = this.dialog.open(AddEmployeeComponent, {
      width: "92vw",
      height: "95vh",
      maxWidth: "100vw",
      data: {
        title: `Add employee to team <strong>${team.name}</strong>`,
        addedEmployeeIds: this.addedEmployeeIds
      }
    })
    ref.afterClosed().subscribe((res) => {
      if (res && res.length) {
        let input = {
          employeeIds: res,
          teamId: team.id
        } as AddEmployeesToTeamDto

        this.teamService.AddEmployeeToTeam(input).subscribe(rs => {
          abp.notify.success(`Successful add ${res.length} employee to ${team.name}`);
          this.refresh()
        })
      }
    })
  }
}
