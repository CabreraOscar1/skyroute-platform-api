# SkyRoute - Handoff para frontend Angular

Este documento esta pensado para que una nueva ventana de contexto pueda arrancar el frontend sin releer toda la conversacion.

## Versiones recomendadas

Instalar manualmente antes de crear el proyecto:

- Node.js: `24.x LTS` recomendado.
- npm: `11.x` o la version que venga con Node 24 LTS.
- Angular CLI: `21.x`.

Comandos de verificacion:

```powershell
node --version
npm --version
ng version
```

Instalar Angular CLI:

```powershell
npm install -g @angular/cli@21
```

Notas de compatibilidad:

- Angular 21 soporta Node `^20.19.0 || ^22.12.0 || ^24.0.0`.
- Angular 21 usa TypeScript `>=5.9.0 <6.0.0`.
- Angular 21 soporta RxJS `^6.5.3 || ^7.4.0`.
- La guia oficial de Angular pide Node `v20.19.0 or newer`; Node 24 LTS cae dentro del rango soportado.
- Referencias oficiales:
  - Angular installation: `https://angular.dev/installation`
  - Angular version compatibility: `https://angular.dev/reference/versions`
  - Node.js release schedule: `https://github.com/nodejs/release`

## Estado del backend

El backend ya esta implementado en:

```text
Backend/
  SkyRoute.Api/
  SkyRoute.Api.Tests/
```

Solucion:

```text
Backend/SkyRoute.Api/SkyRoute.Api.sln
```

Para correr backend desde Visual Studio 2022:

1. Abrir `Backend/SkyRoute.Api/SkyRoute.Api.sln`.
2. Seleccionar perfil `https`.
3. Ejecutar con `F5`.
4. Swagger deberia estar en `https://localhost:7140/swagger`.

Para correr backend por consola:

```powershell
dotnet run --project Backend\SkyRoute.Api\SkyRoute.Api.csproj --launch-profile https
```

El frontend debe usar como base URL:

```text
https://localhost:7140
```

Si el puerto cambia en Visual Studio, actualizar el environment de Angular.

## Scaffold recomendado

Crear el proyecto dentro de `Frontend/`.

```powershell
cd "C:\Users\cabre\Desktop\SkyRoute Project\skyroute-travel-platform"
ng new skyroute-client --directory Frontend/skyroute-client --routing --style=scss
```

Opciones recomendadas:

- Routing: `Yes`.
- Styles: `SCSS`.
- SSR / SSG: `No`, si Angular CLI pregunta. Esta app es una SPA local para challenge.
- Tests: dejar el default si el CLI pregunta; luego se agregan specs puntuales.

Para correr:

```powershell
cd Frontend/skyroute-client
npm start
```

URL esperada:

```text
http://localhost:4200
```

El backend ya tiene CORS para:

```text
http://localhost:4200
https://localhost:4200
```

## Estructura frontend sugerida

Mantenerla simple y explicable:

```text
Frontend/skyroute-client/src/app/
  core/
    api/
      skyroute-api.service.ts
      skyroute-api.models.ts
    config/
      api.config.ts
  features/
    flight-search/
      flight-search-page.component.*
      flight-search-form.component.*
      flight-results.component.*
      flight-sort.ts
    booking/
      booking-page.component.*
      passenger-form.component.*
      booking-confirmation.component.*
  shared/
    format-duration.pipe.ts
```

No hace falta NgRx ni estado global complejo. Para el challenge alcanza con estado local en componentes o servicios simples.

## Endpoints disponibles

### Listar aeropuertos

```http
GET /api/airports
```

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

Uso frontend:

- Cargar al iniciar la pantalla de busqueda.
- Poblar dropdowns de origen y destino.
- Usar `countryCode` para determinar label de documento antes de confirmar booking.

### Buscar vuelos

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
  "criteria": {
    "origin": "EZE",
    "destination": "MIA",
    "departureDate": "2026-06-10",
    "passengerCount": 2,
    "cabinClass": "Economy"
  },
  "results": [
    {
      "offerId": "OFF-GA-20D6CBA4-01",
      "provider": "GlobalAir",
      "flightNumber": "GA543",
      "origin": {
        "code": "EZE",
        "name": "Ministro Pistarini International Airport",
        "city": "Buenos Aires",
        "countryCode": "AR"
      },
      "destination": {
        "code": "MIA",
        "name": "Miami International Airport",
        "city": "Miami",
        "countryCode": "US"
      },
      "departureAt": "2026-06-10T08:15:00",
      "arrivalAt": "2026-06-10T16:43:00",
      "durationMinutes": 508,
      "cabinClass": "Economy",
      "currency": "USD",
      "pricePerPassenger": 400.20,
      "passengerCount": 2,
      "totalPrice": 800.40
    }
  ]
}
```

Reglas frontend:

- Mostrar `totalPrice` como dato principal.
- Mostrar `pricePerPassenger` como dato secundario.
- Ordenar localmente, sin volver a llamar al backend.

### Confirmar booking

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

Importante:

- Hay que hacer search antes de booking.
- Las ofertas viven en memoria por 30 minutos.
- Si se reinicia backend, hay que buscar de nuevo.

## Modelos TypeScript sugeridos

```ts
export type CabinClass = 'Economy' | 'Business' | 'FirstClass';

export interface Airport {
  code: string;
  name: string;
  city: string;
  countryCode: string;
}

export interface FlightSearchRequest {
  origin: string;
  destination: string;
  departureDate: string;
  passengerCount: number;
  cabinClass: CabinClass;
}

export interface FlightSearchResponse {
  searchId: string;
  criteria: FlightSearchCriteria;
  results: FlightOffer[];
}

export interface FlightSearchCriteria {
  origin: string;
  destination: string;
  departureDate: string;
  passengerCount: number;
  cabinClass: CabinClass;
}

export interface FlightOffer {
  offerId: string;
  provider: string;
  flightNumber: string;
  origin: Airport;
  destination: Airport;
  departureAt: string;
  arrivalAt: string;
  durationMinutes: number;
  cabinClass: CabinClass;
  currency: string;
  pricePerPassenger: number;
  passengerCount: number;
  totalPrice: number;
}

export interface BookingRequest {
  offerId: string;
  primaryPassenger: PassengerRequest;
}

export interface PassengerRequest {
  fullName: string;
  email: string;
  documentNumber: string;
}

export interface BookingResponse {
  bookingReference: string;
  status: 'Confirmed';
  offerId: string;
  provider: string;
  flightNumber: string;
  totalPrice: number;
  currency: string;
}
```

## Servicio HTTP sugerido

Crear un servicio `SkyRouteApiService` con metodos:

```ts
getAirports(): Observable<Airport[]>
searchFlights(request: FlightSearchRequest): Observable<FlightSearchResponse>
confirmBooking(request: BookingRequest): Observable<BookingResponse>
```

Usar `HttpClient`.

## Flujo de UI requerido

### Search page

- Formulario con:
  - Origen.
  - Destino.
  - Fecha de salida.
  - Pasajeros: 1 a 9.
  - Cabina: `Economy`, `Business`, `FirstClass`.
- Estados:
  - `idle`
  - `loading`
  - `success`
  - `empty`
  - `error`
- Al buscar:
  - Llamar `POST /api/flights/search`.
  - Guardar resultados localmente.
  - Mostrar loading mientras espera.
  - Mostrar empty state si `results.length === 0`.

### Sorting local

Opciones requeridas:

- Precio menor a mayor.
- Precio mayor a menor.
- Duracion menor a mayor.
- Hora de salida.

Regla clave:

- Cambiar sort no debe llamar al backend.
- Ordenar sobre copia:

```ts
const sorted = [...flights].sort(compareFn);
```

### Booking page

Al seleccionar una oferta:

- Mostrar resumen:
  - Ruta.
  - Provider.
  - Flight number.
  - Horarios.
  - Cabina.
  - Precio por pasajero.
  - Cantidad de pasajeros.
  - Total.
- Formulario pasajero:
  - Full name.
  - Email.
  - Document number.
- Confirmar con `POST /api/bookings`.
- Mostrar `bookingReference`.

## Validacion de documento en frontend

Determinar tipo de documento comparando:

```ts
offer.origin.countryCode === offer.destination.countryCode
```

Si es domestico:

- Label: `National ID`.
- Regex: `^\d{6,12}$`.

Si es internacional:

- Label: `Passport Number`.
- Regex: `^[a-zA-Z0-9]{6,12}$`.

El backend tambien valida esta regla.

## Criterios de aceptacion frontend

- Carga aeropuertos desde `GET /api/airports`.
- Busca vuelos y muestra resultados de ambos providers.
- Diferencia visualmente total y precio por persona.
- Sorting local no dispara nueva request.
- Muestra loading, empty y error states.
- Permite seleccionar un vuelo y avanzar al booking.
- Cambia label y validacion de documento segun ruta.
- Confirma booking y muestra referencia.
- Mantiene UI simple, profesional y facil de explicar.

## Orden recomendado para la proxima ventana

1. Crear app Angular con CLI.
2. Configurar environment/base URL.
3. Crear modelos y `SkyRouteApiService`.
4. Implementar search page.
5. Implementar results + sorting local.
6. Implementar booking page.
7. Agregar tests frontend principales.
8. Verificar flujo end-to-end con backend corriendo.
