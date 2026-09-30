# AceriaData - checkpoint 4.7

Tema: **Consultas ineficientes y traducción**.

EF Core 8 rechaza un filtro no traducible dentro de `Where`; la evaluación cliente se demuestra solo cuando la frontera se hace explícita con `AsEnumerable()`.

E2E esperado: `4.7 OK`.
