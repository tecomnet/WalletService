using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class EmpresaApiTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public EmpresaApiTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Test_Create_Empresa_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create 
        var request = new EmpresaRequest { Nombre = "Empresa Integration Test" };
        var response = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(request));

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
            $"Expected Created or OK. Got {response.StatusCode}. Content: {content}");

        var result = JsonConvert.DeserializeObject<EmpresaResult>(content, _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(request.Nombre, result.Nombre);
        Assert.NotNull(result.Id);
        _output.WriteLine($"Empresa Created: {result.Nombre} (ID: {result.Id})");
    }

    [Fact]
    public async Task Test_Get_Empresas_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create one first
        var request = new EmpresaRequest { Nombre = "Empresa List Test" };
        await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(request));

        // 3. Get List
        var response = await client.GetAsync($"/{ApiVersion}/empresa");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = JsonConvert.DeserializeObject<List<EmpresaResult>>(content, _jsonSettings);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, e => e.Nombre == "Empresa List Test");
    }

    [Fact]
    public async Task Test_Update_Empresa_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create 
        var createRequest = new EmpresaRequest { Nombre = "Empresa Update Test Original" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Update 
        var updateRequest = new EmpresaRequest { Nombre = "Empresa Update Test Updated" };
        var response = await client.PutAsync($"/{ApiVersion}/empresa/{createResult.Id}", CreateContent(updateRequest));
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<EmpresaResult>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.Equal(createResult.Id, result.Id);
        Assert.Equal("Empresa Update Test Updated", result.Nombre);
    }

    [Fact]
    public async Task Test_Get_Empresa_Productos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Setup Data (Empresa, Broker, Proveedor, Producto)
        // Empresa
        var empresaReq = new EmpresaRequest { Nombre = "Empresa With Products" };
        var empresaRes = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(empresaReq));
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(await empresaRes.Content.ReadAsStringAsync(), _jsonSettings);

        // Broker
        var brokerReq = new BrokerRequest { Nombre = "Broker For Products" };
        var brokerRes = await client.PostAsync($"/{ApiVersion}/broker", CreateContent(brokerReq));
        var broker =
            JsonConvert.DeserializeObject<BrokerResult>(await brokerRes.Content.ReadAsStringAsync(), _jsonSettings);

        // Proveedor
        var provReq = new ProveedorRequest { Nombre = "Proveedor For Products", BrokerId = broker.Id , UrlIcono = "https://icon.png" };
        var provRes = await client.PostAsync($"/{ApiVersion}/proveedor", CreateContent(provReq));
        var proveedor =
            JsonConvert.DeserializeObject<ProveedorResult>(await provRes.Content.ReadAsStringAsync(), _jsonSettings);

        // Producto
        var prodReq = new ProductoRequest
        {
            Nombre = "Producto Linked",
            Sku = "SKU-LINKED",
            Precio = 100,
            Categoria = CategoriaEnum.MOVILIDADEnum,
            UrlIcono = "http://icon.png"
        };
        var prodRes =
            await client.PostAsync($"/{ApiVersion}/proveedor/{proveedor.Id}/producto", CreateContent(prodReq));
        var producto =
            JsonConvert.DeserializeObject<ProductoResult>(await prodRes.Content.ReadAsStringAsync(), _jsonSettings);

        // 3. Manually link Empresa and Producto in DB
        using (var context = CreateContext())
        {
            var dbEmpresa = await context.Empresa.FindAsync(empresa.Id);
            var dbProducto = await context.Producto.FindAsync(producto.Id);
            if (dbEmpresa != null && dbProducto != null)
            {
                dbEmpresa.Productos.Add(dbProducto);
                await context.SaveChangesAsync();
            }
        }

        // 4. Test Get Products by Empresa
        var response = await client.GetAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ProductoResult>>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Id == producto.Id);
    }

    [Fact]
    public async Task Test_Get_Empresa_Clientes_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Setup Data (Empresa)
        var empresaReq = new EmpresaRequest { Nombre = "Empresa With Clients" };
        var empresaRes = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(empresaReq));
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(await empresaRes.Content.ReadAsStringAsync(), _jsonSettings);

        // 3. Setup Clients linked to Empresa
        // NOTE: Cannot use API to create client easily without full setup (Usuario, etc).
        // Using DbContext directly to seed clients for this test to be faster and more focused.
        using (var context = CreateContext())
        {
            var dbEmpresa = await context.Empresa.FindAsync(empresa.Id);
            if (dbEmpresa != null)
            {
                var usuario = new Usuario(
                    codigoPais: "+52",
                    telefono: $"9{new Random().Next(100000000, 999999999)}",
                    correoElectronico: $"clientTest{Guid.NewGuid()}@test.com",
                    contrasena: "Password123!",
                    estatus: EstatusRegistroEnum.RegistroCompletado,
                    creationUser: Guid.NewGuid(),
                    testCase: "IntegrationTest");

                var clienteEntity = new Cliente(
                    usuario: usuario,
                    empresa: dbEmpresa,
                    creationUser: Guid.NewGuid(),
                    testCase: "IntegrationTest");

                clienteEntity.AgregarDatosPersonales(
                    nombre: "Juan",
                    primerApellido: "Perez",
                    segundoApellido: "Lopez",
                    fechaNacimiento: new DateOnly(1990, 1, 1),
                    genero: Genero.Masculino,
                    modificationUser: Guid.NewGuid());

                context.Usuario.Add(usuario);
                context.Cliente.Add(clienteEntity);
                await context.SaveChangesAsync();
            }
        }

        // 4. Test Get Clients by Empresa
        var response = await client.GetAsync($"/{ApiVersion}/empresa/{empresa.Id}/clientes");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ClienteResult>>(content, _jsonSettings);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, c => c.Nombre == "Juan");
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
