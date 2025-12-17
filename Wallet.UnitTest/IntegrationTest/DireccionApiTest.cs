using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class DireccionApiTest : DatabaseTestFixture
{
    // Api version
    private const string API_VERSION = "0.1";

    [Fact]
    public async Task Get_Direccion_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Create Cliente and Direccion
        await using var context = CreateContext();

        // Attach user to context to avoid tracking issues if needed, or just use ID
        var userFromDb = await context.Usuario.FindAsync(user.Id);

        var empresa = new Empresa(nombre: "Empresa Test", creationUser: Guid.NewGuid());
        await context.Empresa.AddAsync(entity: empresa);
        await context.SaveChangesAsync();

        var cliente = new Cliente(usuario: userFromDb!, empresa: empresa, creationUser: Guid.NewGuid());
        cliente.AgregarDatosPersonales(
            nombre: "Juan",
            primerApellido: "Perez",
            segundoApellido: "Lopez",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1),
            genero: Wallet.DOM.Enums.Genero.Masculino,
            modificationUser: Guid.NewGuid());

        var direccion = new Direccion(pais: "Mexico", estado: "CDMX", creationUser: Guid.NewGuid());
        direccion.ActualizarDireccion(
            codigoPostal: "12345",
            municipio: "Benito Juarez",
            colonia: "Del Valle",
            calle: "Av Universidad",
            numeroExterior: "100",
            numeroInterior: "1",
            referencia: "Frente al parque",
            modificationUser: Guid.NewGuid());

        cliente.AgregarDireccion(direccion: direccion, creationUser: Guid.NewGuid());

        await context.Cliente.AddAsync(entity: cliente);
        await context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/direccion/{cliente.Id}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DireccionResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: direccion.CodigoPostal, actual: result.CodigoPostal);
        Assert.Equal(expected: direccion.Calle, actual: result.Calle);
    }
}
