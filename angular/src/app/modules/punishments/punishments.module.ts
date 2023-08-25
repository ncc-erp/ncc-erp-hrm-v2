import { IConfig, NgxMaskModule } from 'ngx-mask';
import { PunishmentsRoutingModule } from './punishments-routing.module';
import { CreateEditPunishmentsComponent } from './create-edit-punishment/create-edit-punishment.component';
import { GeneratePunishmentsComponent } from './generate-punishments/generate-punishments.component';
import { PunishmentsComponent } from './punishmets/punishments.component';
import { PunishmentDetailComponent } from './punishments-detail/punishment-detail.component';
import { NgModule } from '@angular/core';
import { ImportFilePunishmentDetailComponent } from './punishments-detail/import-file-punishment-detail/import-file-punishment-detail.component';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
export const options: Partial<IConfig> | (() => Partial<IConfig>) = null;

@NgModule({
  declarations: [
    PunishmentsComponent,
    CreateEditPunishmentsComponent,
    GeneratePunishmentsComponent,
    PunishmentDetailComponent,
    ImportFilePunishmentDetailComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    PunishmentsRoutingModule,
    NgxMaskModule.forRoot()
    
  ]
})
export class PunishmentsModule { 

}
