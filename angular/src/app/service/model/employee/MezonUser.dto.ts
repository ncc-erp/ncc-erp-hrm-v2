export interface IUserMezonDto{
    email: string
    mezon_id: string
    user: {
        avatar_url: string;
        display_name: string;
        id: string;
        username:string;
    }
}
export interface IHashMezonAuthModel {
   hashData: string
    tenancyName: string;
}