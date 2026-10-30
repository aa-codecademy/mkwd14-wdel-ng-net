import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, EMPTY } from 'rxjs';
import { AuthService } from '../services/auth-service';
import { NotificationService } from '../services/notification-service';
import { ErrorResponse } from '../../shared/models/error-response';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 0) {
        notificationService.error("Can't reach the server. Is the API running?");
      } else if (error.status === 401 && authService.isLoggedIn()) {
        // the token expired
        authService.logout();
        notificationService.error('Your session has expired. Please log in again.');
        router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
      } else {
        notificationService.error(toMessage(error));
      }

      return EMPTY;
    })
  );
};

function toMessage(error: HttpErrorResponse): string {
  const body = error.error as ErrorResponse | null;

  if (body?.errors?.length) {
    return body.errors.join(' ');
  }

  return body?.message ?? 'Something went wrong. Please try again.';
}
