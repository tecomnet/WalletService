using System.Net;
using System.Text;
using Newtonsoft.Json;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
using Wallet.DOM.ApplicationDbContext;
using Xunit.Abstractions;

namespace Wallet.UnitTest.IntegrationTest;

public class RegistroApiTest : DatabaseTestFixture, IDisposable
{
    protected ServiceDbContext Context;

    private readonly ITestOutputHelper _output;

    public RegistroApiTest(ITestOutputHelper output)
    {
        _output = output;
        Context = CreateContext();
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    [Fact]
    public async Task FullRegistrationFlow_ShouldSucceed()
    {
        // 0. Seed Company and State
        var empresa = new Empresa("Tecomnet", Guid.NewGuid());
        Context.Empresa.Add(empresa);
        var estado = new Estado("N/A", Guid.NewGuid());
        Context.Estado.Add(estado);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 1. PreRegistro (API Call)
        var preRegistroRequest = new PreRegistroRequest
        {
            CodigoPais = "+52",
            Telefono = "5512345678"
        };
        var responsePreRegistro = await client.PostAsync($"/{version}/registro/preRegistro",
            new StringContent(JsonConvert.SerializeObject(preRegistroRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, responsePreRegistro.StatusCode);
        var resultPreRegistro =
            JsonConvert.DeserializeObject<UsuarioResult>(await responsePreRegistro.Content.ReadAsStringAsync());
        Assert.NotNull(resultPreRegistro);
        Assert.NotNull(resultPreRegistro.Id);
        var usuarioId = resultPreRegistro.Id.Value;
        Assert.Equal(nameof(EstatusRegistroEnum.PreRegistro), resultPreRegistro.Estatus);

        // 2. ConfirmarNumero (API Call)
        var confirmarNumeroRequest = new ConfirmarNumeroRequest
        {
            IdUsuario = usuarioId,
            Codigo = "1234" // Mock accepts any code
        };
        var responseConfirmar = await client.PostAsync($"/{version}/registro/confirmarNumero",
            new StringContent(JsonConvert.SerializeObject(confirmarNumeroRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseConfirmar.StatusCode);
        var resultConfirmar = JsonConvert.DeserializeObject<bool>(await responseConfirmar.Content.ReadAsStringAsync());
        Assert.True(resultConfirmar);

        // 3. CompletarDatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            IdUsuario = usuarioId,
            Nombre = "Juan",
            ApellidoPaterno = "Perez",
            ApellidoMaterno = "Lopez",
            FechaNacimiento = new DateTime(1990, 1, 1)
        };
        var responseDatos = await client.PostAsync($"/{version}/registro/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));

        if (responseDatos.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await responseDatos.Content.ReadAsStringAsync();
            _output.WriteLine($"Error in CompletarDatosCliente: {errorContent}");
        }

        Assert.Equal(HttpStatusCode.OK, responseDatos.StatusCode);
        var resultDatos = JsonConvert.DeserializeObject<UsuarioResult>(await responseDatos.Content.ReadAsStringAsync());
        Assert.NotNull(resultDatos);
        Assert.Equal(nameof(EstatusRegistroEnum.DatosClienteCompletado), resultDatos.Estatus);

        // 4. RegistrarCorreo
        var registrarCorreoRequest = new RegistrarCorreoRequest
        {
            IdUsuario = usuarioId,
            Correo = "juan.perez@example.com"
        };
        var responseCorreo = await client.PostAsync($"/{version}/registro/correo",
            new StringContent(JsonConvert.SerializeObject(registrarCorreoRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseCorreo.StatusCode);
        var resultCorreo =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseCorreo.Content.ReadAsStringAsync());
        Assert.NotNull(resultCorreo);
        Assert.Equal(nameof(EstatusRegistroEnum.CorreoRegistrado), resultCorreo.Estatus);

        // 5. VerificarCorreo (API Call)
        var verificarCorreoRequest = new VerificarCorreoRequest
        {
            IdUsuario = usuarioId,
            Codigo = "1234" // Mock accepts any code
        };
        var responseVerificarCorreo = await client.PostAsync($"/{version}/registro/verificarCorreo",
            new StringContent(JsonConvert.SerializeObject(verificarCorreoRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseVerificarCorreo.StatusCode);
        var resultVerificarCorreo =
            JsonConvert.DeserializeObject<bool>(await responseVerificarCorreo.Content.ReadAsStringAsync());
        Assert.True(resultVerificarCorreo);

        // 6. RegistrarBiometricos
        var biometricosRequest = new RegistrarBiometricosRequest
        {
            IdUsuario = usuarioId,
            IdDispositivo = "device123",
            Token = "token123"
        };
        var responseBiometricos = await client.PostAsync($"/{version}/registro/biometricos",
            new StringContent(JsonConvert.SerializeObject(biometricosRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseBiometricos.StatusCode);
        var resultBiometricos =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseBiometricos.Content.ReadAsStringAsync());
        Assert.NotNull(resultBiometricos);
        Assert.Equal(nameof(EstatusRegistroEnum.DatosBiometricosRegistrado), resultBiometricos.Estatus);

        // 7. AceptarTerminos
        var terminosRequest = new AceptarTerminosRequest
        {
            IdUsuario = usuarioId,
            Version = "1.0",
            AceptoTerminos = true,
            AceptoPrivacidad = true,
            AceptoPld = true
        };
        var responseTerminos = await client.PostAsync($"/{version}/registro/terminos",
            new StringContent(JsonConvert.SerializeObject(terminosRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseTerminos.StatusCode);
        var resultTerminos =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseTerminos.Content.ReadAsStringAsync());
        Assert.NotNull(resultTerminos);
        Assert.Equal(nameof(EstatusRegistroEnum.TerminosCondicionesAceptado), resultTerminos.Estatus);

        // 8. CompletarRegistro
        var completarRequest = new CompletarRegistroRequest
        {
            IdUsuario = usuarioId,
            Contrasena = "Password123!",
            ConfirmacionContrasena = "Password123!"
        };
        var responseCompletar = await client.PostAsync($"/{version}/registro/completar",
            new StringContent(JsonConvert.SerializeObject(completarRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseCompletar.StatusCode);
        var resultCompletar =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseCompletar.Content.ReadAsStringAsync());
        Assert.NotNull(resultCompletar);
        Assert.Equal(nameof(EstatusRegistroEnum.RegistroCompletado), resultCompletar.Estatus);
    }
}
