import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) =>
  next(req).pipe(
    catchError((error) => {
      const detail =
        error.error?.detail ??
        error.error?.title ??
        error.message ??
        'Unexpected error';

      return throwError(() => new Error(detail));
    }),
  );
