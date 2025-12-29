using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ServicioFavoritoApiTest : DatabaseTestFixture
{
    private const string API_URI = "servicioFavorito";
    private const string PROVEEDOR_API_URI = "proveedor";
    private const string CLIENTE_API_URI = "cliente";
    private const string API_VERSION = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    protected ServiceDbContext Context;

    public ServicioFavoritoApiTest()
    {
        Context = CreateContext();
        SetupDataAsync(setupDataAction: async context =>
        {
            if (!await context.Empresa.AnyAsync(e => e.Nombre == "Tecomnet"))
            {
                var empresa = new Empresa(nombre: "Tecomnet", creationUser: Guid.NewGuid());
                await context.Empresa.AddAsync(entity: empresa);
            }

            if (!await context.Broker.AnyAsync(b => b.Nombre == "Broker Test"))
            {
                var broker = new Wallet.DOM.Modelos.Broker(nombre: "Broker Test", creationUser: Guid.NewGuid());
                await context.Broker.AddAsync(entity: broker);
            }

            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Post_ServicioFavorito_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client: client);
        var proveedor = await CreateProveedor(client: client);

        var request = new ServicioFavoritoRequest
        {
            ClienteId = cliente.Id,
            ProveedorId = proveedor.Id!.Value,
            Alias = "My Netflix",
            NumeroReferencia = "REF123"
        };
        var content = CreateContent(body: request);

        // Act
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ServicioFavoritoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Alias, actual: result.Alias);
        Assert.True(condition: result.Id > 0);
    }

    [Fact]
    public async Task Get_ServiciosFavoritos_ByCliente_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var cliente = await CreateCliente(client: client);
        var servicioFavorito = await CreateServicioFavorito(client: client, clienteId: cliente.Id.GetValueOrDefault());

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}/{CLIENTE_API_URI}/{cliente.Id}");

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail(message: $"Get failed with {response.StatusCode}: {error}");
        }

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ServicioFavoritoResult>>(
                value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Contains(collection: result, filter: s => s.Id == servicioFavorito.Id);
    }

    private async Task<ProveedorResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorRequest
        {
            Nombre = "Test Provider",
            UrlIcono = "https://example.com/icon.png",
            BrokerId = 1 // Assuming Broker 1 exists from seed
        };
        var response =
            await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}",
                content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private async Task<ClienteResult> CreateCliente(HttpClient client)
    {
        var usuario = new Usuario("+52", "5512345678", "juan@example.com", null,
            Wallet.DOM.Enums.EstatusRegistroEnum.RegistroCompletado,
            Guid.NewGuid());
        Context.Usuario.Add(usuario);
        await Context.SaveChangesAsync();

        var empresa = await Context.Empresa.FirstAsync(e => e.Nombre == "Tecomnet");

        var cliente = new Cliente(usuario, empresa, Guid.NewGuid());
        cliente.AgregarDatosPersonales("Juan", "Perez", "Lopez", new DateOnly(1990, 1, 1),
            Wallet.DOM.Enums.Genero.Masculino, Guid.NewGuid());
        // cliente.AgregarTipoPersona(Wallet.DOM.Enums.TipoPersona.Fisica, Guid.NewGuid()); // Removed? Cliente constructor doesn't set TipoPersona but method exists.
        // Wait, Cliente model requires TipoPersona for DocumentacionAdjunta but here we are just creating client.
        // Let's keep existing logic if possible but updating constructor as needed.
        // Step 919 shows Cliente constructor takes (Usuario, Empresa, CreationUser). I updated that.

        cliente.AgregarTipoPersona(Wallet.DOM.Enums.TipoPersona.Fisica, Guid.NewGuid());
        cliente.AgregarRfc("ABC1234567890", Guid.NewGuid());
        cliente.AgregarCurp("ABCD123456EFGHIJ01", Guid.NewGuid());
        cliente.AgregarFotoAWS("foto.jpg", Guid.NewGuid());
        // Direccion required? Model says Direccion? property.
        // Let's add address to be safe if tests depend on it.
        cliente.AgregarDireccion(new Direccion("MX", "Yucatan", Guid.NewGuid()), Guid.NewGuid());

        Context.Cliente.Add(cliente);
        await Context.SaveChangesAsync();

        return new ClienteResult
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            PrimerApellido = cliente.PrimerApellido,
            SegundoApellido = cliente.SegundoApellido,
            CorreoElectronico = usuario.CorreoElectronico,
            Telefono = usuario.Telefono
        };
    }

    private async Task<ServicioFavoritoResult> CreateServicioFavorito(HttpClient client, int clienteId)
    {
        var proveedor = await CreateProveedor(client: client);
        var request = new ServicioFavoritoRequest
        {
            ClienteId = clienteId,
            ProveedorId = proveedor.Id!.Value,
            Alias = "Test Favorite",
            NumeroReferencia = "REFTEST"
        };
        var response =
            await client.PostAsync(requestUri: $"{API_VERSION}/{API_URI}", content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode);
        return JsonConvert.DeserializeObject<ServicioFavoritoResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
