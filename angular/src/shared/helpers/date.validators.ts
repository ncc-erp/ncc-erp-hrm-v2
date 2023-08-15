import { AbstractControl} from '@angular/forms'
import * as moment from 'moment'
export function AddSimpleDateValidator(startDateControl: AbstractControl, endDateControl: AbstractControl){
    startDateControl.addValidators((control: AbstractControl) => {
        if(endDateControl.untouched || !endDateControl.value) return;
        if (moment(endDateControl.value).toDate() <= moment(startDateControl.value).toDate()) {
            endDateControl.setErrors({ endDateInput: true })
            return null
        }
    })
    endDateControl.addValidators((control: AbstractControl) => {
        if(!startDateControl.value) return;
        if (moment(endDateControl.value).toDate() <= moment(startDateControl.value).toDate()) {
            return { endDateInput: true}
        }
    })
}