import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/loading/loading-page.component').then((m) => m.LoadingPageComponent),
  },
  {
    path: '',
    component: ShellComponent,
    children: [
      {
        path: 'inicio',
        loadComponent: () =>
          import('./features/home/home-page.component').then((m) => m.HomePageComponent),
      },
      { path: '', redirectTo: 'inicio', pathMatch: 'full' },
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
