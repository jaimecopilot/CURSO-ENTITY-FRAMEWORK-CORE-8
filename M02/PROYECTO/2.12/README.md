# AceriaData - Punto 2.12: Clean Architecture y Arquitectura Hexagonal

Esta carpeta representa el **estado completo y ejecutable de AceriaData al finalizar el Módulo 2**. Parte de `M02/PROYECTO/2.11`, conserva el modelo acumulado y reorganiza el sistema en cuatro proyectos.

## Estructura

```text
2.12/
├── AceriaData.sln
└── src/
    ├── AceriaData.Domain/
    ├── AceriaData.Application/
    ├── AceriaData.Infrastructure/
    └── AceriaData.Console/
```

- **Domain** contiene entidades y no referencia EF Core.
- **Application** contiene puertos y casos de uso y no referencia EF Core.
- **Infrastructure** contiene `AceriaDbContext`, configuraciones, repositorios, unidad de trabajo y migraciones.
- **Console** es la capa de entrada y proyecto de arranque.

## Abrir y compilar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
```

## Validar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

Resultado esperado: la ejecución final contiene `2.12 OK`.

Este es el estado que deberá utilizar `M03/PROYECTO/3.1` como punto de partida.

Consulta la [práctica completa](../../PRACTICA/M02_PRACTICA.md) y la [trazabilidad M2](../../TRAZABILIDAD_M02.md).
