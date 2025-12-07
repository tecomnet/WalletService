using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class UserApiTest : DatabaseTestFixture
{
    // Api URI
    private const string API_URI = "users";

    // Api version
    private const string API_VERSION = "0.1";


    public UserApiTest()
    {
        Factory.UseTestAuth = false;
        SetupDataAsync(setupDataAction: async context =>
        {
            // Create data
            var commonSettings = new CommonSettings();
            // Add data
            //await context.AddRangeAsync(commonSettings.Clientes);
            // Save changes
            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Get_Unauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Act
        var response = await client.GetAsync(
            requestUri: $"{API_VERSION}/usuario/1");
        // Assert
        Assert.Equal(expected: HttpStatusCode.Unauthorized, actual: response.StatusCode);
    }

    [Fact]
    public async Task Get_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}");

        // Assert 
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: user.Id, actual: result.Id);
        Assert.Equal(expected: user.CorreoElectronico, actual: result.CorreoElectronico);
    }

    [Fact]
    public async Task Put_Telefono_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new TelefonoUpdateRequest
        {
            CodigoPais = "+52",
            Telefono = $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}"
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Telefono, actual: result.Telefono);
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