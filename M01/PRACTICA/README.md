# Prácticas del Módulo 1

El documento [M01_PRACTICA.md](M01_PRACTICA.md) contiene el desarrollo completo de las prácticas de los puntos 1.1 a 1.12. Su versión maquetada está en [M01_PRACTICA.pdf](M01_PRACTICA.pdf).

## Relación entre práctica y código

Cada punto de práctica tiene un estado ejecutable correspondiente en [../PROYECTO](../PROYECTO):

| Práctica | Proyecto resultante |
|---|---|
| 1.1 | [../PROYECTO/1.1](../PROYECTO/1.1) |
| 1.2 | [../PROYECTO/1.2](../PROYECTO/1.2) |
| 1.3 | [../PROYECTO/1.3](../PROYECTO/1.3) |
| 1.4 | [../PROYECTO/1.4](../PROYECTO/1.4) |
| 1.5 | [../PROYECTO/1.5](../PROYECTO/1.5) |
| 1.6 | [../PROYECTO/1.6](../PROYECTO/1.6) |
| 1.7 | [../PROYECTO/1.7](../PROYECTO/1.7) |
| 1.8 | [../PROYECTO/1.8](../PROYECTO/1.8) |
| 1.9 | [../PROYECTO/1.9](../PROYECTO/1.9) |
| 1.10 | [../PROYECTO/1.10](../PROYECTO/1.10) |
| 1.11 | [../PROYECTO/1.11](../PROYECTO/1.11) |
| 1.12 | [../PROYECTO/1.12](../PROYECTO/1.12) |

El código es **acumulativo**: el proyecto de 1.2 continúa el de 1.1, 1.3 continúa 1.2 y así sucesivamente hasta 1.12.

## Validación

La CI restaura, compila y ejecuta los doce estados acumulativos. También extrae y ejecuta bloques completos del propio documento de prácticas cuando procede, valida migraciones y SQL Server LocalDB y comprueba que no se adelanten contenidos de puntos posteriores.

La matriz detallada está en [../TRAZABILIDAD_M01.md](../TRAZABILIDAD_M01.md).
