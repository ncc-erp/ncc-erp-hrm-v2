import { AppConsts } from '@shared/AppConsts';
import { TenantAvailabilityState } from '@shared/service-proxies/service-proxies';


export class AppTenantAvailabilityState {
    static Available: number = TenantAvailabilityState._1;
    static InActive: number = TenantAvailabilityState._2;
    static NotFound: number = TenantAvailabilityState._3;
}
export const DATE_FORMATS = {
    parse: {
        dateInput: 'DD/MM/YYYY',
    },
    display: {
        dateInput: 'DD/MM/YYYY',
        monthYearLabel: 'DD MM YYYY',
        dateA11yLabel: 'DD MM YYYY',
        monthYearA11yLabel: 'DD MM YYYY',
    },
};
export const APP_ENUMS = {
    Branch:
    {
        "Hà Nội": 0,
        "Đà Nẵng": 1,
        "Hồ Chí Minh": 2,
        "Vinh": 3,
        Other: 4
    },
    UserBranch: {
        "HN": 0,
        "DN": 1,
        "HCM": 2,
        "Vinh": 3,
    },
    UserLevel:
    {
        Intern_0: 0,
        Intern_1: 1,
        Intern_2: 2,
        Intern_3: 3,
        FresherMinus: 4,
        Fresher: 5,
        FresherPlus: 6,
        JuniorMinus: 7,
        Junior: 8,
        JuniorPlus: 9,
        MiddleMinus: 10,
        Middle: 11,
        MiddlePlus: 12,
        SeniorMinus: 13,
        Senior: 14,
        Principal: 15,
    },
    UserType: {
        Internship: 0,
        Collaborators: 1,
        Staff: 2,
        ProbationaryStaff: 3,
        Vendor: 5

    },

    FilterTypeEnum: {
        SearchText: 0,
        Dropdown: 1,
        Radio: 2,
        DatePicker: 3,
        Compare: 4,
        MultiSelect: 5,
        MultiSelectWithCondition: 6,
        Seniority: 7,
        Birthday: 8,
        Comparison: 9

    },

    FilterMultipleTypeParamEnum: {
        Status: 1,
        UserType: 2,
        UserLevel: 3,
        Branch: 4,
        JobPosition: 5,
        DebtStatus: 6,
        PaymentType: 7,
        ToUserType: 8,
        ToJobPosition: 9,
        ToLevel: 10,
        RequestUpdateInfoStatus: 11
    },

    filterComparison: {
        EQUAL: 0,
        LESS_THAN: 1,
        LESS_THAN_OR_EQUAL: 2,
        GREATER_THAN: 3,
        GREATER_THAN_OR_EQUAL: 4,
        NOTE_EQUAL: 5,
        CONTAINS: 6,
        STARTS_WITH: 7,
        ENDS_WITH: 8,
        IN: 9
    },

    UserStatus: {
        Working: 1,
        Pausing: 2,
        Quit: 3,
        MaternityLeave: 4
    },

    BenefitType:
    {
        CheDoChung: 1,
        CheDoRieng: 2,
        CheDoRemote: 3
    },

    IsActive: {
        Active: true,
        InActive: false
    },

    IsActiveHaveAll: {
        All: '-1',
        Active: 1,
        InActive: 0,
    },

    InsuranceStatus:
    {
        BHXH: 1,
        PVI: 2,
        NONE: 3,
        PVIandBHXH: 4
    },

    Gender: {
        Male: 1,
        Female: 2
    },

    SortDirectionEnum: {
        Ascending: 0,
        Descending: 1
    },

    PagingActionEnum: {
        PAGE_CHANGE: 0,
        PAGE_SIZE_CHANGE: 1,
        REFRESH: 2
    },

    PunishmentStatus: {
        Active: 1,
        InActive: 0,

    },

    BonusStatusHaveAll: {
        All: '-1',
        Active: 1,
        InActive: 0,
    },


    BonusStatus: {
        Active: true,
        InActive: false,
    },

    BenefitTypeColor: {
        1: '#007bff',
        2: '#28a745',
        3: '#ff3b00'
    },

    PayrollStatus: {
        "New": 1,
        "PendingKT": 2,
        "PendingCEO": 3,
        "RejectedByKT": 4,
        "RejectedByCEO": 5,
        "ApprovedByCEO": 6,
        "Executed": 7
    },

    SeniorityType: {
        "Day": 1,
        "Month": 2,
        "Year": 3
    },

    ESalaryType: {
        "SalaryNormal" : 1,
        "SalaryOT" : 2,
        "SalaryMaternityLeave" : 3,
        "Benefit" : 4,
        "Bonus" : 5,
        "Punishment" : 6,
        "Debt" : 7
    },

    ESalaryRequestEmployeeType: {
        Initial : 1,
        Change : 2,
        MaternityLeave : 3,
        BackToWork : 4,
        StopWorking : 5
    },

    PayslipConfirmStatus:
    {
        NotConfirm: 0,
        ConfirmRight: 1,
        ConfirmWrong: 2
    },

    TalentOnboardStatus:
    {
        AcceptedOffer: 8,
        RejectedOffer: 9,
        Onboarded: 10
    },
    RequestUpdateInfoStatus: {
        Pending : 1,
        Approved : 2,
        Rejected : 3
    },
    DATE_TIME_OPTIONS : {
        All: 0,
        Day: 1,
        Week: 2,
        Month: 3,
        Quarter: 4,
        Year: 5,
        CustomTime: 6
    },
    PaymentTypeColor: {
        1: '#007bff',
        2:'#fd7e4e'
    },

    DateFilterMode:{
        BasicDate: 1,
        Birthday: 2,
        HomeMonthYear: 3
    }
}

export enum EmailFunc  {
    Payslip = 1,
    ContractBM = 2,
    ContractDT = 3,
    ContractTV = 4,
    ContractCTV = 5,
    ContractLD = 6,
    Debt = 7,
    Bonus = 8,
    Checkpoint = 9,
    PayrollPendingCEO = 10,
    PayrollApprovedByCEP = 11,
    PayrollRejectedByCEO= 12,
    PayrollExecuted = 13,
}

export enum TemplateType{
    Mail = 1,
    Print = 2
}
