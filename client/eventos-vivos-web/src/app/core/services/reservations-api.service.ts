import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CancelReservationResponse,
  ConfirmPaymentResponse,
  CreateReservationRequest,
  CreateReservationResponse,
} from '../models/event.models';

@Injectable({ providedIn: 'root' })
export class ReservationsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/reservations`;

  create(eventId: string, request: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>(
      `${environment.apiUrl}/events/${eventId}/reservations`,
      request,
    );
  }

  confirmPayment(reservationId: string): Observable<ConfirmPaymentResponse> {
    return this.http.post<ConfirmPaymentResponse>(
      `${this.baseUrl}/${reservationId}/confirm-payment`,
      {},
    );
  }

  cancel(reservationId: string): Observable<CancelReservationResponse> {
    return this.http.post<CancelReservationResponse>(
      `${this.baseUrl}/${reservationId}/cancel`,
      {},
    );
  }
}
