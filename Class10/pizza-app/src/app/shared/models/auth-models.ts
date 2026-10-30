import { Role } from "./user-models";

export interface RegisterRequestDto {
    userName: string;
    email: string;
    password: string;
}

export interface LoginRequestDto {
    userName: string;
    password: string;
}

export interface LoginResponseDto {
    token: string;
    expiresAt: string;
    userName: string;
    roles: Role[];
}

