import { CreateEditPunishmentDto } from './../../../service/model/punishments/punishments.dto';
import { PunishmentService } from '../../../service/api/punishment/punishment.service';
import { Component, OnInit, Injector } from '@angular/core';
import { PunishmentsDto } from '@app/service/model/punishments/punishments.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-create-edit-punishment',
  templateUrl: './create-edit-punishment.component.html',
  styleUrls: ['./create-edit-punishment.component.css']
})
export class CreateEditPunishmentsComponent extends DialogComponentBase<CreateEditPunishmentDto> implements OnInit {
  public punishment = {} as CreateEditPunishmentDto;
  public listDate: any = [];
  public searchDate: string = "";
  constructor(injector: Injector, private punishmentService: PunishmentService) { super(injector) }
  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.punishment = this.dialogData;

      this.title = `Edit punishment: <strong>${this.punishment.name}</strong>`;
    } else {
      this.title = "Create new punishment";
      let now = new Date();
      now.setMonth(now.getMonth() - 1);
      this.punishment.date = moment(now).format("MM/YYYY");
    }
    this.getListDate();
    this.punishment.isAbleUpdateNote = false;
  }

  public saveAndClose() {
    this.trimData(this.punishment)
    if (!this.dialogData?.id) {
      this.subscription.push(
        this.punishmentService.create(this.punishment).subscribe((res) => {
          abp.notify.success(`Create Punishment successful`)
          this.dialogRef.close(true)
        }))
    } else {
      this.subscription.push(
        this.punishmentService.update(this.punishment).subscribe((res) => {
          abp.notify.success(`Update Punishment successful`)
          this.dialogRef.close(true)
        }))
    }
  }

  public getListDate() {
    this.subscription.push(
      this.punishmentService.getListDate().subscribe((res) => {
        this.listDate = res.result.map(date => {
          return moment(date).format("MM/YYYY");
        });
      }))
  }
}
