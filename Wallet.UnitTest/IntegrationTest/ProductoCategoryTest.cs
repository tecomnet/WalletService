using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit;

namespace Wallet.UnitTest.IntegrationTest;

[Trait("Category", "Integration")]
public class ProductoCategoryTest : DatabaseTestFixture
{
    private const string API_VERSION = "0.1";

    public ProductoCategoryTest()
    {
        Factory.UseTestAuth = false;
        SetupDataAsync(async context =>
        {
            // Create Provider and Check validation
            var broker = await context.Broker.FirstOrDefaultAsync();
            if (broker == null)
            {
                // Create dummy broker if needed or assume one exists from previous seed?
                // Usually Migration or Seed is run.
                // DatabaseTestFixture might seed some data.
                // Let's look for existing Broker or create one.
                // Let's look for existing Broker or create one.
                broker = new Broker("Test Broker", Guid.NewGuid());
                context.Broker.Add(broker);
                await context.SaveChangesAsync();
            }

            var proveedor = new Proveedor(nombre: "Proveedor Test", urlIcono: "http://icon.com", broker: broker,
                creationUser: Guid.NewGuid());
            context.Proveedor.Add(proveedor);

            // Create Products
            var productoElectronica = new Producto(proveedor: proveedor, sku: "ELEC01", nombre: "TV 4K",
                urlIcono: "url", categoria: "Electronica", precio: 1000m, creationUser: Guid.NewGuid());

            var productoHogar = new Producto(proveedor: proveedor, sku: "HOG01", nombre: "Silla",
                urlIcono: "url", categoria: "Hogar", precio: 50m, creationUser: Guid.NewGuid());

            context.Producto.Add(productoElectronica);
            context.Producto.Add(productoHogar);

            var productoRecarga = new Producto(proveedor: proveedor, sku: "REC01", nombre: "Recarga Cell",
                urlIcono: "url", categoria: nameof(ProductoCategoria.Recargas), precio: 20m,
                creationUser: Guid.NewGuid());
            context.Producto.Add(productoRecarga);

            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Get_Productos_Por_Categoria_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"/{API_VERSION}/producto?categoria={nameof(ProductoCategoria.Recargas)}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productos = await response.Content.ReadFromJsonAsync<List<ProductoResult>>();
        Assert.NotNull(productos);
        Assert.Single(productos);
        Assert.Equal("Recarga Cell", productos.First().Nombre);
    }

    [Fact]
    public async Task Get_Productos_Por_Categoria_No_Existente_Retorna_Vacio()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"/{API_VERSION}/producto?categoria=Inexistente");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
