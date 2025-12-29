using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ProductoApiTest : DatabaseTestFixture
{
    private const string API_URI = "producto";
    private const string PROVEEDOR_API_URI = "proveedor";
    private const string API_VERSION = "0.1";

    public ProductoApiTest()
    {
        SetupDataAsync(setupDataAction: async context =>
        {
            if (!await context.Broker.AnyAsync(b => b.Nombre == "Broker Test"))
            {
                var broker = new Wallet.DOM.Modelos.Broker(nombre: "Broker Test", creationUser: Guid.NewGuid());
                await context.Broker.AddAsync(entity: broker);
            }

            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public async Task Post_Producto_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create provider
        var provider = await CreateProveedor(client: client);

        var request = new ProductoRequest
        {
            Sku = "NETFLIX-PREM",
            Nombre = "Netflix Premium",
            Precio = 15.99m,
            UrlIcono = "https://netflix.com/icon.png",
            Categoria = CategoriaEnum.SERVICIOSEnum
        };
        var content = CreateContent(body: request);

        // Act
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/producto",
            content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Sku, actual: result.Sku);
        Assert.Equal(expected: provider.Id, actual: result.ProveedorId);
    }

    [Fact]
    public async Task Get_Producto_ById_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        var product = await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}/{product.Id}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: product.Id, actual: result.Id);
    }

    [Fact]
    public async Task Put_Producto_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        var product = await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());

        var updateRequest = new ProductoRequest
        {
            Sku = "NETFLIX-STD",
            Nombre = "Netflix Standard",
            Precio = 10.99m,
            UrlIcono = "https://netflix.com/icon.png",
            Categoria = CategoriaEnum.SERVICIOSEnum
        };

        // Act
        var response = await client.PutAsync(requestUri: $"{API_VERSION}/{API_URI}/{product.Id}",
            content: CreateContent(body: updateRequest));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: updateRequest.Sku, actual: result.Sku);
        Assert.Equal(expected: updateRequest.Precio, actual: result.Precio);
    }

    [Fact]
    public async Task Delete_Producto_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        var product = await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());

        // Act
        var response = await client.DeleteAsync(requestUri: $"{API_VERSION}/{API_URI}/{product.Id}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.False(condition: result.IsActive);
    }

    [Fact]
    public async Task Get_ProductosPorProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());
        await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault(), sku: "OTHER-SKU",
            nombre: "OTHER-Nombre");

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/productos");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ProductoResult>>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.True(condition: result.Count >= 2);
    }

    [Fact]
    public async Task Get_Productos_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());
        await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault(), sku: "SKU-2",
            nombre: "Prod 2");

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ProductoResult>>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.True(condition: result.Count >= 2);
    }

    [Fact]
    public async Task Put_ActualizarProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var providerA = await CreateProveedor(client: client);
        var providerB = await CreateProveedor(client: client);
        var product = await CreateProducto(client: client, providerId: providerA.Id.GetValueOrDefault());

        // Act
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/{API_URI}/{product.Id}/actualizarProveedor",
            content: CreateContent(body: providerB.Id.GetValueOrDefault()));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);

        // Verify update
        var getResponse = await client.GetAsync(requestUri: $"{API_VERSION}/{API_URI}/{product.Id}");
        var getResult = JsonConvert.DeserializeObject<ProductoResult>(
            value: await getResponse.Content.ReadAsStringAsync(),
            settings: _jsonSettings);
        Assert.NotNull(getResult);
        Assert.Equal(expected: providerB.Id, actual: getResult.ProveedorId);
    }

    private async Task<ProveedorResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorRequest
        {
            Nombre = "Test Provider " + Guid.NewGuid(),
            BrokerId = 1, // Assuming Seeded Broker Id 1 exists or I need to fetch it? SetupDataConfig seeds brokers? Yes CommonSettings creates brokers.
            UrlIcono = "https://icon.png",
        };
     
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}",
            content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode,
            userMessage: "Failed to create provider: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProveedorResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private async Task<ProductoResult> CreateProducto(HttpClient client, int providerId,
        string sku = "TEST-SKU", string nombre = "Test Product")
    {
        var request = new ProductoRequest
        {
            Sku = sku,
            Nombre = nombre,
            Precio = 10.0m,
            UrlIcono = "icon",
            Categoria = CategoriaEnum.SERVICIOSEnum
        };
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{providerId}/producto",
            content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode,
            userMessage: "Failed to create product: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProductoResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
