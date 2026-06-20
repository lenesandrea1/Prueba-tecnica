# EventosVivos

Sistema de reservas para **EventosVivos**: startup que organiza eventos culturales, conferencias y talleres. Este repositorio implementa el núcleo de gestión de eventos y reservas con **.NET 8** y **PostgreSQL**.

## Descripción

La solución cubre la creación y consulta de eventos, el ciclo completo de reservas (crear, confirmar pago, cancelar) y reportes de ocupación, con reglas de negocio explícitas en el dominio.

## Por qué esta arquitectura

| Elección | Motivo |
|----------|--------|
| **PostgreSQL** (EF Core) | Modelo relacional con integridad referencial entre venues, eventos y reservas. Transacciones fuertes para evitar sobreventa de entradas. |
| **Clean Architecture** | Dominio sin dependencias de infraestructura; aplicación define puertos; infraestructura implementa adaptadores. |
| **CQRS ligero** | Separación entre comando (`CreateEvent`) y consulta (`ListEvents`) según responsabilidades distintas. |
| **ProblemDetails** | Errores HTTP predecibles (400/404/409) alineados con validaciones de negocio. |

## Estructura del código

| Proyecto | Rol |
|----------|-----|
| `EventosVivos.Domain` | Entidades, reglas RN-01 a RN-07, excepciones de dominio |
| `EventosVivos.Application` | Handlers, puertos y criterios de búsqueda |
| `EventosVivos.Infrastructure` | EF Core, PostgreSQL, seed y migraciones |
| `EventosVivos.Api` | Controllers HTTP, Swagger, validación FluentValidation |
| `EventosVivos.Tests.Unit` | Dominio, guards y handlers con dobles |

## Datos de seed

Tras migrar con base vacía se cargan los **venues del enunciado** y tres eventos demo:

| Id | Nombre | Capacidad | Ciudad |
|----|--------|-----------|--------|
| 1 | Auditorio Central | 200 | Bogotá |
| 2 | Sala Norte | 50 | Bogotá |
| 3 | Arena Sur | 500 | Medellín |

Eventos demo con GUIDs estables en `KnownIds` para pruebas manuales vía Swagger.

## Reglas implementadas

| Regla | Descripción |
|-------|-------------|
| **RN-01** | Capacidad del evento ≤ capacidad del venue |
| **RN-02** | Dos eventos activos no pueden solaparse en el mismo venue |
| **RN-03** | En fin de semana el inicio no puede ser después de las 22:00 UTC |
| **RN-04** | No se permiten reservas si el evento inicia en menos de 1 hora |
| **RN-05** | Eventos con precio > $100 limitan a 10 entradas por transacción |
| **RN-06** | Eventos activos pasados se marcan `completado` automáticamente (job cada minuto) |
| **RN-07** | Cancelar confirmada con < 48 h antes del evento → estado `perdida` (no libera cupo) |

### Requerimientos funcionales

| RF | Endpoint |
|----|----------|
| RF-01 Crear evento | `POST /api/events` |
| RF-02 Listar eventos | `GET /api/events` |
| RF-03 Reservar entrada | `POST /api/events/{eventId}/reservations` |
| RF-04 Confirmar pago | `POST /api/reservations/{id}/confirm-payment` |
| RF-05 Cancelar reserva | `POST /api/reservations/{id}/cancel` |
| RF-06 Reporte ocupación | `GET /api/events/{eventId}/occupancy-report` |
| Venues de referencia | `GET /api/venues` |

## Puesta en marcha

```bash
docker compose up -d
dotnet run --project src/EventosVivos.Api
```

Abre Swagger en la URL HTTPS/HTTP de `launchSettings.json` (`/swagger`).

Requisitos: **.NET 8 SDK**, **Docker** para PostgreSQL local.

Cadena por defecto en `appsettings.json`: `eventos` / `local`, base `eventosvivos`.

## Ejemplos HTTP

**Listar eventos en Bogotá (venue 1):**

```
GET /api/events?venueId=1
```

**Crear evento:**

```
POST /api/events
Content-Type: application/json

{
  "title": "Foro de Innovación",
  "description": "Jornada de conferencias sobre tecnología aplicada a eventos culturales.",
  "venueId": 1,
  "maxCapacity": 150,
  "startAtUtc": "2027-03-10T15:00:00Z",
  "endAtUtc": "2027-03-10T19:00:00Z",
  "ticketPrice": 85.50,
  "type": 1
}
```

Tipos: `1` conferencia, `2` taller, `3` concierto.

**Reservar entradas:**

```
POST /api/events/{eventId}/reservations
Content-Type: application/json

{
  "quantity": 2,
  "buyerName": "Ana López",
  "buyerEmail": "ana@example.com"
}
```

**Confirmar pago (admin):**

```
POST /api/reservations/{reservationId}/confirm-payment
```

**Reporte de ocupación:**

```
GET /api/events/{eventId}/occupancy-report
```

## Compilar y probar

```bash
dotnet build
dotnet test
```

## Migraciones EF

```bash
dotnet ef migrations add Nombre --project src/EventosVivos.Infrastructure --startup-project src/EventosVivos.Api --output-dir Persistence/Migrations
```

## Licencia

Uso educativo / prueba técnica.
