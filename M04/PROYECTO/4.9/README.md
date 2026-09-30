# AceriaData - checkpoint 4.9

Tema: **Compiled Queries**.

La prueba valida equivalencia funcional y reutilización. No convierte una micro-medición en una promesa de mejora: EF Core ya cachea consultas por forma y `EF.CompileQuery` evita parte del trabajo del pipeline de EF, no el plan de ejecución de SQL Server.

E2E esperado: `4.9 OK`.
