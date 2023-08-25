import { ESalaryChangeRequestStatus } from "./GetSalaryChangeRequestDto";

export interface UpdateChangeRequestDto{
    requestId: number,
    status: ESalaryChangeRequestStatus
}