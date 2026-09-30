# Plan de despliegue de migraciones — AceriaData

Este plan separa la generación del artefacto de la ejecución sobre la base de datos.

## 1. Preflight

- Confirmar la versión de aplicación y la migración objetivo.
- Ejecutar dotnet ef migrations has-pending-model-changes.
- Generar el script SQL idempotente y/o el migration bundle.
- Revisar la migración y, si el proceso exige aprobación SQL, revisar el script generado.
- Validar el artefacto sobre una base de datos de prueba restaurada desde una copia representativa.

## 2. Copia de seguridad

Antes de un cambio de esquema potencialmente destructivo, crear una copia de seguridad con las herramientas soportadas por el entorno y probar que la restauración funciona. Guardar un archivo no basta si nunca se ha verificado el restore.

## 3. Estrategia de ejecución

- Script SQL: usar cuando un DBA o un proceso de aprobación debe inspeccionar, modificar o archivar el SQL.
- Migration bundle: usar para un job de despliegue automatizado y controlado.
- CLI: reservar principalmente para desarrollo, pruebas o un entorno de despliegue controlado.
- Runtime: no hacer que cada réplica de EF Core 8 migre el esquema al arrancar.

La identidad de despliegue puede tener permisos de esquema. La identidad normal de ejecución debería tener solo los permisos que necesita la aplicación.

## 4. Verificación posterior

- Comprobar que no quedan migraciones pendientes.
- Verificar la tabla de historial configurada: __EFMigrationsHistory.
- Ejecutar smoke tests sobre las operaciones de lectura y escritura críticas.
- Revisar logs y métricas del despliegue antes de aumentar tráfico.

## 5. Reversión

Un downgrade de EF ejecuta los métodos Down y puede destruir datos. No se trata como sustituto de un backup.

Orden de preferencia según el incidente:

1. detener el despliegue y corregir hacia delante si es seguro;
2. ejecutar un downgrade solo si su Down ha sido revisado y probado con datos;
3. restaurar la copia de seguridad cuando sea necesario recuperar datos/esquema de forma consistente.

La decisión debe incluir también compatibilidad entre la versión de aplicación y la versión del esquema.