import { Component, Injector, OnInit, ViewChildren } from '@angular/core';
import { PagedListingComponentBase , PagedRequestDto} from '@shared/paged-listing-component-base';
import { BackgoundJobsDto} from '../../../service/model/backgound-jobs/backgound-jobs.dto'
import { BackgroundJobsService } from '../../../service/api/background-jobs/background-jobs.service';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { RetryBackgroundJobComponent } from './retry-background-job/retry-background-job.component';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
@Component({
  selector: 'app-background-jobs',
  templateUrl: './background-jobs.component.html',
  styleUrls: ['./background-jobs.component.css']
})
export class BackgroundJobsComponent extends PagedListingComponentBase<BackgoundJobsDto> implements OnInit {

  constructor(injector: Injector,
    private backgroundJobsService: BackgroundJobsService) {
    super(injector);
  }
  public backgroundJobs:BackgoundJobsDto[] = [];
  public searchById:string = "";
  @ViewChildren (MatMenuTrigger) menuTrigger: any;
  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:' Background Job'}];
      
    this.bindDefaultFilter();
    this.refresh();

  }
  public list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void{
    this.isLoading = true;
    let input = {
      searchById: this.searchById,
      param: request
    } as InputToGetAllDto;
    this.subscription.push(
      this.backgroundJobsService.getAllBackgroundJobs(input).subscribe((rs)=>{
        this.backgroundJobs = rs.result.items;
        this.showPaging(rs.result, pageNumber);
        this.isLoading = false;
      }, () => this.isLoading = false)
    )
  }
  public onDelete(id){
    this.menuTrigger._results.forEach(e => e.closeMenu());
    abp.message.confirm("Are you sure to delete this job?", "", (rs)=>{
      if(rs){
        this.subscription.push(
          this.backgroundJobsService.delete(id).subscribe((rs)=>{
            if(rs){
              abp.message.success("Delete job successful");
              this.refresh();
            }
          })
        )
      }
    })
  }

  public onRetry(id){
    var dialog = this.dialog.open(RetryBackgroundJobComponent,{
      data: id,
      width: "750px"
    })

    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }

  public bindDefaultFilter(){
    this.sortDirection = this.APP_ENUM.SortDirectionEnum.Descending;
    this.sortProperty = 'creationTime';
  }

  public onSearchEnterId() {
    this.getDataPage(1)
  }

  public isShowBtnDelete(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_BackgroundJob_Delete);
  }
  public isShowBtnRetry(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_BackgroundJob_Retry);
  }
  
}
export class InputToGetAllDto{
  searchById: string;
  param: PagedRequestDto
}

export class RetryBackgroundJobDto{

}