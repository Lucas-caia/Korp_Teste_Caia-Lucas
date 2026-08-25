import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const token = auth.token();
  const targetsApi = [environment.authApiUrl, environment.inventoryApiUrl, environment.billingApiUrl]
    .some(url => request.url.startsWith(url));

  const authenticatedRequest = token && targetsApi
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authenticatedRequest).pipe(
    catchError(error => {
      if (error.status === 401 && !request.url.endsWith('/auth/login')) {
        auth.logout();
      }
      return throwError(() => error);
    })
  );
};
