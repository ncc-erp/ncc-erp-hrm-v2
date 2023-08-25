import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'branch'
})
export class BranchPipe implements PipeTransform {

  transform(branchEnum: number, isStyle:boolean){
    switch(branchEnum){
      case 0: {
        return isStyle ? "badge badge-pill badge-danger" : "HN"
      }
      case 1:{
        return isStyle ? "badge badge-pill badge-success" : "DN"
      }
      case 2:{
        return isStyle ? "badge badge-pill badge-primary" : "HCM"
      }
      case 3:{
        return isStyle ? "badge badge-pill badge-warning" : "Vinh"
      }
    }
    return "";
  }

}
