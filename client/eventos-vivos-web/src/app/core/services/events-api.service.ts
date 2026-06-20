import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateEventRequest,
  CreateEventResponse,
  EventFilters,
  EventListItem,
  OccupancyReport,
} from '../models/event.models';

@Injectable({ providedIn: 'root' })
export class EventsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/events`;

  list(filters: EventFilters = {}): Observable<EventListItem[]> {
    let params = new HttpParams();

    if (filters.type != null) params = params.set('type', filters.type);
    if (filters.venueId != null) params = params.set('venueId', filters.venueId);
    if (filters.status != null) params = params.set('status', filters.status);
    if (filters.startFromUtc) params = params.set('startFromUtc', filters.startFromUtc);
    if (filters.startToUtc) params = params.set('startToUtc', filters.startToUtc);
    if (filters.search) params = params.set('search', filters.search);

    return this.http.get<EventListItem[]>(this.baseUrl, { params });
  }

  create(request: CreateEventRequest): Observable<CreateEventResponse> {
    return this.http.post<CreateEventResponse>(this.baseUrl, request);
  }

  occupancyReport(eventId: string): Observable<OccupancyReport> {
    return this.http.get<OccupancyReport>(`${this.baseUrl}/${eventId}/occupancy-report`);
  }
}
