# Decisiones de desarrollo backend - SkyRoute

Documento de apoyo para explicar las decisiones tecnicas del backend durante una entrevista.

## Resumen ejecutivo

El backend se construyo con .NET 8 como una Web API simple, organizada por responsabilidades dentro de un unico proyecto. Para el alcance del challenge, se eligio una arquitectura inspirada en Clean Architecture, pero sin separar fisicamente en muchos proyectos para no agregar complejidad innecesaria.

La API expone tres capacidades principales: consultar aeropuertos, buscar vuelos y confirmar reservas. Internamente separa controllers, contracts, domain, services, providers, pricing, validation y almacenamiento temporal de ofertas.

La decision principal fue mantener controllers finos, reglas de negocio en servicios, contratos HTTP separados del dominio y proveedores/pricing desacoplados mediante interfaces.

## Estructura elegida

La estructura principal es:

```text
SkyRoute.Api/
  Controllers/
  Contracts/
  Domain/
  Pricing/
  Providers/
  Services/
  Validation/
  Program.cs

SkyRoute.Api.Tests/
```

`Controllers` expone endpoints HTTP y traduce resultados a responses.

`Contracts` contiene requests y responses de la API. Estos modelos representan el contrato externo con el frontend.

`Domain` contiene modelos internos del negocio, como `FlightOffer`, `FlightSearchCriteria`, `Airport` y `BookingConfirmation`.

`Services` contiene los casos de uso principales: busqueda de vuelos, booking, catalogo de aeropuertos y almacenamiento temporal de ofertas.

`Providers` simula proveedores de vuelos. Hoy existen `GlobalAir` y `BudgetWings`.

`Pricing` contiene las estrategias de precio por proveedor.

`Validation` concentra reglas de validacion y construccion de errores consistentes.

`SkyRoute.Api.Tests` contiene tests unitarios de reglas criticas.

## Relacion con Clean Architecture

La solucion no divide en proyectos `.Api`, `.Application`, `.Domain` e `.Infrastructure` porque el challenge es acotado. Sin embargo, aplica los principios de Clean Architecture a nivel de carpetas y dependencias internas.

La capa externa es la API HTTP: controllers, swagger, CORS y configuracion.

La capa de aplicacion esta representada por servicios como `FlightSearchService` y `BookingService`. Ahi viven los casos de uso.

La capa de dominio esta representada por records y enums de negocio.

La infraestructura mockeada esta representada por providers e in-memory store.

Esta decision permite explicar que se priorizo una arquitectura limpia pero pragmatica: facil de leer para un challenge, y lista para evolucionar a multiples proyectos si el sistema crece.

## Separacion de responsabilidades

Los controllers no contienen reglas de negocio complejas. Por ejemplo, `FlightsController` recibe el request, llama a `IFlightSearchService` y convierte el resultado a `FlightSearchResponse`.

`FlightSearchService` valida criterios, consulta providers, aplica pricing, normaliza ofertas y las guarda temporalmente.

`BookingService` valida que el `offerId` exista, valida datos del pasajero, valida documento segun tipo de ruta y genera la confirmacion.

`DocumentValidationService` decide si corresponde `National ID` o `Passport Number` y valida el formato.

`InMemoryOfferStore` encapsula la persistencia temporal de ofertas con TTL de 30 minutos.

## Aplicacion de SOLID

SRP, Single Responsibility Principle: cada clase tiene una responsabilidad clara. Los controllers exponen HTTP, los services ejecutan casos de uso, pricing calcula precios, providers entregan vuelos mockeados y validation valida reglas.

OCP, Open Closed Principle: se puede agregar un nuevo proveedor implementando `IFlightProvider` y una nueva estrategia implementando `IPricingStrategy`, sin reescribir los controllers ni el flujo principal.

LSP, Liskov Substitution Principle: cualquier implementacion de `IFlightProvider` puede ser usada por `FlightSearchService` mientras cumpla el contrato de devolver vuelos para un criterio valido.

ISP, Interface Segregation Principle: las interfaces son chicas y enfocadas: `IFlightSearchService`, `IBookingService`, `IFlightProvider`, `IPricingStrategy`, `IOfferStore`, `IAirportCatalog`.

DIP, Dependency Inversion Principle: los servicios dependen de abstracciones inyectadas por DI, no de implementaciones concretas. Esto mejora testabilidad y permite reemplazar proveedores mock por integraciones reales en el futuro.

## Controllers finos

Los controllers se mantuvieron finos para que sean faciles de leer y testear indirectamente.

`FlightsController` solo:

- recibe `FlightSearchRequest`;
- llama a `IFlightSearchService`;
- devuelve `FlightSearchResponse`;
- transforma errores de validacion en `ValidationProblemDetails`.

`BookingsController` sigue la misma idea para confirmar reservas.

Esta decision evita mezclar HTTP con reglas de negocio.

## Contracts y modelos de dominio

Se separaron `Contracts` y `Domain` para no acoplar el contrato externo de la API al modelo interno.

Los requests y responses son DTOs de transporte. Por ejemplo, `FlightSearchResponse` se construye desde `FlightSearchResult` mediante un metodo factory.

Esto permite cambiar detalles internos sin romper al frontend, y permite controlar exactamente que informacion sale por HTTP.

## Providers y pricing

Los proveedores mockeados se implementaron detras de `IFlightProvider`.

Cada proveedor tiene su propia estrategia de pricing mediante `IPricingStrategy`.

Esto evita condicionales grandes en el controller o en el servicio principal. La logica queda extensible y explicable: agregar una aerolinea nueva requiere sumar un provider y, si corresponde, una estrategia de pricing.

Ejemplos actuales:

- `GlobalAirPricingStrategy`
- `BudgetWingsPricingStrategy`

## Validacion

La validacion importante ocurre en backend porque el servidor es la fuente de verdad.

La busqueda valida:

- origen obligatorio;
- destino obligatorio;
- origen y destino distintos;
- aeropuertos soportados;
- fecha de salida obligatoria;
- fecha de salida actual o futura;
- pasajeros entre 1 y 9;
- cabina valida.

El booking valida:

- `offerId` obligatorio;
- oferta existente y no expirada;
- pasajero principal obligatorio;
- nombre;
- email;
- documento.

Para documento se aplica una regla de negocio:

- ruta domestica: `National ID`, numerico, 6 a 12 caracteres;
- ruta internacional: `Passport Number`, alfanumerico, 6 a 12 caracteres.

## Manejo de errores

Se usan excepciones de validacion propias, como `FlightSearchValidationException` y `BookingValidationException`, para separar errores esperables de errores inesperados.

Los controllers capturan esas excepciones y devuelven `ValidationProblemDetails` con HTTP 400.

Esto da una respuesta estandar y facil de consumir desde el frontend.

## API design

Endpoints actuales:

```text
GET  /api/airports
POST /api/flights/search
POST /api/bookings
```

La API separa lectura de catalogo, busqueda de ofertas y confirmacion de reserva.

La busqueda usa `POST` porque recibe criterios en el body y genera una busqueda con `searchId` y ofertas temporales. Aunque semanticamente consulta datos, no es una lectura simple por URL.

El booking usa `POST` porque crea una confirmacion de reserva.

## Versioning

El versionado explicito no se implemento todavia porque el challenge tiene una unica version de API.

La decision es defendible porque los endpoints estan centralizados y son pocos.

Si el sistema creciera, se podria evolucionar a:

```text
/api/v1/airports
/api/v1/flights/search
/api/v1/bookings
```

O usar API Versioning de ASP.NET Core. La estructura actual no impide ese cambio.

## Idempotencia

La busqueda de vuelos se puede repetir sin impacto real de negocio.

La confirmacion de booking no es idempotente por naturaleza, porque podria representar la creacion de una reserva. Para el challenge se mantiene simple: se valida el `offerId`, se confirma y se genera una referencia.

Si se quisiera llevar a una practica mas cercana a produccion, se agregaria un header `Idempotency-Key` en `POST /api/bookings` y se guardaria el resultado asociado a esa key para evitar reservas duplicadas ante reintentos.

## Como explicar versioning e idempotencia en entrevista

Versioning e idempotencia no se implementaron como features completas porque no eran requisitos directos del challenge y podian agregar complejidad innecesaria.

La respuesta defendible es:

> Para este challenge mantuve una unica version de API y documente el camino de evolucion. Si el producto creciera, agregaria versionado explicito con `/api/v1` o API Versioning de ASP.NET Core. La estructura actual ya concentra endpoints y contratos, por lo que el cambio seria localizado.

Sobre idempotencia:

> El booking es una operacion sensible porque podria crear una reserva. En un entorno productivo agregaria un `Idempotency-Key` por request de booking y persistiria el resultado asociado a esa key. Asi, si el cliente reintenta por timeout o error de red, el backend devuelve la misma confirmacion en lugar de crear una reserva duplicada.

Para el challenge, la decision fue mantenerlo como trade-off documentado y evitar sumar almacenamiento adicional solo para simular idempotencia.

## Pagination y filtering

No se implemento paginacion porque los providers mockeados devuelven pocas ofertas.

El ordenamiento se dejo para el frontend, como pide el challenge, para evitar nuevas llamadas al backend al cambiar el sort.

Si el volumen creciera, el backend podria sumar:

- filtros por provider;
- filtros por rango de precio;
- filtros por cabina;
- paginacion server side;
- metadata como `page`, `pageSize`, `totalItems`.

No agregarlo ahora evita complejidad innecesaria.

## Persistencia en memoria

Las ofertas se guardan en `InMemoryOfferStore` durante 30 minutos.

Esto permite que el flujo de booking valide un `offerId` real obtenido por una busqueda previa.

El trade-off es claro: si se reinicia la API, se pierden las ofertas. Para el challenge es aceptable porque no se pidio base de datos ni persistencia real.

En produccion, esto evolucionaria a una base de datos, cache distribuida o storage transaccional segun necesidades.

## Testabilidad

La testabilidad fue una decision central.

Al depender de interfaces, los servicios pueden testearse sin levantar la API HTTP.

Tests actuales cubren:

- busqueda normalizada desde ambos providers;
- validacion de criterios invalidos;
- pricing por proveedor;
- validacion de documento domestico e internacional;
- booking valido;
- booking con oferta inexistente.

Esto demuestra que las reglas de negocio estan fuera de los controllers y se pueden validar con tests unitarios rapidos.

## Configuracion y DI

`Program.cs` registra dependencias con DI:

- `IAirportCatalog` como singleton;
- `IOfferStore` como singleton;
- providers como scoped;
- pricing strategies como scoped;
- services como scoped.

Tambien configura:

- controllers;
- serializacion de enums como string;
- CORS para Angular local;
- Swagger en desarrollo;
- HTTPS redirection.

Serializar enums como string mejora la legibilidad del contrato API. Por ejemplo, el frontend recibe `Economy` en lugar de un numero.

## CORS y frontend local

Se configuro una politica CORS especifica para:

```text
http://localhost:4200
https://localhost:4200
```

Esto permite integrar Angular localmente sin abrir CORS a cualquier origen.

Es una decision segura y practica para desarrollo.

## Trade-offs aceptados

Se aceptaron algunos trade-offs por el alcance del challenge:

- sin base de datos;
- sin autenticacion;
- sin pagos;
- sin proveedores externos reales;
- sin colas ni mensajeria;
- sin arquitectura en multiples proyectos;
- sin versionado explicito todavia;
- sin idempotency key todavia;
- sin paginacion server side.

Lo importante es que todos esos puntos tienen un camino claro de evolucion.

## Como defender la arquitectura

Una respuesta corta seria:

> Elegi una arquitectura limpia pero pragmatica. No separe en cuatro proyectos porque el challenge es chico, pero mantuve las responsabilidades separadas por carpetas: controllers, contracts, domain, services, providers, pricing y validation. Eso deja el codigo facil de leer, testear y extender.

Otra respuesta:

> Los controllers son finos y no contienen reglas de negocio. La busqueda vive en `FlightSearchService`, el booking en `BookingService`, y las reglas especificas como pricing o documento estan aisladas en servicios propios.

Otra:

> Si tuviera que agregar un tercer proveedor, implementaria `IFlightProvider` y su `IPricingStrategy`. No tendria que tocar el controller ni cambiar el contrato HTTP.

## Preguntas esperables de entrevista

Pregunta: Por que no separaste en proyectos Application, Domain e Infrastructure?

Respuesta sugerida:

> Por el alcance del challenge preferi evitar sobreingenieria. Aplique la separacion de responsabilidades dentro de un unico proyecto para mantenerlo facil de navegar. Si el sistema crece, la estructura actual permite extraer Domain, Application e Infrastructure con bajo impacto.

Pregunta: Por que usar DTOs/contracts si el dominio es parecido?

Respuesta sugerida:

> Porque el contrato HTTP no deberia depender directamente del modelo interno. Hoy se parecen, pero separarlos permite evolucionar la API o el dominio sin romper al consumidor.

Pregunta: Como se aplica SOLID?

Respuesta sugerida:

> Principalmente con responsabilidades claras e inversion de dependencias. Los services dependen de interfaces como `IFlightProvider`, `IPricingStrategy` e `IOfferStore`, lo que permite extender y testear sin acoplarse a implementaciones concretas.

Pregunta: Que faltaria para produccion?

Respuesta sugerida:

> Persistencia real, autenticacion, observabilidad, versionado formal, idempotency key para booking, manejo global de errores, integraciones reales con providers y posiblemente paginacion server side.

## Respuesta final para entrevista

El backend fue construido con foco en claridad, testabilidad y escalabilidad progresiva. La arquitectura es simple pero separa bien responsabilidades: los controllers exponen HTTP, los services contienen casos de uso, el dominio modela conceptos internos, los contracts definen el contrato externo, providers simulan aerolineas, pricing encapsula reglas de precio y validation concentra reglas de negocio. No agregue complejidad de produccion innecesaria, pero deje puntos claros de evolucion para versioning, idempotencia, persistencia, paginacion y proveedores reales.
