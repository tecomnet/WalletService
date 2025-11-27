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
        SetupDataAsync(async context => { await context.SaveChangesAsync(); }).GetAwaiter().GetResult();
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
        var provider = await CreateProveedor(client);

        var request = new ProductoProveedorRequest
        {
            Sku = "NETFLIX-PREM",
            Nombre = "Netflix Premium",
            Monto = 15.99m,
            Descripcion = "Premium Subscription"
        };
        var content = CreateContent(request);

        // Act
        var response = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/producto", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(request.Sku, result.Sku);
        Assert.Equal(provider.Id, result.ProveedorServicioId);
    }

    [Fact]
    public async Task Get_ProductoProveedor_ById_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client);
        var product = await CreateProducto(client, provider.Id.GetValueOrDefault());

        // Act
        var response = await client.GetAsync($"{API_VERSION}/{API_URI}/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task Put_ProductoProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client);
        var product = await CreateProducto(client, provider.Id.GetValueOrDefault());

        var updateRequest = new ProductoProveedorRequest
        {
            Sku = "NETFLIX-STD",
            Nombre = "Netflix Standard",
            Monto = 10.99m,
            Descripcion = "Standard Subscription"
        };

        // Act
        var response = await client.PutAsync($"{API_VERSION}/{API_URI}/{product.Id}", CreateContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.Equal(updateRequest.Sku, result.Sku);
        Assert.Equal(updateRequest.Monto, result.Monto);
    }

    [Fact]
    public async Task Delete_ProductoProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client);
        var product = await CreateProducto(client, provider.Id.GetValueOrDefault());

        // Act
        var response = await client.DeleteAsync($"{API_VERSION}/{API_URI}/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<ProductoProveedorResult>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task Get_ProductosPorProveedor_Ok()
    {
        // Arrange
        var client = Factory.CreateAuthenticatedClient();
        var provider = await CreateProveedor(client);
        await CreateProducto(client, provider.Id.GetValueOrDefault());
        await CreateProducto(client, provider.Id.GetValueOrDefault(), "OTHER-SKU");

        // Act
        var response = await client.GetAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{provider.Id}/productos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result =
            JsonConvert.DeserializeObject<List<ProductoProveedorResult>>(await response.Content.ReadAsStringAsync(),
                _jsonSettings);
        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
    }

    private async Task<ProveedorServicioResult> CreateProveedor(HttpClient client)
    {
        var request = new ProveedorServicioRequest
        {
            Nombre = "Test Provider " + Guid.NewGuid(),
            Categoria = "Servicios",
            UrlIcono = "http://test.com/icon.png"
        };
        var response = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}", CreateContent(request));
        Assert.True(response.IsSuccessStatusCode,
            "Failed to create provider: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProveedorServicioResult>(await response.Content.ReadAsStringAsync(),
            _jsonSettings)!;
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
        var response = await client.PostAsync($"{API_VERSION}/{PROVEEDOR_API_URI}/{providerId}/producto",
            CreateContent(request));
        Assert.True(response.IsSuccessStatusCode,
            "Failed to create product: " + await response.Content.ReadAsStringAsync());
        return JsonConvert.DeserializeObject<ProductoProveedorResult>(await response.Content.ReadAsStringAsync(),
            _jsonSettings)!;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(body, _jsonSettings);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
