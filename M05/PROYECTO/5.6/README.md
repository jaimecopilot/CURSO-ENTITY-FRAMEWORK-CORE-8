# AceriaData - Punto 5.6: Migraciones en entornos de producción

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.6**.

Parte físicamente de M05/PROYECTO/5.5. No cambia el modelo: la última migración sigue siendo M5_5_2_ConcurrencyTokens.

## Delta del punto

- Configuración explícita de MigrationsAssembly en Infrastructure.
- Tabla de historial personalizada: __AceriaMigraciones.
- Aplicación programática controlada mediante IMigrator.
- Generador PowerShell de script SQL idempotente y migration bundle.
- Plan de despliegue con preflight, backup, ejecución, verificación y reversión.
- Ejemplo de pipeline que genera artefactos; el despliegue real queda en un job protegido y usa secretos externos.

## Elección de estrategia

- SQL script cuando el SQL debe revisarse/aprobarse o entregarse a un DBA.
- Migration bundle para automatización de despliegue.
- CLI para desarrollo, pruebas o jobs controlados.
- Runtime migration solo cuando se aceptan explícitamente sus trade-offs. En EF Core 8 no se usa como patrón de arranque de múltiples réplicas.

## IMigrator en este laboratorio

El programa elimina únicamente la base de demostración y deja que IMigrator aplique la cadena completa. Después comprueba:

- migraciones aplicadas > 0;
- migraciones pendientes = 0;
- última migración = M5_5_2_ConcurrencyTokens;
- existencia física de __AceriaMigraciones.

No se usa EnsureCreated().

## Generar artefactos de despliegue

Desde la raíz del punto:

    ./deployment/generate-production-artifacts.ps1

Se generan localmente:

    deployment/artifacts/aceria-idempotent.sql
    deployment/artifacts/aceria-efbundle.exe

No se debe incrustar una cadena de conexión de producción en el repositorio ni en el bundle. La conexión de despliegue se proporciona desde el sistema de secretos.

## Reversión

Un script de downgrade ejecuta operaciones Down y puede perder datos. Debe revisarse y probarse. Para incidentes con riesgo de pérdida de información, la copia de seguridad y su restauración verificada forman parte del plan.

## Compilar y ejecutar

    dotnet restore AceriaData.sln
    dotnet build AceriaData.sln --configuration Release
    dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release

La ejecución debe terminar con:

    5.6 OK