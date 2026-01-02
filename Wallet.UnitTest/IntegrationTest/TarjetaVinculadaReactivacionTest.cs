using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionCliente;
using Wallet.DOM.Modelos.GestionEmpresa;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.DOM.Modelos.GestionWallet;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class TarjetaVinculadaReactivacionTest : DatabaseTestFixture
{
    private readonly ITestOutputHelper _output;
    private const string ApiVersion = "0.1";

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public TarjetaVinculadaReactivacionTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body, settings: _jsonSettings);
        return new StringContent(content: json, encoding: Encoding.UTF8, mediaType: "application/json");
    }

    [Fact]
    public async Task Test_Vincular_Desvincular_Reactivar_Targeta()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int idCliente;
        int idCuentaWallet;
        int idTarjeta;

        // Setup Data
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa(nombre: "Tecomnet " + guid, creationUser: guid);
            context.Empresa.Add(entity: empresa);
            await context.SaveChangesAsync();

            var usuario = new Usuario(codigoPais: "+52",
                telefono: "55" + new Random().Next(minValue: 10000000, maxValue: 99999999),
                correoElectronico: "testCard" + guid + "@test.com", contrasena: "Pass123!",
                estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario);

            var clienteEntity = new Cliente(usuario: usuario, empresa: empresa, creationUser: guid);
            clienteEntity.AgregarDatosPersonales(nombre: "Test", primerApellido: "User", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
                modificationUser: guid);
            context.Cliente.Add(entity: clienteEntity);
            await context.SaveChangesAsync();
            idCliente = clienteEntity.Id;

            var cuentaWallet = new CuentaWallet(idCliente: clienteEntity.Id, moneda: "MXN",
                cuentaCLABE: "123456789012345678", creationUser: guid);
            context.CuentaWallet.Add(entity: cuentaWallet);
            await context.SaveChangesAsync();
            idCuentaWallet = cuentaWallet.Id;
        }

        // 2. Vincular Tarjeta
        // POST /cliente/{idCliente}/tarjetasVinculadas
        var request1 = new VincularTarjetaRequest
        {
            Alias = "Mi Tarjeta Personal",
            NumeroTarjeta = "1234567890123456",
            Marca = (MarcaTarjetaEnum)1, // Assuming 1 is Visa or Mastercard, checked in Enum but using int cast as in original test
            FechaExpiracion = DateTime.UtcNow.AddYears(2)
        };

        var response1 = await client.PostAsync(requestUri: $"/{ApiVersion}/cliente/{idCliente}/tarjetasVinculadas",
            content: CreateContent(request1));
        Assert.Equal(expected: HttpStatusCode.Created, actual: response1.StatusCode);

        var tarjetaVinculada1 =
            JsonConvert.DeserializeObject<TarjetaVinculadaResult>(value: await response1.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(tarjetaVinculada1);
        Assert.NotNull(tarjetaVinculada1.Id);
        idTarjeta = tarjetaVinculada1.Id.Value;

        // 3. Desvincular Tarjeta
        // DELETE /tarjetasVinculadas/{idTarjeta}
        var response2 = await client.DeleteAsync(requestUri: $"/{ApiVersion}/tarjetasVinculadas/{idTarjeta}");
        Assert.Equal(expected: HttpStatusCode.OK, actual: response2.StatusCode);

        // Verify it is inactive in DB
        using (var context = CreateContext())
        {
            var tarjeta = await context.TarjetaVinculada.FindAsync(idTarjeta);
            Assert.NotNull(tarjeta);
            Assert.False(tarjeta.IsActive);
        }

        // 4. Re-Vincular LA MISMA Tarjeta (Should Reactivate)
        var request2 = new VincularTarjetaRequest
        {
            Alias = "Mi Tarjeta Reactivada", // Changed Alias
            NumeroTarjeta = "1234567890123456",
            Marca = (MarcaTarjetaEnum)1,
            FechaExpiracion = DateTime.UtcNow.AddYears(3) // Changed Expiration
        };

        var response3 = await client.PostAsync(requestUri: $"/{ApiVersion}/cliente/{idCliente}/tarjetasVinculadas",
            content: CreateContent(request2));
        Assert.Equal(expected: HttpStatusCode.Created, actual: response3.StatusCode);

        var tarjetaVinculada2 =
            JsonConvert.DeserializeObject<TarjetaVinculadaResult>(value: await response3.Content.ReadAsStringAsync(),
                settings: _jsonSettings);
        Assert.NotNull(tarjetaVinculada2);

        // ASSERTIONS:
        // ID should be the SAME
        Assert.Equal(expected: idTarjeta, actual: tarjetaVinculada2.Id.Value);

        using (var context = CreateContext())
        {
            var tarjeta = await context.TarjetaVinculada.FindAsync(idTarjeta);
            Assert.NotNull(tarjeta);
            Assert.True(tarjeta.IsActive);
            Assert.Equal(expected: "Mi Tarjeta Reactivada", actual: tarjeta.Alias);
        }
    }

    [Fact]
    public async Task Test_Vincular_OtroUsuario_MismaTarjeta_NoReactiva()
    {
        // 1. Auth & Setup
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int idCliente1, idCliente2;
        int idTarjeta1;

        // Setup Data for Two Users
        using (var context = CreateContext())
        {
            var guid = Guid.NewGuid();
            var empresa = new Empresa(nombre: "Tecomnet " + guid, creationUser: guid);
            context.Empresa.Add(entity: empresa);
            await context.SaveChangesAsync();

            // User 1
            var usuario1 = new Usuario(codigoPais: "+52", telefono: "55" + new Random().Next(10000000, 99999999),
                correoElectronico: "user1_" + guid + "@test.com", contrasena: "Pass123!",
                estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario1);

            var cliente1 = new Cliente(usuario: usuario1, empresa: empresa, creationUser: guid);
            cliente1.AgregarDatosPersonales(nombre: "UserOne", primerApellido: "Test", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1990, month: 1, day: 1), genero: Genero.Masculino,
                modificationUser: guid);
            context.Cliente.Add(entity: cliente1);

            // User 2
            var usuario2 = new Usuario(codigoPais: "+52", telefono: "55" + new Random().Next(10000000, 99999999),
                correoElectronico: "user2_" + guid + "@test.com", contrasena: "Pass123!",
                estatus: EstatusRegistroEnum.RegistroCompletado, creationUser: guid);
            context.Usuario.Add(entity: usuario2);

            var cliente2 = new Cliente(usuario: usuario2, empresa: empresa, creationUser: guid);
            cliente2.AgregarDatosPersonales(nombre: "UserTwo", primerApellido: "Test", segundoApellido: "Client",
                fechaNacimiento: new DateOnly(year: 1992, month: 2, day: 2), genero: Genero.Femenino,
                modificationUser: guid);
            context.Cliente.Add(entity: cliente2);

            await context.SaveChangesAsync();
            idCliente1 = cliente1.Id;
            idCliente2 = cliente2.Id;

            var cw1 = new CuentaWallet(idCliente: cliente1.Id, moneda: "MXN", cuentaCLABE: "123456789012345678",
                creationUser: guid);
            var cw2 = new CuentaWallet(idCliente: cliente2.Id, moneda: "MXN", cuentaCLABE: "876543210987654321",
                creationUser: guid);
            context.CuentaWallet.AddRange(cw1, cw2);
            await context.SaveChangesAsync();
        }

        string cardNumber = "4111222233334444";

        // 2. User 1 links card
        var req1 = new VincularTarjetaRequest
        {
            Alias = "Card U1",
            NumeroTarjeta = cardNumber,
            Marca = (MarcaTarjetaEnum)1,
            FechaExpiracion = DateTime.UtcNow.AddYears(1)
        };
        var resp1 = await client.PostAsync(requestUri: $"/{ApiVersion}/cliente/{idCliente1}/tarjetasVinculadas",
            content: CreateContent(req1));
        Assert.Equal(HttpStatusCode.Created, resp1.StatusCode);
        var res1 = JsonConvert.DeserializeObject<TarjetaVinculadaResult>(value: await resp1.Content.ReadAsStringAsync(),
            settings: _jsonSettings);
        idTarjeta1 = res1.Id.Value;

        // 3. User 1 unlinks card
        await client.DeleteAsync($"/{ApiVersion}/tarjetasVinculadas/{idTarjeta1}");

        // 4. User 2 links SAME card number
        // Should create NEW record, NOT reactivate User 1's card
        var req2 = new VincularTarjetaRequest
        {
            Alias = "Card U2",
            NumeroTarjeta = cardNumber,
            Marca = (MarcaTarjetaEnum)1,
            FechaExpiracion = DateTime.UtcNow.AddYears(1)
        };
        var resp2 = await client.PostAsync(requestUri: $"/{ApiVersion}/cliente/{idCliente2}/tarjetasVinculadas",
            content: CreateContent(req2));
        Assert.Equal(HttpStatusCode.Created, resp2.StatusCode);
        var res2 = JsonConvert.DeserializeObject<TarjetaVinculadaResult>(value: await resp2.Content.ReadAsStringAsync(),
            settings: _jsonSettings);

        Assert.NotNull(res2);
        Assert.NotEqual(idTarjeta1, res2.Id.Value); // Must be different ID

        using (var context = CreateContext())
        {
            var t1 = await context.TarjetaVinculada.FindAsync(idTarjeta1);
            var t2 = await context.TarjetaVinculada.FindAsync(res2.Id.Value);

            Assert.False(t1.IsActive); // User 1's card still inactive
            Assert.True(t2.IsActive); // User 2's card active
            Assert.NotEqual(t1.IdCuentaWallet, t2.IdCuentaWallet);
        }
    }
}
