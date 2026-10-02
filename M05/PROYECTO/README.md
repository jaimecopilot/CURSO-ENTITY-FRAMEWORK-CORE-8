# Proyecto acumulativo AceriaData — Módulo 5

Cada punto contiene una solución completa `AceriaData.sln`. La secuencia continúa desde `M04/PROYECTO/4.12` y cada estado conserva lo incorporado anteriormente.

| Punto | Tema | Carpeta |
|---|---|---|
| 5.1 | Concurrencia optimista: concepto y necesidad | [5.1](5.1/) |
| 5.2 | Configuración de tokens de concurrencia | [5.2](5.2/) |
| 5.3 | Resolución de conflictos de concurrencia | [5.3](5.3/) |
| 5.4 | Transacciones, `SaveChanges` y savepoints | [5.4](5.4/) |
| 5.5 | Transacciones ambientales | [5.5](5.5/) |
| 5.6 | Migraciones en producción | [5.6](5.6/) |
| 5.7 | Scripts SQL idempotentes | [5.7](5.7/) |
| 5.8 | Migraciones en equipos | [5.8](5.8/) |
| 5.9 | Repository y Unit of Work | [5.9](5.9/) |
| 5.10 | Logging y diagnóstico | [5.10](5.10/) |
| 5.11 | Testing con EF Core | [5.11](5.11/) |
| 5.12 | Buenas prácticas y anti-patrones | [5.12](5.12/) |

## Abrir el estado final

```powershell
cd M05/PROYECTO/5.12
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

El marcador final de la aplicación es:

```text
5.12 OK
```
