import { Component, OnInit, inject } from '@angular/core';

import { Router, RouterLink } from '@angular/router';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatSelectModule } from '@angular/material/select';

import { MatButtonModule } from '@angular/material/button';

import { MatDatepickerModule } from '@angular/material/datepicker';

import { EventsApiService } from '../../../core/services/events-api.service';

import { VenuesApiService } from '../../../core/services/venues-api.service';

import { EVENT_TYPE_LABELS, EventType, Venue } from '../../../core/models/event.models';

import { combineDateAndTime, futureDateTimeParts } from '../../../core/utils/api-error-messages';



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

    MatDatepickerModule,

  ],

  template: `

    <mat-card class="glass-card">

      <mat-card-header>

        <mat-card-title>Crear evento</mat-card-title>

        <mat-card-subtitle>Completa los datos para publicar un nuevo evento</mat-card-subtitle>

      </mat-card-header>



      <mat-card-content>

        <form [formGroup]="form" class="form" (ngSubmit)="submit()">

          <mat-form-field appearance="fill">

            <mat-label>Título</mat-label>

            <input matInput formControlName="title" />

          </mat-form-field>



          <mat-form-field appearance="fill" class="full">

            <mat-label>Descripción</mat-label>

            <textarea matInput rows="3" formControlName="description"></textarea>

          </mat-form-field>



          <mat-form-field appearance="fill">

            <mat-label>Venue</mat-label>

            <mat-select formControlName="venueId">

              @for (venue of venues; track venue.id) {

                <mat-option [value]="venue.id">

                  {{ venue.name }} (cap. {{ venue.capacity }})

                </mat-option>

              }

            </mat-select>

          </mat-form-field>



          <mat-form-field appearance="fill">

            <mat-label>Capacidad máxima</mat-label>

            <input matInput type="number" formControlName="maxCapacity" />

          </mat-form-field>



          <div class="datetime-group full">

            <mat-form-field appearance="fill" class="date-field">

              <mat-label>Inicio</mat-label>

              <input matInput [matDatepicker]="startPicker" formControlName="startDate" [min]="minDate" />

              <mat-datepicker-toggle matIconSuffix [for]="startPicker" />

              <mat-datepicker #startPicker />

              <mat-hint>Debe ser una fecha futura (dd/mm/aaaa)</mat-hint>

            </mat-form-field>

            <mat-form-field appearance="fill" class="time-field">

              <mat-label>Hora</mat-label>

              <input matInput type="time" formControlName="startTime" />

            </mat-form-field>

          </div>



          <div class="datetime-group full">

            <mat-form-field appearance="fill" class="date-field">

              <mat-label>Fin</mat-label>

              <input matInput [matDatepicker]="endPicker" formControlName="endDate" [min]="minDate" />

              <mat-datepicker-toggle matIconSuffix [for]="endPicker" />

              <mat-datepicker #endPicker />

            </mat-form-field>

            <mat-form-field appearance="fill" class="time-field">

              <mat-label>Hora</mat-label>

              <input matInput type="time" formControlName="endTime" />

            </mat-form-field>

          </div>



          <mat-form-field appearance="fill">

            <mat-label>Precio entrada</mat-label>

            <input matInput type="number" step="0.01" formControlName="ticketPrice" />

          </mat-form-field>



          <mat-form-field appearance="fill">

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

            <a mat-button class="btn-text" routerLink="/events">Cancelar</a>

            <button mat-flat-button color="primary" class="btn-gradient" type="submit" [disabled]="form.invalid || saving">

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

      gap: 0.5rem 1rem;

      margin-top: 0.5rem;

    }



    .full {

      grid-column: 1 / -1;

    }



    .datetime-group {

      display: grid;

      grid-template-columns: 1fr auto;

      gap: 0.75rem;

      align-items: start;

    }



    .date-field {

      min-width: 0;

    }



    .time-field {

      width: 140px;

    }



    .actions {

      grid-column: 1 / -1;

      display: flex;

      justify-content: flex-end;

      align-items: center;

      gap: 0.75rem;

      margin-top: 0.5rem;

      padding-top: 0.5rem;

    }



    .error,

    .success {

      grid-column: 1 / -1;

    }



    @media (max-width: 520px) {

      .datetime-group {

        grid-template-columns: 1fr;

      }



      .time-field {

        width: 100%;

      }

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

  readonly minDate = new Date();



  venues: Venue[] = [];

  error = '';

  success = '';

  saving = false;



  private readonly defaultStart = futureDateTimeParts(24);

  private readonly defaultEnd = futureDateTimeParts(28);



  form = this.fb.group({

    title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],

    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],

    venueId: [null as number | null, Validators.required],

    maxCapacity: [null as number | null, [Validators.required, Validators.min(1)]],

    startDate: [this.defaultStart.date, Validators.required],

    startTime: [this.defaultStart.time, Validators.required],

    endDate: [this.defaultEnd.date, Validators.required],

    endTime: [this.defaultEnd.time, Validators.required],

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



    const value = this.form.getRawValue();

    const startAt = combineDateAndTime(value.startDate!, value.startTime!);

    const endAt = combineDateAndTime(value.endDate!, value.endTime!);



    if (startAt <= new Date()) {

      this.error = 'La fecha de inicio debe ser posterior a ahora.';

      return;

    }



    if (endAt <= startAt) {

      this.error = 'La fecha de fin debe ser posterior al inicio.';

      return;

    }



    this.saving = true;

    this.error = '';

    this.success = '';



    this.eventsApi

      .create({

        title: value.title!,

        description: value.description!,

        venueId: value.venueId!,

        maxCapacity: value.maxCapacity!,

        startAtUtc: startAt.toISOString(),

        endAtUtc: endAt.toISOString(),

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

}


