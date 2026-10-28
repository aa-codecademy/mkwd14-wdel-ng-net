export type Role = 'Admin' | 'Customer';

export interface UserDto {
    id: string;
    userName: string;
    email: string;
    roles: Role[];
}