import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
    Router,
    RouterEvent,
    NavigationEnd,
    PRIMARY_OUTLET
} from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    selector: 'sidebar-menu',
    templateUrl: './sidebar-menu.component.html'
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
    menuItems: MenuItem[];
    menuItemsMap: { [key: number]: MenuItem } = {};
    activatedMenuItems: MenuItem[] = [];
    routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
    homeRoute = '/app/home';

    constructor(injector: Injector, private router: Router) {
        super(injector);
        this.router.events.subscribe(this.routerEvents);
    }

    ngOnInit(): void {
        this.menuItems = this.getMenuItems();
        this.patchMenuItems(this.menuItems);
        this.routerEvents
            .pipe(filter((event) => event instanceof NavigationEnd))
            .subscribe((event) => {
                const currentUrl = event.url !== '/' ? event.url : this.homeRoute;
                const primaryUrlSegmentGroup = this.router.parseUrl(currentUrl).root
                    .children[PRIMARY_OUTLET];
                if (primaryUrlSegmentGroup) {
                    this.activateMenuItems('/' + primaryUrlSegmentGroup.toString());
                }
            });
    }

    getMenuItems(): MenuItem[] {
        return [
            new MenuItem(this.l('HomePage'), '/app/home', 'fas fa-home', 'Home'),

            new MenuItem(this.l('Admin'), '', 'fas fa-circle', 'Admin', [
                new MenuItem(
                    this.l('Role'),
                    '/app/admin/roles',
                    'fas fa-theater-masks',
                    'Admin.Role.View'
                ),
                new MenuItem(
                    this.l('Tenant'),
                    '/app/admin/tenants',
                    'fas fa-building',
                    'Admin.Tenant.View'
                ),
                new MenuItem(
                    this.l('User'),
                    '/app/admin/users',
                    'fas fa-users',
                    'Admin.User.View'
                ),
                new MenuItem(
                    this.l('Configuration'),
                    '/app/admin/configurations',
                    'fas fa-gear',
                    'Admin.Configuration.View'
                ),
                new MenuItem(
                    this.l('Email template'),
                    '/app/admin/email-templates',
                    'fas fa-envelope',
                    'Admin.EmailTemplate.View'
                ),
                new MenuItem(
                    this.l('Background Job'),
                    '/app/admin/background-jobs',
                    'fas fa-clock',
                    'Admin.BackgroundJob.View'
                ),
                new MenuItem(
                    this.l('AuditLog'),
                    '/app/admin/audit-logs',
                    'fas fa-cogs',
                    'Admin.AuditLog.View'),
            ]),
            new MenuItem(
                "Category",
                '',
                'fas fa-folder',
                'Category', [
                new MenuItem(
                    this.l("Branch"),
                    '/app/categories/branches',
                    'fas fa-building',
                    'Category.Branch.View'
                ),
                new MenuItem(
                    this.l("User Type"),
                    '/app/categories/user-types',
                    'fas fa-user-check',
                    'Category.Usertype.View'
                ),
                new MenuItem(
                    this.l("Job Position"),
                    '/app/categories/job-positions',
                    'fas fa-boxes-stacked',
                    'Category.JobPosition.View'
                ),
                new MenuItem(
                    this.l("Level"),
                    '/app/categories/levels',
                    'fas fa-layer-group',
                    'Category.Level.View'
                ),
                new MenuItem(
                    this.l("Skill"),
                    '/app/categories/skills',
                    'fas fa-code',
                    'Category.Skill.View'
                ),
                new MenuItem(
                    this.l("Team"),
                    '/app/categories/teams',
                    'fas fa-people-group',
                    'Category.Team.View'
                ),
                new MenuItem(
                    this.l("Bank"),
                    '/app/categories/banks',
                    'fas fa-building-columns',
                    'Category.Bank.View'
                ),
                new MenuItem(
                    this.l("Punishment Type"),
                    '/app/categories/punishment-type',
                    'fas fa-money-check-dollar',
                    'Category.PunishmentType.View'
                ),
                new MenuItem(
                    this.l("Issued By"),
                    '/app/categories/issued-by',
                    'fa-solid fa-address-card',
                    'Category.IssuedBy.View'
                ),
            ]
            ),
            new MenuItem(
                this.l('Employee'),
                '/app/employees/list-employee',
                'fas fa-users',
                'Employee.View'
            ),
            new MenuItem(
                this.l('Warning Employee'),
                '',
                'fas fa-users',
                'WarningEmployee',[
                    new MenuItem(
                        this.l("Back To Work"),
                        '/app/warning-for-employee/back-to-work',
                        'fas fa-id-card-clip',
                        'WarningEmployee.BackToWork.View',
                    ),
                    new MenuItem(
                        this.l("Contract Expired"),
                        'warning-for-employee/update-contract',
                        'fas fa-id-card',
                        'WarningEmployee.ContractExpired.View'
                    ),
                    new MenuItem(
                        this.l("Plan Onboard Employee"),
                        'warning-for-employee/temp-employee-talent',
                        'fa-solid fa-users-between-lines',
                        'WarningEmployee.PlanOnboard'
                    ),
                    new MenuItem(
                        this.l("Request Change Info"),
                        'warning-for-employee/request-change-info',
                        'fa-solid fa-user',
                        'WarningEmployee.RequestChangeInfo.View'
                    ),
                    new MenuItem(
                        this.l("Plan Quit Employee"),
                        'warning-for-employee/plan-quit-employee',
                        'fa-solid fa-user',
                        'WarningEmployee.PlanQuitEmployee.View'
                    )
                ]
            ),
            new MenuItem(
                this.l("Debt"),
                '/app/debt/list-debt',
                'fas fa-money-bill-wave',
                'Debt.View'
            ),
            new MenuItem(
                this.l("Refund"),
                '/app/refunds/list-refund',
                'fa-regular fa-money-bill-1',
                'Refund'
            ),
            new MenuItem(
                this.l('Punishment'),
                '/app/punishments/list-punishments',
                'fas fa-money-check-dollar',
                'Punishment.View'
            ),
            new MenuItem(
                this.l('Punishment Fund'),
                '/app/punishment-funds/list-punishment-funds',
                'fas fa-filter-circle-dollar',
                'PunishmentFund.View'
            ),
            new MenuItem(
                this.l('Bonus'),
                '/app/bonuses/list-bonus',
                'fas fa-sack-dollar',
                'Bonus.View'
            ),
            new MenuItem(
                this.l('Benefit'),
                '/app/benefits/list-benefit',
                'fas fa-heart-circle-check',
                'Benefit.View'
            ),
            new MenuItem(
                this.l('Payroll'),
                '/app/payroll/list-payroll',
                'fas fa-sack-dollar',
                'Payroll.View'
            ),
             new MenuItem(
                this.l('Salary change request'),
                '/app/salary-change-requests/list-request',
                'fas fa-file-invoice-dollar',
                'SalaryChangeRequest.View'
            ),
            new MenuItem(
                this.l('Guideline'),
                'https://docs.google.com/document/d/1b--62hwvelkzwa1hcybwOfyPH7WKRbIHSodXTYysaxM/edit#heading=h.2x6675vns67e',
                'fa-solid fa-book',
                ''
            ),
            new MenuItem(
                this.l('Release note'),
                'https://docs.google.com/document/d/1huoDnNVLPwyBvlzSp7s-hPFQuPwXjrqa9SopawdatxM/edit',
                'fa-solid fa-clipboard',
                ''
            ),
        ];
    }

    patchMenuItems(items: MenuItem[], parentId?: number): void {
        items.forEach((item: MenuItem, index: number) => {
            item.id = parentId ? Number(parentId + '' + (index + 1)) : index + 1;
            if (parentId) {
                item.parentId = parentId;
            }
            if (parentId || item.children) {
                this.menuItemsMap[item.id] = item;
            }
            if (item.children) {
                this.patchMenuItems(item.children, item.id);
            }
        });
    }

    activateMenuItems(url: string): void {
        this.deactivateMenuItems(this.menuItems);
        this.activatedMenuItems = [];
        const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
        foundedItems.forEach((item) => {
            this.activateMenuItem(item);
        });
    }

    deactivateMenuItems(items: MenuItem[]): void {
        items.forEach((item: MenuItem) => {
            item.isActive = false;
            item.isCollapsed = true;
            if (item.children) {
                this.deactivateMenuItems(item.children);
            }
        });
    }

    findMenuItemsByUrl(
        url: string,
        items: MenuItem[],
        foundedItems: MenuItem[] = []
    ): MenuItem[] {
        items.forEach((item: MenuItem) => {
            if (url.includes(item.route) && !item.children) {
                foundedItems.push(item);
            } else if (item.children) {
                this.findMenuItemsByUrl(url, item.children, foundedItems);
            }
        });
        return foundedItems;
    }

    activateMenuItem(item: MenuItem): void {
        item.isActive = true;
        if (item.children) {
            item.isCollapsed = false;
        }
        this.activatedMenuItems.push(item);
        if (item.parentId) {
            this.activateMenuItem(this.menuItemsMap[item.parentId]);
        }
    }

    isMenuItemVisible(item: MenuItem): boolean {
        if (!item.permissionName) {
            return true;
        }

        if (this.permission.isGranted(item.permissionName)) {
            return true;
        }

        if (item.children != null && item.children.length > 0) {
            var check = item.children.find(i => this.permission.isGranted(i.permissionName));
            return check != null;
        }
        return this.permission.isGranted(item.permissionName);
    }
}
