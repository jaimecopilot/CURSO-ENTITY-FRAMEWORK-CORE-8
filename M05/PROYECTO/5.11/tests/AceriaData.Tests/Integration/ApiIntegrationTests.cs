using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AceriaData.Tests.Integration;

[Collection(SqlServerCollection.Name)]
public sealed class ApiIntegrationTests
{
    private readonly SqlServerDatabaseFixture _fixture;

    public ApiIntegrationTests(SqlServerDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task WebApplicationFactory_UsaApiRealYSqlServerDePruebas()
    {
        await _fixture.ResetAsync();

        await using var factory = new AceriaApiFactory(_fixture.ConnectionString);
        using var client = factory.CreateClient();

        var create = await client.PostAsJsonAsync("/api/ordenes", new
        {
            NumeroOrden = "OF-HTTP-511",
            Cliente = "Cliente HTTP"
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var response = await client.GetAsync("/api/ordenes/count");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CountResponse>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Total);
    }

    private sealed record CountResponse(int Total);
}
