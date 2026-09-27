# AceriaData - Punto 2.12: Clean Architecture y Arquitectura Hexagonal

Esta carpeta representa el **estado completo y ejecutable del proyecto al finalizar 2.12**.

- Estado anterior: `M02/PROYECTO/2.11`
- Estado siguiente: `M03/PROYECTO/3.1`
- Solución local: `AceriaData.sln`

Cada punto conserva lo ya construido y añade únicamente el contenido correspondiente a 2.12. No es un ejemplo aislado.

## Abrir y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console.csproj --configuration Release
```

Las migraciones acumulativas se validan automáticamente en GitHub Actions contra SQL Server LocalDB.

Consulta la práctica completa en [M02_PRACTICA.md](../../PRACTICA/M02_PRACTICA.md) y la matriz de trazabilidad en [TRAZABILIDAD_M02.md](../../TRAZABILIDAD_M02.md).
