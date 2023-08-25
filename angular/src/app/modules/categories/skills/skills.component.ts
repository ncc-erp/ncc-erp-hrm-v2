import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { SkillDto } from '@app/service/model/categories/skill.dto'
import { SkillService } from '@app/service/api/categories/skill.service'
import { finalize } from 'rxjs/operators';
import { CreateEditSkillDialogComponent } from './create-edit-skill-dialog/create-edit-skill-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-skills',
  templateUrl: './skills.component.html',
  styleUrls: ['./skills.component.css']
})
export class SkillsComponent extends PagedListingComponentBase<SkillDto> implements OnInit {
  public skillList: SkillDto[] = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.skillService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.skillList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        }))
  }

  constructor(injector: Injector, private skillService: SkillService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Skills'}],
    this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Skill_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Skill_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Skill_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditSkillDialogComponent);
  }

  onUpdate(skill: SkillDto) {
    this.openDialog(CreateEditSkillDialogComponent, { ...skill })
  }

  onDelete(skill: SkillDto) {
    this.confirmDelete(`Delete skill <strong>${skill.name}</strong>`, () => {
      this.skillService.delete(skill.id).subscribe(() => {
        abp.notify.success(`Deleted skill ${skill.name}`)
        this.refresh();
      })
    })
  }
}
