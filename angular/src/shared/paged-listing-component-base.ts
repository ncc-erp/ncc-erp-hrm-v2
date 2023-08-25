import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { isNull } from 'lodash-es';
import { APP_ENUMS } from './AppEnums';
import { SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
export class PagedResultDto {
    items: any[];
    totalCount: number;
}
export class FilterDto {
    propertyName: string;
    value: any;
    comparision: number;
    filterType?: number
    dropdownData?: any[]
}
export class EntityDto {
    id: number;
}

export class PagedRequestDto {
    skipCount: number;
    maxResultCount: number;
    searchText: string;
    filterItems: FilterDto[] = [];
    sort: string;
    sortDirection: number;
}
export class PagedResultResultDto {
    result: PagedResultDto;
}

@Component({
    template: ''
})
export abstract class PagedListingComponentBase<TEntityDto> extends AppComponentBase implements OnInit {
    [x: string]: any;
    public pageSize: number = 5;
    public pageNumber: number = 1;
    public totalPages: number = 1;
    public totalItems: number;
    public searchText: string = '';
    public filterItems: FilterDto[] = [];
    public pageSizeType: number = 20;
    public advancedFiltersVisible: boolean = false;
    public sortProperty: string = "";
    public sortDirection: number = null
    public activatedRoute: ActivatedRoute;
    public router: Router;
    public dialog: MatDialog;
    public sortDirectionEnum = this.APP_ENUM.SortDirectionEnum
    public filterTypeEnum = this.APP_ENUM.FilterTypeEnum
    public isDialog: boolean = false;
    public teamIds:number[] = [];
    public isAndCondition:boolean = false;
    public statusIds:number[] = [];
    public levelIds:number[] = [];
    public userTypes:number[] = [];
    public branchIds:number[] = [];
    public jobPositionsId:number[] = [];
    public debtstatusIds:number[] = [];
    public paymentTypeIds:number[] = [];
    public toJobPositionIds: number[] = [];
    public toLevelIds: number[] = [];
    public toUserTypeIds: number[] = [];
    public seniorityFilterInput = {} as SeniorityFilterDto;
    public filterParamType = APP_ENUMS.FilterMultipleTypeParamEnum;
    public request:PagedRequestDto;
    constructor(injector: Injector) {
        super(injector);
        this.activatedRoute = injector.get(ActivatedRoute);
        this.router = injector.get(Router);
        this.dialog = injector.get(MatDialog);
        if (!this.isDialog) {
            this.activatedRoute.queryParams.subscribe(params => {
                this.pageNumber = params['pageNumber'] ? params['pageNumber'] : 1;
                this.pageSize = params['pageSize'] ? params['pageSize'] : 20;
                this.searchText = params['searchText'] ? params['searchText'] : '';
                this.filterItems = params['filterItems'] ? JSON.parse(params['filterItems']) : [];
                this.pageSizeType = Number(params['pageSize'] ? params['pageSize'] : 20);
            });
        }
    }

    ngOnInit(): void {

        this.refresh();
    }

    public refresh(): void {
        if(this.isDialog){
            this.setDefaultDialogMode()
        }
        this.getDataPage(this.pageNumber);
    }

    public showPaging(result: PagedResultDto, pageNumber: number): void {
        this.totalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;
        this.totalItems = result.totalCount;
        this.pageNumber = pageNumber;
    }

    public setDefaultDialogMode(){
         this.pageNumber = 1;
         this.totalPages = 1;
         this.pageSizeType = 10;
         this.pageSize = 10;
    }

    public getDataPage(page: number): void {
        const req = new PagedRequestDto();
        req.maxResultCount = Number(this.pageSize);
        req.skipCount = (page - 1) * Number(this.pageSize);
        if (req.skipCount < 0) {
            req.skipCount = 0
        }
        req.filterItems = this.filterItems;
        if (this.sortProperty) {
            req.sort = this.sortProperty;
            req.sortDirection = this.sortDirection;
        }
        req.searchText = this.searchText;
        this.isLoading = true;
        this.pageNumber = page;
        if (!this.isDialog) {
            this.router.navigate([], {
                queryParamsHandling: "merge",
                replaceUrl: true,
                queryParams: { pageNumber: this.pageNumber, pageSize: this.pageSize, searchText: this.searchText, filterItems: JSON.stringify(this.filterItems) }
            })
                .then(_ => this.list(req, page, () => {
                    this.isLoading = false;
                }));
        }
        else {
            this.list(req, page, () => {
                this.isLoading = false
            })
        }
    }

    public onEmitChange(event, i) {
        const { name, value } = event
        this.filterItems[i][name] = value
    }
    public changePageSize(page) {
        this.pageSizeType = page
        if (this.pageSize > this.totalItems) {
            this.pageNumber = 1;
        }
        this.pageSize = this.pageSizeType;
        this.getDataPage(1);
    }

    public onTableFilter(filterType: number, value: any, propertyName?: string, comparision?: number) {
        switch (filterType) {
            case this.filterTypeEnum.SearchText: {
                this.searchText = value
                break;
            }
            case this.filterTypeEnum.Radio: case this.filterTypeEnum.Dropdown: {
                this.clearDuplicateFilter(propertyName)
                if ( value != DEFAULT_FILTER_VALUE) {
                    let filterItem = { propertyName: propertyName, value: value, comparision: 0 } as FilterDto
                    this.filterItems.push(filterItem)
                }
                break;
            }
            case this.filterTypeEnum.DatePicker:{
                this.clearDuplicateFilter(propertyName)
                if(!isNull(value) && (value !== "Invalid date")){
                    let filterItem = {propertyName: propertyName, value: value, comparision: comparision} as FilterDto
                    this.filterItems.push(filterItem)
                }
                break;
            }
        }
        this.getDataPage(1)
    }
    public onTableMultiSelectWithConditionFilter(teamsFilterInput:TeamsFilterInputDto){
        this.teamIds = teamsFilterInput?.teamIds;
        this.isAndCondition = teamsFilterInput?.isAndCondition;
        this.getDataPage(1);
    }

    private clearDuplicateFilter(propertyName: string) {
        this.filterItems.forEach(item => {
            if (item.propertyName == propertyName) {
                this.filterItems.splice(this.filterItems.indexOf(item), 1)
            }
        })
    }

    public onPageChange(event) {
        this.pageNumber = event.pageIndex + 1
        this.pageSize = event.pageSize
        this.getDataPage(this.pageNumber);
    }

    public onSortChange(property: string) {
        if (this.sortProperty != property) {
            this.sortDirection = null
        }
        if (property) {
            switch (this.sortDirection) {
                case null: {
                    this.sortDirection = this.sortDirectionEnum.Ascending
                    this.sortProperty = property
                    break;
                }
                case this.sortDirectionEnum.Ascending: {
                    this.sortDirection = this.sortDirectionEnum.Descending
                    this.sortProperty = property
                    break;
                }
                case this.sortDirectionEnum.Descending: {
                    this.sortDirection = null
                    this.sortProperty = ""
                    break;
                }
            }
        }
        this.refresh()
    }

    public onSearchEnter(searhValue: string) {
        this.searchText = searhValue
        this.getDataPage(1)
    }

    public openDialog(component, dialogData?: any) {
        let ref = this.dialog.open(component, {
            width: "700px",
            data: dialogData
        })
        ref.afterClosed().subscribe(rs => {
            if (rs) {
                this.refresh()
            }
        })
    }

    public confirmDelete(message: string, callbackFunction: Function) {
        abp.message.confirm(
            message,
            "",
            async (result: boolean) => {
                if (result) {
                    await callbackFunction()
                    this.calPageAfterDelete()
                    this.refresh()
                }
            },
            true
        )
    }
    private calPageAfterDelete() {
        if (this.pageNumber > 1 && (this.totalItems - 1) % (this.pageSizeType) == 0) {
            this.totalPages -= 1
            this.pageNumber -= 1
        }
    }

    public pageControlActions(event: any) {
        let pagingAction = this.APP_ENUM.PagingActionEnum
        switch (event.action) {
            case pagingAction.PAGE_CHANGE: {
                this.getDataPage(event.value)
                break;
            }
            case pagingAction.PAGE_SIZE_CHANGE: {
                this.changePageSize(event.value)
                break;
            }
            case pagingAction.REFRESH: {
                this.refresh()
                break;
            }
        }
    }

    public setDefaultFilter(filterItem: FilterDto) {
        let isExist = this.filterItems.find(x => x.propertyName === filterItem.propertyName)
        if (!isExist) {
            this.filterItems.push(filterItem)
        }
    }

    public onTableMultiSelectFilter(listData: any, property: number){
        let filterParam = {
          value : listData,
          property: property
        }
        this.onMultiFilter(filterParam);
      }

    public onMultiFilter(dataMultiFilter:any){

        switch(dataMultiFilter.property){

          case this.filterParamType.Status: {
            this.statusIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.UserType: {
            this.userTypes = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.UserLevel: {
            this.levelIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.Branch: {
            this.branchIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.JobPosition: {
            this.jobPositionIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.DebtStatus: {
            this.debtstatusIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.PaymentType: {
            this.paymentTypeIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.ToUserType: {
            this.toUserTypeIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.ToLevel: {
            this.toLevelIds = dataMultiFilter.value;
            break;
          }

          case this.filterParamType.ToJobPosition: {
            this.toJobPositionIds = dataMultiFilter.value;
            break;
          }
        }
        this.refresh();

      }

    protected abstract list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void;

    public convertToConstant(array: any[], keyProperty: string, valueProperty: string) {
        return array.reduce((prev, cur) => {
            prev[cur[keyProperty]] = cur[valueProperty]
            return prev;
        }, {})
    }
}

export const DEFAULT_FILTER_VALUE = '-1'
export interface TeamsFilterInputDto {
    teamIds: [],
    isAndCondition: boolean;

  }
