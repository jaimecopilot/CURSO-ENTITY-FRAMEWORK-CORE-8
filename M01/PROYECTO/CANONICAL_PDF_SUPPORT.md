# Soporte pedagógico canónico M01

Estos archivos se generan desde el contrato editorial cerrado de M01 y sirven para validar PDF → CÓDIGO → E2E.

- La práctica canónica congelada para esta rama está en `M01/PRACTICA/M01_PRACTICA_CANONICA.md`.
- Cada checkpoint `M01/PROYECTO/1.x` contiene `CanonicalPdfBlocks.cs` con **todos los bloques C# del punto, literales y comentados**.
- Los bloques incluyen fragmentos intermedios y reto. No se pretende descomentarlos todos a la vez: cada marcador identifica el paso o reto al que pertenece.
- El código activo del checkpoint representa el estado acumulativo final del punto; los E2E activarán variantes aisladas en copias temporales cuando corresponda.
- En 1.11, los bloques SQLite/PostgreSQL permanecen preservados por trazabilidad, pero están marcados en la práctica como **NO EJECUTAR EN M01** y no forman parte del código activo.
