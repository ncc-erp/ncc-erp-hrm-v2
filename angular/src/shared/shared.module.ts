import { InputOnCellComponent } from './components/input-on-cell/input-on-cell.component';
import { DMYDateFormatPipe } from './pipes/dmy-date-format.pipe';
import { EditDeleteButtonComponent } from './components/common/edit-delete-button/edit-delete-button.component';
import { CreateButtonComponent } from './components/common/create-button/create-button.component';
import { CommonModule } from '@angular/common';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppSessionService } from './session/app-session.service';
import { AppUrlService } from './nav/app-url.service';
import { AppAuthService } from './auth/app-auth.service';
import { AppRouteGuard } from './auth/auth-route-guard';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

import { AbpPaginationControlsComponent } from './components/pagination/abp-pagination-controls.component';
import { AbpValidationSummaryComponent } from './components/validation/abp-validation.summary.component';
import { AbpModalHeaderComponent } from './components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from './components/modal/abp-modal-footer.component';
import { LayoutStoreService } from './layout/layout-store.service';

import { BusyDirective } from './directives/busy.directive';
import { EqualValidator } from './directives/equal-validator.directive';

//import angular material
import {MatAutocompleteModule} from '@angular/material/autocomplete'
import {MatBadgeModule} from '@angular/material/badge'
import {MatBottomSheetModule} from '@angular/material/bottom-sheet'
import {MatButtonModule} from '@angular/material/button'
import {MatButtonToggleModule} from '@angular/material/button-toggle'
import {MatCardModule} from '@angular/material/card'
import {MatCheckboxModule} from '@angular/material/checkbox'
import {MatChipsModule} from '@angular/material/chips'
import {MatDatepickerModule} from '@angular/material/datepicker'
import {MatDialogModule} from '@angular/material/dialog'
import {MatDividerModule} from '@angular/material/divider'
import {MatExpansionModule} from '@angular/material/expansion'
import {MatGridListModule} from '@angular/material/grid-list'
import {MatIconModule} from '@angular/material/icon'
import {MatInputModule} from '@angular/material/input'
import {MatListModule} from '@angular/material/list'
import {MatMenuModule} from '@angular/material/menu'
import {DateAdapter, MatNativeDateModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from '@angular/material/core'
import {MatPaginatorModule} from '@angular/material/paginator'
import {MatProgressBarModule} from '@angular/material/progress-bar'
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner'
import {MatRadioModule} from '@angular/material/radio'
import {MatSidenavModule} from '@angular/material/sidenav'
import {MatSliderModule} from '@angular/material/slider'
import {MatSlideToggleModule} from '@angular/material/slide-toggle'
import {MatSnackBarModule} from '@angular/material/snack-bar'
import {MatSortModule} from '@angular/material/sort'
import {MatStepperModule} from '@angular/material/stepper'
import {MatTableModule} from '@angular/material/table'
import {MatTabsModule} from '@angular/material/tabs'
import {MatToolbarModule} from '@angular/material/toolbar'
import {MatTooltipModule} from '@angular/material/tooltip'
import {MatTreeModule} from '@angular/material/tree'
import {DragDropModule} from '@angular/cdk/drag-drop';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import { PercentageMaskDirective } from './directives/percentage-mask.directive';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { ListFilterPipe } from './pipes/list-filter.pipe';
import { SortableComponent } from './components/sortable/sortable.component';
import { DialogHeaderComponent } from './components/common/dialog-header/dialog-header.component';
import { DialogFooterComponent } from './components/common/dialog-footer/dialog-footer.component';
import { PaginationControlComponent } from './components/pagination-control/pagination-control.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { BranchPipe } from './pipes/branch.pipe';
import { UserTypePipe } from './pipes/user-type.pipe';
import { TableFilterComponent } from './components/table-filter/table-filter.component';
import { TableToggleColumnComponent } from './components/table-toggle-column/table-toggle-column.component';
import { UserLevelPipe } from './pipes/user-level.pipe';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { SearchComponent } from './components/search/search.component';
import { AddEmployeeComponent } from './components/employee/add-employee/add-employee.component';
import { EmployeeFilterComponent } from './components/employee/employee-filter/employee-filter.component';
import { UserStatusPipe } from './pipes/user-status.pipe';
import { IsActivePipe } from './pipes/is-active.pipe';
import { ListEmployeeComponent } from './components/employee/list-employee/list-employee.component';
import { SelectedEmployeeComponent } from './components/employee/selected-employee/selected-employee.component';
import { MultiSelectBoxComponent } from './components/multi-select-box/multi-select-box.component';
import {
    NgxMatDateFormats,
    NgxMatDatetimePickerModule,
    NgxMatNativeDateModule,
    NgxMatTimepickerModule,
    NGX_MAT_DATE_FORMATS
} from '@angular-material-components/datetime-picker';

// import {DialogComponentModule} from './dialog-component/dialog-component.module';
// import { ErrorPermissionComponent } from './interceptor-errors/error-permission/error-permission.component'
import { NgxMaskModule } from 'ngx-mask';
import { SelectSearchComponent } from './components/select-search/select-search.component';
import { SaveCancelButtonComponent } from './components/common/save-cancel-button/save-cancel-button.component';
import { BreadCrumbComponent } from './components/common/bread-crumb/bread-crumb.component';
import { UploadFileComponent } from './components/upload-file/upload-file.component';
import {TeamPipe} from './pipes/team.pipe'
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { APP_DATE_FORMATS, CUSTOM_MAT_DATE_FORMATS } from './custom-date-adapter';
import { ShortMoneyPipe } from './pipes/short-money.pipe'
import { ForbiddenValueDirective } from 'shared/directives/forbiddenValue.directive'
import { FocusDirective } from 'shared/directives/focus.directive';
import { MoneyFormatPipe } from './pipes/money-format.pipe'
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';
import { ChipComponent } from './components/chip/chip.component'
import { EditButtonComponent } from './components/common/edit-button/edit-button.component';
import { DeleteButtonComponent } from './components/common/delete-button/delete-button.component'
import { NgxMatMomentModule } from '@angular-material-components/moment-adapter';
import { DmyHmDateFormatPipe } from './pipes/dmy-hm-date-format.pipe';
import { MyDateFormatPipe } from './pipes/my-date-format.pipe';
import { DateSelectorComponent } from './date-selector/date-selector/date-selector.component';
import { CustomTimeComponent } from './date-selector/custom-time/custom-time.component';
import { BirthdayDateFormatDirective } from './directives/birthday-date-format.directive';
@NgModule({
    imports: [
        CommonModule,
        RouterModule,
        NgxPaginationModule,
        MatAutocompleteModule,
        MatBadgeModule,
        MatBottomSheetModule,
        MatButtonModule,
        MatButtonToggleModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDatepickerModule,
        MatDialogModule,
        MatDividerModule,
        MatExpansionModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatListModule,
        MatMenuModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSidenavModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatSortModule,
        MatStepperModule,
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        MatTooltipModule,
        MatTreeModule,
        MatFormFieldModule,
        MatSelectModule,
        FormsModule,
        DragDropModule,
        NgxMatSelectSearchModule,
        ReactiveFormsModule,
        NgxMaskModule.forRoot(),
        EditorModule,
        NgxMatDatetimePickerModule,
        NgxMatTimepickerModule,
        NgxMatNativeDateModule,
        NgxMatMomentModule,

    ],
    declarations: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        PercentageMaskDirective,
        UserInfoComponent,
        FileUploadComponent,
        ListFilterPipe,
        SortableComponent,
        DialogHeaderComponent,
        DialogFooterComponent,
        PaginationControlComponent,
        BranchPipe,
        UserTypePipe,
        TableFilterComponent,
        TableToggleColumnComponent,
        UserLevelPipe,
        SearchComponent,
        AddEmployeeComponent,
        AddEmployeeComponent,
        EmployeeFilterComponent,
        UserStatusPipe,
        IsActivePipe,
        ListEmployeeComponent,
        SelectedEmployeeComponent,
        MultiSelectBoxComponent,
        SelectSearchComponent,
        CreateButtonComponent,
        EditDeleteButtonComponent,
        SaveCancelButtonComponent,
        BreadCrumbComponent,
        UploadFileComponent,
        TeamPipe,
        ShortMoneyPipe,
        ForbiddenValueDirective,
        FocusDirective,
        ChipComponent,
        DMYDateFormatPipe,
        MoneyFormatPipe,
        EditButtonComponent,
        DeleteButtonComponent,
        DmyHmDateFormatPipe,
        MyDateFormatPipe,
        DateSelectorComponent,
        CustomTimeComponent,
        BirthdayDateFormatDirective,
        UserInfoComponent,
        InputOnCellComponent,

    ],
    exports: [
        AbpPaginationControlsComponent,
        AbpValidationSummaryComponent,
        AbpModalHeaderComponent,
        AbpModalFooterComponent,
        SortableComponent,
        LocalizePipe,
        BusyDirective,
        EqualValidator,
        MatAutocompleteModule,
        MatBadgeModule,
        MatBottomSheetModule,
        MatButtonModule,
        MatButtonToggleModule,
        MatCardModule,
        MatCheckboxModule,
        MatChipsModule,
        MatDatepickerModule,
        MatDialogModule,
        MatDividerModule,
        MatExpansionModule,
        MatGridListModule,
        MatIconModule,
        MatInputModule,
        MatListModule,
        MatMenuModule,
        MatNativeDateModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSelectModule,
        MatSidenavModule,
        MatSliderModule,
        MatSlideToggleModule,
        MatSnackBarModule,
        MatSortModule,
        MatStepperModule,
        MatTableModule,
        MatTabsModule,
        MatToolbarModule,
        MatTooltipModule,
        MatTreeModule,
        MatFormFieldModule,
        PercentageMaskDirective,
        UserInfoComponent,
        FileUploadComponent,
        ListFilterPipe,
        DragDropModule,
        DialogHeaderComponent,
        DialogFooterComponent,
        PaginationControlComponent,
        UserTypePipe,
        BranchPipe,
        UserLevelPipe,
        TableFilterComponent,
        TableToggleColumnComponent,
        FormsModule,
        TabsModule,
        NgxPaginationModule,
        SearchComponent,
        AddEmployeeComponent,
        IsActivePipe,
        NgxMatSelectSearchModule,
        EmployeeFilterComponent,
        ListEmployeeComponent,
        NgxMatSelectSearchModule,
        EmployeeFilterComponent,
        MultiSelectBoxComponent,
        NgxMaskModule,
        SelectSearchComponent,
        CreateButtonComponent,
        EditDeleteButtonComponent,
        SaveCancelButtonComponent,
        BreadCrumbComponent,
        CreateButtonComponent,
        NgxMaskModule,
        UploadFileComponent,
        ShortMoneyPipe,
        ForbiddenValueDirective,
        FocusDirective,
        TeamPipe,
        DMYDateFormatPipe,
        DmyHmDateFormatPipe,
        MyDateFormatPipe,
        MoneyFormatPipe,
        EditorModule,
        NgxMatDatetimePickerModule,
        NgxMatTimepickerModule,
        NgxMatNativeDateModule,
        NgxMatMomentModule,
        ChipComponent,
        EditButtonComponent,
        DeleteButtonComponent,
        DateSelectorComponent,
        CustomTimeComponent,
        InputOnCellComponent,
    ],
    providers:[
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
        {
          provide: MAT_DATE_FORMATS,
          useValue: APP_DATE_FORMATS
        },
        { provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' },
        { provide: NGX_MAT_DATE_FORMATS, useValue: CUSTOM_MAT_DATE_FORMATS }
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
        return {
            ngModule: SharedModule,
            providers: [
                AppSessionService,
                AppUrlService,
                AppAuthService,
                AppRouteGuard,
                LayoutStoreService,
            ]
        };
    }
}

