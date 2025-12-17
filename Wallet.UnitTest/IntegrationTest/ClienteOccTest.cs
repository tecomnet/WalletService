using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
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
        Context.Empresa.AddRange(commonSettings.Empresas);
        Context.Estado.AddRange(commonSettings.Estados);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 1. Create Authenticated User
        var (userModel, token) = await CreateAuthenticatedUserAsync();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Fetch user into current context to link entities
        var user = await Context.Usuario.FirstAsync(u => u.Id == userModel.Id);

        var empresa = await Context.Empresa.FirstAsync();
        var estado = await Context.Estado.FirstAsync();

        var cliente = new Cliente(user, empresa, user.CreationUser);
        cliente.AgregarDatosPersonales(
            nombre: "Initial",
            primerApellido: "User",
            segundoApellido: "Test",
            fechaNacimiento: new DateOnly(1990, 1, 1),
            genero: Genero.Masculino,
            modificationUser: user.CreationUser);
        cliente.AgregarEstado(estado, user.CreationUser);

        var checkton = new ValidacionCheckton(TipoCheckton.Curp, true, user.CreationUser);
        cliente.AgregarValidacionCheckton(checkton, user.CreationUser);
        cliente.AgregarCurp("AAAA000000HDFXXX00", user.CreationUser);

        var verif = new Verificacion2FA("sid123", DateTime.Now.AddMinutes(10), Tipo2FA.Sms, user.CreationUser);
        user.AgregarVerificacion2Fa(verif, user.CreationUser);
        verif.MarcarComoVerificado("1234", user.CreationUser);

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        // Detach to ensure we are testing real API behavior
        Context.ChangeTracker.Clear();

        // 2. Get Cliente to obtain ConcurrencyToken
        var responseGet = await client.GetAsync($"/{version}/cliente/{cliente.Id}");
        Assert.Equal(HttpStatusCode.OK, responseGet.StatusCode);
        var clienteResult = JsonConvert.DeserializeObject<ClienteResult>(await responseGet.Content.ReadAsStringAsync());
        Assert.NotNull(clienteResult);
        Assert.NotNull(clienteResult.ConcurrencyToken);
        var originalToken = clienteResult.ConcurrencyToken;

        // 3. Update with VALID token
        var updateRequest = new DatosClienteUpdateRequest
        {
            Nombre = "Updated Name",
            ApellidoPaterno = "Updated Last",
            ApellidoMaterno = "Updated Mat",
            FechaNacimiento = new DateTime(1991, 2, 2),
            NombreEstado = estado.Nombre,
            Genero = (int)GeneroEnum.FemeninoEnum, // Changed
            ConcurrencyToken = originalToken
        };

        var responsePut1 = await client.PutAsync($"/{version}/cliente/{cliente.Id}",
            new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json"));

        if (responsePut1.StatusCode != HttpStatusCode.OK)
        {
            _output.WriteLine("Update failed: " + await responsePut1.Content.ReadAsStringAsync());
        }

        Assert.Equal(HttpStatusCode.OK, responsePut1.StatusCode);

        var resultPut1 = JsonConvert.DeserializeObject<ClienteResult>(await responsePut1.Content.ReadAsStringAsync());
        Assert.NotEqual(originalToken, resultPut1.ConcurrencyToken); // Token should change

        // 4. Update with STALE token (Failure Expected)
        var updateRequestStale = new DatosClienteUpdateRequest
        {
            Nombre = "Conflict Name",
            ApellidoPaterno = "Conflict Last",
            ApellidoMaterno = "Conflict Mat",
            FechaNacimiento = new DateTime(1991, 2, 2),
            NombreEstado = estado.Nombre,
            Genero = (int)GeneroEnum.FemeninoEnum,
            ConcurrencyToken = originalToken // Using OLD token
        };

        var responsePut2 = await client.PutAsync($"/{version}/cliente/{cliente.Id}",
            new StringContent(JsonConvert.SerializeObject(updateRequestStale), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Conflict, responsePut2.StatusCode);
    }
}
