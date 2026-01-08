using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace Wallet.UnitTest.IntegrationTest;

public class EstadoApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public EstadoApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GetEstados_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();
            // Assuming CommonSettings has Estados
            context.Estado.AddRange(commonSettings.Estados);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync($"/{ApiVersion}/estado");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<EstadoResult>>(content, _jsonSettings);
        
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
