import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  OnDestroy,
  ChangeDetectionStrategy,
  SimpleChange,
  OnChanges,
  ElementRef,
} from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatSelect } from "@angular/material/select";
import { ReplaySubject, Subject } from "rxjs";
import { take, takeUntil } from "rxjs/operators";

@Component({
  selector: "chart-select",
  templateUrl: "./chart-select.component.html",
  styleUrls: ["./chart-select.component.css"],
})
export class ChartSelectComponent {
  @Input() dropdownData: IChartDataType[] = [];
  @Input() placeholder: string = "";
  @Input() defaultValue: IChartDataType[] = [];
  @Input() label?: string = "";
  @Input() className?: string = "";
  @Output() onSelect = new EventEmitter();
  listSelectedId: number[] = [];
  searchText: string = "";

  @ViewChild("multiSelect") multiSelect: MatSelect;
  @ViewChild("inputSearch") inputSearch: ElementRef;

  ngOnChanges(change: SimpleChange) {
    if (change["defaultValue"]) {
      this.listSelectedId = change["defaultValue"].currentValue;
    }
    if (change["dropdownData"]) {
      this.dropdownData = change["dropdownData"].currentValue;
    }
  }

  constructor() {}

  ngOnInit() {}

  handleSearch() {
    const searchText = this.searchText.trim().toLowerCase();
    if (this.searchText) {
      this.dropdownData = this.dropdownData.map((item) => {
        if (!item.name.trim().toLowerCase().includes(searchText))
          item.hidden = true;
        else item.hidden = false;
        return item;
      });
    } else {
      this.dropdownData = this.dropdownData.map((s) => {
        s.hidden = false;
        return s;
      });
    }
  }

  handleOpenChange(event: boolean) {
    if (event) {
      this.inputSearch.nativeElement.focus();
    }
  }

  public handleSelectAll() {
    this.listSelectedId = [
      ...new Set([
        ...this.listSelectedId,
        ...this.dropdownData.filter((s) => !s.hidden).map((item) => item.value),
      ]),
    ];
    this.onOptionSelect();
  }

  public handleClear() {
    const listSelectBySearch: number[] = this.dropdownData
      .filter((s) => !s.hidden)
      .map((item) => item.value);
    this.listSelectedId = this.listSelectedId.filter(
      (item) => !listSelectBySearch.includes(item) && item
    );
    this.onOptionSelect();
  }

  public onOptionSelect() {
    this.onSelect.emit(this.listSelectedId);
  }
  public isHiddenAll() {
    return !this.dropdownData.some((s) => !s.hidden);
  }
  handleOptionClick(index: number) {}
}

export interface IChartDataType {
  name: string;
  value: any;
  dataType: number;
  hidden: boolean;
}
