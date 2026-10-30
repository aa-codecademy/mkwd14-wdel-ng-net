import { HttpClient } from '@angular/common/http';
import { computed, inject, Service, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LoginRequestDto, LoginResponseDto } from '../../shared/models/auth-models';
import { Observable, tap } from 'rxjs';

const SESSION_KEY = 'pizza-app.session';

@Service()
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly controller = 'auth';
    private readonly url = `${environment.apiUrl}/${this.controller}`;

    private readonly userSession = signal<LoginResponseDto | null>(readSavedUserSession());

    readonly isLoggedIn = computed(() => this.userSession() !== null);
    readonly isAdmin = computed(() => this.userSession()?.roles.includes('Admin') ?? false);
    readonly userName = computed(() => this.userSession()?.userName ?? null);
    readonly token = computed(() => this.userSession()?.token ?? null);

    login(credentials: LoginRequestDto): Observable<LoginResponseDto> {
        return this.http
            .post<LoginResponseDto>(`${this.url}/login`, credentials)
            // .post<LoginResponseDto>(`${this.url}/login`, credentials, {
            //     headers: {
            //         Authorization: `Bearer ${this.token()}`
            //     }
            // })
            .pipe(tap(user => this.saveUserSession(user)));
    }

    logout(): void {
        localStorage.removeItem(SESSION_KEY);
        this.userSession.set(null);
    }

    private saveUserSession(session: LoginResponseDto): void {
        localStorage.setItem(SESSION_KEY, JSON.stringify(session));
        this.userSession.set(session);
    }
}

function readSavedUserSession(): LoginResponseDto | null {
    try {
        const savedUser = localStorage.getItem(SESSION_KEY);
        if (!savedUser) {
            return null;
        }

        const session = JSON.parse(savedUser) as LoginResponseDto;
        if (new Date(session.expiresAt) > new Date()) {
            return session;
        }
    } catch {
        // Not valid JSON (someone edited it by hand): treat it like no session
    }

    localStorage.removeItem(SESSION_KEY);
    return null;
}