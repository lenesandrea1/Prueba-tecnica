import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EventsApiService } from '../../../core/services/events-api.service';
import { ReservationsApiService } from '../../../core/services/reservations-api.service';
import {
  EVENT_STATUS_LABELS,
  EVENT_TYPE_LABELS,
  EventListItem,
  EventStatus,
} from '../../../core/models/event.models';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
  template: `
    @if (loading) {
      <div class="center"><mat-spinner diameter="40" /></div>
    } @else if (!event) {
      <p class="error">{{ error || 'Evento no encontrado.' }}</p>
    } @else {
      <mat-card class="info">
        <mat-card-header>
          <mat-card-title>{{ event.title }}</mat-card-title>
          <mat-card-subtitle>{{ typeLabels[event.type] }} · {{ statusLabels[event.status] }}</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <p>{{ event.description }}</p>
          <ul>
            <li><strong>Venue:</strong> {{ event.venueName }} ({{ event.venueCity }})</li>
            <li><strong>Capacidad:</strong> {{ event.maxCapacity }}</li>
            <li><strong>Inicio:</strong> {{ event.startAtUtc | date: 'medium' }}</li>
            <li><strong>Fin:</strong> {{ event.endAtUtc | date: 'medium' }}</li>
            <li><strong>Precio:</strong> {{ event.ticketPrice | currency: 'USD' }}</li>
          </ul>
          <a mat-button [routerLink]="['/events', event.id, 'report']">Ver reporte de ocupación</a>
        </mat-card-content>
      </mat-card>

      @if (event.status === activeStatus) {
        <mat-card>
          <mat-card-header>
            <mat-card-title>Reservar entradas</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <form [formGroup]="reservationForm" class="form" (ngSubmit)="reserve()">
              <mat-form-field>
                <mat-label>Cantidad</mat-label>
                <input matInput type="number" formControlName="quantity" min="1" />
              </mat-form-field>
              <mat-form-field>
                <mat-label>Nombre comprador</mat-label>
                <input matInput formControlName="buyerName" />
              </mat-form-field>
              <mat-form-field>
                <mat-label>Email comprador</mat-label>
                <input matInput type="email" formControlName="buyerEmail" />
              </mat-form-field>

              @if (reservationError) {
                <p class="error">{{ reservationError }}</p>
              }
              @if (reservationSuccess) {
                <p class="success">{{ reservationSuccess }}</p>
              }

              <button mat-flat-button color="primary" type="submit" [disabled]="reservationForm.invalid || reserving">
                Reservar
              </button>
            </form>
          </mat-card-content>
        </mat-card>
      }
    }
  `,
  styles: `
    .info {
      margin-bottom: 1rem;
    }

    .form {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 0.75rem;
    }

    .center {
      display: flex;
      justify-content: center;
      padding: 2rem;
    }

    .error {
      color: #b3261e;
    }

    .success {
      color: #1e8e3e;
      grid-column: 1 / -1;
    }
  `,
})
export class EventDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly eventsApi = inject(EventsApiService);
  private readonly reservationsApi = inject(ReservationsApiService);
  private readonly fb = inject(FormBuilder);

  readonly typeLabels = EVENT_TYPE_LABELS;
  readonly statusLabels = EVENT_STATUS_LABELS;
  readonly activeStatus = EventStatus.Activo;

  event: EventListItem | null = null;
  loading = true;
  error = '';
  reservationError = '';
  reservationSuccess = '';
  reserving = false;

  reservationForm = this.fb.group({
    quantity: [1, [Validators.required, Validators.min(1)]],
    buyerName: ['', [Validators.required, Validators.minLength(2)]],
    buyerEmail: ['', [Validators.required, Validators.email]],
  });

  ngOnInit(): void {
    const eventId = this.route.snapshot.paramMap.get('id')!;
    this.eventsApi.list().subscribe({
      next: (events) => {
        this.event = events.find((item) => item.id === eventId) ?? null;
        this.loading = false;
        if (!this.event) this.error = 'No se encontró el evento solicitado.';
      },
      error: (err: Error) => {
        this.error = err.message;
        this.loading = false;
      },
    });
  }

  reserve(): void {
    if (!this.event || this.reservationForm.invalid) return;

    this.reserving = true;
    this.reservationError = '';
    this.reservationSuccess = '';

    const value = this.reservationForm.getRawValue();
    this.reservationsApi
      .create(this.event.id, {
        quantity: value.quantity!,
        buyerName: value.buyerName!,
        buyerEmail: value.buyerEmail!,
      })
      .subscribe({
        next: (response) => {
          this.reservationSuccess = `Reserva creada (${response.status}). ID: ${response.reservationId}`;
          this.reserving = false;
        },
        error: (err: Error) => {
          this.reservationError = err.message;
          this.reserving = false;
        },
      });
  }
}
