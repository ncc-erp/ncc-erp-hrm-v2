import {
  Component,
  ChangeDetectionStrategy,
  Injector,
  OnInit
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Component({
  selector: 'sidebar-user-panel',
  templateUrl: './sidebar-user-panel.component.html',
})
export class SidebarUserPanelComponent extends AppComponentBase
  implements OnInit {
  shownLoginName = '';

  constructor(injector: Injector, private _authService: AppAuthService) {
    super(injector);
  }

  ngOnInit() {
    this.shownLoginName = this.appSession.getShownLoginName();
  }

  logout(): void {
    this._authService.logout();
  }
}
