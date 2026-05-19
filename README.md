# SkyRoute Platform API

.NET backend for the SkyRoute Travel Platform technical challenge.

This API supports the flight search and booking flow consumed by the Angular frontend. It mocks multiple airline providers, applies provider-specific pricing rules, validates booking data, and returns normalized responses for the UI.

## Stack

- .NET 8
- ASP.NET Core Web API
- Swagger/OpenAPI
- xUnit

## Requirements

- .NET 8 SDK.
- Visual Studio 2022 or the .NET CLI.
- Optional: Angular frontend running at `http://localhost:4200`.

## Implemented Features

- `GET /api/airports`
- `POST /api/flights/search`
- `POST /api/bookings`
- Mocked providers:
  - `GlobalAir`
  - `BudgetWings`
  - `ArcticAir`
- Provider-specific pricing:
  - `GlobalAir`: base fare + 15%, rounded to 2 decimals.
  - `BudgetWings`: base fare - 10%, minimum final price of `USD 29.99`.
  - `ArcticAir`: base fare + 20%, then `USD 10.00` loyalty discount, minimum final price of `USD 49.99`.
- Hardcoded airport catalog: 6 airports across 2 countries.
- Realistic results for any valid search.
- Per-passenger price and total price for all passengers.
- Flight search validation, including future departure date validation.
- Booking validation.
- Route-based document validation:
  - `National ID` for domestic flights.
  - `Passport Number` for international flights.
- Offers stored in memory for 30 minutes so they can be booked after search.
- CORS configured for the local Angular frontend.
- Unit tests for services, pricing, and validation.

## Running From Visual Studio 2022

1. Open:

```text
SkyRoute.Api/SkyRoute.Api.sln
```

2. Select the `https` profile.
3. Run with `F5` or the green play button.
4. Open Swagger:

```text
https://localhost:7140/swagger
```

## Running From The Command Line

```powershell
dotnet run --project SkyRoute.Api\SkyRoute.Api.csproj --launch-profile https
```

## Tests

```powershell
dotnet test SkyRoute.Api.Tests\SkyRoute.Api.Tests.csproj
```

## Endpoints

### List Airports

```http
GET /api/airports
```

Returns the airport catalog consumed by the frontend. The catalog is hardcoded for the challenge and includes 6 airports across 2 countries.

### Search Flights

```http
POST /api/flights/search
```

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
  "searchId": "SRCH-20D6CBA4",
  "results": [
    {
      "offerId": "OFF-GA-20D6CBA4-01",
      "provider": "GlobalAir",
      "flightNumber": "GA543",
      "pricePerPassenger": 400.20,
      "passengerCount": 2,
      "totalPrice": 800.40,
      "currency": "USD"
    }
  ]
}
```

### Confirm Booking

```http
POST /api/bookings
```

Request:

```json
{
  "offerId": "OFF-GA-20D6CBA4-01",
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
  "bookingReference": "SKY-20260610-2E6B2",
  "status": "Confirmed",
  "offerId": "OFF-GA-20D6CBA4-01",
  "provider": "GlobalAir",
  "flightNumber": "GA543",
  "totalPrice": 800.40,
  "currency": "USD"
}
```

Important: search flights first and use a real `offerId` from the search response. Offers are stored in memory for 30 minutes and are lost when the API restarts.

## Backend Architecture

```text
SkyRoute.Api/
  Controllers/
  Contracts/
  Domain/
  Pricing/
  Providers/
  Services/
  Validation/
```

## Technical Decisions

Controllers are thin: they receive requests, call services, and return responses.

`Contracts` contains HTTP request and response DTOs. `Domain` contains internal business models.

`Services` contains use cases such as flight search and booking.

`Providers` simulates airline providers. A new provider can be added by implementing `IFlightProvider`.

`Pricing` encapsulates provider-specific pricing rules through `IPricingStrategy`.

`Validation` centralizes validation rules and consistent `ValidationProblemDetails` responses.

The solution applies Clean Architecture principles pragmatically inside a single project. For a larger system, it could evolve into separate `.Api`, `.Application`, `.Domain`, and `.Infrastructure` projects.

## Validation

The backend is the final source of truth for business validation. The API validates:

- Supported origin and destination airports.
- Different origin and destination.
- Required departure date.
- Departure date must be today or a future date.
- Passenger count between 1 and 9.
- Supported cabin class.
- Existing and non-expired offer id before booking.
- Passenger name, email, and document number.
- Route-based document format:
  - domestic flights require `National ID`;
  - international flights require `Passport Number`.

## Trade-offs

- In-memory persistence, enough for the challenge scope.
- No authentication.
- No payment flow.
- No real external provider integrations.
- No formal API versioning yet.
- No idempotency key for booking yet.
- No server-side pagination because the mocked result set is small.
