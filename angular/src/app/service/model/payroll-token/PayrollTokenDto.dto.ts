export class PayrollTokenDto{
    public month: string;
    public payrollId: number;
}

export class PayslipTokenDto{
    public emailAddress: string;
    public note: string;
    public amount: number;
    public status : number;
}