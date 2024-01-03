import { SelectionNameIdDto, SelectionKeyValueDto } from './selection-base-info.dto'

export class ChartDetailSelectionDto {
    branches: SelectionNameIdDto[] 
    jobPositions: SelectionNameIdDto[] 
    levels: SelectionNameIdDto[] 
    teams: SelectionNameIdDto[] 
    payslipDetailTypes: SelectionKeyValueDto[] 
    userTypes: SelectionKeyValueDto[] 
    workingStatuses: SelectionKeyValueDto[] 
    gender: SelectionKeyValueDto[]

}