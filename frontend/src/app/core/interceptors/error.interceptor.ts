import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred';
      if (error.error?.title) {
        message = error.error.title;
      } else if (error.error?.detail) {
        message = error.error.detail;
      } else if (typeof error.error === 'string') {
        message = error.error;
      } else if (error.error?.errors) {
        const errs = error.error.errors as Record<string, string[]>;
        message = Object.values(errs).flat().join('; ');
      } else if (error.message) {
        message = error.message;
      }
      snackBar.open(message, 'Dismiss', {
        duration: 6000,
        panelClass: ['error-snackbar'],
      });
      return throwError(() => error);
    })
  );
};
