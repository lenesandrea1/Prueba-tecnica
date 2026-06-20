export enum EventType {
  Conferencia = 1,
  Taller = 2,
  Concierto = 3,
}

export enum EventStatus {
  Activo = 1,
  Cancelado = 2,
  Completado = 3,
}

export interface EventListItem {
  id: string;
  title: string;
  description: string;
  venueId: number;
  venueName: string;
  venueCity: string;
  maxCapacity: number;
  startAtUtc: string;
  endAtUtc: string;
  ticketPrice: number;
  type: EventType;
  status: EventStatus;
}

export interface CreateEventRequest {
  title: string;
  description: string;
  venueId: number;
  maxCapacity: number;
  startAtUtc: string;
  endAtUtc: string;
  ticketPrice: number;
  type: EventType;
}

export interface CreateEventResponse {
  eventId: string;
  title: string;
  status: EventStatus;
}

export interface Venue {
  id: number;
  name: string;
  capacity: number;
  city: string;
}

export interface CreateReservationRequest {
  quantity: number;
  buyerName: string;
  buyerEmail: string;
}

export interface CreateReservationResponse {
  reservationId: string;
  eventId: string;
  quantity: number;
  status: string;
}

export interface ConfirmPaymentResponse {
  reservationId: string;
  confirmationCode: string;
  status: string;
}

export interface CancelReservationResponse {
  reservationId: string;
  status: string;
  cancelledAtUtc: string;
}

export interface OccupancyReport {
  eventId: string;
  eventTitle: string;
  maxCapacity: number;
  soldTickets: number;
  availableTickets: number;
  occupancyPercentage: number;
  totalRevenue: number;
  eventStatus: EventStatus;
}

export interface EventFilters {
  type?: EventType;
  venueId?: number;
  status?: EventStatus;
  startFromUtc?: string;
  startToUtc?: string;
  search?: string;
}

export const EVENT_TYPE_LABELS: Record<EventType, string> = {
  [EventType.Conferencia]: 'Conferencia',
  [EventType.Taller]: 'Taller',
  [EventType.Concierto]: 'Concierto',
};

export const EVENT_STATUS_LABELS: Record<EventStatus, string> = {
  [EventStatus.Activo]: 'Activo',
  [EventStatus.Cancelado]: 'Cancelado',
  [EventStatus.Completado]: 'Completado',
};
