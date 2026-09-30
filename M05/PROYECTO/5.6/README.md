# AceriaData - Punto 5.6: Migraciones en entornos de producción

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.6**.

Parte físicamente de `M05/PROYECTO/5.5`. No cambia el modelo: la última migración sigue siendo `M5_5_2_ConcurrencyTokens`.

## Delta del punto

- Configuración explícita de `MigrationsAssembly` en Infrastructure.
- Conservación de la tabla de historial heredada `__EFMigrationsHistory`.
- Explicación específica de `MigrationsHistoryTable` y del riesgo de cambiarla después de haber aplicado migraciones.
- Aplicación programática controlada mediante `IMigrator`.
- Generador PowerShell de script SQL idempotente y migration bundle.
- Plan de despliegue con preflight, backup, ejecución, verificación y reversión.
- Ejemplo de pipeline que genera artefactos; el despliegue real queda en un job protegido y usa secretos externos.

## Por qué no se renombra el historial en este punto

AceriaData llega a 5.6 con una cadena real de migraciones ya aplicada en puntos anteriores. Cambiar ahora de `__EFMigrationsHistory` a otro nombre sin mover también sus filas haría que EF Core perdiera la referencia de qué migraciones ya están aplicadas.

La personalización con `MigrationsHistoryTable` es válida, pero debe decidirse desde el inicio o acompañarse de una migración/operación explícita del propio historial. El archivo `deployment/MIGRATIONS_HISTORY_TABLE.md` documenta el patrón.

## Elección de estrategia

- **SQL script** cuando el SQL debe revisarse/aprobarse o entregarse a un DBA.
- **Migration bundle** para automatización de despliegue.
- **CLI** para desarrollo, pruebas o jobs controlados.
- **Runtime migration** solo cuando se aceptan explícitamente sus trade-offs. En EF Core 8 no se usa como patrón de arranque de múltiples réplicas.

## IMigrator en este laboratorio

El programa elimina únicamente la base de demostración y deja que `IMigrator` aplique la cadena completa. Después comprueba:

- migraciones aplicadas > 0;
- migraciones pendientes = 0;
- última migración = `M5_5_2_ConcurrencyTokens`;
- existencia física de `__EFMigrationsHistory`.

No se usa `EnsureCreated()`.

## Generar artefactos de despliegue

Desde la raíz del punto:

```powershell
./deployment/generate-production-artifacts.ps1
```

Se generan localmente:

```text
deployment/artifacts/aceria-idempotent.sql
deployment/artifacts/aceria-efbundle.exe
```

No se debe incrustar una cadena de conexión de producción en el repositorio ni en el bundle. La conexión de despliegue se proporciona desde el sistema de secretos.

## Reversión

Un script de downgrade ejecuta operaciones `Down` y puede perder datos. Debe revisarse y probarse. Para incidentes con riesgo de pérdida de información, la copia de seguridad y su restauración verificada forman parte del plan.

## Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe terminar con:

```text
5.6 OK
```
