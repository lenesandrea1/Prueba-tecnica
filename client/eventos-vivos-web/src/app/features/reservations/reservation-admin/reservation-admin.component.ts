import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReservationsApiService } from '../../../core/services/reservations-api.service';
import { isValidGuid } from '../../../core/utils/api-error-messages';

function reservationIdValidator(control: AbstractControl): ValidationErrors | null {
  const value = (control.value as string | null)?.trim() ?? '';
  if (!value) return null;
  return isValidGuid(value) ? null : { invalidGuid: true };
}

@Component({
  selector: 'app-reservation-admin',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  template: `
    <mat-card class="glass-card">
      <mat-card-header>
        <mat-card-title>Gestión de reservas</mat-card-title>
        <mat-card-subtitle>Confirmar pagos o cancelar reservas por UUID</mat-card-subtitle>
      </mat-card-header>

      <mat-card-content>
        <p class="hint">
          Tras reservar entradas en un evento, copia el <strong>ID de reserva</strong> (UUID) del mensaje de éxito
          y pégalo aquí. No uses texto libre como "testeo".
        </p>

        <form [formGroup]="form" class="form">
          <mat-form-field appearance="fill" class="full">
            <mat-label>ID de reserva (UUID)</mat-label>
            <input matInput formControlName="reservationId" placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6" />
            @if (form.controls.reservationId.hasError('invalidGuid')) {
              <mat-error>Ingresa un UUID válido (36 caracteres con guiones).</mat-error>
            }
          </mat-form-field>

          @if (message) {
            <p [class]="messageType">{{ message }}</p>
          }

          <div class="actions">
            <button
              mat-flat-button
              color="primary"
              class="btn-gradient"
              type="button"
              [disabled]="form.invalid || loading"
              (click)="confirmPayment()"
            >
              Confirmar pago
            </button>
            <button
              mat-stroked-button
              color="warn"
              type="button"
              [disabled]="form.invalid || loading"
              (click)="cancel()"
            >
              Cancelar reserva
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .form {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .full {
      width: 100%;
    }

    .hint {
      margin: 0 0 1rem;
      color: var(--ev-text-muted);
      line-height: 1.5;
    }

    .actions {
      display: flex;
      gap: 0.75rem;
      flex-wrap: wrap;
    }
  `,
})
export class ReservationAdminComponent {
  private readonly fb = inject(FormBuilder);
  private readonly reservationsApi = inject(ReservationsApiService);

  loading = false;
  message = '';
  messageType: 'success' | 'error' = 'success';

  form = this.fb.group({
    reservationId: ['', [Validators.required, reservationIdValidator]],
  });

  confirmPayment(): void {
    this.runAction(() =>
      this.reservationsApi.confirmPayment(this.form.value.reservationId!),
      (response) => `Pago confirmado. Código: ${response.confirmationCode}`,
    );
  }

  cancel(): void {
    this.runAction(
      () => this.reservationsApi.cancel(this.form.value.reservationId!),
      (response) => `Reserva ${response.status.toLowerCase()} el ${response.cancelledAtUtc}`,
    );
  }

  private runAction<T>(action: () => import('rxjs').Observable<T>, successMessage: (value: T) => string): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.message = '';

    action().subscribe({
      next: (value) => {
        this.message = successMessage(value);
        this.messageType = 'success';
        this.loading = false;
      },
      error: (err: Error) => {
        this.message = err.message;
        this.messageType = 'error';
        this.loading = false;
      },
    });
  }
}
