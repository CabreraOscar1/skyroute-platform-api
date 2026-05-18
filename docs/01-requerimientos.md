# SkyRoute - Requerimientos del proyecto

Fuente revisada: `[arrivia] - SkyRoute_Developer_Challenge 2.docx`

## Resumen

SkyRoute es una plataforma agregadora de vuelos. El challenge pide construir un modulo de busqueda y reserva de vuelos como una porcion realista del producto, con frontend, backend y una arquitectura mantenible.

Stack requerido:

- Frontend: Angular.
- Backend: .NET.
- Nivel esperado: Senior Full-Stack.
- Tiempo estimado: 3 a 4 horas.
- Herramientas de IA: permitidas y alentadas.

## Ajustes detectados al repasar el docx

No aparecen requisitos funcionales nuevos respecto del analisis inicial, pero conviene documentar estas precisiones:

- Los mocks de proveedores deben devolver un set realista de vuelos para cualquier busqueda valida.
- El precio principal en resultados debe ser siempre el total de todos los pasajeros; el precio por pasajero es informacion secundaria.
- El ordenamiento debe ocurrir solo en frontend y cambiar el sort no debe disparar una nueva llamada a la API.
- La cabina puede mostrarse como `First Class` en UI, pero el contrato tecnico puede usar `FirstClass` para evitar espacios en enums.
- La etiqueta y validacion del documento deben cambiar segun la ruta seleccionada; el backend debe repetir la validacion como fuente final de verdad.
- El README final debe explicar setup, ejecucion local, decisiones de arquitectura, trade-offs y limitaciones.
- En la entrevista se espera un walkthrough del codigo, por lo que las decisiones deben ser simples de defender.

## Objetivo funcional

Construir una aplicacion local que permita:

- Buscar vuelos disponibles.
- Comparar resultados de `GlobalAir` y `BudgetWings`.
- Ordenar resultados en frontend.
- Seleccionar un vuelo.
- Completar datos de pasajero.
- Confirmar una reserva.
- Recibir un codigo de referencia de booking.

## Contexto de negocio

SkyRoute agrega datos de multiples proveedores aereos. Para este challenge se deben integrar dos proveedores mockeados:

- `GlobalAir`
- `BudgetWings`

La solucion debe dejar claro como se agregarian nuevos proveedores sin reescribir el flujo principal.

## Reglas de pricing

| Proveedor | Regla | Notas |
| --- | --- | --- |
| GlobalAir | `baseFare + 15% fuel surcharge` | Redondear siempre a 2 decimales. |
| BudgetWings | `baseFare - 10% promotional discount` | Precio minimo final: `USD 29.99`. El descuento aplica solo sobre la tarifa base. |

El resultado debe mostrar dos importes distintos:

- Precio total de todos los pasajeros combinados.
- Precio por pasajero como informacion secundaria.

Ejemplo esperado:

```text
USD 320.00 total / USD 160.00 per person
```

## Busqueda de vuelos

El formulario debe capturar:

- Aeropuerto de origen.
- Aeropuerto de destino.
- Fecha de salida.
- Cantidad de pasajeros, entre 1 y 9.
- Clase de cabina: `Economy`, `Business` o `First Class`.

Aeropuertos:

- No se requiere API externa.
- Se pueden hardcodear.
- Deben existir al menos 6 aeropuertos.
- Deben cubrir al menos 2 paises.

Los resultados deben mostrar:

- Proveedor aereo.
- Numero de vuelo.
- Hora de salida.
- Hora de llegada.
- Duracion.
- Clase de cabina.
- Precio total.
- Precio por pasajero.

## Resultados y ordenamiento

Los resultados deben poder ordenarse por:

- Precio de menor a mayor.
- Precio de mayor a menor.
- Duracion, primero la mas corta.
- Hora de salida.

Reglas:

- El ordenamiento ocurre en frontend.
- Cambiar el orden no debe llamar nuevamente al backend.
- Debe mostrarse loading durante la busqueda.
- Debe mostrarse empty state si no hay vuelos compatibles.

## Flujo de reserva

Al seleccionar un vuelo, la pantalla de booking debe incluir:

- Resumen de ruta, proveedor, horarios y cabina.
- Desglose de precio:
  - Precio por pasajero.
  - Cantidad de pasajeros.
  - Total.
- Formulario de pasajero:
  - Nombre completo.
  - Email.
  - Numero de documento.
- Accion `Confirm Booking`.

El backend debe responder con un codigo de referencia.

## Documento del pasajero

| Tipo de vuelo | Condicion | Etiqueta | Validacion |
| --- | --- | --- | --- |
| Internacional | Origen y destino en paises distintos | `Passport Number` | Alfanumerico, 6 a 12 caracteres. |
| Domestico | Origen y destino en el mismo pais | `National ID` | Numerico, 6 a 12 digitos. |

La UI aplica la regla para mejorar la experiencia. El backend la aplica nuevamente para no confiar solo en el cliente.

## Entregables

- Aplicacion funcionando localmente.
- Frontend y backend.
- Codigo fuente en repositorio publico o compartido.
- README con setup, ejecucion, arquitectura, trade-offs y limitaciones.
- No se requiere despliegue cloud.

## Criterios de aceptacion

- El usuario puede buscar vuelos con los campos requeridos.
- El backend devuelve resultados mockeados de `GlobalAir` y `BudgetWings`.
- Los precios respetan las reglas de cada proveedor.
- El total para todos los pasajeros se muestra separado del precio por pasajero.
- El usuario puede ordenar resultados sin nueva llamada al backend.
- Hay loading y empty state.
- El usuario puede seleccionar un vuelo y avanzar al booking.
- El tipo de documento cambia correctamente entre vuelos domesticos e internacionales.
- El backend valida el documento segun la ruta.
- La confirmacion devuelve una referencia de reserva.
- El codigo permite agregar nuevos proveedores con cambios localizados.
