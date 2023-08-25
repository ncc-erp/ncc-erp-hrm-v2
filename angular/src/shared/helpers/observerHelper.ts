import { Observable, of } from 'rxjs'
import { tap, switchMap } from 'rxjs/operators'
export function startWithTap<T>(callback: () => void) {
    return (source: Observable<T>) =>
        of({}).pipe(tap(callback), switchMap((o) => source));
}