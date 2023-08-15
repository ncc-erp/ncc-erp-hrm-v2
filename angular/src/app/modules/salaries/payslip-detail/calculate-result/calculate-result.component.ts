import { Router } from '@angular/router';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CalculateResultDto, ErroResultDto } from '@app/service/model/salaries/salaries.dto';

@Component({
  selector: 'app-calculate-result',
  templateUrl: './calculate-result.component.html',
  styleUrls: ['./calculate-result.component.css']
})
export class CalculateResultComponent implements OnInit {

  calculateResult = {} as CalculateResultDto

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private router:Router) {
    this.calculateResult = data
   }

  ngOnInit(): void {

  }

  public viewDetail(result: ErroResultDto){
    const url = this.router.serializeUrl(
      this.router.createUrlTree([`/app/debt/list-debt/detail/${result.referenceId}`])
    );

    window.open(url, '_blank');
  }

}
