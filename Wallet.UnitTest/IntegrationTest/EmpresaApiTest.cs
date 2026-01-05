using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
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
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create 
        var request = new EmpresaRequest { Nombre = "Empresa Integration Test" };
        var response =
            await client.PostAsync(requestUri: $"/{ApiVersion}/empresa", content: CreateContent(body: request));

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(condition: response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
            userMessage: $"Expected Created or OK. Got {response.StatusCode}. Content: {content}");

        var result = JsonConvert.DeserializeObject<EmpresaResult>(value: content, settings: _jsonSettings);
        Assert.NotNull(@object: result);
        Assert.Equal(expected: request.Nombre, actual: result.Nombre);
        Assert.NotNull(value: result.Id);
        _output.WriteLine(message: $"Empresa Created: {result.Nombre} (ID: {result.Id})");
    }

    [Fact]
    public async Task Test_Get_Empresas_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create one first
        var request = new EmpresaRequest { Nombre = "Empresa List Test" };
        await client.PostAsync(requestUri: $"/{ApiVersion}/empresa", content: CreateContent(body: request));

        // 3. Get List
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

        var result = JsonConvert.DeserializeObject<List<EmpresaResult>>(value: content, settings: _jsonSettings);
        Assert.NotNull(@object: result);
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: e => e.Nombre == "Empresa List Test");
    }

    [Fact]
    public async Task Test_Get_Empresa_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create an Empresa
        var createRequest = new EmpresaRequest { Nombre = "Empresa Get By Id Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Get Empresa by Id
        var getResponse = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}");
        var getContent = await getResponse.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: getResponse.StatusCode);
        var getResult = JsonConvert.DeserializeObject<EmpresaResult>(value: getContent, settings: _jsonSettings);

        Assert.NotNull(@object: getResult);
        Assert.Equal(expected: createResult.Id, actual: getResult.Id);
        Assert.Equal(expected: createResult.Nombre, actual: getResult.Nombre);
    }

    [Fact]
    public async Task Test_Update_Empresa_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create 
        var createRequest = new EmpresaRequest { Nombre = "Empresa Update Test Original" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Update 
        // 3. Update 
        // 3. Update 
        var updateRequest = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Update Test Updated",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var response = await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}",
            content: CreateContent(body: updateRequest));
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<EmpresaResult>(value: content, settings: _jsonSettings);

        Assert.NotNull(@object: result);
        Assert.Equal(expected: createResult.Id, actual: result.Id);
        Assert.Equal(expected: "Empresa Update Test Updated", actual: result.Nombre);
    }

    [Fact]
    public async Task Test_Get_Empresa_Productos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Setup Data (Empresa, Broker, Proveedor, Producto)
        // Empresa
        var empresaReq = new EmpresaRequest { Nombre = "Empresa With Products" };
        var empresaRes = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: empresaReq));
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await empresaRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // Broker
        var brokerReq = new BrokerRequest { Nombre = "Broker For Products" };
        var brokerRes =
            await client.PostAsync(requestUri: $"/{ApiVersion}/broker", content: CreateContent(body: brokerReq));
        var broker =
            JsonConvert.DeserializeObject<BrokerResult>(value: await brokerRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // Proveedor
        var provReq = new ProveedorRequest
            { Nombre = "Proveedor For Products", BrokerId = broker.Id, UrlIcono = "https://icon.png" };
        var provRes =
            await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor", content: CreateContent(body: provReq));
        var proveedor =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await provRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // Producto
        var prodReq = new ProductoRequest
        {
            Nombre = "Producto Linked",
            Sku = "SKU-LINKED",
            Precio = 100,
            Categoria = CategoriaEnum.MOVILIDAD,
            UrlIcono = "http://icon.png"
        };
        var prodRes =
            await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor/{proveedor.Id}/producto",
                content: CreateContent(body: prodReq));
        var producto =
            JsonConvert.DeserializeObject<ProductoResult>(value: await prodRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Manually link Empresa and Producto in DB
        using (var context = CreateContext())
        {
            var dbEmpresa = await context.Empresa.FindAsync(keyValues: empresa.Id);
            var dbProducto = await context.Producto.FindAsync(keyValues: producto.Id);
            if (dbEmpresa != null && dbProducto != null)
            {
                dbEmpresa.Productos.Add(item: dbProducto);
                await context.SaveChangesAsync();
            }
        }

        // 4. Test Get Products by Empresa
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ProductoResult>>(value: content, settings: _jsonSettings);

        Assert.NotNull(@object: result);
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: p => p.Id == producto.Id);
    }

    [Fact]
    public async Task Test_Get_Empresa_Clientes_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Setup Data (Empresa)
        var empresaReq = new EmpresaRequest { Nombre = "Empresa With Clients" };
        var empresaRes = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: empresaReq));
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await empresaRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Setup Clients linked to Empresa
        // NOTE: Cannot use API to create client easily without full setup (Usuario, etc).
        // Using DbContext directly to seed clients for this test to be faster and more focused.
        using (var context = CreateContext())
        {
            var dbEmpresa = await context.Empresa.FindAsync(keyValues: empresa.Id);
            if (dbEmpresa != null)
            {
                var usuario = new Usuario(
                    codigoPais: "+52",
                    telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
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
                    fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1),
                    genero: Genero.Masculino,
                    modificationUser: Guid.NewGuid());

                context.Usuario.Add(entity: usuario);
                context.Cliente.Add(entity: clienteEntity);
                await context.SaveChangesAsync();
            }
        }

        // 4. Test Get Clients by Empresa
        var response = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/clientes");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<List<ClienteResult>>(value: content, settings: _jsonSettings);

        Assert.NotNull(@object: result);
        Assert.NotEmpty(collection: result);
        Assert.Contains(collection: result, filter: c => c.Nombre == "Juan");
    }

    [Fact]
    public async Task Test_Asignar_Y_Desasignar_Productos_Ok()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var suffix = Guid.NewGuid().ToString().Substring(startIndex: 0, length: 8);

        // 2. Setup Data
        // Empresa
        var empresaReq = new EmpresaRequest { Nombre = $"Empresa Assign {suffix}" };
        var empresaRes = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: empresaReq));
        Assert.Equal(expected: HttpStatusCode.OK, actual: empresaRes.StatusCode);
        var empresa =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await empresaRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // Broker & Proveedor
        var brokerRes = await client.PostAsync(requestUri: $"/{ApiVersion}/broker",
            content: CreateContent(body: new BrokerRequest { Nombre = $"Broker Assign {suffix}" }));
        Assert.Equal(expected: HttpStatusCode.Created, actual: brokerRes.StatusCode);
        var broker =
            JsonConvert.DeserializeObject<BrokerResult>(value: await brokerRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        var provReq = new ProveedorRequest
            { Nombre = $"Prov Assign {suffix}", BrokerId = broker.Id, UrlIcono = "http://icon.com" };
        var provRes =
            await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor", content: CreateContent(body: provReq));
        Assert.Equal(expected: HttpStatusCode.Created,
            actual: provRes.StatusCode); // Verify status code (likely 200 or 201)
        var proveedor =
            JsonConvert.DeserializeObject<ProveedorResult>(value: await provRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // Products
        var prod1Req = new ProductoRequest
        {
            Nombre = $"P1 {suffix}", Sku = $"SKU1-{suffix}", Precio = 10, Categoria = CategoriaEnum.OTROS,
            UrlIcono = "http://icon.com"
        };
        var prod1Res =
            await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor/{proveedor.Id}/producto",
                content: CreateContent(body: prod1Req));
        Assert.True(condition: prod1Res.IsSuccessStatusCode,
            userMessage: $"Product 1 creation failed: {prod1Res.StatusCode}");
        var prod1 = JsonConvert.DeserializeObject<ProductoResult>(value: await prod1Res.Content.ReadAsStringAsync(),
            settings: _jsonSettings);

        var prod2Req = new ProductoRequest
        {
            Nombre = $"P2 {suffix}", Sku = $"SKU2-{suffix}", Precio = 20, Categoria = CategoriaEnum.OTROS,
            UrlIcono = "http://icon.com"
        };
        var prod2Res =
            await client.PostAsync(requestUri: $"/{ApiVersion}/proveedor/{proveedor.Id}/producto",
                content: CreateContent(body: prod2Req));
        Assert.True(condition: prod2Res.IsSuccessStatusCode,
            userMessage: $"Product 2 creation failed: {prod2Res.StatusCode}");
        var prod2 = JsonConvert.DeserializeObject<ProductoResult>(value: await prod2Res.Content.ReadAsStringAsync(),
            settings: _jsonSettings);


        // 3. Assign Products
        Assert.NotNull(@object: prod1);
        Assert.NotNull(value: prod1.Id);
        Assert.NotNull(@object: prod2);
        Assert.NotNull(value: prod2.Id);
        var assignIds = new AsignarProductosRequest { ProductoIds = new List<int?> { prod1.Id.Value, prod2.Id.Value } };
        var assignRes =
            await client.PostAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos",
                content: CreateContent(body: assignIds));
        Assert.Equal(expected: HttpStatusCode.OK, actual: assignRes.StatusCode);

        // 4. Verify Assignment
        var getRes = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var products =
            JsonConvert.DeserializeObject<List<ProductoResult>>(value: await getRes.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.Equal(expected: 2, actual: products.Count);
        Assert.Contains(collection: products, filter: p => p.Id == prod1.Id);
        Assert.Contains(collection: products, filter: p => p.Id == prod2.Id);

        // 5. Test Idempotency (Duplicated Assignment)
        var assignResDuplicate =
            await client.PostAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos",
                content: CreateContent(body: assignIds));
        Assert.Equal(expected: HttpStatusCode.OK, actual: assignResDuplicate.StatusCode);

        var getResDup = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var productsDup =
            JsonConvert.DeserializeObject<List<ProductoResult>>(value: await getResDup.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.Equal(expected: 2, actual: productsDup.Count); // Should still be 2

        // 6. Remove One Product
        var removeIds = new AsignarProductosRequest { ProductoIds = new List<int?> { prod1.Id.Value } };
        var removeRes = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos/remover",
            content: CreateContent(body: removeIds));
        Assert.Equal(expected: HttpStatusCode.OK, actual: removeRes.StatusCode);

        // 7. Verify Removal
        var getResFinal = await client.GetAsync(requestUri: $"/{ApiVersion}/empresa/{empresa.Id}/productos");
        var productsFinal =
            JsonConvert.DeserializeObject<List<ProductoResult>>(value: await getResFinal.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.Single(collection: productsFinal);
        Assert.Contains(collection: productsFinal, filter: p => p.Id == prod2.Id); // Only P2 remains
        Assert.DoesNotContain(collection: productsFinal, filter: p => p.Id == prod1.Id);
    }

    [Fact]
    public async Task Test_Update_Empresa_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create 
        var createRequest = new EmpresaRequest { Nombre = "Empresa Conflict Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Update 1 (Success)
        var updateRequest1 = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Conflict Test Updated 1",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var response1 =
            await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}",
                content: CreateContent(body: updateRequest1));
        Assert.Equal(expected: HttpStatusCode.OK, actual: response1.StatusCode);

        // 4. Update 2 (Stale Token - Should Fail)
        var updateRequest2 = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Conflict Test Updated 2",
            ConcurrencyToken = createResult.ConcurrencyToken // Stale token
        };
        var response2 =
            await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}",
                content: CreateContent(body: updateRequest2));

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response2.StatusCode);
    }

    [Fact]
    public async Task Test_Delete_Empresa_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create 
        var createRequest = new EmpresaRequest { Nombre = "Empresa Delete Conflict Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Update (to change ConcurrencyToken)
        var updateRequest = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Delete Conflict Test Updated",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}",
            content: CreateContent(body: updateRequest));

        // 4. Delete with OLD Token (Should Fail)
        var oldTokenUrlEncoded = System.Web.HttpUtility.UrlEncode(str: createResult.ConcurrencyToken);
        var response =
            await client.DeleteAsync(
                requestUri: $"/{ApiVersion}/empresa/{createResult.Id}?concurrencyToken={oldTokenUrlEncoded}");

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response.StatusCode);
    }

    [Fact]
    public async Task Test_Activate_Empresa_Returns_Conflict_With_Stale_Token()
    {
        // 1. Auth
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 2. Create (Created active by default)
        var createRequest = new EmpresaRequest { Nombre = "Empresa Activate Conflict Test" };
        var createResponse = await client.PostAsync(requestUri: $"/{ApiVersion}/empresa",
            content: CreateContent(body: createRequest));
        var createResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await createResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 3. Update (to change ConcurrencyToken)
        var updateRequest = new EmpresaUpdateRequest
        {
            Nombre = "Empresa Activate Conflict Test Updated",
            ConcurrencyToken = createResult.ConcurrencyToken
        };
        var updateResponse =
            await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}",
                content: CreateContent(body: updateRequest));
        var updateResult =
            JsonConvert.DeserializeObject<EmpresaResult>(value: await updateResponse.Content.ReadAsStringAsync(),
                settings: _jsonSettings);

        // 4. Delete (Deactivate) to allow Activation attempts
        var deactivateTokenEncoded = System.Web.HttpUtility.UrlEncode(str: updateResult.ConcurrencyToken);
        var deleteResponse =
            await client.DeleteAsync(
                requestUri: $"/{ApiVersion}/empresa/{createResult.Id}?concurrencyToken={deactivateTokenEncoded}");
        Assert.Equal(expected: HttpStatusCode.OK, actual: deleteResponse.StatusCode);

        // 5. Try Activate with OLD Token (from createResult) -> Should Fail
        var activateRequest = new StatusChangeRequest { ConcurrencyToken = createResult.ConcurrencyToken };
        var response = await client.PutAsync(requestUri: $"/{ApiVersion}/empresa/{createResult.Id}/activar",
            content: CreateContent(body: activateRequest));

        Assert.Equal(expected: HttpStatusCode.Conflict, actual: response.StatusCode);
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
