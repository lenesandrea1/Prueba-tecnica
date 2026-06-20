import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Venue } from '../models/event.models';

@Injectable({ providedIn: 'root' })
export class VenuesApiService {
  private readonly http = inject(HttpClient);

  list(): Observable<Venue[]> {
    return this.http.get<Venue[]>(`${environment.apiUrl}/venues`);
  }
}
