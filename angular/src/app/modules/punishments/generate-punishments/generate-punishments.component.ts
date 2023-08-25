import { Component, OnInit, Injector } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { PunishmentTypeService } from '@app/service/api/categories/punishmentType.service';
import { PunishmentService } from '@app/service/api/punishment/punishment.service';
import {  PunishmentTypeDto, ResultGeneratePunishmentDto } from '@app/service/model/categories/punishmentType.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-generate-punishments',
  templateUrl: './generate-punishments.component.html',
  styleUrls: ['./generate-punishments.component.css']
})
export class GeneratePunishmentsComponent extends DialogComponentBase<PunishmentTypeDto> implements OnInit {
  constructor(
    injector: Injector,
    private punishmentTypeService: PunishmentTypeService,
    private punishmentService: PunishmentService,
    public dialogRef: MatDialogRef<GeneratePunishmentsComponent>,
  ) {
    super(injector)
  }

  public punishmentTypeList: PunishmentTypeDto[] = []
  public selectedAll: boolean = false;
  public selectedPunishmentTypeList: PunishmentTypeDto[] = []
  public listPunishmentTypeId: number[] = [];
  public listDate: any = [];
  public searchDate: string = "";
  public defaultDate: string = "";
  public results: ResultGeneratePunishmentDto[] = [];


  ngOnInit() {
    this.getAll();
    this.getListDate();
    this.title = "List Punishment Types";
    let now = new Date();
    now.setMonth(now.getMonth() - 1);
    this.defaultDate = moment(now).format("MM/YYYY");
  }

  public getAll() {
    this.subscription.push(
      this.punishmentTypeService.getAll().subscribe((rs) => {
        this.punishmentTypeList = rs.result;
      })
    )
  }

  public onSelectAll(completed: boolean) {
    this.selectedAll = completed;
    if (this.punishmentTypeList == null) {
      return;
    }
    if (this.selectedAll) {
      this.selectedPunishmentTypeList.push(...this.punishmentTypeList);
    }
    else {
      let listPunishmentTypeIds = this.punishmentTypeList.map(x => x.id)
      this.selectedPunishmentTypeList = this.selectedPunishmentTypeList.filter(type => !listPunishmentTypeIds.includes(type.id))
    }
    this.punishmentTypeList.forEach(t => (t.selected = completed));
  }

  public onGeneratePunishment() {
    this.listPunishmentTypeId = this.selectedPunishmentTypeList.map((type) => type.id);
    this.isLoading = true;
    this.subscription.push(
      this.punishmentService.generatePunishment(this.listPunishmentTypeId, this.defaultDate).subscribe((rs) => {
        this.isLoading = false;
        if (rs.success) {
          this.results = rs.result;
          this.punishmentTypeList.forEach((punishmentType)=>{
            this.results.forEach((rs)=>{
              if(rs.punishmentTypeId == punishmentType.id){
                punishmentType.message = rs.message;
              }
            })
          })
        }
      })
    )

  }

  public onSelectType(type: PunishmentTypeDto) {
    this.selectedAll = this.punishmentTypeList != null && this.punishmentTypeList.every(t => t.selected);
    if (type.selected) {
      this.selectedPunishmentTypeList.push(type);
    } else {
      this.selectedPunishmentTypeList.splice(this.selectedPunishmentTypeList.indexOf(type), 1);
    }
  }

  public getMessage(result) {
    var messages = result.failedList.length > 0 ? `<div class='row'>
    <div class="col-7"><b>Punishment Types</b></div>
    <div class="col-5"><b>Erorr</b></div>
    </div>`: "";
    result.failedList.forEach(mess => {
      messages += `<div class='row'>
      <div class="col-7 text-left">${mess.punishmentTypeName}
      </div>
      <div class="col-5 text-left">${mess.reasonFailed}</div>
      </div>`
    });
    return messages;
  }

  public getListDate() {
    this.subscription.push(
      this.punishmentService.getListDate().subscribe((res) => {
        this.listDate = res.result.map(date => {
          return moment(date).format("MM/YYYY");
        });
      })
    )
  }
}
