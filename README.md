# SkyRoute Travel Platform

Repositorio para el challenge tecnico full-stack de SkyRoute.

## Objetivo

Construir un modulo local de busqueda y reserva de vuelos con:

- Frontend Angular.
- Backend .NET.
- Proveedores mockeados `GlobalAir` y `BudgetWings`.
- Pricing por proveedor.
- Ordenamiento de resultados en frontend, sin nueva llamada al backend.
- Flujo de booking con validacion de documento domestico/internacional.

## Documentacion

- `docs/01-requerimientos.md`: requisitos funcionales refinados desde el `.docx`.
- `docs/02-arquitectura-propuesta.md`: decisiones tecnicas, contratos y responsabilidades.
- `docs/03-tareas-front-back.md`: tareas separadas entre backend, frontend e integracion.
- `docs/04-frontend-angular-handoff.md`: guia para arrancar el frontend Angular en otra ventana de contexto.

## Estado

La rama `dev` contiene la preparacion documental del proyecto y el backend principal:

- API .NET 8 creada.
- `GET /api/airports` implementado.
- `POST /api/flights/search` implementado.
- `POST /api/bookings` implementado.
- Providers mockeados `GlobalAir` y `BudgetWings`.
- Pricing por proveedor.
- Validacion de `Passport Number` para rutas internacionales.
- Validacion de `National ID` para rutas domesticas.
- CORS listo para Angular local.
- Tests unitarios backend agregados.

Aun falta implementar frontend Angular y tests frontend.

## Backend

### Ejecutar desde Visual Studio 2022

1. Abrir `Backend/SkyRoute.Api/SkyRoute.Api.sln`.
2. Seleccionar el perfil `https`.
3. Ejecutar con `F5` o el boton verde.
4. Abrir Swagger en la URL que levante Visual Studio, normalmente:

```text
https://localhost:7140/swagger
```

### Ejecutar por consola

```powershell
dotnet run --project Backend\SkyRoute.Api\SkyRoute.Api.csproj --launch-profile https
```

### Ejecutar tests backend

```powershell
dotnet test Backend\SkyRoute.Api.Tests\SkyRoute.Api.Tests.csproj
```

Si los paquetes ya estan restaurados y se quiere una corrida rapida:

```powershell
dotnet test Backend\SkyRoute.Api.Tests\SkyRoute.Api.Tests.csproj --no-build --no-restore
```

### Endpoints disponibles

#### Listar aeropuertos

```http
GET /api/airports
```

Devuelve los aeropuertos hardcodeados que debe usar el frontend.

#### Buscar vuelos

```http
POST /api/flights/search
```

Body:

```json
{
  "origin": "EZE",
  "destination": "MIA",
  "departureDate": "2026-06-10",
  "passengerCount": 2,
  "cabinClass": "Economy"
}
```

La respuesta incluye resultados normalizados de `GlobalAir` y `BudgetWings`, con `pricePerPassenger` y `totalPrice`.

#### Confirmar booking

```http
POST /api/bookings
```

Body:

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

Importante: primero hay que ejecutar una busqueda y copiar un `offerId` real de la respuesta. Las ofertas se guardan en memoria por 30 minutos y se pierden al reiniciar la API.

## Decisiones de arquitectura

- `Contracts` contiene los modelos HTTP de entrada y salida.
- `Domain` contiene modelos internos del negocio.
- `Providers` simula aerolineas y deja preparado el agregado de nuevos proveedores.
- `Pricing` encapsula las reglas de precio por proveedor.
- `Services` concentra los casos de uso de busqueda y booking.
- `Validation` contiene validaciones compartidas y formateo consistente de errores.

## Trade-offs conocidos

- Persistencia en memoria para ofertas; suficiente para el challenge, no para produccion.
- Sin autenticacion, pagos ni integraciones reales con aerolineas.
- Los vuelos son mockeados pero deterministas segun ruta/cabina.
- El booking confirma solo un pasajero principal, aunque el precio contempla todos los pasajeros buscados.

## Proximos pasos

1. Scaffold de frontend Angular.
2. Implementacion del flujo de busqueda en frontend.
3. Implementacion del flujo de booking en frontend.
4. Tests frontend.
5. Verificacion manual end-to-end.
