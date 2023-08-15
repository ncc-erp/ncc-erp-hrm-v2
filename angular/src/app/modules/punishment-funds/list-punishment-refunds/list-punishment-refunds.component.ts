import { Component, Injector, OnInit } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { APP_ENUMS } from '@shared/AppEnums';
import { BreadCrumbDto } from '@shared/components/common/bread-crumb/bread-crumb.component';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import {PunishmentFundsService} from '../../../service/api/punishment-funds/punishment-funds.service';
import {FilterByComparisonDto, InputToGetAllPagingDto, PunishmentFundDto} from '../../../service/model/punishment-fund/punishment-fund.dto';
import { CreateEditDisburseComponent } from './create-edit-disburse/create-edit-disburse.component';
@Component({
  selector: 'app-list-punishment-refunds',
  templateUrl: './list-punishment-refunds.component.html',
  styleUrls: ['./list-punishment-refunds.component.css']
})
export class ListPunishmentRefundsComponent extends PagedListingComponentBase<any>  implements OnInit {
  
  constructor(injector: Injector,
    public punishmentFundsService: PunishmentFundsService) {
    super(injector);
  }
  listBreadCrumb: BreadCrumbDto[] = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
    { name: ' Punishment Fund ' }
  ];
  public OPERATOR_LIST = {
    [APP_ENUMS.filterComparison.LESS_THAN]: {
      keySign: '<',
      key: 'Less than',
      value: APP_ENUMS.filterComparison.LESS_THAN
    },
    [APP_ENUMS.filterComparison.GREATER_THAN]: {
      keySign: '>',
      key: 'Greater than',
      value: APP_ENUMS.filterComparison.GREATER_THAN
    },
    [APP_ENUMS.filterComparison.EQUAL]: {
      keySign: '=',
      key: 'Equal',
      value: APP_ENUMS.filterComparison.EQUAL
    }
  }
  public dialogType ={
    ADD : 1,
    DISBURSE: 2,
    EDIT: 3
  }
  public filterOperators = Object.values(this.OPERATOR_LIST);
  public listPunishmentFunds:PunishmentFundDto[] = [];
  public inputToGetAllPaging = {} as InputToGetAllPagingDto;
  public dataFilterByOperatorComparison = null as FilterByComparisonDto;
  ngOnInit(): void {
    this.refresh();
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.isLoading = true;
    this.inputToGetAllPaging.gridParam = request;
    this.inputToGetAllPaging.filterByComparision = this.dataFilterByOperatorComparison;
    this.subscription.push(
      this.punishmentFundsService.getAllPunishmentFundsPagging(this.inputToGetAllPaging).subscribe((rs)=>{
        if(rs){
          this.listPunishmentFunds = rs.result.items;
          this.isLoading = false;
          this.showPaging(rs.result, pageNumber);
        }
      },()=> this.isLoading = false)
    )
  }


  onAdd(){
    this.onOpenDialog(null, "Add new punishment fund", this.dialogType.ADD);
  }
  onDisburse(){
    this.onOpenDialog(null, "Disburse new punishment fund", this.dialogType.DISBURSE);
  }
  onUpdate(pf){
    this.onOpenDialog(pf,  `Edit punishment fund`, this.dialogType.EDIT)
  }
  onDelete(pf){
    abp.message.confirm("Are you sure to delete this punishment fund", "", (rs)=>{
      if(rs){
        this.punishmentFundsService.delete(pf.id).subscribe((rs)=>{
          if(rs){
            abp.message.success("Delete punishment fund successful");
            this.refresh();
          }
        })
      }
    })
  }
  onTableFilterByOperatorComparison(dataFilter: FilterByComparisonDto){
    if(dataFilter.operatorComparison != null && dataFilter.value != "" && dataFilter.value != "-" ){
      this.dataFilterByOperatorComparison = dataFilter
    }else{
      this.dataFilterByOperatorComparison = null;
    }
    this.getDataPage(1);
  }

  onOpenDialog(punishmentFund, title,type){
    const dia = this.dialog.open(CreateEditDisburseComponent, {
      data: {
        punishmentFund : {...punishmentFund},
        title: title,
        type: type
      },
      width: "700px"

    })
    dia.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }

  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.PunishmentFund_Add);
  }
  isShowDisburseBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.PunishmentFund_Disburse);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.PunishmentFund_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.PunishmentFund_Delete);
  }

}