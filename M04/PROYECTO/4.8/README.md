# AceriaData - checkpoint 4.8

Tema: **Split Queries: cuándo y cómo usarlas**.

Se comparan `AsSingleQuery()` y `AsSplitQuery()` sobre el mismo grafo y se cuentan los comandos SQL ejecutados. La coherencia entre varias consultas se tratará como una cuestión de aislamiento/transacción, no como la afirmación incorrecta de que EF crea una transacción independiente por subconsulta.

E2E esperado: `4.8 OK`.
