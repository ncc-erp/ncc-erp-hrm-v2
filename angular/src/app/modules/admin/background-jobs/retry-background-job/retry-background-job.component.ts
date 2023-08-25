import { Component, Injector, OnInit } from '@angular/core';
import { BackgroundJobsService } from '@app/service/api/background-jobs/background-jobs.service';
import { BackgoundJobsDto } from '@app/service/model/backgound-jobs/backgound-jobs.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-retry-background-job',
  templateUrl: './retry-background-job.component.html',
  styleUrls: ['./retry-background-job.component.css']
})
export class RetryBackgroundJobComponent extends DialogComponentBase<any> implements OnInit {

  constructor(injector: Injector,
    private backgroundJobsService: BackgroundJobsService) {
    super(injector);
    this.title = "Retry background job"
  }

  public inputToRetry = {} as InputToRetryDto;

  ngOnInit(): void {
    this.inputToRetry.jobId = this.dialogData;
    console.log(this.dialogData)
    this.inputToRetry.timeToExecute = 10;
  }

  onSaveAndClose(){
    this.isLoading = true;
    this.subscription.push(
      this.backgroundJobsService.retryBackgroundJob(this.inputToRetry).subscribe((rs)=>{
        if(rs){
          abp.message.success(`Background job will be execute in ${this.inputToRetry.timeToExecute} seconds later`);
          this.dialogRef.close(true);
          this.isLoading = false;
        }
      },()=> this.isLoading = false)  
    )
  }


}
export class InputToRetryDto{
  timeToExecute: number
  jobId: number;
}
