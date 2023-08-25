import { Directive, forwardRef, Input, OnInit } from '@angular/core'
import { Validator, AbstractControl, NG_VALIDATORS, ValidationErrors } from '@angular/forms';
import { isNull, includes } from 'lodash-es';
@Directive({
    selector: '[appForbiddenValue]',
    providers: [
        {
            provide: NG_VALIDATORS,
            useExisting: forwardRef(() => ForbiddenValueDirective),
            multi: true
        }
    ]
})
export class ForbiddenValueDirective implements Validator, OnInit {
    @Input('appForbiddenValue') forbiddenValue = 0

    ngOnInit() {
    }
    validate(c: AbstractControl): ValidationErrors | null {
        if (isNull(c.value)) return { required: true }
        if (!isNull(c.value)) {
            switch (typeof c.value) {
                case 'object':
                    if (includes(c.value, this.forbiddenValue))
                        return { forbiddenValue: true }
                case 'number':
                    if (c.value == this.forbiddenValue)
                        return { forbiddenValue: true }
                case 'string':
                    if (Number.isNaN(c.value)) return { forbiddenValue: true }
                    if (Number(c.value) == this.forbiddenValue)
                        return { forbiddenValue: true }
                default:
                    return null;
            }
        }
        return null
    }
}