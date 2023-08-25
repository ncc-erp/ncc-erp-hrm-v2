export enum EPaymentType{
    TruLuong = 1,
    TienMat = 2
}
export const PAYMENT_TYPES: PaymentType[] = [
    {
        key: 'Trừ lương',
        value: EPaymentType.TruLuong,
    },
    {
        key: 'Tiền mặt',
        value: EPaymentType.TienMat
    }
]
export const PAYMENT_TYPES_FILTER = [
    {
        key: 'All',
        value: 'all'
    },
    ...PAYMENT_TYPES,
]


export interface PaymentType {
    key: string,
    value: number
}

export const PAYMENT_METHOD = {
    [EPaymentType.TienMat]:{
        key: 'Tiền mặt',
        value: EPaymentType.TienMat
    },
    [EPaymentType.TruLuong]:{
        key: 'Trừ lương',
        value: EPaymentType.TruLuong
    }
}