# Proyecto acumulativo AceriaData — Módulo 1

Este directorio contiene **doce estados completos y autónomos** del mismo proyecto AceriaData:

`1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6 → 1.7 → 1.8 → 1.9 → 1.10 → 1.11 → 1.12`

## Estructura de cada punto

Cada carpeta `1.x` contiene, como mínimo:

```text
1.x/
├── AceriaData.sln
├── AceriaData.Console.csproj
├── Program.cs
└── README.md
```

A partir de los puntos que lo requieren se añaden también `Migrations/`, `appsettings.json` y otros archivos acumulativos.

**No existe una solución central en la raíz del repositorio.** Para trabajar un punto, se abre la solución situada dentro de ese punto.

## Abrir en Visual Studio

Ejemplo para 1.8:

```text
M01/PROYECTO/1.8/AceriaData.sln
```

## Ejecutar desde consola

Desde cualquier carpeta `1.x`:

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console.csproj --configuration Release
```

El estado final del módulo es **1.12** y será la base del primer punto de M2.

La trazabilidad detallada está en [../TRAZABILIDAD_M01.md](../TRAZABILIDAD_M01.md).
