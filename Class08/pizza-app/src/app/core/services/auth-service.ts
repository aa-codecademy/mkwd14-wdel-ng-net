import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LoginRequestDto, LoginResponseDto } from '../../shared/models/auth-models';
import { Observable } from 'rxjs';

@Service()
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly controller = 'auth';
    private readonly url = `${environment.apiUrl}/${this.controller}`

    login(credentials: LoginRequestDto): Observable<LoginResponseDto> {
        return this.http.post<LoginResponseDto>(`${this.url}/login`, credentials);
    }
}
