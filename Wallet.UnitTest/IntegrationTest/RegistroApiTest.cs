using System.Net;
using Microsoft.EntityFrameworkCore;
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
        // 0. Seed Company and State
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(commonSettings.Empresas);
        Context.Estado.AddRange(commonSettings.Estados);
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
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
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

    [Fact]
    public async Task ResumedRegistrationFlow_ShouldSucceed()
    {
        // 0. Seed Company and State
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(commonSettings.Empresas);
        Context.Estado.AddRange(commonSettings.Estados);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";
        var telefono = "5599887766";

        // --- ROUND 1 ---

        // 1. PreRegistro
        var preRegistroRequest = new PreRegistroRequest
        {
            CodigoPais = "+52",
            Telefono = telefono
        };
        var responsePre1 = await client.PostAsync($"/{version}/registro/preRegistro",
            new StringContent(JsonConvert.SerializeObject(preRegistroRequest), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, responsePre1.StatusCode);
        var resultPre1 = JsonConvert.DeserializeObject<UsuarioResult>(await responsePre1.Content.ReadAsStringAsync());
        Assert.NotNull(resultPre1);
        Assert.NotNull(resultPre1.Id);
        var usuarioId = resultPre1.Id.Value;

        // 2. ConfirmarNumero
        var confirmarRequest1 = new ConfirmarNumeroRequest { IdUsuario = usuarioId, Codigo = "1234" };
        await client.PostAsync($"/{version}/registro/confirmarNumero",
            new StringContent(JsonConvert.SerializeObject(confirmarRequest1), Encoding.UTF8, "application/json"));

        // 3. DatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            IdUsuario = usuarioId,
            Nombre = "Test",
            ApellidoPaterno = "User",
            ApellidoMaterno = "Resume",
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
            FechaNacimiento = new DateTime(1990, 1, 1)
        };
        await client.PostAsync($"/{version}/registro/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));

        // --- ROUND 2 (RESUME) ---

        // 4. PreRegistro (Again)
        var responsePre2 = await client.PostAsync($"/{version}/registro/preRegistro",
            new StringContent(JsonConvert.SerializeObject(preRegistroRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, responsePre2.StatusCode);
        var resultPre2 = JsonConvert.DeserializeObject<UsuarioResult>(await responsePre2.Content.ReadAsStringAsync());
        Assert.NotNull(resultPre2);
        Assert.Equal(usuarioId, resultPre2.Id.Value); // Same ID
        Assert.Equal(nameof(EstatusRegistroEnum.PreRegistro), resultPre2.Estatus); // Status reset

        // Verify DB side: Should have 2 SMS verifications
        var userDb = await Context.Usuario.Include(u => u.Verificaciones2Fa)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);
        Assert.NotNull(userDb);
        Assert.Equal(2, userDb.Verificaciones2Fa.Count(v => v.Tipo == Tipo2FA.Sms));

        // --- COMPLETE FLOW ---

        // 5. ConfirmarNumero (Again)
        var confirmarRequest2 = new ConfirmarNumeroRequest { IdUsuario = usuarioId, Codigo = "1234" };
        var responseConf2 = await client.PostAsync($"/{version}/registro/confirmarNumero",
            new StringContent(JsonConvert.SerializeObject(confirmarRequest2), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, responseConf2.StatusCode);

        // 6. DatosCliente (Again)
        var responseDatos2 = await client.PostAsync($"/{version}/registro/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, responseDatos2.StatusCode);

        // 7. RegistrarCorreo
        var correoRequest = new RegistrarCorreoRequest { IdUsuario = usuarioId, Correo = "resume@test.com" };
        await client.PostAsync($"/{version}/registro/correo",
            new StringContent(JsonConvert.SerializeObject(correoRequest), Encoding.UTF8, "application/json"));

        // 8. VerificarCorreo
        var verifCorreoRequest = new VerificarCorreoRequest { IdUsuario = usuarioId, Codigo = "1234" };
        await client.PostAsync($"/{version}/registro/verificarCorreo",
            new StringContent(JsonConvert.SerializeObject(verifCorreoRequest), Encoding.UTF8, "application/json"));

        // 9. Biometricos
        var bioRequest = new RegistrarBiometricosRequest { IdUsuario = usuarioId, IdDispositivo = "d2", Token = "t2" };
        await client.PostAsync($"/{version}/registro/biometricos",
            new StringContent(JsonConvert.SerializeObject(bioRequest), Encoding.UTF8, "application/json"));

        // 10. Terminos
        var terminosRequest = new AceptarTerminosRequest
            { IdUsuario = usuarioId, Version = "1", AceptoTerminos = true, AceptoPrivacidad = true, AceptoPld = true };
        await client.PostAsync($"/{version}/registro/terminos",
            new StringContent(JsonConvert.SerializeObject(terminosRequest), Encoding.UTF8, "application/json"));

        // 11. Completar
        var completarRequest = new CompletarRegistroRequest
            { IdUsuario = usuarioId, Contrasena = "Pass123!", ConfirmacionContrasena = "Pass123!" };
        var responseFinal = await client.PostAsync($"/{version}/registro/completar",
            new StringContent(JsonConvert.SerializeObject(completarRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseFinal.StatusCode);
        var resultFinal = JsonConvert.DeserializeObject<UsuarioResult>(await responseFinal.Content.ReadAsStringAsync());
        Assert.NotNull(resultFinal);
        Assert.Equal(nameof(EstatusRegistroEnum.RegistroCompletado), resultFinal.Estatus);
    }

    [Fact]
    public async Task Registration_ShouldFail_WhenUserAlreadyCompleted()
    {
        // 0. Seed Company and State
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(commonSettings.Empresas);
        Context.Estado.AddRange(commonSettings.Estados);
        await Context.SaveChangesAsync();

        // 1. Create a completed user
        var completedUser = new Usuario(
            codigoPais: "+52",
            telefono: "5511223344",
            correoElectronico: "completed@test.com",
            contrasena: "Pass123!",
            estatus: EstatusRegistroEnum.RegistroCompletado,
            creationUser: Guid.NewGuid(),
            testCase: "IntegrationTest"
        );
        Context.Usuario.Add(completedUser);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 2. Attempt PreRegistro with same phone
        var preRegistroRequest = new PreRegistroRequest
        {
            CodigoPais = "+52",
            Telefono = "5511223344"
        };
        var response = await client.PostAsync($"/{version}/registro/preRegistro",
            new StringContent(JsonConvert.SerializeObject(preRegistroRequest), Encoding.UTF8, "application/json"));

        // 3. Assert Failure
        Assert.False(response.IsSuccessStatusCode, "Should return error status code");

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("CLIENTE-YA-REGISTRADO", responseContent);
    }
}
