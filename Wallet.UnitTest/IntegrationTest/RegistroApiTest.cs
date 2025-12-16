using System.Net;
using Wallet.DOM.Errors;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.RestAPI.Models;
using Wallet.UnitTest.FixtureBase;
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
        var confirmarNumeroRequest = new ConfirmacionRequest
        {
            Tipo = Tipo2FAEnum.SMSEnum,
            Codigo = "1234" // Mock accepts any code
        };
        var responseConfirmar = await client.PutAsync($"/{version}/registro/{usuarioId}/confirmar",
            new StringContent(JsonConvert.SerializeObject(confirmarNumeroRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseConfirmar.StatusCode);
        var resultConfirmar = JsonConvert.DeserializeObject<bool>(await responseConfirmar.Content.ReadAsStringAsync());
        Assert.True(resultConfirmar);

        // 3. CompletarDatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            Nombre = "Juan",
            ApellidoPaterno = "Perez",
            ApellidoMaterno = "Lopez",
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
            FechaNacimiento = new DateTime(1990, 1, 1),
        };
        var responseDatos = await client.PostAsync($"/{version}/registro/{usuarioId}/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));

// ... (In ResumedRegistrationFlow) ...


        // --- COMPLETE FLOW ---
        if (responseDatos.StatusCode != HttpStatusCode.Created) // Changed from OK to CREATED to match assertion
        {
            var errorContent = await responseDatos.Content.ReadAsStringAsync();
            _output.WriteLine($"Error in CompletarDatosCliente: {errorContent}");
        }

        Assert.Equal(expected: HttpStatusCode.Created, actual: responseDatos.StatusCode);
        var resultDatos = JsonConvert.DeserializeObject<UsuarioResult>(await responseDatos.Content.ReadAsStringAsync());
        Assert.NotNull(resultDatos);
        Assert.Equal(nameof(EstatusRegistroEnum.DatosClienteCompletado), resultDatos.Estatus);

        var userAfterDatos = await Context.Usuario.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
        Assert.Equal(nameof(EstatusRegistroEnum.DatosClienteCompletado), userAfterDatos.Estatus.ToString());

        var token = resultDatos.ConcurrencyToken;

        // 4. RegistrarCorreo
        var registrarCorreoRequest = new RegistrarCorreoRequest
        {
            Correo = "juan.perez@example.com",
            ConcurrencyToken = token
        };
        var responseCorreo = await client.PutAsync($"/{version}/registro/{usuarioId}/correo",
            new StringContent(JsonConvert.SerializeObject(registrarCorreoRequest), Encoding.UTF8, "application/json"));

        if (responseCorreo.StatusCode != HttpStatusCode.OK)
        {
            _output.WriteLine("RegistrarCorreo Failed: " + await responseCorreo.Content.ReadAsStringAsync());
        }

        Assert.Equal(HttpStatusCode.OK, responseCorreo.StatusCode);
        var resultCorreo =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseCorreo.Content.ReadAsStringAsync());
        Assert.NotNull(resultCorreo);
        Assert.Equal(nameof(EstatusRegistroEnum.CorreoRegistrado), resultCorreo.Estatus);
        token = resultCorreo.ConcurrencyToken;

        // 5. VerificarCorreo (API Call)
        var verificarCorreoRequest = new ConfirmacionRequest
        {
            Tipo = Tipo2FAEnum.EMAILEnum,
            Codigo = "1234" // Mock accepts any code
        };
        var responseVerificarCorreo = await client.PutAsync($"/{version}/registro/{usuarioId}/confirmar",
            new StringContent(JsonConvert.SerializeObject(verificarCorreoRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.OK, responseVerificarCorreo.StatusCode);
        var resultVerificarCorreo =
            JsonConvert.DeserializeObject<bool>(await responseVerificarCorreo.Content.ReadAsStringAsync());
        Assert.True(resultVerificarCorreo);

        // Fetch fresh token after verify (status likely changed to CorreoConfirmado?)
        var userAfterConfirm = await Context.Usuario.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
        token = userAfterConfirm.ConcurrencyToken;

        // 6. RegistrarBiometricos
        var biometricosRequest = new RegistrarBiometricosRequest
        {
            IdDispositivo = "device123",
            Token = "token123",
            Nombre = "Dispositivo Test",
            Caracteristicas = "Unit Test Specs",
            ConcurrencyToken = token
        };
        var responseBiometricos = await client.PostAsync($"/{version}/registro/{usuarioId}/biometricos",
            new StringContent(JsonConvert.SerializeObject(biometricosRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, responseBiometricos.StatusCode);
        var resultBiometricos =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseBiometricos.Content.ReadAsStringAsync());
        Assert.NotNull(resultBiometricos);
        Assert.Equal(nameof(EstatusRegistroEnum.DatosBiometricosRegistrado), resultBiometricos.Estatus);
        token = resultBiometricos.ConcurrencyToken;

        // 7. AceptarTerminos
        var terminosRequest = new AceptarTerminosRequest
        {
            Version = "1.0",
            AceptoTerminos = true,
            AceptoPrivacidad = true,
            AceptoPld = true,
            ConcurrencyToken = token
        };
        var responseTerminos = await client.PostAsync($"/{version}/registro/{usuarioId}/terminos",
            new StringContent(JsonConvert.SerializeObject(terminosRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, responseTerminos.StatusCode);
        var resultTerminos =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseTerminos.Content.ReadAsStringAsync());
        Assert.NotNull(resultTerminos);
        Assert.Equal(nameof(EstatusRegistroEnum.TerminosCondicionesAceptado), resultTerminos.Estatus);
        token = resultTerminos.ConcurrencyToken;

        // 8. CompletarRegistro
        var completarRequest = new CompletarRegistroRequest
        {
            Contrasena = "Password123!",
            ConfirmacionContrasena = "Password123!",
            ConcurrencyToken = token
        };
        var responseCompletar = await client.PutAsync($"/{version}/registro/{usuarioId}/completar",
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
        var token = resultPre1.ConcurrencyToken;

        // 2. ConfirmarNumero
        var confirmarRequest1 = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.SMSEnum, Codigo = "1234" };
        await client.PutAsync($"/{version}/registro/{usuarioId}/confirmar",
            new StringContent(JsonConvert.SerializeObject(confirmarRequest1), Encoding.UTF8, "application/json"));

        // Fetch fresh
        var userDb = await Context.Usuario.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
        token = userDb.ConcurrencyToken;

        // 3. DatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            Nombre = "Test",
            ApellidoPaterno = "User",
            ApellidoMaterno = "Resume",
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
            FechaNacimiento = new DateTime(1990, 1, 1),
        };
        await client.PostAsync($"/{version}/registro/{usuarioId}/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));

        // --- ROUND 2 (RESUME) ---

        // 4. PreRegistro (Again) - Resets status, updates user
        var responsePre2 = await client.PostAsync($"/{version}/registro/preRegistro",
            new StringContent(JsonConvert.SerializeObject(preRegistroRequest), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, responsePre2.StatusCode);
        var resultPre2 = JsonConvert.DeserializeObject<UsuarioResult>(await responsePre2.Content.ReadAsStringAsync());
        Assert.NotNull(resultPre2);
        Assert.Equal(usuarioId, resultPre2.Id.Value); // Same ID
        Assert.Equal(nameof(EstatusRegistroEnum.PreRegistro), resultPre2.Estatus); // Status reset
        token = resultPre2.ConcurrencyToken;

        // Verify DB side: Should have 2 SMS verifications
        userDb = await Context.Usuario.Include(u => u.Verificaciones2Fa)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);
        Assert.NotNull(userDb);
        Assert.Equal(2, userDb.Verificaciones2Fa.Count(v => v.Tipo == Tipo2FA.Sms));

        // --- COMPLETE FLOW ---

        // 5. ConfirmarNumero (Again)
        var confirmarRequest2 = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.SMSEnum, Codigo = "1234" };
        var responseConf2 = await client.PutAsync($"/{version}/registro/{usuarioId}/confirmar",
            new StringContent(JsonConvert.SerializeObject(confirmarRequest2), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, responseConf2.StatusCode);

        // Fetch fresh
        userDb = await Context.Usuario.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
        token = userDb.ConcurrencyToken;

        // 6. DatosCliente (Again)


        var responseDatos2 = await client.PostAsync($"/{version}/registro/{usuarioId}/datosCliente",
            new StringContent(JsonConvert.SerializeObject(datosClienteRequest), Encoding.UTF8, "application/json"));
        if (responseDatos2.StatusCode != HttpStatusCode.Created)
        {
            _output.WriteLine("Resumed DatosCliente Failed: " + await responseDatos2.Content.ReadAsStringAsync());
        }

        Assert.Equal(HttpStatusCode.Created, responseDatos2.StatusCode);
        var resultDatos2 =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseDatos2.Content.ReadAsStringAsync());
        token = resultDatos2.ConcurrencyToken;

        // 7. RegistrarCorreo
        var correoRequest = new RegistrarCorreoRequest { Correo = "resume@test.com", ConcurrencyToken = token };
        var responseCorreo = await client.PutAsync($"/{version}/registro/{usuarioId}/correo",
            new StringContent(JsonConvert.SerializeObject(correoRequest), Encoding.UTF8, "application/json"));
        var resultCorreo =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseCorreo.Content.ReadAsStringAsync());
        token = resultCorreo.ConcurrencyToken;

        // 8. VerificarCorreo
        var verifCorreoRequest = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.EMAILEnum, Codigo = "1234" };
        await client.PutAsync($"/{version}/registro/{usuarioId}/confirmar",
            new StringContent(JsonConvert.SerializeObject(verifCorreoRequest), Encoding.UTF8, "application/json"));

        // Fetch fresh
        userDb = await Context.Usuario.AsNoTracking().FirstAsync(u => u.Id == usuarioId);
        token = userDb.ConcurrencyToken;

        // 9. RegistrarBiometricos
        var bioRequest = new RegistrarBiometricosRequest
        {
            IdDispositivo = "deviceResume",
            Token = "tokenResume",
            Nombre = "Dispositivo Resume",
            Caracteristicas = "Unit Test Resume",
            ConcurrencyToken = token
        };
        var responseBio = await client.PostAsync($"/{version}/registro/{usuarioId}/biometricos",
            new StringContent(JsonConvert.SerializeObject(bioRequest), Encoding.UTF8, "application/json"));
        var resultBio = JsonConvert.DeserializeObject<UsuarioResult>(await responseBio.Content.ReadAsStringAsync());
        token = resultBio.ConcurrencyToken;

        // 10. Terminos
        var terminosRequest = new AceptarTerminosRequest
        {
            Version = "1", AceptoTerminos = true, AceptoPrivacidad = true, AceptoPld = true, ConcurrencyToken = token
        };
        var responseTerminos = await client.PostAsync($"/{version}/registro/{usuarioId}/terminos",
            new StringContent(JsonConvert.SerializeObject(terminosRequest), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, responseTerminos.StatusCode);
        var resultTerm =
            JsonConvert.DeserializeObject<UsuarioResult>(await responseTerminos.Content.ReadAsStringAsync());
        token = resultTerm.ConcurrencyToken;

        // 11. Completar
        var completarRequest = new CompletarRegistroRequest
            { Contrasena = "Pass123!", ConfirmacionContrasena = "Pass123!", ConcurrencyToken = token };
        var responseFinal = await client.PutAsync($"/{version}/registro/{usuarioId}/completar",
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
        Assert.Contains(ServiceErrorsBuilder.ClienteYaRegistrado, responseContent);
    }
}
