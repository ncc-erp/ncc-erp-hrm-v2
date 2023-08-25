import { BehaviorSubject } from "rxjs";

export class AppConsts {

    static remoteServiceBaseUrl: string;
    static appBaseUrl: string;
    static appBaseHref: string; // returns angular's base-href parameter value if used during the publish
    static googleClientId: string;
    static localeMappings: any = [];
    static enableNormalLogin: boolean;
    static calSalaryProcess =  new BehaviorSubject<any>({})
    static readonly userManagement = {
        defaultAdminUserName: 'admin'
    };

    static readonly localization = {
        defaultLocalizationSourceName: 'HRMv2'
    };

    static readonly authorization = {
        encryptedAuthTokenName: 'enc_auth_token'
    };

    static readonly userBranchStyle = {
        0: "badge badge-pill badge-danger",
        1: "badge badge-pill badge-success",
        2: "badge badge-pill badge-primary",
        3: "badge badge-pill badge-warning",
    }

    static readonly userType = {
        0: {
            name: "TTS",
            class: "badge badge-success"
        },
        1: {
            name: "CTV",
            class: "badge badge-primary"
        },
        2: {
            name: "Staff",
            class: "badge badge-danger"
        },
        3: {
            name: "T.Việc",
            class: "badge badge-warning"
        },
        4: {
            name: "FakeUser",
            class: "badge badge-secondary"
        },
        5: {
            name: "Vendor",
            class: "badge vendor-userType"
        },
    }

    static readonly userStatus = {
        1: {
            name: "Working",
            class: "badge badge-pill bg-success"
        },
        2: {
            name: "Pausing",
            class: "badge badge-pill bg-warning"
        },
        3: {
            name: "Quited",
            class: "badge badge-pill bg-secondary"
        },
        4: {
            name: "MaternityLeave",
            class: "badge badge-pill bg-primary"
        },
    }

    static readonly userLevel =
        {
            0: {
                name: "Intern_0",
                style: { 'background-color': '#B2BEB5' }
            },
            1: {
                name: "Intern_1",
                style: { 'background-color': '#8F9779' }
            },
            2: {
                name: "Intern_2",
                style: { 'background-color': '#665D1E', 'color': 'white' }
            },
            3: {
                name: "Intern_3",
                style: { 'background-color': '#777' }
            },
            4: {
                name: "Fresher-",
                style: { 'background-color': '#60b8ff' }
            },
            5: {
                name: "Fresher",
                style: { 'background-color': '#318CE7' }
            },
            6: {
                name: "Fresher+",
                style: { 'background-color': '#1f75cb' }
            },
            7: {
                name: "Junior-",
                style: { 'background-color': '#ad9fa1' }
            },
            8: {
                name: "Junior",
                style: { 'background-color': '#A57164' }
            },
            9: {
                name: "Junior+",
                style: { 'background-color': '#3B2F2F' }
            },
            10: {
                name: "Middle-",
                style: { 'background-color': '#A4C639' }
            },
            11: {
                name: "Middle",
                style: { 'background-color': '#3bab17' }
            },
            12: {
                name: "Middle+",
                style: { 'background-color': '#008000' }
            },
            13: {
                name: "Senior-",
                style: { 'background-color': '#c36285' }
            },
            14: {
                name: "Senior",
                style: { 'background-color': '#AB274F' }
            },
            15: {
                name: "Principal",
                style: { 'background-color': '#902ee1' }
            },
        }
    static readonly defaultBadgeColor = '#000000'
    static readonly punishmentStatus = {
        0: {
            name: "InActive",
            class: "badge badge-pill badge-warning",
        },
        1: {
            name: "Active",
            class: "badge badge-pill badge-success",
        },
    }
    static readonly payRollStatus = {
        "New": "badge badge-pill badge-primary",
        "Pending KT": "badge badge-pill badge-warning",
        "Pending CEO": "badge badge-pill statusPendingCEO",
        "Rejected by KT": "badge badge-pill badge-secondary",
        "Rejected by CEO": "badge badge-pill badge-danger",
        "Approved by CEO": "badge badge-pill badge-success",
        "Executed": "badge badge-pill statusExecuted"

    }
    static readonly DEFAULT_ALL_FILTER_VALUE = -1
    static readonly NORMAL_DATE = 'dd-MM-YYYY'
    static readonly LONG_DATE = 'dd-MM-YYYY hh:mm'
    static readonly MONTH_YEAR = 'MM-YYYY'
    static readonly TemplateType = {
        1: {
            name: "Mail",
            class: "badge badge-success"
        },
        2: {
            name: "Print",
            class: "badge badge-warning"
        }
    }
    static readonly PlanQuitEmployeeStatus = {
        "Confirm quit": "badge badge-pill badge-secondary",
        "Confirm pause": "badge badge-pill badge-warning",
        "Plan quit": "badge badge-pill badge-danger",
        "Plan pause": "badge badge-pill statusPendingCEO",

    }
}

export const BenefitType =
{
    "Chế độ chung": 1,
    "Chế độ riêng": 2,
    "Chế độ remote": 3
}

export const UserTypeCode =
{
    Intern : "INTERN",
    Staff : "STAFF",
    PROBATIONARY_STAFF : "PROBATIONARYSTAFF",
    COLLABORATORS : "COLLABORATORS",
    Vendor: "Vendor"
}
export const DATE_TIME_OPTIONS = {
    "All": 0,
    "Day": 1,
    "Week": 2,
    "Month": 3,
    "Quarter": 4,
    "Year": 5,
    "Custom Time": 6
}

