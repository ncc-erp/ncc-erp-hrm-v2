export interface RefundDto
{
    id:number,
    name: string,
    date:any,
    isActive: boolean
    updateNote?:boolean
}

export interface CreateRefundDto{
    name: string,
    date:string,
}

export interface UpdateRefundDto{
    id:number,
    name: string,
    date:string,
    isActive:boolean
}
