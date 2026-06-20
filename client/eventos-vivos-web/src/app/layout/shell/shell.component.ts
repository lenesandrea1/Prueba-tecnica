import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule],
  template: `
    <mat-toolbar color="primary">
      <span class="brand">EventosVivos</span>
      <nav>
        <a mat-button routerLink="/events" routerLinkActive="active">Eventos</a>
        <a mat-button routerLink="/events/new" routerLinkActive="active">Crear evento</a>
        <a mat-button routerLink="/reservations" routerLinkActive="active">Reservas</a>
      </nav>
    </mat-toolbar>

    <main class="page">
      <router-outlet />
    </main>
  `,
  styles: `
    .brand {
      font-weight: 600;
      margin-right: 1.5rem;
    }

    nav {
      display: flex;
      gap: 0.25rem;
    }

    .active {
      background: rgba(255, 255, 255, 0.12);
    }

    .page {
      max-width: 1100px;
      margin: 0 auto;
      padding: 1.5rem;
    }
  `,
})
export class ShellComponent {}
