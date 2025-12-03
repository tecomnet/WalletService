using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ConsentimientoUsuarioApiTest : DatabaseTestFixture
{
    // Api URI
    private const string API_VERSION = "0.1";

    public ConsentimientoUsuarioApiTest()
    {
        Factory.UseTestAuth = false;
        SetupDataAsync(setupDataAction: async context =>
        {
            // Create data
            var commonSettings = new CommonSettings();
            // Add data if needed
            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Post_Consentimiento_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new ConsentimientoUsuarioRequest
        {
            TipoDocumento = TipoDocumentoConsentimientoEnum.Terminos,
            Version = "v1.0"
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/consentimiento", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ConsentimientoUsuarioResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Version, actual: result.Version);
        Assert.Equal(expected: request.TipoDocumento, actual: result.TipoDocumento);
        Assert.Equal(expected: user.Id, actual: result.IdUsuario);
    }

    [Fact]
    public async Task Get_Consentimientos_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Post a consent first
        var request = new ConsentimientoUsuarioRequest
        {
            TipoDocumento = TipoDocumentoConsentimientoEnum.Privacidad,
            Version = "v2.0"
        };
        var content = CreateContent(body: request);
        await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/consentimiento", content: content);

        // Act
        var response = await client.GetAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/consentimiento");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<ConsentimientoUsuarioResult>>(value: responseContentString);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var privacidad = result.FirstOrDefault(c => c.TipoDocumento == TipoDocumentoConsentimientoEnum.Privacidad);
        Assert.NotNull(privacidad);
        Assert.Equal(expected: "v2.0", actual: privacidad.Version);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body);
        return new StringContent(
            content: json,
            encoding: Encoding.UTF8,
            mediaType: "application/json");
    }
}
