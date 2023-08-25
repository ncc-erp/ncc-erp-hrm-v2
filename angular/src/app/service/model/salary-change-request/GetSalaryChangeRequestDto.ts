export interface GetSalaryChangeRequestDto {
    id: number,
    name: string,
    applyMonth: string,
    status: number,
    creatorUser: string,
    creationTime: string,
    lastModifyUser: string,
    lastModifyTime: string
}

export interface SalaryChangeRequest {
    id: number,
    name: string,
    applyMonth: string,
    creationTime: string,
    creatorUser: string,
    status: number,
}

export enum ESalaryChangeRequestStatus {
    New = 1,
    Pending = 2,
    Approved = 3,
    Rejected = 4,
    Executed = 5
}
export const SalaryChangeRequestStatusList = {
    [ESalaryChangeRequestStatus.New]: {
        key: 'New',
        value: ESalaryChangeRequestStatus.New,
        color: '#007bff',
        class: 'bg-primary'
    },
    [ESalaryChangeRequestStatus.Pending]: {
        key: 'Pending',
        value: ESalaryChangeRequestStatus.Pending,
        color: '#20c997',
        class: 'bg-cyan'
    },
    [ESalaryChangeRequestStatus.Approved]: {
        key: 'Approved',
        value: ESalaryChangeRequestStatus.Approved,
        color: '#28a745',
        class: 'bg-success'
    },
    [ESalaryChangeRequestStatus.Rejected]: {
        key: 'Rejected',
        value: ESalaryChangeRequestStatus.Rejected,
        color: '#dc3545',
        class: 'bg-danger'
    },
    [ESalaryChangeRequestStatus.Executed]: {
        key: 'Executed',
        value: ESalaryChangeRequestStatus.Executed,
        color: '#5454E8',
        class: 'bg-indigo'
    },
}