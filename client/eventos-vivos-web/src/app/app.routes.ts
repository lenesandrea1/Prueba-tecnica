import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', redirectTo: 'events', pathMatch: 'full' },
      {
        path: 'events',
        loadComponent: () =>
          import('./features/events/event-list/event-list.component').then((m) => m.EventListComponent),
      },
      {
        path: 'events/new',
        loadComponent: () =>
          import('./features/events/event-form/event-form.component').then((m) => m.EventFormComponent),
      },
      {
        path: 'events/:id/report',
        loadComponent: () =>
          import('./features/events/occupancy-report/occupancy-report.component').then(
            (m) => m.OccupancyReportComponent,
          ),
      },
      {
        path: 'events/:id',
        loadComponent: () =>
          import('./features/events/event-detail/event-detail.component').then((m) => m.EventDetailComponent),
      },
      {
        path: 'reservations',
        loadComponent: () =>
          import('./features/reservations/reservation-admin/reservation-admin.component').then(
            (m) => m.ReservationAdminComponent,
          ),
      },
    ],
  },
];
