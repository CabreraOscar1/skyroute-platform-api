# SkyRoute - Arquitectura propuesta

## Principios de diseno

La solucion deberia priorizar:

- Controllers finos y logica de negocio en servicios.
- Respuestas normalizadas antes de llegar al frontend.
- Reglas de pricing encapsuladas por proveedor.
- Ordenamiento local en frontend.
- Validacion importante duplicada conscientemente: UX en frontend, autoridad final en backend.
- Estructura simple, defendible en entrevista y facil de extender.

## Estructura sugerida

```text
skyroute-travel-platform/
  Backend/
    SkyRoute.Api/
      Controllers/
      Contracts/
      Domain/
      Providers/
      Services/
      Validation/
  Frontend/
    skyroute-client/
      src/app/
        core/
        features/flight-search/
        features/booking/
        shared/
  docs/
```

Para el challenge alcanza con un unico proyecto API bien organizado. Se puede mencionar como evolucion futura una separacion en `.Api`, `.Application`, `.Domain` e `.Infrastructure`.

## Responsabilidades por capa

| Capa | Responsabilidades |
| --- | --- |
| Frontend Angular | Formularios, estados de UI, llamada HTTP, ordenamiento local, seleccion de oferta, validacion inmediata de documento. |
| Backend .NET API | Validar criterios, consultar providers mockeados, aplicar pricing, normalizar ofertas, persistir ofertas en memoria, validar booking y generar referencia. |
| Providers | Simular `GlobalAir` y `BudgetWings` con resultados realistas para cualquier busqueda valida. |
| Pricing | Aplicar formulas por proveedor sin mezclar reglas en controllers ni UI. |

## Backend .NET

Componentes principales:

| Componente | Responsabilidad |
| --- | --- |
| `AirportsController` | Exponer `GET /api/airports`. |
| `FlightsController` | Exponer `POST /api/flights/search`. |
| `BookingsController` | Exponer `POST /api/bookings`. |
| `FlightSearchService` | Validar criteria, orquestar providers y devolver ofertas normalizadas. |
| `BookingService` | Validar offer, pasajero y documento; generar confirmacion. |
| `IFlightProvider` | Contrato comun para proveedores. |
| `GlobalAirProvider` | Mock de resultados de GlobalAir. |
| `BudgetWingsProvider` | Mock de resultados de BudgetWings. |
| `IPricingStrategy` | Contrato para calcular precio final por pasajero. |
| `DocumentValidationService` | Determinar `Passport Number` vs `National ID` y validar formato. |
| `OfferStore` | Guardar ofertas buscadas en memoria para confirmar booking. |

Modelo de dominio sugerido:

```csharp
public enum CabinClass
{
    Economy,
    Business,
    FirstClass
}

public sealed record Airport(
    string Code,
    string Name,
    string City,
    string CountryCode
);

public sealed record FlightSearchCriteria(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass
);

public sealed record FlightOffer(
    string OfferId,
    string Provider,
    string FlightNumber,
    Airport Origin,
    Airport Destination,
    DateTime DepartureAt,
    DateTime ArrivalAt,
    int DurationMinutes,
    CabinClass CabinClass,
    decimal BaseFare,
    decimal PricePerPassenger,
    int PassengerCount,
    decimal TotalPrice,
    string Currency
);
```

## Contratos API propuestos

### Listar aeropuertos

`GET /api/airports`

Response:

```json
[
  {
    "code": "EZE",
    "name": "Ministro Pistarini International Airport",
    "city": "Buenos Aires",
    "countryCode": "AR"
  }
]
```

Notas:

- El frontend puede consumir este endpoint para no duplicar el catalogo de aeropuertos.
- El catalogo sigue siendo hardcodeado, como permite el challenge.

### Buscar vuelos

`POST /api/flights/search`

Request:

```json
{
  "origin": "EZE",
  "destination": "MIA",
  "departureDate": "2026-06-10",
  "passengerCount": 2,
  "cabinClass": "Economy"
}
```

Response:

```json
{
  "searchId": "SRCH-8D4C1A2B",
  "criteria": {
    "origin": "EZE",
    "destination": "MIA",
    "departureDate": "2026-06-10",
    "passengerCount": 2,
    "cabinClass": "Economy"
  },
  "results": [
    {
      "offerId": "OFF-GA-8D4C1A2B-01",
      "provider": "GlobalAir",
      "flightNumber": "GA123",
      "origin": {
        "code": "EZE",
        "city": "Buenos Aires",
        "countryCode": "AR"
      },
      "destination": {
        "code": "MIA",
        "city": "Miami",
        "countryCode": "US"
      },
      "departureAt": "2026-06-10T09:30:00",
      "arrivalAt": "2026-06-10T17:15:00",
      "durationMinutes": 465,
      "cabinClass": "Economy",
      "currency": "USD",
      "pricePerPassenger": 184.00,
      "passengerCount": 2,
      "totalPrice": 368.00
    }
  ]
}
```

Validaciones:

- `origin` y `destination` obligatorios.
- `origin != destination`.
- Ambos codigos deben existir en el catalogo hardcodeado.
- `departureDate` obligatoria.
- `passengerCount` entre 1 y 9.
- `cabinClass` dentro de `Economy`, `Business`, `FirstClass`.

### Confirmar reserva

`POST /api/bookings`

Request:

```json
{
  "offerId": "OFF-GA-8D4C1A2B-01",
  "primaryPassenger": {
    "fullName": "Ana Perez",
    "email": "ana.perez@example.com",
    "documentNumber": "A1234567"
  }
}
```

Response:

```json
{
  "bookingReference": "SKY-20260610-8F3K2",
  "status": "Confirmed",
  "offerId": "OFF-GA-8D4C1A2B-01",
  "provider": "GlobalAir",
  "flightNumber": "GA123",
  "totalPrice": 368.00,
  "currency": "USD"
}
```

## Pricing

- `GlobalAir`: `Math.Round(baseFare * 1.15m, 2)`.
- `BudgetWings`: `Math.Max(Math.Round(baseFare * 0.90m, 2), 29.99m)`.
- `TotalPrice`: `Math.Round(pricePerPassenger * passengerCount, 2)`.

## Frontend Angular

Features sugeridas:

| Area | Responsabilidad |
| --- | --- |
| `flight-search` | Formulario, loading, error, empty state, resultados y sorting. |
| `booking` | Resumen, formulario de pasajero, validacion de documento y confirmacion. |
| `core/api` | Servicios HTTP y modelos compartidos. |
| `shared` | Pipes, helpers y componentes reutilizables. |

Estados de busqueda:

- `idle`
- `loading`
- `success`
- `empty`
- `error`

El sort debe operar sobre una copia local:

```ts
const sortedFlights = [...flights].sort(compareBySelectedSort);
```

## Aeropuertos sugeridos

| Codigo | Ciudad | Pais |
| --- | --- | --- |
| EZE | Buenos Aires | AR |
| AEP | Buenos Aires | AR |
| COR | Cordoba | AR |
| MIA | Miami | US |
| JFK | New York | US |
| LAX | Los Angeles | US |

## Trade-offs aceptables

- Persistencia en memoria para ofertas y reservas.
- Sin autenticacion.
- Sin pagos.
- Sin APIs externas de aeropuertos o aerolineas.
- Sin despliegue cloud.
- Formulario de un pasajero principal aunque la busqueda tenga varios pasajeros.
