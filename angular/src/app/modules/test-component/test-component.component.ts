import { TestService } from './service/test.service';
import { UserDto } from '@shared/service-proxies/service-proxies';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { TestDialogComponent } from './test-dialog/test-dialog.component';
import { finalize, take, takeUntil } from 'rxjs/operators';
import { MatSelect } from '@angular/material/select';
import { FormControl } from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';

@Component({
  selector: 'app-test-component',
  templateUrl: './test-component.component.html',
  styleUrls: ['./test-component.component.css']
})
export class TestComponentComponent extends PagedListingComponentBase<UserDto> implements OnInit {

  public listUser: UserDto[] = []
  public filterOption: number = 0
  public statusList = [
    {
      key: "All",
      value: ""
    },
    {
      key: "Working",
      value: 0
    },
    {
      key: "Quited",
      value: 1
    }
  ]

  public columnList = [
    {
      name: "Stt",
      displayName: "#",
      isShow: true
    },
    {
      name: "User",
      displayName: "User",
      isShow: true
    },
    {
      name: "Status",
      displayName: "Status",
      isShow: true
    },
    {
      name: "Active",
      displayName: "Active",
      isShow: true
    },
    {
      name: "Salary",
      displayName: "Salary",
      isShow: true
    },
    {
      name: "CreationTime",
      displayName: "CreationTime",
      isShow: true
    },
  ]
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.testService.getAllUserPaging(request)
      .pipe(finalize(() => {
        finishedCallback();
      })).subscribe(rs => {
        this.listUser = rs.result.items
        this.showPaging(rs.result, pageNumber);
      })
  }

  constructor(private testService: TestService, injector: Injector) {
    super(injector);
  }

  public openDialog() {
    this.dialog.open(TestDialogComponent, {
      width: "600px",
      panelClass: 'dialog-container-custom'
    })
  }


  protected banks: Bank[] = BANKS;

  public bankMultiCtrl: FormControl = new FormControl();

  public bankMultiFilterCtrl: FormControl = new FormControl();

  public filteredBanksMulti: ReplaySubject<any[]> = new ReplaySubject<any[]>(
    1
  );

  @ViewChild("multiSelect") multiSelect: MatSelect;

  protected _onDestroy = new Subject<void>();


  ngOnInit() {
    this.refresh()

    this.bankMultiCtrl.setValue([
      this.banks[2],
      this.banks[4],
      this.banks[7],
    ]);

    this.filteredBanksMulti.next(this.banks.slice());

    this.bankMultiFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBanksMulti();
      });

    // let currentColumnList = localStorage.getItem("userTableColumn")
    // if (!currentColumnList) {
    //   localStorage.setItem("userTableColumn", JSON.stringify(this.columnList))
    // }
    // else {
    //   if (this.compareWithLocalStorage()) {
    //     this.columnList = JSON.parse(currentColumnList)
    //   }
    //   else{
    //     localStorage.setItem("userTableColumn", JSON.stringify(this.columnList))
    //   }
    // }
  }
  
  // compareWithLocalStorage() {
  //   let localColumnList = JSON.parse(localStorage.getItem("userTableColumn"))
  //   localColumnList = localColumnList.map(item => item.name)
  //   let currentColumnList = this.columnList.map(item => item.name)
  //   if (JSON.stringify(localColumnList) == JSON.stringify(currentColumnList)) {
  //     return true
  //   }
  //   return false
  // }

  ngAfterViewInit() {
    this.setInitialValue();
  }

  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  /**
   */
  protected setInitialValue() {
    this.filteredBanksMulti
      .pipe(
        take(1),
        takeUntil(this._onDestroy)
      )
      .subscribe(() => {
        this.multiSelect.compareWith = (a: any, b: any) =>
          a && b && a.id === b.id;
      });
  }

  protected filterBanksMulti() {
    if (!this.banks) {
      return;
    }
    let search = this.bankMultiFilterCtrl.value;
    if (!search) {
      this.filteredBanksMulti.next(this.banks.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredBanksMulti.next(
      this.banks.filter(bank => bank.name.toLowerCase().indexOf(search) > -1)
    );
  }

  clearSelected(item) {
    this.bankMultiCtrl.value.splice(this.bankMultiCtrl.value.indexOf(item),1)
    let index = this.banks.indexOf(item)
    let itemToinsert = {...item}
    this.banks.splice(this.banks.indexOf(item),1)
    this.banks.splice(index,0,itemToinsert )
    let search = this.bankMultiFilterCtrl.value;
    if (!search) {
      this.filteredBanksMulti.next(this.banks.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredBanksMulti.next(
      this.banks.filter(bank => bank.name.toLowerCase().indexOf(search) > -1)
    );
  }
  clearAll(){
    let a = [...this.banks]
    this.bankMultiCtrl.reset()
    this.filteredBanksMulti.next(a);
  }



}
export const BANKS: Bank[] = [
  { name: '.Net', id: 'A' },
  { name: 'Angular', id: 'B' },
  { name: 'Unit test', id: 'C' },
  { name: 'Auto test', id: 'D' },
  { name: 'Java', id: 'E' },
  { name: 'React', id: 'F' },
  { name: 'Vue', id: 'G' },
  { name: 'Python', id: 'H' },
  { name: 'Kolin', id: 'I' },

];

export interface Bank {
  id: string;
  name: string;
}
