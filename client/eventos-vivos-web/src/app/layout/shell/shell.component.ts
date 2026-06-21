import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatButtonModule],
  template: `
    <div class="app-shell">
      <div class="aurora-bg" aria-hidden="true">
        <div class="aurora-blob blob-1"></div>
        <div class="aurora-blob blob-2"></div>
        <div class="aurora-blob blob-3"></div>
        <div class="dot-grid"></div>
        <span class="bg-watermark">EVENTOS</span>
      </div>

      <header class="glass-nav">
        <a class="brand" routerLink="/inicio">EventosVivos</a>
        <nav>
          <a routerLink="/inicio" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Inicio</a>
          <a routerLink="/events" routerLinkActive="active">Eventos</a>
          <a routerLink="/events/new" routerLinkActive="active">Crear evento</a>
          <a routerLink="/reservations" routerLinkActive="active">Reservas</a>
        </nav>
        <a mat-flat-button color="primary" class="btn-gradient nav-cta" routerLink="/events/new">
          Nuevo evento
        </a>
      </header>

      <main class="page">
        <router-outlet />
      </main>
    </div>
  `,
})
export class ShellComponent {}
