import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatCardModule],
  template: `
    <section class="hero">
      <span class="hero-badge">Plataforma de eventos en vivo</span>
      <h1 class="hero-title">
        Organiza, publica y gestiona<br />
        <span class="hero-highlight">eventos memorables</span>
      </h1>
      <p class="hero-subtitle">
        EventosVivos te permite crear conferencias, talleres y conciertos,
        administrar reservas y consultar reportes de ocupación en un solo lugar.
      </p>

      <div class="hero-actions">
        <a mat-flat-button color="primary" class="btn-gradient" routerLink="/events">
          Explorar eventos
        </a>
        <a mat-button class="btn-text" routerLink="/events/new">Crear mi evento</a>
      </div>
    </section>

    <section class="stats">
      <div class="stat-card glass-card">
        <span class="stat-value">3</span>
        <span class="stat-label">Tipos de evento</span>
      </div>
      <div class="stat-card glass-card">
        <span class="stat-value">24/7</span>
        <span class="stat-label">Reservas en línea</span>
      </div>
      <div class="stat-card glass-card">
        <span class="stat-value">100%</span>
        <span class="stat-label">Control de ocupación</span>
      </div>
    </section>

    <section class="features">
      <h2 class="section-title">Todo lo que necesitas</h2>
      <p class="section-subtitle">Herramientas pensadas para organizadores y asistentes</p>

      <div class="feature-grid">
        <mat-card class="glass-card feature-card">
          <div class="feature-inner">
            <div class="feature-icon events">📅</div>
            <mat-card-title>Catálogo de eventos</mat-card-title>
            <mat-card-content>
              <p>Explora y filtra conferencias, talleres y conciertos por venue, tipo y estado.</p>
              <a mat-button class="btn-text feature-link" routerLink="/events">Ver eventos →</a>
            </mat-card-content>
          </div>
        </mat-card>

        <mat-card class="glass-card feature-card">
          <div class="feature-inner">
            <div class="feature-icon create">✨</div>
            <mat-card-title>Crear eventos</mat-card-title>
            <mat-card-content>
              <p>Publica nuevos eventos con fecha, capacidad, precio y venue en pocos pasos.</p>
              <a mat-button class="btn-text feature-link" routerLink="/events/new">Crear evento →</a>
            </mat-card-content>
          </div>
        </mat-card>

        <mat-card class="glass-card feature-card">
          <div class="feature-inner">
            <div class="feature-icon reservations">🎟️</div>
            <mat-card-title>Reservas y reportes</mat-card-title>
            <mat-card-content>
              <p>Gestiona reservas, confirma pagos y consulta reportes de ocupación en tiempo real.</p>
              <a mat-button class="btn-text feature-link" routerLink="/reservations">Gestionar reservas →</a>
            </mat-card-content>
          </div>
        </mat-card>
      </div>
    </section>

    <section class="cta-banner glass-card">
      <div class="cta-content">
        <h2>¿Listo para tu próximo evento?</h2>
        <p>Empieza ahora y lleva el control total de tu agenda de eventos.</p>
      </div>
      <a mat-flat-button color="primary" class="btn-gradient" routerLink="/events/new">
        Comenzar ahora
      </a>
    </section>
  `,
  styles: `
    .hero {
      text-align: center;
      padding: 2rem 1rem 3rem;
      max-width: 720px;
      margin: 0 auto;
    }

    .hero-badge {
      display: inline-block;
      padding: 0.4rem 1rem;
      border-radius: var(--ev-radius-pill);
      background: rgba(108, 74, 182, 0.1);
      color: var(--ev-primary);
      font-size: 0.85rem;
      font-weight: 600;
      margin-bottom: 1.25rem;
    }

    .hero-title {
      margin: 0 0 1rem;
      font-size: clamp(2rem, 5vw, 2.75rem);
      font-weight: 800;
      line-height: 1.15;
      color: var(--ev-text);
    }

    .hero-highlight {
      background: linear-gradient(135deg, var(--ev-primary) 0%, var(--ev-accent) 50%, var(--ev-accent-blue) 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .hero-subtitle {
      margin: 0 auto 2rem;
      max-width: 560px;
      font-size: 1.05rem;
      line-height: 1.65;
      color: var(--ev-text-muted);
    }

    .hero-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }

    .stats {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 1rem;
      margin-bottom: 3rem;
      align-items: stretch;
    }

    .stat-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 120px;
      height: 100%;
      margin: 0;
      padding: 1.5rem 1rem !important;
      text-align: center;
    }

    .stat-value {
      font-size: 1.75rem;
      font-weight: 800;
      color: var(--ev-primary);
      line-height: 1;
    }

    .stat-label {
      margin-top: 0.5rem;
      min-height: 2.5em;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.85rem;
      color: var(--ev-text-muted);
      font-weight: 500;
      line-height: 1.3;
    }

    .features {
      margin-bottom: 3rem;
    }

    .section-title {
      text-align: center;
      margin: 0 0 0.5rem;
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--ev-text);
    }

    .section-subtitle {
      text-align: center;
      margin: 0 0 2rem;
      color: var(--ev-text-muted);
      font-size: 0.95rem;
    }

    .feature-grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: 1.25rem;
      align-items: stretch;
    }

    .feature-card {
      height: 100%;
      margin: 0;
      padding: 0 !important;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .feature-inner {
      display: flex;
      flex-direction: column;
      height: 100%;
      min-height: 260px;
      padding: 1.5rem;
    }

    .feature-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 40px rgba(108, 74, 182, 0.18) !important;
    }

    .feature-card .mat-mdc-card-title {
      font-size: 1.15rem !important;
      margin: 0 0 0.75rem !important;
      padding: 0 !important;
    }

    .feature-card .mat-mdc-card-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      padding: 0 !important;
      margin: 0 !important;
    }

    .feature-card p {
      flex: 1;
      min-height: 4.8em;
      color: var(--ev-text-muted);
      line-height: 1.6;
      margin: 0 0 1rem;
      font-size: 0.9rem;
    }

    .feature-link {
      margin-top: auto;
      align-self: flex-start;
      padding-left: 0 !important;
    }

    .feature-icon {
      width: 48px;
      height: 48px;
      flex-shrink: 0;
      border-radius: 14px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.4rem;
      margin-bottom: 1rem;
    }

    .feature-icon.events {
      background: linear-gradient(135deg, rgba(124, 92, 252, 0.15), rgba(91, 141, 239, 0.15));
    }

    .feature-icon.create {
      background: linear-gradient(135deg, rgba(232, 160, 191, 0.2), rgba(184, 167, 255, 0.2));
    }

    .feature-icon.reservations {
      background: linear-gradient(135deg, rgba(108, 74, 182, 0.15), rgba(209, 233, 255, 0.3));
    }

    .cta-banner {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 1.5rem;
      padding: 2rem 2.5rem !important;
      margin-bottom: 1rem;
      background: linear-gradient(
        135deg,
        rgba(237, 232, 255, 0.6) 0%,
        rgba(255, 255, 255, 0.72) 100%
      ) !important;
    }

    .cta-content h2 {
      margin: 0 0 0.35rem;
      font-size: 1.25rem;
      font-weight: 700;
      color: var(--ev-text);
    }

    .cta-content p {
      margin: 0;
      color: var(--ev-text-muted);
      font-size: 0.9rem;
    }

    @media (max-width: 768px) {
      .stats,
      .feature-grid {
        grid-template-columns: 1fr;
      }

      .feature-inner {
        min-height: auto;
      }

      .feature-card p {
        min-height: auto;
      }

      .cta-banner {
        flex-direction: column;
        text-align: center;
        padding: 1.5rem !important;
      }
    }
  `,
})
export class HomePageComponent {}
