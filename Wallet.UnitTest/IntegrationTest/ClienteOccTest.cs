using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class ClienteOccTest : DatabaseTestFixture, IDisposable
{
    protected ServiceDbContext Context;
    private readonly ITestOutputHelper _output;

    public ClienteOccTest(ITestOutputHelper output)
    {
        _output = output;
        Context = CreateContext();
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task UpdateCliente_ShouldSucceed_And_CheckOCC()
    {
        // 0. Seed Company and State needed for registration
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(entities: commonSettings.Empresas);
        Context.Estado.AddRange(entities: commonSettings.Estados);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 1. Create Authenticated User
        var (userModel, token) = await CreateAuthenticatedUserAsync();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Fetch user into current context to link entities
        var user = await Context.Usuario.FirstAsync(predicate: u => u.Id == userModel.Id);

        var empresa = await Context.Empresa.FirstAsync();
        var estado = await Context.Estado.FirstAsync();

        var cliente = new Cliente(usuario: user, empresa: empresa, creationUser: user.CreationUser);
        cliente.AgregarDatosPersonales(
            nombre: "Initial",
            primerApellido: "User",
            segundoApellido: "Test",
            fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1),
            genero: Genero.Masculino,
            modificationUser: user.CreationUser);
        cliente.AgregarEstado(estado: estado, modificationUser: user.CreationUser);

        var checkton = new ValidacionCheckton(tipoCheckton: TipoCheckton.Curp, resultado: true,
            creationUser: user.CreationUser);
        cliente.AgregarValidacionCheckton(validacion: checkton, modificationUser: user.CreationUser);
        cliente.AgregarCurp(curp: "AAAA000000HDFXXX00", modificationUser: user.CreationUser);

        var verif = new Verificacion2FA(twilioSid: "sid123", fechaVencimiento: DateTime.Now.AddMinutes(value: 10),
            tipo: Tipo2FA.Sms, creationUser: user.CreationUser);
        user.AgregarVerificacion2Fa(verificacion: verif, modificationUser: user.CreationUser);
        verif.MarcarComoVerificado(codigo: "1234", modificationUser: user.CreationUser);

        Context.Cliente.Add(entity: cliente);
        await Context.SaveChangesAsync();

        // Detach to ensure we are testing real API behavior
        Context.ChangeTracker.Clear();

        // 2. Get Cliente to obtain ConcurrencyToken
        var responseGet = await client.GetAsync(requestUri: $"/{version}/cliente/{cliente.Id}");
        Assert.Equal(expected: HttpStatusCode.OK, actual: responseGet.StatusCode);
        var clienteResult =
            JsonConvert.DeserializeObject<ClienteResult>(value: await responseGet.Content.ReadAsStringAsync());
        Assert.NotNull(@object: clienteResult);
        Assert.NotNull(@object: clienteResult.ConcurrencyToken);
        var originalToken = clienteResult.ConcurrencyToken;

        // 3. Update with VALID token
        var updateRequest = new DatosClienteUpdateRequest
        {
            Nombre = "Updated Name",
            ApellidoPaterno = "Updated Last",
            ApellidoMaterno = "Updated Mat",
            FechaNacimiento = new DateTime(year: 1991, month: 2, day: 2),
            NombreEstado = estado.Nombre,
            Genero = GeneroEnum.FemeninoEnum, // Changed
            ConcurrencyToken = originalToken
        };

        var responsePut1 = await client.PutAsync(requestUri: $"/{version}/cliente/{cliente.Id}",
            content: new StringContent(content: JsonConvert.SerializeObject(value: updateRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        if (responsePut1.StatusCode != HttpStatusCode.OK)
        {
            _output.WriteLine(message: "Update failed: " + await responsePut1.Content.ReadAsStringAsync());
        }

        Assert.Equal(expected: HttpStatusCode.OK, actual: responsePut1.StatusCode);

        var resultPut1 =
            JsonConvert.DeserializeObject<ClienteResult>(value: await responsePut1.Content.ReadAsStringAsync());
        Assert.NotEqual(expected: originalToken, actual: resultPut1.ConcurrencyToken); // Token should change

        // 4. Update with STALE token (Failure Expected)
        var updateRequestStale = new DatosClienteUpdateRequest
        {
            Nombre = "Conflict Name",
            ApellidoPaterno = "Conflict Last",
            ApellidoMaterno = "Conflict Mat",
            FechaNacimiento = new DateTime(year: 1991, month: 2, day: 2),
            NombreEstado = estado.Nombre,
            Genero = GeneroEnum.FemeninoEnum,
            ConcurrencyToken = originalToken // Using OLD token
        };

        var responsePut2 = await client.PutAsync(requestUri: $"/{version}/cliente/{cliente.Id}",
            content: new StringContent(content: JsonConvert.SerializeObject(value: updateRequestStale),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: responsePut2.StatusCode);
    }
}
