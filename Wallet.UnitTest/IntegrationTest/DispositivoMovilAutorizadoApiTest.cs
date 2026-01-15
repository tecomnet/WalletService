using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class DispositivoMovilAutorizadoApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public DispositivoMovilAutorizadoApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task RegistrarDispositivo_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int clienteId;

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();
            context.Empresa.AddRange(commonSettings.Empresas);
            context.Estado.AddRange(commonSettings.Estados);
            await context.SaveChangesAsync();

            var dbUser = await context.Usuario.FindAsync(user.Id);
            var empresa = await context.Empresa.FirstAsync(e => e.Nombre == "Tecomnet");
            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser!, empresa, Guid.NewGuid());
            cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Wallet.DOM.Enums.Genero.Masculino,
                modificationUser: Guid.NewGuid());
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;
        }

        var request = new DispositivoMovilAutorizadoRequest
        {
            IdDispositivo = "DEVICE123",
            Token = "TOKEN123",
            Nombre = "My iPhone",
            Caracteristicas = "iPhone 15 Pro, iOS 18"
        };

        // Act
        var response = await client.PostAsync($"/{ApiVersion}/dispositivoMovilAutorizado/{clienteId}",
            CreateContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DispositivoMovilAutorizadoResult>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.Equal("DEVICE123", result.IdDispositivo);

        using (var context = CreateContext())
        {
            // Dispositivo is linked to User, not directly to Client in DOM
            var dbUser = await context.Cliente.Where(c => c.Id == clienteId).Select(c => c.Usuario)
                .FirstOrDefaultAsync();
            Assert.NotNull(dbUser);

            var dispositivo =
                await context.DispositivoMovilAutorizado.FirstOrDefaultAsync(d =>
                    d.UsuarioId == dbUser.Id && d.IdDispositivo == "DEVICE123");
            Assert.NotNull(dispositivo);
        }
    }

    [Fact]
    public async Task GetDispositivoMovilAutorizado_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int clienteId;

        using (var context = CreateContext())
        {
            var commonSettings = new CommonSettings();
            context.Empresa.AddRange(commonSettings.Empresas);
            context.Estado.AddRange(commonSettings.Estados);
            await context.SaveChangesAsync();

            var dbUser = await context.Usuario.FindAsync(user.Id);
            var empresa = await context.Empresa.FirstAsync(e => e.Nombre == "Tecomnet");
            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(dbUser!, empresa, Guid.NewGuid());
            cliente.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Wallet.DOM.Enums.Genero.Masculino,
                modificationUser: Guid.NewGuid());
            context.Cliente.Add(cliente);
            await context.SaveChangesAsync();
            clienteId = cliente.Id;

            // Note: DispositivoMovilAutorizado is linked to UsuarioId, not directly to ClienteId (though Cliente has Usuario)
            // But the Controller takes IdCliente?
            // Let's check the controller logic again.
            // Controller: GetDispositivoMovilAutorizadoAsync(idCliente, ...)
            // Facade likely checks if device belongs to user of that client.
            // We need to link the device to the User ID of the Client.

            // var dispositivo = new Wallet.DOM.Modelos.GestionUsuario.DispositivoMovilAutorizado("TOKEN_EXISTING", "DEVICE_EXISTING", "Existing Phone", "Android", Guid.NewGuid());
            // How do we link to User? The constructor doesn't take UsuarioId?
            // Inspecting Code... The class has "public int UsuarioId { get; private set; }" but constructor doesn't set it?
            // Wait, DispositivoMovilAutorizado in DOM/GestionUsuario might be the wrong one?
            // Or maybe it's added to Usuario? "context.Usuario.Add(user); user.Dispositivos.Add(d)"?
            // Let's assume for now we can add it to context, but we need to set the relationship.
            // If the constructor doesn't set it, maybe it's set via navigation property or we need to add to the User's collection.

            // Re-reading DispositivoMovilAutorizado code... It doesn't seem to set UsuarioId in constructor!
            // It might be intended to be added to a User's collection.

            // dbUser.AgregarDispositivo(dispositivo); // Assuming method exists or we add to collection
            // If no method, try adding to collection directly if exposed.
            // Or maybe we need to set it via property if protected setter allows? No, private set.
            // Let's try adding to context directly and see if EF resolves it if we don't set it? No, it needs foreign key.
            // Inspecting Usuario class would be useful, but let's try assuming standard collection pattern.

            // For now, let's look at how Facade saves it.
            // Facade.GuardarDispositivoAutorizadoAsync -> gets User, adds device to User?

            // To proceed with TEST, we will use the POST endpoint to create it implies logic is covered there.
            // But for GET test, we need to seed it.
            // If I can't easily seed it because of missing constructor param, maybe I should rely on POST test to create it?
            // But separate tests are better.

            // Let's try simply adding it to the DbSet. EF might complain about missing FK.
            // Let's assume we can't easily seed it without a helper method on User.
            // Let's change the GET test to first POST then GET.
        }

        // Setup via POST for this test since we don't know how to seed correctly yet without User methods
        var request = new DispositivoMovilAutorizadoRequest
        {
            IdDispositivo = "DEVICE_EXISTING",
            Token = "TOKEN_EXISTING",
            Nombre = "Existing Phone",
            Caracteristicas = "Android"
        };
        await client.PostAsync($"/{ApiVersion}/dispositivoMovilAutorizado/{clienteId}", CreateContent(request));

        // Act
        var response =
            await client.GetAsync(
                $"/{ApiVersion}/dispositivoMovilAutorizado/{clienteId}?idDispositivo=DEVICE_EXISTING&token=TOKEN_EXISTING");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var esAutorizado = JsonConvert.DeserializeObject<bool?>(content);

        Assert.True(esAutorizado);
    }
}
