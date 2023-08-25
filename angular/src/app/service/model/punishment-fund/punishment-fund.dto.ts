import { PagedRequestDto } from "@shared/paged-listing-component-base";

export class PunishmentFundDto{
    id: number;
    amount: number;
    date: string;
    note: string;
}
export class InputToGetAllPagingDto{
    gridParam: PagedRequestDto;
    filterByComparision: FilterByComparisonDto;
}
export class FilterByComparisonDto{
    operatorComparison :number;
    value :string;
}

