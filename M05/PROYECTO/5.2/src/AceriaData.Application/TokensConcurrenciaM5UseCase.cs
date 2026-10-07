using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TokensConcurrenciaM5UseCase
{
    private readonly ITokensConcurrenciaM5Repositorio _repositorio;

    public TokensConcurrenciaM5UseCase(ITokensConcurrenciaM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public void Ejecutar()
    {
        Console.WriteLine("=== 5.2 CONFIGURACIÓN DE TOKENS DE CONCURRENCIA ===");

        var rowVersion = _repositorio.DemostrarRowVersion();
        Console.WriteLine("\n--- rowversion de SQL Server ---");
        Console.WriteLine($"RowVersion A inicial: {rowVersion.RowVersionAInicial}");
        Console.WriteLine($"RowVersion B inicial: {rowVersion.RowVersionBInicial}");
        Console.WriteLine($"RowVersion A después de guardar: {rowVersion.RowVersionADespues}");
        Console.WriteLine($"¿Conflicto detectado con rowversion?: {rowVersion.ConflictoDetectado}");
        MostrarSql(rowVersion.ComandosSql);

        if (!rowVersion.ConflictoDetectado)
            throw new InvalidOperationException("rowversion no detectó el conflicto de concurrencia.");

        var token = _repositorio.DemostrarTokenDePropiedad();
        Console.WriteLine("\n--- Token de concurrencia sobre EstadoDetalle ---");
        Console.WriteLine($"Estado A inicial: {token.EstadoAInicial}");
        Console.WriteLine($"Estado B inicial: {token.EstadoBInicial}");
        Console.WriteLine($"Estado A después de guardar: {token.EstadoADespues}");
        Console.WriteLine($"¿Conflicto detectado con token de propiedad?: {token.ConflictoDetectado}");
        MostrarSql(token.ComandosSql);

        if (!token.ConflictoDetectado)
            throw new InvalidOperationException("El token de propiedad no detectó el conflicto.");

        var indice = _repositorio.ComprobarIndiceRowVersion();
        Console.WriteLine("\n--- Índices ---");
        Console.WriteLine($"¿SQL Server creó automáticamente un índice sobre RowVersion?: {indice.ExisteIndiceRowVersion}");

        if (indice.ExisteIndiceRowVersion)
            throw new InvalidOperationException("No se esperaba un índice RowVersion creado automáticamente.");

        Console.WriteLine(
            "\nConclusión: IsRowVersion configura un token generado por SQL Server; " +
            "IsConcurrencyToken puede proteger una propiedad normal. Un token de concurrencia " +
            "no crea por sí mismo un índice.");
    }

    private static void MostrarSql(IReadOnlyList<string> comandos)
    {
        Console.WriteLine("SQL observado:");
        foreach (var comando in comandos)
        {
            Console.WriteLine("---");
            Console.WriteLine(comando);
        }
    }

    /*
    // RETO M05 5.2 - TOKEN ORIGINAL EN PREDICADO SQL
    public void EjecutarRetoPredicadoConcurrencia()
    {
        var rowVersion = _repositorio.DemostrarRowVersion();
        var tokenPropiedad = _repositorio.DemostrarTokenDePropiedad();

        static bool TokenApareceEnWhere(string sql, string token)
        {
            var where = sql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);
            return where >= 0 &&
                   sql[where..].Contains(token, StringComparison.OrdinalIgnoreCase);
        }

        var updateRowVersion = rowVersion.ComandosSql.FirstOrDefault(
            sql => sql.Contains("UPDATE [OrdenesFabricacion]", StringComparison.OrdinalIgnoreCase) &&
                   TokenApareceEnWhere(sql, "[RowVersion]"));

        var updateTokenPropiedad = tokenPropiedad.ComandosSql.FirstOrDefault(
            sql => sql.Contains("UPDATE [DetallesOrden]", StringComparison.OrdinalIgnoreCase) &&
                   TokenApareceEnWhere(sql, "[EstadoDetalle]"));

        if (updateRowVersion is null || updateTokenPropiedad is null)
            throw new InvalidOperationException(
                "Reto 5.2: no se localizaron los tokens originales en los predicados UPDATE.");

        Console.WriteLine(
            "Reto 5.2 OK | RowVersion y EstadoDetalle aparecen en el WHERE de sus UPDATE de concurrencia");
    }
    */

}
