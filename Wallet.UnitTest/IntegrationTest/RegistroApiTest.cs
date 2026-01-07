using System.Net;
using Wallet.DOM.Errors;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Wallet.DOM.ApplicationDbContext;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos.GestionUsuario;
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
        Context.Empresa.AddRange(entities: commonSettings.Empresas);
        Context.Estado.AddRange(entities: commonSettings.Estados);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 1. PreRegistro (API Call)
        var preRegistroRequest = new PreRegistroRequest
        {
            CodigoPais = "+52",
            Telefono = "5512345678"
        };
        var responsePreRegistro = await client.PostAsync(requestUri: $"/{version}/registro/preRegistro",
            content: new StringContent(content: JsonConvert.SerializeObject(value: preRegistroRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.Created, actual: responsePreRegistro.StatusCode);
        var resultPreRegistro =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responsePreRegistro.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultPreRegistro);
        Assert.NotNull(value: resultPreRegistro.Id);
        var usuarioId = resultPreRegistro.Id.Value;
        Assert.Equal(expected: nameof(EstatusRegistroEnum.PreRegistro), actual: resultPreRegistro.Estatus);

        // 2. ConfirmarNumero (API Call)
        var confirmarNumeroRequest = new ConfirmacionRequest
        {
            Tipo = Tipo2FAEnum.SMSEnum,
            Codigo = "1234" // Mock accepts any code
        };
        var responseConfirmar = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/confirmar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: confirmarNumeroRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.OK, actual: responseConfirmar.StatusCode);
        var resultConfirmar =
            JsonConvert.DeserializeObject<bool>(value: await responseConfirmar.Content.ReadAsStringAsync());
        Assert.True(condition: resultConfirmar);

        // 3. CompletarDatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            Nombre = "Juan",
            ApellidoPaterno = "Perez",
            ApellidoMaterno = "Lopez",
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
            FechaNacimiento = new DateTime(year: 1990, month: 1, day: 1),
        };
        var responseDatos = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/datosCliente",
            content: new StringContent(content: JsonConvert.SerializeObject(value: datosClienteRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

// ... (In ResumedRegistrationFlow) ...


        // --- COMPLETE FLOW ---
        if (responseDatos.StatusCode != HttpStatusCode.Created) // Changed from OK to CREATED to match assertion
        {
            var errorContent = await responseDatos.Content.ReadAsStringAsync();
            _output.WriteLine(message: $"Error in CompletarDatosCliente: {errorContent}");
        }

        Assert.Equal(expected: HttpStatusCode.Created, actual: responseDatos.StatusCode);
        var resultDatos =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseDatos.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultDatos);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.DatosClienteCompletado), actual: resultDatos.Estatus);

        var userAfterDatos = await Context.Usuario.AsNoTracking().FirstAsync(predicate: u => u.Id == usuarioId);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.DatosClienteCompletado),
            actual: userAfterDatos.Estatus.ToString());

        var token = resultDatos.ConcurrencyToken;

        // 4. RegistrarCorreo
        var registrarCorreoRequest = new RegistrarCorreoRequest
        {
            Correo = "juan.perez@example.com"
        };
        var responseCorreo = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/correo",
            content: new StringContent(content: JsonConvert.SerializeObject(value: registrarCorreoRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        if (responseCorreo.StatusCode != HttpStatusCode.OK)
        {
            _output.WriteLine(message: "RegistrarCorreo Failed: " + await responseCorreo.Content.ReadAsStringAsync());
        }

        Assert.Equal(expected: HttpStatusCode.OK, actual: responseCorreo.StatusCode);
        var resultCorreo =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseCorreo.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultCorreo);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.CorreoRegistrado), actual: resultCorreo.Estatus);
        token = resultCorreo.ConcurrencyToken;

        // 5. VerificarCorreo (API Call)
        var verificarCorreoRequest = new ConfirmacionRequest
        {
            Tipo = Tipo2FAEnum.EMAILEnum,
            Codigo = "1234" // Mock accepts any code
        };
        var responseVerificarCorreo = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/confirmar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: verificarCorreoRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.OK, actual: responseVerificarCorreo.StatusCode);
        var resultVerificarCorreo =
            JsonConvert.DeserializeObject<bool>(value: await responseVerificarCorreo.Content.ReadAsStringAsync());
        Assert.True(condition: resultVerificarCorreo);

        // Fetch fresh token after verify (status likely changed to CorreoConfirmado?)
        var userAfterConfirm = await Context.Usuario.AsNoTracking().FirstAsync(predicate: u => u.Id == usuarioId);
        token = Convert.ToBase64String(inArray: userAfterConfirm.ConcurrencyToken);

        // 6. RegistrarBiometricos
        var biometricosRequest = new RegistrarBiometricosRequest
        {
            IdDispositivo = "device123",
            Token = "token123",
            Nombre = "Dispositivo Test",
            Caracteristicas = "Unit Test Specs"
        };
        var responseBiometricos = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/biometricos",
            content: new StringContent(content: JsonConvert.SerializeObject(value: biometricosRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.Created, actual: responseBiometricos.StatusCode);
        var resultBiometricos =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseBiometricos.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultBiometricos);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.DatosBiometricosRegistrado),
            actual: resultBiometricos.Estatus);
        token = resultBiometricos.ConcurrencyToken;

        // 7. AceptarTerminos
        var terminosRequest = new AceptarTerminosRequest
        {
            Version = "1.0",
            AceptoTerminos = true,
            AceptoPrivacidad = true,
            AceptoPld = true
        };
        var responseTerminos = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/terminos",
            content: new StringContent(content: JsonConvert.SerializeObject(value: terminosRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.Created, actual: responseTerminos.StatusCode);
        var resultTerminos =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseTerminos.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultTerminos);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.TerminosCondicionesAceptado), actual: resultTerminos.Estatus);
        token = resultTerminos.ConcurrencyToken;

        // 8. CompletarRegistro
        var completarRequest = new CompletarRegistroRequest
        {
            Contrasena = "Password123!",
            ConfirmacionContrasena = "Password123!"
        };
        var responseCompletar = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/completar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: completarRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.OK, actual: responseCompletar.StatusCode);
        var resultCompletar =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseCompletar.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultCompletar);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.RegistroCompletado), actual: resultCompletar.Estatus);
    }

    [Fact]
    public async Task ResumedRegistrationFlow_ShouldSucceed()
    {
        // 0. Seed Company and State
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(entities: commonSettings.Empresas);
        Context.Estado.AddRange(entities: commonSettings.Estados);
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
        var responsePre1 = await client.PostAsync(requestUri: $"/{version}/registro/preRegistro",
            content: new StringContent(content: JsonConvert.SerializeObject(value: preRegistroRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));
        Assert.Equal(expected: HttpStatusCode.Created, actual: responsePre1.StatusCode);
        var resultPre1 =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responsePre1.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultPre1);
        Assert.NotNull(value: resultPre1.Id);
        var usuarioId = resultPre1.Id.Value;
        var token = resultPre1.ConcurrencyToken;

        // 2. ConfirmarNumero
        var confirmarRequest1 = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.SMSEnum, Codigo = "1234" };
        await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/confirmar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: confirmarRequest1),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        // Fetch fresh
        var userDb = await Context.Usuario.AsNoTracking().FirstAsync(predicate: u => u.Id == usuarioId);
        token = Convert.ToBase64String(inArray: userDb.ConcurrencyToken);

        // 3. DatosCliente
        var datosClienteRequest = new DatosClienteRequest
        {
            Nombre = "Test",
            ApellidoPaterno = "User",
            ApellidoMaterno = "Resume",
            Genero = GeneroEnum.MasculinoEnum,
            NombreEstado = "Campeche",
            FechaNacimiento = new DateTime(year: 1990, month: 1, day: 1),
        };
        await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/datosCliente",
            content: new StringContent(content: JsonConvert.SerializeObject(value: datosClienteRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        // --- ROUND 2 (RESUME) ---

        // 4. PreRegistro (Again) - Resets status, updates user
        var responsePre2 = await client.PostAsync(requestUri: $"/{version}/registro/preRegistro",
            content: new StringContent(content: JsonConvert.SerializeObject(value: preRegistroRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.Created, actual: responsePre2.StatusCode);
        var resultPre2 =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responsePre2.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultPre2);
        Assert.Equal(expected: usuarioId, actual: resultPre2.Id.Value); // Same ID
        Assert.Equal(expected: nameof(EstatusRegistroEnum.PreRegistro), actual: resultPre2.Estatus); // Status reset
        token = resultPre2.ConcurrencyToken;

        // Verify DB side: Should have 2 SMS verifications
        userDb = await Context.Usuario.Include(navigationPropertyPath: u => u.Verificaciones2Fa)
            .FirstOrDefaultAsync(predicate: u => u.Id == usuarioId);
        Assert.NotNull(@object: userDb);
        Assert.Equal(expected: 2, actual: userDb.Verificaciones2Fa.Count(predicate: v => v.Tipo == Tipo2FA.Sms));

        // --- COMPLETE FLOW ---

        // 5. ConfirmarNumero (Again)
        var confirmarRequest2 = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.SMSEnum, Codigo = "1234" };
        var responseConf2 = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/confirmar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: confirmarRequest2),
                encoding: Encoding.UTF8, mediaType: "application/json"));
        Assert.Equal(expected: HttpStatusCode.OK, actual: responseConf2.StatusCode);

        // Fetch fresh
        userDb = await Context.Usuario.AsNoTracking().FirstAsync(predicate: u => u.Id == usuarioId);
        token = Convert.ToBase64String(inArray: userDb.ConcurrencyToken);

        // 6. DatosCliente (Again)


        var responseDatos2 = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/datosCliente",
            content: new StringContent(content: JsonConvert.SerializeObject(value: datosClienteRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));
        if (responseDatos2.StatusCode != HttpStatusCode.Created)
        {
            _output.WriteLine(message: "Resumed DatosCliente Failed: " +
                                       await responseDatos2.Content.ReadAsStringAsync());
        }

        Assert.Equal(expected: HttpStatusCode.Created, actual: responseDatos2.StatusCode);
        var resultDatos2 =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseDatos2.Content.ReadAsStringAsync());
        token = resultDatos2.ConcurrencyToken;

        // 7. RegistrarCorreo
        var correoRequest = new RegistrarCorreoRequest { Correo = "resume@test.com" };
        var responseCorreo = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/correo",
            content: new StringContent(content: JsonConvert.SerializeObject(value: correoRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));
        var resultCorreo =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseCorreo.Content.ReadAsStringAsync());
        token = resultCorreo.ConcurrencyToken;

        // 8. VerificarCorreo
        var verifCorreoRequest = new ConfirmacionRequest
            { Tipo = Tipo2FAEnum.EMAILEnum, Codigo = "1234" };
        await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/confirmar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: verifCorreoRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        // Fetch fresh
        userDb = await Context.Usuario.AsNoTracking().FirstAsync(predicate: u => u.Id == usuarioId);
        token = Convert.ToBase64String(inArray: userDb.ConcurrencyToken);

        // 9. RegistrarBiometricos
        var bioRequest = new RegistrarBiometricosRequest
        {
            IdDispositivo = "deviceResume",
            Token = "tokenResume",
            Nombre = "Dispositivo Resume",
            Caracteristicas = "Unit Test Resume"
        };
        var responseBio = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/biometricos",
            content: new StringContent(content: JsonConvert.SerializeObject(value: bioRequest), encoding: Encoding.UTF8,
                mediaType: "application/json"));
        var resultBio =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseBio.Content.ReadAsStringAsync());
        token = resultBio.ConcurrencyToken;

        // 10. Terminos
        var terminosRequest = new AceptarTerminosRequest
        {
            Version = "1", AceptoTerminos = true, AceptoPrivacidad = true, AceptoPld = true
        };
        var responseTerminos = await client.PostAsync(requestUri: $"/{version}/registro/{usuarioId}/terminos",
            content: new StringContent(content: JsonConvert.SerializeObject(value: terminosRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));
        Assert.Equal(expected: HttpStatusCode.Created, actual: responseTerminos.StatusCode);
        var resultTerm =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseTerminos.Content.ReadAsStringAsync());
        token = resultTerm.ConcurrencyToken;

        // 11. Completar
        var completarRequest = new CompletarRegistroRequest
            { Contrasena = "Pass123!", ConfirmacionContrasena = "Pass123!" };
        var responseFinal = await client.PutAsync(requestUri: $"/{version}/registro/{usuarioId}/completar",
            content: new StringContent(content: JsonConvert.SerializeObject(value: completarRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        Assert.Equal(expected: HttpStatusCode.OK, actual: responseFinal.StatusCode);
        var resultFinal =
            JsonConvert.DeserializeObject<UsuarioResult>(value: await responseFinal.Content.ReadAsStringAsync());
        Assert.NotNull(@object: resultFinal);
        Assert.Equal(expected: nameof(EstatusRegistroEnum.RegistroCompletado), actual: resultFinal.Estatus);
    }

    [Fact]
    public async Task Registration_ShouldFail_WhenUserAlreadyCompleted()
    {
        // 0. Seed Company and State
        var commonSettings = new CommonSettings();
        Context.Empresa.AddRange(entities: commonSettings.Empresas);
        Context.Estado.AddRange(entities: commonSettings.Estados);
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
        Context.Usuario.Add(entity: completedUser);
        await Context.SaveChangesAsync();

        var client = Factory.CreateClient();
        var version = "0.1";

        // 2. Attempt PreRegistro with same phone
        var preRegistroRequest = new PreRegistroRequest
        {
            CodigoPais = "+52",
            Telefono = "5511223344"
        };
        var response = await client.PostAsync(requestUri: $"/{version}/registro/preRegistro",
            content: new StringContent(content: JsonConvert.SerializeObject(value: preRegistroRequest),
                encoding: Encoding.UTF8, mediaType: "application/json"));

        // 3. Assert Failure
        Assert.False(condition: response.IsSuccessStatusCode, userMessage: "Should return error status code");

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains(expectedSubstring: ServiceErrorsBuilder.ClienteYaRegistrado, actualString: responseContent);
    }
}
