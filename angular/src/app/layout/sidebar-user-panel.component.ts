import {
  Component,
  ChangeDetectionStrategy,
  Injector,
  OnInit
} from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
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
  
  canUpdatePassword() {
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_ResetPassword) 
    
  }
  logout(): void {
    this._authService.logout();
  }
}
