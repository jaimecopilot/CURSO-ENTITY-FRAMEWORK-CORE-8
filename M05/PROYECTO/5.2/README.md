# AceriaData - Punto 5.1: Concurrencia optimista, concepto y necesidad

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.1**.

Parte físicamente de `M04/PROYECTO/4.12`, mantiene las cuatro capas —Domain, Application, Infrastructure y Console— y conserva íntegra la historia de migraciones heredada. El punto **no cambia el modelo**, por lo que no añade una migración nueva.

La demostración corrige un error habitual al explicar concurrencia:

- si dos `DbContext` modifican **la misma propiedad** sin token de concurrencia, el último guardado puede sobrescribir el cambio anterior;
- si modifican **propiedades distintas** y las entidades se cargaron con tracking normal, EF Core actualiza las propiedades modificadas y ambos cambios pueden conservarse;
- todavía no hay token de concurrencia: ese mecanismo se introduce en 5.2.

La salida muestra el SQL realmente observado por el interceptor, no sentencias escritas manualmente como si hubieran sido generadas por EF Core.

## Compilar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
```

## Validar migraciones heredadas

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe conservarse como última migración heredada **`M2_2_12_Architecture`**.

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe demostrar los dos escenarios y terminar con:

```text
5.1 OK
```
