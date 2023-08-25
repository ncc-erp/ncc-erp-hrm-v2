import { LayoutStoreService } from './../../shared/layout/layout-store.service';
import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'sidebar-logo',
  templateUrl: './sidebar-logo.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarLogoComponent {
  public sidebarExpanded: boolean = false;
  constructor( private _layoutStore : LayoutStoreService){}
  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    }); 
  }
  toggleSidebar(): void {
    this._layoutStore.setSidebarExpanded(!this.sidebarExpanded);
  }
}
