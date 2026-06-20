import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReservationsApiService } from '../../../core/services/reservations-api.service';

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
    <mat-card>
      <mat-card-header>
        <mat-card-title>Gestión de reservas</mat-card-title>
        <mat-card-subtitle>Confirmar pagos o cancelar reservas por ID</mat-card-subtitle>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="form" class="form">
          <mat-form-field class="full">
            <mat-label>ID de reserva</mat-label>
            <input matInput formControlName="reservationId" placeholder="UUID de la reserva" />
          </mat-form-field>

          @if (message) {
            <p [class]="messageType">{{ message }}</p>
          }

          <div class="actions">
            <button
              mat-flat-button
              color="primary"
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

    .actions {
      display: flex;
      gap: 0.75rem;
      flex-wrap: wrap;
    }

    .success {
      color: #1e8e3e;
    }

    .error {
      color: #b3261e;
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
    reservationId: ['', Validators.required],
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
