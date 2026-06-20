import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { EventsApiService } from '../../../core/services/events-api.service';
import { VenuesApiService } from '../../../core/services/venues-api.service';
import { EVENT_TYPE_LABELS, EventType, Venue } from '../../../core/models/event.models';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Crear evento</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <form [formGroup]="form" class="form" (ngSubmit)="submit()">
          <mat-form-field>
            <mat-label>Título</mat-label>
            <input matInput formControlName="title" />
          </mat-form-field>

          <mat-form-field class="full">
            <mat-label>Descripción</mat-label>
            <textarea matInput rows="3" formControlName="description"></textarea>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Venue</mat-label>
            <mat-select formControlName="venueId">
              @for (venue of venues; track venue.id) {
                <mat-option [value]="venue.id">
                  {{ venue.name }} (cap. {{ venue.capacity }})
                </mat-option>
              }
            </mat-select>
          </mat-form-field>

          <mat-form-field>
            <mat-label>Capacidad máxima</mat-label>
            <input matInput type="number" formControlName="maxCapacity" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Inicio (UTC)</mat-label>
            <input matInput type="datetime-local" formControlName="startAtLocal" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Fin (UTC)</mat-label>
            <input matInput type="datetime-local" formControlName="endAtLocal" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Precio entrada</mat-label>
            <input matInput type="number" step="0.01" formControlName="ticketPrice" />
          </mat-form-field>

          <mat-form-field>
            <mat-label>Tipo</mat-label>
            <mat-select formControlName="type">
              @for (type of eventTypes; track type) {
                <mat-option [value]="type">{{ typeLabels[type] }}</mat-option>
              }
            </mat-select>
          </mat-form-field>

          @if (error) {
            <p class="error">{{ error }}</p>
          }

          @if (success) {
            <p class="success">{{ success }}</p>
          }

          <div class="actions">
            <a mat-button routerLink="/events">Cancelar</a>
            <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || saving">
              Guardar
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: `
    .form {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 0.75rem;
    }

    .full {
      grid-column: 1 / -1;
    }

    .actions {
      grid-column: 1 / -1;
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
    }

    .error {
      grid-column: 1 / -1;
      color: #b3261e;
    }

    .success {
      grid-column: 1 / -1;
      color: #1e8e3e;
    }
  `,
})
export class EventFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly eventsApi = inject(EventsApiService);
  private readonly venuesApi = inject(VenuesApiService);
  private readonly router = inject(Router);

  readonly typeLabels = EVENT_TYPE_LABELS;
  readonly eventTypes = [EventType.Conferencia, EventType.Taller, EventType.Concierto];

  venues: Venue[] = [];
  error = '';
  success = '';
  saving = false;

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    venueId: [null as number | null, Validators.required],
    maxCapacity: [null as number | null, [Validators.required, Validators.min(1)]],
    startAtLocal: ['', Validators.required],
    endAtLocal: ['', Validators.required],
    ticketPrice: [null as number | null, [Validators.required, Validators.min(0.01)]],
    type: [EventType.Conferencia, Validators.required],
  });

  ngOnInit(): void {
    this.venuesApi.list().subscribe({
      next: (venues) => (this.venues = venues),
      error: (err: Error) => (this.error = err.message),
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.saving = true;
    this.error = '';
    this.success = '';

    const value = this.form.getRawValue();
    this.eventsApi
      .create({
        title: value.title!,
        description: value.description!,
        venueId: value.venueId!,
        maxCapacity: value.maxCapacity!,
        startAtUtc: this.toUtcIso(value.startAtLocal!),
        endAtUtc: this.toUtcIso(value.endAtLocal!),
        ticketPrice: value.ticketPrice!,
        type: value.type!,
      })
      .subscribe({
        next: (response) => {
          this.success = `Evento "${response.title}" creado correctamente.`;
          this.saving = false;
          setTimeout(() => this.router.navigate(['/events', response.eventId]), 800);
        },
        error: (err: Error) => {
          this.error = err.message;
          this.saving = false;
        },
      });
  }

  private toUtcIso(localValue: string): string {
    return new Date(localValue).toISOString();
  }
}
