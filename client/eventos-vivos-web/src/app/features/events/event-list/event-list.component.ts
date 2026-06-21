import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EventsApiService } from '../../../core/services/events-api.service';
import { VenuesApiService } from '../../../core/services/venues-api.service';
import {
  EVENT_STATUS_LABELS,
  EVENT_TYPE_LABELS,
  EventListItem,
  EventStatus,
  EventType,
  Venue,
} from '../../../core/models/event.models';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <mat-card class="glass-card">
      <mat-card-header>
        <mat-card-title>Eventos</mat-card-title>
        <mat-card-subtitle>Consulta y filtra eventos disponibles</mat-card-subtitle>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="filters" class="filters" (ngSubmit)="search()">
          <mat-form-field appearance="fill">
            <mat-label>Búsqueda</mat-label>
            <input matInput formControlName="search" placeholder="Título" />
          </mat-form-field>

          <mat-form-field appearance="fill">
            <mat-label>Tipo</mat-label>
            <mat-select formControlName="type">
              <mat-option [value]="null">Todos</mat-option>
              @for (type of eventTypes; track type) {
                <mat-option [value]="type">{{ typeLabels[type] }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="fill">
            <mat-label>Venue</mat-label>
            <mat-select formControlName="venueId">
              <mat-option [value]="null">Todos</mat-option>
              @for (venue of venues; track venue.id) {
                <mat-option [value]="venue.id">{{ venue.name }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="fill">
            <mat-label>Estado</mat-label>
            <mat-select formControlName="status">
              <mat-option [value]="null">Todos</mat-option>
              @for (status of eventStatuses; track status) {
                <mat-option [value]="status">{{ statusLabels[status] }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          <button mat-flat-button color="primary" class="btn-gradient filter-btn" type="submit">Filtrar</button>
        </form>

        @if (loading) {
          <div class="center"><mat-spinner diameter="40" /></div>
        } @else if (error) {
          <p class="error">{{ error }}</p>
        } @else {
          <table mat-table [dataSource]="events" class="table glass-table">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Título</th>
              <td mat-cell *matCellDef="let row">{{ row.title }}</td>
            </ng-container>

            <ng-container matColumnDef="venue">
              <th mat-header-cell *matHeaderCellDef>Venue</th>
              <td mat-cell *matCellDef="let row">{{ row.venueName }} ({{ row.venueCity }})</td>
            </ng-container>

            <ng-container matColumnDef="schedule">
              <th mat-header-cell *matHeaderCellDef>Fecha</th>
              <td mat-cell *matCellDef="let row">{{ row.startAtUtc | date: 'medium' }}</td>
            </ng-container>

            <ng-container matColumnDef="price">
              <th mat-header-cell *matHeaderCellDef>Precio</th>
              <td mat-cell *matCellDef="let row">{{ row.ticketPrice | currency: 'USD' }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Estado</th>
              <td mat-cell *matCellDef="let row">{{ statusLabel(row.status) }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let row">
                <a mat-button class="btn-text" [routerLink]="['/events', row.id]">Ver</a>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns"></tr>
          </table>

          @if (events.length === 0) {
            <p class="empty">No hay eventos con los filtros seleccionados.</p>
          }
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .filters {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 0.5rem 1rem;
      align-items: start;
      margin-bottom: 1.5rem;
    }

    .filter-btn {
      align-self: end;
      margin-bottom: 1.25rem;
    }

    .table {
      width: 100%;
      border-radius: 12px;
      overflow: hidden;
    }
  `,
})
export class EventListComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly eventsApi = inject(EventsApiService);
  private readonly venuesApi = inject(VenuesApiService);

  readonly typeLabels = EVENT_TYPE_LABELS;
  readonly statusLabels = EVENT_STATUS_LABELS;
  readonly eventTypes = [EventType.Conferencia, EventType.Taller, EventType.Concierto];
  readonly eventStatuses = [EventStatus.Activo, EventStatus.Cancelado, EventStatus.Completado];
  readonly columns = ['title', 'venue', 'schedule', 'price', 'status', 'actions'];

  venues: Venue[] = [];
  events: EventListItem[] = [];
  loading = false;
  error = '';

  filters = this.fb.group({
    search: [''],
    type: [null as EventType | null],
    venueId: [null as number | null],
    status: [null as EventStatus | null],
  });

  search(): void {
    this.loading = true;
    this.error = '';

    const value = this.filters.getRawValue();
    this.eventsApi
      .list({
        search: value.search || undefined,
        type: value.type ?? undefined,
        venueId: value.venueId ?? undefined,
        status: value.status ?? undefined,
      })
      .subscribe({
        next: (events) => {
          this.events = events;
          this.loading = false;
        },
        error: (err: Error) => {
          this.error = err.message;
          this.loading = false;
        },
      });
  }

  statusLabel(status: EventStatus): string {
    return this.statusLabels[status];
  }

  ngOnInit(): void {
    this.venuesApi.list().subscribe({
      next: (venues) => (this.venues = venues),
      error: () => (this.venues = []),
    });
    this.search();
  }
}
