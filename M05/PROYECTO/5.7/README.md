# AceriaData - Punto 5.7: Scripts idempotentes

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.7**.

Parte físicamente de `M05/PROYECTO/5.6`. No cambia el modelo: la última migración continúa siendo `M5_5_2_ConcurrencyTokens`.

## Delta del punto

- Generación real del script SQL completo.
- Generación real del script idempotente con `--idempotent`.
- Generación de un script de rango entre `M2_2_12_Architecture` y `M5_5_2_ConcurrencyTokens`.
- Generación de un script de downgrade en el sentido inverso.
- Aplicación del mismo script idempotente dos veces sobre una base SQL Server LocalDB aislada.
- Validación del historial `__EFMigrationsHistory` después de ambas aplicaciones.
- Validación del esquema resultante.
- Comprobación de que la segunda aplicación no duplica migraciones.
- Tratamiento explícito del downgrade como una operación potencialmente destructiva.

## Qué significa idempotencia en este laboratorio

La demostración no se limita a inspeccionar el texto SQL.

El script `deployment/validate-idempotent-scripts.ps1`:

1. genera los artefactos SQL desde las migraciones reales;
2. crea una base LocalDB aislada;
3. aplica el script idempotente una primera vez;
4. registra el número de filas de `__EFMigrationsHistory`;
5. comprueba la migración final y elementos del esquema;
6. aplica exactamente el mismo script una segunda vez;
7. vuelve a contar el historial;
8. exige que el número de migraciones aplicadas no cambie.

Si la segunda ejecución duplica historial, modifica de nuevo el esquema de forma incorrecta o falla, el laboratorio falla.

## Scripts generados

Desde la raíz del punto:

```powershell
./deployment/validate-idempotent-scripts.ps1
```

Se generan localmente:

```text
deployment/artifacts-5.7/aceria-completo.sql
deployment/artifacts-5.7/aceria-idempotente.sql
deployment/artifacts-5.7/aceria-rango.sql
deployment/artifacts-5.7/aceria-downgrade.sql
```

Los archivos generados son artefactos y no sustituyen al historial de migraciones del proyecto.

## Script de rango

El rango usa nombres que existen realmente en AceriaData:

```text
M2_2_12_Architecture
        ->
M5_5_2_ConcurrencyTokens
```

No se utilizan nombres ficticios como `InitialCreate` o `AddRowVersion` cuando no corresponden al historial real del repositorio.

## Downgrade

El script inverso se genera para estudiar y revisar las operaciones `Down`.

Un downgrade puede eliminar columnas, índices, tablas o datos. Que EF Core pueda generar el script no implica que sea una estrategia de recuperación segura. Antes de aplicarlo en un entorno con datos deben existir revisión, copia de seguridad y un plan de recuperación probado.

## Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
./deployment/validate-idempotent-scripts.ps1
```

La aplicación termina con:

```text
5.7 OK
```

La validación de scripts termina con:

```text
5.7 IDEMPOTENCIA OK
```
