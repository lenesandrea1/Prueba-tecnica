const ERROR_MESSAGES: Record<string, string> = {
  'Event start must be in the future.':
    'La fecha de inicio debe ser posterior a ahora.',
  'Event end must be after the start time.':
    'La fecha de fin debe ser posterior al inicio.',
  'Weekend events cannot start after 22:00 UTC.':
    'En fin de semana el evento no puede iniciar después de las 22:00 UTC.',
  'An error occurred while saving the entity changes. See the inner exception for details.':
    'No se pudo guardar en la base de datos. Verifica los datos e intenta de nuevo.',
};

const HTTP_ERROR_PATTERNS: Array<{ pattern: RegExp; message: string }> = [
  {
    pattern: /404 Not Found.*\/reservations\/[^/]+\/(confirm-payment|cancel)/i,
    message:
      'ID de reserva inválido. Debe ser el UUID completo devuelto al reservar (ej. 3fa85f64-5717-4562-b3fc-2c963f66afa6).',
  },
];

export function translateApiError(message: string): string {
  for (const { pattern, message: translated } of HTTP_ERROR_PATTERNS) {
    if (pattern.test(message)) {
      return translated;
    }
  }

  return ERROR_MESSAGES[message] ?? message;
}

export const GUID_PATTERN =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

export function isValidGuid(value: string): boolean {
  return GUID_PATTERN.test(value.trim());
}

export function toDateTimeLocalValue(date: Date): string {
  const offset = date.getTimezoneOffset();
  const local = new Date(date.getTime() - offset * 60_000);
  return local.toISOString().slice(0, 16);
}

export function minFutureDateTimeLocal(hoursFromNow = 2): string {
  return toDateTimeLocalValue(new Date(Date.now() + hoursFromNow * 60 * 60 * 1000));
}

export function futureDateTimeParts(hoursFromNow = 24): { date: Date; time: string } {
  const value = new Date(Date.now() + hoursFromNow * 60 * 60 * 1000);
  return {
    date: new Date(value.getFullYear(), value.getMonth(), value.getDate()),
    time: `${String(value.getHours()).padStart(2, '0')}:${String(value.getMinutes()).padStart(2, '0')}`,
  };
}

export function combineDateAndTime(date: Date, time: string): Date {
  const [hours, minutes] = time.split(':').map(Number);
  const combined = new Date(date);
  combined.setHours(hours, minutes, 0, 0);
  return combined;
}
