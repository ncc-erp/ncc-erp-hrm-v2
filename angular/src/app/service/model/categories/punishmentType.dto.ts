export interface PunishmentTypeDto{
    id: number,
    name: string,
    isActive: boolean,
    api: string,
    selected?: boolean,
    message?: string
}

export interface ResultGeneratePunishmentDto{
    punishmentTypeId: number
    message: string
}

