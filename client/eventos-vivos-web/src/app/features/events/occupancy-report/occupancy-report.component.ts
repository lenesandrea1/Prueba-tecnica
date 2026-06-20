import { CurrencyPipe, PercentPipe } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EventsApiService } from '../../../core/services/events-api.service';
import { EVENT_STATUS_LABELS, OccupancyReport } from '../../../core/models/event.models';

@Component({
  selector: 'app-occupancy-report',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, PercentPipe, MatCardModule, MatButtonModule, MatProgressSpinnerModule],
  template: `
    @if (loading) {
      <div class="center"><mat-spinner diameter="40" /></div>
    } @else if (!report) {
      <p class="error">{{ error || 'Reporte no disponible.' }}</p>
    } @else {
      <mat-card>
        <mat-card-header>
          <mat-card-title>Reporte de ocupación</mat-card-title>
          <mat-card-subtitle>{{ report.eventTitle }}</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <dl class="metrics">
            <div><dt>Capacidad máxima</dt><dd>{{ report.maxCapacity }}</dd></div>
            <div><dt>Entradas vendidas</dt><dd>{{ report.soldTickets }}</dd></div>
            <div><dt>Disponibles</dt><dd>{{ report.availableTickets }}</dd></div>
            <div><dt>Ocupación</dt><dd>{{ report.occupancyPercentage / 100 | percent: '1.0-2' }}</dd></div>
            <div><dt>Ingresos</dt><dd>{{ report.totalRevenue | currency: 'USD' }}</dd></div>
            <div><dt>Estado evento</dt><dd>{{ statusLabels[report.eventStatus] }}</dd></div>
          </dl>
          <a mat-button [routerLink]="['/events', report.eventId]">Volver al evento</a>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: `
    .metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 1rem;
      margin-bottom: 1rem;
    }

    dt {
      font-size: 0.85rem;
      color: #5f6368;
    }

    dd {
      margin: 0.25rem 0 0;
      font-size: 1.25rem;
      font-weight: 600;
    }

    .center {
      display: flex;
      justify-content: center;
      padding: 2rem;
    }

    .error {
      color: #b3261e;
    }
  `,
})
export class OccupancyReportComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly eventsApi = inject(EventsApiService);

  readonly statusLabels = EVENT_STATUS_LABELS;

  report: OccupancyReport | null = null;
  loading = true;
  error = '';

  ngOnInit(): void {
    const eventId = this.route.snapshot.paramMap.get('id')!;
    this.eventsApi.occupancyReport(eventId).subscribe({
      next: (report) => {
        this.report = report;
        this.loading = false;
      },
      error: (err: Error) => {
        this.error = err.message;
        this.loading = false;
      },
    });
  }
}
