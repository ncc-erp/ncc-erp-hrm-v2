import { PagedRequestDto } from '../../../../shared/paged-listing-component-base';
export interface GetInputFilterDto {
    gridParam: PagedRequestDto
    addedEmployeeIds?: number[],
    statusIds: number[],
    teamIds: number[],
    branchIds: number[],
    levelIds: number[],
    userTypes: number[],
    jobPositionIds: number[],
    isAndCondition: boolean,
    seniority?: SeniorityFilterDto,
    birthdayFromDate: string,
    birthdayToDate: string,
    daysLeftContractEndDate?: number
}

export interface SeniorityFilterDto{
    comparison: number,
    seniorityType: number,
    seniorityValue: string
}

export interface GetInputFilterRequestUpdateInfoDto {
    gridParam: PagedRequestDto,
    requestStatuses: number[],
}