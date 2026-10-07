# AceriaData - Punto 5.8: Migraciones en equipos

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.8**.

Parte físicamente de `M05/PROYECTO/5.7`. El modelo oficial no cambia en este punto y la última migración oficial continúa siendo `M5_5_2_ConcurrencyTokens`.

## Criterio técnico importante

En árboles de migración divergentes no se considera una solución válida "renombrar la migración para ordenar las fechas".

Una migración de EF Core no es solo el nombre del archivo. Sus metadatos y su archivo `.Designer.cs` representan el modelo objetivo que existía cuando se generó. Si dos desarrolladores generan migraciones en paralelo desde el mismo snapshot, cada migración desconoce el cambio de modelo de la otra rama.

La estrategia enseñada en AceriaData es:

1. conservar el cambio de modelo propio;
2. si la migración propia todavía no está compartida/aplicada, retirarla o descartarla;
3. incorporar la migración del compañero;
4. aplicar de nuevo el cambio de modelo propio;
5. regenerar la migración propia sobre el snapshot ya fusionado;
6. ejecutar `dotnet ef migrations has-pending-model-changes`;
7. aplicar la cadena resultante en una base aislada y verificar el esquema.

Si una migración ya fue compartida o aplicada a una base compartida, no se borra ni se reescribe unilateralmente. Se coordina rollback cuando sea seguro o se crea una migración correctiva.

## Laboratorio reproducible

Ejecutar:

```powershell
./team-migrations/validate-team-migrations.ps1
```

El script crea tres copias temporales y desechables del proyecto:

- **Rama A**: añade `EquipoRevisionA` y genera `M5_5_8_TeamA`.
- **Rama B paralela**: añade `EquipoRevisionB` y genera `M5_5_8_TeamBParallel` desde el mismo baseline.
- **Estado fusionado correcto**: parte de Rama A, añade el cambio B y genera `M5_5_8_TeamBRegenerated`.

La prueba exige que:

- el designer de la migración paralela B no conozca `EquipoRevisionA`;
- el designer regenerado B sí contenga los cambios A y B;
- `has-pending-model-changes` termine correctamente;
- la cadena fusionada se aplique a SQL Server LocalDB;
- ambas columnas existan en el esquema final;
- `__EFMigrationsHistory` contenga las dos migraciones regeneradas esperadas.

El laboratorio termina con:

```text
5.8 EQUIPOS OK
```

## Compilar y ejecutar el checkpoint

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
./team-migrations/validate-team-migrations.ps1
```

La aplicación termina con:

```text
5.8 OK
```
