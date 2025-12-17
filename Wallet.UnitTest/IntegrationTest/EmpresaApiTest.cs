using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionUsuario;
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
    public async Task Test_Get_Empresa_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create an Empresa
        var createRequest = new EmpresaRequest { Nombre = "Empresa Get By Id Test" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Get Empresa by Id
        var getResponse = await client.GetAsync($"/{ApiVersion}/empresa/{createResult.Id}");
        var getContent = await getResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResult = JsonConvert.DeserializeObject<EmpresaResult>(getContent, _jsonSettings);

        Assert.NotNull(getResult);
        Assert.Equal(createResult.Id, getResult.Id);
        Assert.Equal(createResult.Nombre, getResult.Nombre);
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
        // 3. Update 
        // 3. Update 
        var updateRequest = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Update Test Updated",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
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
        var provReq = new ProveedorRequest
            { Nombre = "Proveedor For Products", BrokerId = broker.Id, UrlIcono = "https://icon.png" };
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

    [Fact]
    public async Task Test_Asignar_Y_Desasignar_Productos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var suffix = Guid.NewGuid().ToString().Substring(0, 8);

        // 2. Setup Data
        // Empresa
        var empresaReq = new EmpresaRequest { Nombre = $"Empresa Assign {suffix}" };
        var empresaRes = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(empresaReq));
        Assert.Equal(HttpStatusCode.OK, empresaRes.StatusCode);
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(await empresaRes.Content.ReadAsStringAsync(), _jsonSettings);

        // Broker & Proveedor
        var brokerRes = await client.PostAsync($"/{ApiVersion}/broker",
            CreateContent(new BrokerRequest { Nombre = $"Broker Assign {suffix}" }));
        Assert.Equal(HttpStatusCode.Created, brokerRes.StatusCode);
        var broker =
            JsonConvert.DeserializeObject<BrokerResult>(await brokerRes.Content.ReadAsStringAsync(), _jsonSettings);

        var provReq = new ProveedorRequest
            { Nombre = $"Prov Assign {suffix}", BrokerId = broker.Id, UrlIcono = "http://icon.com" };
        var provRes = await client.PostAsync($"/{ApiVersion}/proveedor", CreateContent(provReq));
        Assert.Equal(HttpStatusCode.Created, provRes.StatusCode); // Verify status code (likely 200 or 201)
        var proveedor =
            JsonConvert.DeserializeObject<ProveedorResult>(await provRes.Content.ReadAsStringAsync(), _jsonSettings);

        // Products
        var prod1Req = new ProductoRequest
        {
            Nombre = $"P1 {suffix}", Sku = $"SKU1-{suffix}", Precio = 10, Categoria = CategoriaEnum.OTROSEnum,
            UrlIcono = "http://icon.com"
        };
        var prod1Res =
            await client.PostAsync($"/{ApiVersion}/proveedor/{proveedor.Id}/producto", CreateContent(prod1Req));
        Assert.True(prod1Res.IsSuccessStatusCode, $"Product 1 creation failed: {prod1Res.StatusCode}");
        var prod1 = JsonConvert.DeserializeObject<ProductoResult>(await prod1Res.Content.ReadAsStringAsync(),
            _jsonSettings);

        var prod2Req = new ProductoRequest
        {
            Nombre = $"P2 {suffix}", Sku = $"SKU2-{suffix}", Precio = 20, Categoria = CategoriaEnum.OTROSEnum,
            UrlIcono = "http://icon.com"
        };
        var prod2Res =
            await client.PostAsync($"/{ApiVersion}/proveedor/{proveedor.Id}/producto", CreateContent(prod2Req));
        Assert.True(prod2Res.IsSuccessStatusCode, $"Product 2 creation failed: {prod2Res.StatusCode}");
        var prod2 = JsonConvert.DeserializeObject<ProductoResult>(await prod2Res.Content.ReadAsStringAsync(),
            _jsonSettings);


        // 3. Assign Products
        Assert.NotNull(prod1);
        Assert.NotNull(prod1.Id);
        Assert.NotNull(prod2);
        Assert.NotNull(prod2.Id);
        var assignIds = new AsignarProductosRequest { ProductoIds = new List<int?> { prod1.Id.Value, prod2.Id.Value } };
        var assignRes =
            await client.PostAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos", CreateContent(assignIds));
        Assert.Equal(HttpStatusCode.OK, assignRes.StatusCode);

        // 4. Verify Assignment
        var getRes = await client.GetAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var products =
            JsonConvert.DeserializeObject<List<ProductoResult>>(await getRes.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.Equal(2, products.Count);
        Assert.Contains(products, p => p.Id == prod1.Id);
        Assert.Contains(products, p => p.Id == prod2.Id);

        // 5. Test Idempotency (Duplicated Assignment)
        var assignResDuplicate =
            await client.PostAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos", CreateContent(assignIds));
        Assert.Equal(HttpStatusCode.OK, assignResDuplicate.StatusCode);

        var getResDup = await client.GetAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var productsDup =
            JsonConvert.DeserializeObject<List<ProductoResult>>(await getResDup.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.Equal(2, productsDup.Count); // Should still be 2

        // 6. Remove One Product
        var removeIds = new AsignarProductosRequest { ProductoIds = new List<int?> { prod1.Id.Value } };
        var removeRes = await client.PostAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos/remover",
            CreateContent(removeIds));
        Assert.Equal(HttpStatusCode.OK, removeRes.StatusCode);

        // 7. Verify Removal
        var getResFinal = await client.GetAsync($"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var productsFinal =
            JsonConvert.DeserializeObject<List<ProductoResult>>(await getResFinal.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.Single(productsFinal);
        Assert.Contains(productsFinal, p => p.Id == prod2.Id); // Only P2 remains
        Assert.DoesNotContain(productsFinal, p => p.Id == prod1.Id);
    }

    [Fact]
    public async Task Test_Update_Empresa_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Create 
        var createRequest = new EmpresaRequest { Nombre = "Empresa Conflict Test" };
        var createResponse = await client.PostAsync($"/{ApiVersion}/empresa", CreateContent(createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(await createResponse.Content.ReadAsStringAsync(),
                _jsonSettings);

        // 3. Update 1 (Success)
        var updateRequest1 = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Conflict Test Updated 1",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var response1 =
            await client.PutAsync($"/{ApiVersion}/empresa/{createResult.Id}", CreateContent(updateRequest1));
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        // 4. Update 2 (Stale Token - Should Fail)
        var updateRequest2 = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Conflict Test Updated 2",
            ConcurrencyToken = createResult.ConcurrencyToken // Stale token
        };
        var response2 =
            await client.PutAsync($"/{ApiVersion}/empresa/{createResult.Id}", CreateContent(updateRequest2));

        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
