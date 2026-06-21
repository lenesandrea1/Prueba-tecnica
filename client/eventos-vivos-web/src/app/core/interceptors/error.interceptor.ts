import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { translateApiError } from '../utils/api-error-messages';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error) => {
      const detail =
        error.error?.detail ??
        error.error?.title ??
        error.message ??
        'Error inesperado';

      return throwError(() => new Error(translateApiError(detail)));
    }),
  );
