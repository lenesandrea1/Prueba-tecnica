import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-page',
  standalone: true,
  imports: [MatProgressSpinnerModule],
  template: `
    <div class="loading-screen">
      <div class="aurora-bg" aria-hidden="true">
        <div class="aurora-blob blob-1"></div>
        <div class="aurora-blob blob-2"></div>
        <div class="aurora-blob blob-3"></div>
        <div class="dot-grid"></div>
        <span class="bg-watermark">EVENTOS</span>
      </div>

      <div class="loading-card glass-card">
        <h1 class="loading-brand">EventosVivos</h1>
        <p class="loading-tagline">Gestión de eventos en vivo</p>

        <mat-spinner class="loading-spinner" diameter="48" />

        <p class="loading-text">Cargando eventos...</p>

        <div class="skeleton-bars" aria-hidden="true">
          <span class="skeleton-bar"></span>
          <span class="skeleton-bar short"></span>
          <span class="skeleton-bar medium"></span>
        </div>
      </div>
    </div>
  `,
  styles: `
    .loading-screen {
      position: fixed;
      inset: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .loading-card {
      position: relative;
      z-index: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: 3rem 2.5rem;
      width: min(420px, 90vw);
    }

    .loading-brand {
      margin: 0;
      font-size: 2rem;
      font-weight: 800;
      background: linear-gradient(135deg, var(--ev-primary) 0%, var(--ev-accent) 100%);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .loading-tagline {
      margin: 0.5rem 0 2rem;
      color: var(--ev-text-muted);
      font-size: 0.95rem;
    }

    .loading-spinner {
      --mdc-circular-progress-active-indicator-color: var(--ev-accent);
    }

    .loading-text {
      margin: 1.25rem 0 1.75rem;
      color: var(--ev-primary);
      font-weight: 600;
      font-size: 0.95rem;
    }

    .skeleton-bars {
      display: flex;
      flex-direction: column;
      gap: 0.6rem;
      width: 100%;
    }

    .skeleton-bar {
      display: block;
      height: 10px;
      border-radius: 999px;
      background: linear-gradient(
        90deg,
        rgba(108, 74, 182, 0.08) 0%,
        rgba(108, 74, 182, 0.18) 50%,
        rgba(108, 74, 182, 0.08) 100%
      );
      background-size: 200% 100%;
      animation: shimmer 1.6s ease-in-out infinite;
    }

    .skeleton-bar.short {
      width: 60%;
      align-self: center;
    }

    .skeleton-bar.medium {
      width: 80%;
      align-self: center;
    }

    @keyframes shimmer {
      0% {
        background-position: 200% 0;
      }
      100% {
        background-position: -200% 0;
      }
    }
  `,
})
export class LoadingPageComponent implements OnInit {
  private readonly router = inject(Router);

  ngOnInit(): void {
    setTimeout(() => this.router.navigate(['/inicio']), 2200);
  }
}
