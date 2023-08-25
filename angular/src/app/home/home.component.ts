import {Component,Injector,ChangeDetectionStrategy,OnInit,} from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { HomePageService } from "../service/api/homepage/homepage.service";
import {HomepageEmployeeStatisticDto,LastEmployeeWorkingHistoryDto,} from "../../app/service/model/homepage/HomepageEmployeeStatistic.dto";
import { MatDialog } from "@angular/material/dialog";
import { ListInfoComponent } from "./listinfo/list-info/list-info.component";
@Component({
  templateUrl: "./home.component.html",
  animations: [appModuleAnimation()],
  styleUrls: ['./home.component.css']
})
export class HomeComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private homePageService: HomePageService,
    private dialog: MatDialog
  ) {
    super(injector);
  }
  public listCty: HomepageEmployeeStatisticDto[] = [];
  public filterFromDate: string;
  public filterToDate: string;

  ngOnInit(): void {
    var date = new Date();
    this.filterFromDate = new Date(date.getFullYear(), date.getMonth(), 1).toDateString();
    this.filterToDate = new Date(date.getFullYear(), date.getMonth() + 1, 0).toDateString();
  }
  getData(startDate: string, endDate: string) {
    this.homePageService
      .GetAllWorkingHistory(startDate, endDate)
      .subscribe((rs) => {
        if (rs.success) {
          this.listCty = rs.result;
        }
      });
  }
  getValueOrNone(value: number) {
    if (value == 0) return "";
    return value;
  }
  getTextBold(branchName: string) {
    if (branchName == "Toàn công ty") {
      return "text-bold";
    }
  }
  showList(
    list: LastEmployeeWorkingHistoryDto[],
    action: string,
    branchName: string,
    title: string,
    userType?: number,
    isOnboardAndQuit?:boolean
  ) {
    if (list.length > 0) {
      switch(userType){
        case this.APP_ENUM.UserType.Internship:{
          list = list.filter(x=> x.userType == this.APP_ENUM.UserType.Internship);
          break;
        }
        case this.APP_ENUM.UserType.Staff:{
          list = list.filter(x=> x.userType != this.APP_ENUM.UserType.Internship)
          break;
        }
        default:{
          list = list;
        }
      }

      this.dialog.open(ListInfoComponent, {
        data: {
          listInfo: list,
          action: action,
          branchName: branchName,
          title:title,
          isOnboardAndQuit: isOnboardAndQuit
        },
        minWidth: "50%",
        autoFocus: false,
        restoreFocus: false
      });
    }
  }

  public onDateSelectorChange(data) {
    this.filterFromDate = data?.fromDate;
    this.filterToDate = data?.toDate;
    this.getData(this.filterFromDate, this.filterToDate);
  }
}
