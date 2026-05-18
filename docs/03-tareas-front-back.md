# SkyRoute - Tareas separadas por front y back

Este checklist separa responsabilidades para trabajar sin mezclar capas antes de escribir codigo.

## Backend

### Base del proyecto

- [x] Crear solucion .NET dentro de `Backend/`.
- [x] Crear proyecto `SkyRoute.Api`.
- [x] Configurar CORS para el frontend local.
- [x] Definir contratos request/response.
- [x] Configurar manejo consistente de errores de validacion.

### Catalogo y dominio

- [x] Crear catalogo hardcodeado de aeropuertos.
- [x] Modelar `CabinClass`, `Airport`, `FlightSearchCriteria` y `FlightOffer`.
- [x] Validar origen, destino, fecha, pasajeros y cabina.
- [x] Asegurar que `origin != destination`.

### Providers y pricing

- [x] Crear contrato `IFlightProvider`.
- [x] Implementar `GlobalAirProvider`.
- [x] Implementar `BudgetWingsProvider`.
- [x] Hacer que cada provider devuelva resultados realistas para cualquier busqueda valida.
- [x] Crear contrato `IPricingStrategy`.
- [x] Implementar pricing de `GlobalAir`: `baseFare * 1.15`.
- [x] Implementar pricing de `BudgetWings`: `max(baseFare * 0.90, 29.99)`.
- [x] Calcular precio total multiplicando por cantidad de pasajeros.

### Search API

- [x] Crear `FlightSearchService`.
- [x] Consultar ambos providers desde el servicio.
- [x] Normalizar resultados en un contrato comun.
- [x] Guardar ofertas generadas en memoria para booking.
- [x] Exponer `POST /api/flights/search`.
- [x] Exponer `GET /api/airports` para evitar duplicar catalogo en frontend.

### Booking API

- [x] Crear `OfferStore` en memoria.
- [x] Crear `DocumentValidationService`.
- [x] Determinar si el vuelo es domestico o internacional desde la oferta.
- [x] Validar `Passport Number` para vuelos internacionales.
- [x] Validar `National ID` para vuelos domesticos.
- [x] Crear `BookingService`.
- [x] Validar que `offerId` exista.
- [x] Generar referencia `SKY-YYYYMMDD-XXXXX`.
- [x] Exponer `POST /api/bookings`.

### Tests backend

- [x] Pricing de `GlobalAir`.
- [x] Pricing de `BudgetWings`, incluyendo minimo `USD 29.99`.
- [x] Validacion de pasajeros fuera de rango.
- [x] Validacion de origen igual a destino.
- [x] Agregacion de resultados de ambos providers.
- [x] Validacion de documento domestico e internacional.
- [x] Booking con offer valido.
- [x] Error de booking con offer inexistente.

## Frontend

### Base del proyecto

- [ ] Crear app Angular dentro de `Frontend/`.
- [ ] Configurar rutas principales.
- [ ] Crear modelos TypeScript alineados con la API.
- [ ] Crear servicio HTTP para flights y bookings.
- [ ] Configurar environment con URL del backend local.

### Busqueda

- [ ] Crear pantalla `flight-search`.
- [ ] Crear formulario con origen, destino, fecha, pasajeros y cabina.
- [ ] Usar dropdowns para aeropuertos hardcodeados o provistos por config local.
- [ ] Validar campos requeridos.
- [ ] Validar pasajeros entre 1 y 9.
- [ ] Enviar busqueda a `POST /api/flights/search`.
- [ ] Mostrar loading durante la llamada.
- [ ] Mostrar error state ante fallos.
- [ ] Mostrar empty state cuando no haya resultados.

### Resultados y ordenamiento

- [ ] Mostrar proveedor, numero de vuelo, salida, llegada, duracion, cabina y precio.
- [ ] Mostrar precio total como dato principal.
- [ ] Mostrar precio por pasajero como dato secundario.
- [ ] Implementar sort local por precio ascendente.
- [ ] Implementar sort local por precio descendente.
- [ ] Implementar sort local por duracion.
- [ ] Implementar sort local por hora de salida.
- [ ] Verificar que cambiar el sort no llame nuevamente al backend.

### Booking

- [ ] Permitir seleccionar una oferta.
- [ ] Crear pantalla `booking`.
- [ ] Mostrar resumen de ruta, proveedor, horarios y cabina.
- [ ] Mostrar desglose de precio por pasajero, pasajeros y total.
- [ ] Crear formulario de pasajero con nombre, email y documento.
- [ ] Cambiar label a `Passport Number` para vuelos internacionales.
- [ ] Cambiar label a `National ID` para vuelos domesticos.
- [ ] Aplicar validacion frontend segun tipo de documento.
- [ ] Enviar confirmacion a `POST /api/bookings`.
- [ ] Mostrar referencia de reserva confirmada.

### Tests frontend

- [ ] Validacion del formulario de busqueda.
- [ ] Cambio de label de documento segun ruta.
- [ ] Ordenamiento por precio ascendente y descendente.
- [ ] Ordenamiento por duracion.
- [ ] Estado loading.
- [ ] Estado empty.

## Integracion y documentacion

- [ ] Verificar flujo internacional `EZE` a `MIA`, 2 pasajeros, `Economy`.
- [ ] Verificar flujo domestico `JFK` a `LAX`.
- [ ] Verificar booking internacional con `Passport Number`.
- [ ] Verificar booking domestico con `National ID`.
- [x] Documentar comandos de setup y ejecucion en README.
- [x] Documentar decisiones de arquitectura.
- [x] Documentar trade-offs y limitaciones conocidas.

## Orden sugerido de trabajo

1. Backend search con providers y pricing.
2. Frontend search con resultados.
3. Sorting local en frontend.
4. Frontend booking y confirmacion.
5. Tests principales y README final.
