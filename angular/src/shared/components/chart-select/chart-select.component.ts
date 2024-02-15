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
import * as _ from "lodash";

@Component({
  selector: "chart-select",
  templateUrl: "./chart-select.component.html",
  styleUrls: ["./chart-select.component.css"],
})
export class ChartSelectComponent {
    @Input() dropdownData: IChartDataType[] = [];
    @Input() placeholder: string = "";
    @Input() defaultValue: number[] = [];
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
    }
    public handleOk(){
      this.multiSelect.close();
      this.onOptionSelect();
    }
    public handleClear() {
      const listSelectBySearch: number[] = this.dropdownData
        .filter((s) => !s.hidden)
        .map((item) => item.value);
      this.listSelectedId = this.listSelectedId.filter(
        (item) => !listSelectBySearch.includes(item) && item
      );
    }
    public handleClearAll() {
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
    onCancelSelect() {
      this.listSelectedId = this.defaultValue;
      this.multiSelect.close();
      this.onOptionSelect();
    }
    public isHiddenAll() {
      return !this.dropdownData.some((s) => !s.hidden);
    }
    handleOptionClick(index: number) {}

    getDataTypeLabel(dataType: number): string {
      switch (dataType) {
        case 0:
          return "Employee";
        case 1:
          return "Salary";
        default:
          return "Unknown";
      }
    }

    truncateNameOnMatOption(text, maxLength) {
      if (text.length > maxLength) {
        return text.substring(0, maxLength - 3) + "...";
      } else {
        return text;
      }
    }
  }
  export interface IChartDataType {
    name: string;
    value: number;
    dataType: number;
    hidden: boolean;
  }

 