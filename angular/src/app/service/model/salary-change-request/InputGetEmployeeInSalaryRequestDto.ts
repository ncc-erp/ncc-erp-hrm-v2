import { PagedRequestDto } from '../../../../shared/paged-listing-component-base';

export interface InputGetEmployeeInSalaryRequestDto {

    gridParam: PagedRequestDto
    branchIds: number[],
    toJobPositionIds: number[],
    toLevelIds: number[],
    toUsertypes: number[],
}
