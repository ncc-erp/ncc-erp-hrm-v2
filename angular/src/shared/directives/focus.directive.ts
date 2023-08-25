import { Directive,Input, OnInit } from "@angular/core";

@Directive({
    selector: '[appFocus]',
})
export class FocusDirective implements OnInit {
    @Input('appFocus') _elementRef: HTMLInputElement
    ngOnInit(): void {
        if(this._elementRef instanceof HTMLInputElement){
            this._elementRef.focus()
        }
    }
}