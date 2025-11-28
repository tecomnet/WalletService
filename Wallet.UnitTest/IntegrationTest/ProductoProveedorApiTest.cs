using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;

namespace Wallet.UnitTest.IntegrationTest;

public class ProductoProveedorApiTest : DatabaseTestFixture
{
    private const string API_URI = "productoProveedor";
    private const string PROVEEDOR_API_URI = "proveedorServicio";
    private const string API_VERSION = "0.1";

    public ProductoProveedorApiTest()
    {
        SetupDataAsync(setupDataAction: async context => { await context.SaveChangesAsync(); }).GetAwaiter().GetResult();
    }

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    [Fact]
    public async Task Post_ProductoProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        // Create provider
        var provider = await CreateProveedor(client: client);

        var request = new ProductoProveedorRequest
        {
            Sku = "NETFLIX-PREM",
            Nombre = "Netflix Premium",
            Monto = 15.99m,
            Descripcion = "Premium Subscription"
        };
        var content = CreateContent(body: request);

        // Act
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/producto", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Sku, actual: result.Sku);
        Assert.Equal(expected: provider.Id, actual: result.ProveedorServicioId);
    }

    [Fact]
    public async Task Get_ProductoProveedor_ById_Ok()
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
            JsonConvert.DeserializeObject<ProductoProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: product.Id, actual: result.Id);
    }

    [Fact]
    public async Task Put_ProductoProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client: client);
        var product = await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault());

        var updateRequest = new ProductoProveedorRequest
        {
            Sku = "NETFLIX-STD",
            Nombre = "Netflix Standard",
            Monto = 10.99m,
            Descripcion = "Standard Subscription"
        };

        // Act
        var response = await client.PutAsync(requestUri: $"{API_VERSION}/{API_URI}/{product.Id}", content: CreateContent(body: updateRequest));

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(expected: updateRequest.Sku, actual: result.Sku);
        Assert.Equal(expected: updateRequest.Monto, actual: result.Monto);
    }

    [Fact]
    public async Task Delete_ProductoProveedor_Ok()
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
            JsonConvert.DeserializeObject<ProductoProveedorResult>(value: await response.Content.ReadAsStringAsync(),
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
        await CreateProducto(client: client, providerId: provider.Id.GetValueOrDefault(), sku: "OTHER-SKU");

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/productos");

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ProductoProveedorResult>>(value: await response.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(result);
        Assert.True(condition: result.Count >= 2);
    }

    private async Task<ProveedorServicioResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorServicioRequest
        {
            Nombre = "Test Provider " + Guid.NewGuid(),
            Categoria = "Servicios",
            UrlIcono = "http://test.com/icon.png"
        };
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}", content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode,
            userMessage: "Failed to create provider: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProveedorServicioResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private async Task<ProductoProveedorResult> CreateProducto(HttpClient client, int providerId,
        string sku = "TEST-SKU")
    {
        var request = new ProductoProveedorRequest
        {
            Sku = sku,
            Nombre = "Test Product",
            Monto = 10.0m,
            Descripcion = "Test Description"
        };
        var response = await client.PostAsync(requestUri: $"{API_VERSION}/{PROVEEDOR_API_URI}/{providerId}/producto",
            content: CreateContent(body: request));
        Assert.True(condition: response.IsSuccessStatusCode,
            userMessage: "Failed to create product: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProductoProveedorResult>(value: await response.Content.ReadAsStringAsync(),
            settings: _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }
}
