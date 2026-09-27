# Prácticas del Módulo 1

El documento [M01_PRACTICA.md](M01_PRACTICA.md) contiene el desarrollo completo de las prácticas de los puntos 1.1 a 1.12. Su versión maquetada está en [M01_PRACTICA.pdf](M01_PRACTICA.pdf).

## Relación entre práctica y solución

Cada punto tiene un estado acumulativo autónomo en [../PROYECTO](../PROYECTO). **Cada estado contiene su propia `AceriaData.sln`**, preparada para abrir directamente en Visual Studio:

| Práctica | Solución resultante |
|---|---|
| 1.1 | [../PROYECTO/1.1/AceriaData.sln](../PROYECTO/1.1/AceriaData.sln) |
| 1.2 | [../PROYECTO/1.2/AceriaData.sln](../PROYECTO/1.2/AceriaData.sln) |
| 1.3 | [../PROYECTO/1.3/AceriaData.sln](../PROYECTO/1.3/AceriaData.sln) |
| 1.4 | [../PROYECTO/1.4/AceriaData.sln](../PROYECTO/1.4/AceriaData.sln) |
| 1.5 | [../PROYECTO/1.5/AceriaData.sln](../PROYECTO/1.5/AceriaData.sln) |
| 1.6 | [../PROYECTO/1.6/AceriaData.sln](../PROYECTO/1.6/AceriaData.sln) |
| 1.7 | [../PROYECTO/1.7/AceriaData.sln](../PROYECTO/1.7/AceriaData.sln) |
| 1.8 | [../PROYECTO/1.8/AceriaData.sln](../PROYECTO/1.8/AceriaData.sln) |
| 1.9 | [../PROYECTO/1.9/AceriaData.sln](../PROYECTO/1.9/AceriaData.sln) |
| 1.10 | [../PROYECTO/1.10/AceriaData.sln](../PROYECTO/1.10/AceriaData.sln) |
| 1.11 | [../PROYECTO/1.11/AceriaData.sln](../PROYECTO/1.11/AceriaData.sln) |
| 1.12 | [../PROYECTO/1.12/AceriaData.sln](../PROYECTO/1.12/AceriaData.sln) |

El código es acumulativo: 1.2 continúa 1.1, 1.3 continúa 1.2 y así sucesivamente hasta 1.12.

## Validación

La CI restaura y compila las doce soluciones locales, ejecuta los doce proyectos y realiza las comprobaciones E2E y de trazabilidad descritas en [../TRAZABILIDAD_M01.md](../TRAZABILIDAD_M01.md).
