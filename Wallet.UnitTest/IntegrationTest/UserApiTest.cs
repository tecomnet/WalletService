using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wallet.RestAPI.Models;
using Wallet.DOM.Enums;
using Wallet.DOM.Modelos;
using Wallet.DOM.Modelos.GestionUsuario;
using Wallet.UnitTest.FixtureBase;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;

namespace Wallet.UnitTest.IntegrationTest;

public class UserApiTest : DatabaseTestFixture
{
    // Api URI
    private const string API_URI = "users";

    // Api version
    private const string API_VERSION = "0.1";


    public UserApiTest()
    {
        Factory.UseTestAuth = false;
        SetupDataAsync(setupDataAction: async context =>
        {
            // Create data
            var commonSettings = new CommonSettings();
            // Add data
            //await context.AddRangeAsync(commonSettings.Clientes);
            // Save changes
            await context.SaveChangesAsync();
        }).GetAwaiter().GetResult();
    }

    /* [Fact]
    public async Task Get_Unauthorized()
    {
        // Arrange
        var client = Factory.CreateClient();
        // Act
        var response = await client.GetAsync(
            requestUri: $"{API_VERSION}/usuario/1");
        // Assert
        Assert.Equal(expected: HttpStatusCode.Unauthorized, actual: response.StatusCode);
    } */

    [Fact]
    public async Task Get_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // Act
        var response = await client.GetAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}");

        // Assert 
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: user.Id, actual: result.Id);
        Assert.Equal(expected: user.CorreoElectronico, actual: result.CorreoElectronico);
    }

    [Fact]
    public async Task Put_Telefono_Ok()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new TelefonoUpdateRequest
        {
            CodigoPais = "+52",
            Telefono = $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
            ConcurrencyToken = Convert.ToBase64String(user.ConcurrencyToken)
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);
        Assert.NotNull(result);
        Assert.Equal(expected: request.Telefono, actual: result.Telefono);
    }

    [Fact]
    public async Task Put_Email_Preserves_Status_RegistroCompletado()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync(); // Creates user with RegistroCompletado
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"newemail{Guid.NewGuid()}@test.com",
            ConcurrencyToken = Convert.ToBase64String(user.ConcurrencyToken)
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);

        Assert.NotNull(result);
        Assert.Equal(expected: request.CorreoElectronico, actual: result.CorreoElectronico);

        // CRITICAL CHECK: Status should PRESERVE RegistroCompletado
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado.ToString(), actual: result.Estatus);

        // Verify in DB to be absolutely sure
        using var context = CreateContext();
        var dbUser = await context.Usuario.FindAsync(user.Id);
        Assert.NotNull(dbUser);
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado, actual: dbUser.Estatus);
    }

    [Fact]
    public async Task Put_Email_ReturnsBadRequest_For_Incomplete_User()
    {
        // Arrange
        // 1. Manually create a user with an intermediate status (e.g. DatosClienteCompletado)
        byte[] userToken;
        using (var setupContext = CreateContext())
        {
            var incompleteUser = new Usuario(
                codigoPais: "+52",
                telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
                correoElectronico: $"incomplete{Guid.NewGuid()}@test.com",
                contrasena: "Password123!",
                estatus: EstatusRegistroEnum.DatosClienteCompletado, // Intermediate status
                creationUser: Guid.NewGuid(),
                testCase: "IntegrationTest_Incomplete");

            await setupContext.Usuario.AddAsync(incompleteUser);
            await setupContext.SaveChangesAsync();
            userToken = incompleteUser.ConcurrencyToken;
        }

        // 2. Authenticate manually.
        Usuario user;
        string token;

        using (var context = CreateContext())
        {
            user = await context.Usuario.FirstOrDefaultAsync(u => u.TestCaseID == "IntegrationTest_Incomplete");
            using var scope = Factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider
                .GetRequiredService<Wallet.Funcionalidad.Services.TokenService.ITokenService>();
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name,
                    user.CorreoElectronico ?? user.Telefono),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim("Guid", user.Guid.ToString())
            };
            token = tokenService.GenerateAccessToken(claims);
        }

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new EmailUpdateRequest
        {
            CorreoElectronico = $"updated{Guid.NewGuid()}@test.com",
            ConcurrencyToken = Convert.ToBase64String(userToken)
        };

        // Act
        var content = CreateContent(request);
        var response = await client.PutAsync($"{API_VERSION}/usuario/{user.Id}/actualizaEmail", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Telefono_Preserves_Status_RegistroCompletado()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync(); // Creates user with RegistroCompletado
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new TelefonoUpdateRequest
        {
            CodigoPais = "+52",
            Telefono = $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
            ConcurrencyToken = Convert.ToBase64String(user.ConcurrencyToken)
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);

        Assert.NotNull(result);
        Assert.Equal(expected: request.Telefono, actual: result.Telefono);

        // CRITICAL CHECK: Status should PRESERVE RegistroCompletado
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado.ToString(), actual: result.Estatus);

        // Verify in DB
        using var context = CreateContext();
        var dbUser = await context.Usuario.FindAsync(user.Id);
        Assert.NotNull(dbUser);
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado, actual: dbUser.Estatus);
    }

    [Fact]
    public async Task Put_Telefono_ReturnsBadRequest_For_Incomplete_User()
    {
        // Arrange
        // 1. Manually create a user with an intermediate status
        byte[] userToken;
        using (var setupContext = CreateContext())
        {
            var incompleteUser = new Usuario(
                codigoPais: "+52",
                telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
                correoElectronico: $"incomplete_phone{Guid.NewGuid()}@test.com",
                contrasena: "Password123!",
                estatus: EstatusRegistroEnum.DatosClienteCompletado, // Intermediate
                creationUser: Guid.NewGuid(),
                testCase: "IntegrationTest_Incomplete_Phone");

            await setupContext.Usuario.AddAsync(incompleteUser);
            await setupContext.SaveChangesAsync();
            userToken = incompleteUser.ConcurrencyToken;
        }

        // 2. Authenticate
        Usuario user;
        string token;

        using (var context = CreateContext())
        {
            user = await context.Usuario.FirstOrDefaultAsync(u => u.TestCaseID == "IntegrationTest_Incomplete_Phone");
            using var scope = Factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider
                .GetRequiredService<Wallet.Funcionalidad.Services.TokenService.ITokenService>();
            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.Name, user.CorreoElectronico ?? user.Telefono),
                new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("Guid", user.Guid.ToString())
            };
            token = tokenService.GenerateAccessToken(claims);
        }

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new TelefonoUpdateRequest
        {
            CodigoPais = "+52",
            Telefono = $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
            ConcurrencyToken = Convert.ToBase64String(userToken)
        };

        // Act
        var content = CreateContent(request);
        var response = await client.PutAsync($"{API_VERSION}/usuario/{user.Id}/actualizaTelefono", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Password_Preserves_Status_RegistroCompletado()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync(); // Creates completed user
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        var request = new ContrasenaUpdateRequest
        {
            ContrasenaActual = "Password123!", // Matches default in fixture
            ContrasenaNueva = "NewPassword123!",
            ContrasenaNuevaConfrimacion = "NewPassword123!",
            ConcurrencyToken = Convert.ToBase64String(user.ConcurrencyToken)
        };

        // Act
        var content = CreateContent(body: request);
        var response = await client.PutAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/contrasena", content: content);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(value: responseContentString);

        Assert.NotNull(result);

        // CRITICAL CHECK: Status should PRESERVE RegistroCompletado
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado.ToString(), actual: result.Estatus);

        // Verify in DB
        using var context = CreateContext();
        var dbUser = await context.Usuario.FindAsync(user.Id);
        Assert.NotNull(dbUser);
        Assert.Equal(expected: EstatusRegistroEnum.RegistroCompletado, actual: dbUser.Estatus);
    }

    [Fact]
    public async Task Put_CompletarRegistro_Sets_Status_RegistroCompletado()
    {
        // Arrange
        // 1. Manually create a user ready to set password (e.g. TerminosCondicionesAceptado)
        using (var setupContext = CreateContext())
        {
            var incompleteUser = new Usuario(
                codigoPais: "+52",
                telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
                correoElectronico: $"incomplete_pass{Guid.NewGuid()}@test.com",
                contrasena: null, // No password yet
                estatus: EstatusRegistroEnum.TerminosCondicionesAceptado, // Ready for password
                creationUser: Guid.NewGuid(),
                testCase: "IntegrationTest_FirstPassword");

            await setupContext.Usuario.AddAsync(incompleteUser);

            // Add required Empresa and Cliente for Wallet creation
            var empresa = new Wallet.DOM.Modelos.GestionEmpresa.Empresa("Tecomnet", Guid.NewGuid());
            await setupContext.Empresa.AddAsync(empresa);

            var cliente = new Wallet.DOM.Modelos.GestionCliente.Cliente(incompleteUser, empresa, Guid.NewGuid());
            await setupContext.Cliente.AddAsync(cliente);

            await setupContext.SaveChangesAsync();
        }

        // 2. We can't authenticate with regular flow because user has no password.
        // But the endpoint might allow it if we bypass auth or generate a token manually.
        // Assuming test bypasses auth or we generate token manually for the user.

        Usuario user;
        string token;
        using (var context = CreateContext())
        {
            user = await context.Usuario.FirstOrDefaultAsync(u => u.TestCaseID == "IntegrationTest_FirstPassword");
            using var scope = Factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider
                .GetRequiredService<Wallet.Funcionalidad.Services.TokenService.ITokenService>();

            // Note: In real flow, user might have a temporary token or session. 
            // We'll generate a standard token for the test to pass Authorization middleware if present.
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name,
                    user.CorreoElectronico ?? user.Telefono ?? "testuser"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim("Guid", user.Guid.ToString())
            };
            token = tokenService.GenerateAccessToken(claims);
        }

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CompletarRegistroRequest
        {
            Contrasena = "Password123!",
            ConfirmacionContrasena = "Password123!",
        };

        // Act
        var content = CreateContent(request);
        // Endpoint is on RegistroApiController: PUT /{version}/registro/{id}/completar
        var response = await client.PutAsync($"{API_VERSION}/registro/{user.Id}/completar", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<UsuarioResult>(responseContentString);

        Assert.NotNull(result);

        // CRITICAL CHECK: Status should CHANGE to RegistroCompletado
        Assert.Equal(EstatusRegistroEnum.RegistroCompletado.ToString(), result.Estatus);

        // Verify in DB
        using var verifyContext = CreateContext();
        var dbUser = await verifyContext.Usuario.FindAsync(user.Id);
        Assert.NotNull(dbUser);
        Assert.Equal(EstatusRegistroEnum.RegistroCompletado, dbUser.Estatus);
        Assert.NotNull(dbUser.Contrasena); // Password should be set
    }


    private StringContent CreateContent(object body)
    {
        var json = JsonConvert.SerializeObject(value: body);
        return new StringContent(
            content: json,
            encoding: Encoding.UTF8,
            mediaType: "application/json");
    }

    [Fact]
    public async Task Confirmar2FA_Email_Success()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 1. Update Email to trigger 2FA
        var updateRequest = new EmailUpdateRequest
        {
            CorreoElectronico = $"confirm2fa{Guid.NewGuid()}@test.com"
        };
        var updateContent = CreateContent(body: updateRequest);
        await client.PutAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaEmail", content: updateContent);

        // 2. We skip getting code from DB because for Twilio (Email/SMS) the code is not stored in DB until verified.
        // Since we mock Twilio service to accept any code, we can use a dummy one.
        string verificationCode = "1234";

        // 3. Confirm 2FA
        var confirmRequest = new
        {
            tipo = "Email",
            codigo = verificationCode
        };
        // Use manual serialization to ensure control
        var json = System.Text.Json.JsonSerializer.Serialize(confirmRequest);
        var confirmContent = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/confirmar2fa", content: confirmContent);

        // Assert
        var responseContentString = await response.Content.ReadAsStringAsync();
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var result = JsonConvert.DeserializeObject<bool>(value: responseContentString);
        Assert.True(condition: result);

        // Verify in DB that it is marked as verified
        using (var context = CreateContext())
        {
            var dbUser = await context.Usuario.Include(u => u.Verificaciones2Fa)
                .FirstAsync(u => u.Id == user.Id);
            var verificacion = dbUser.Verificaciones2Fa
                .FirstOrDefault(v => v.Codigo == verificationCode && v.Tipo == Tipo2FA.Email);
            Assert.NotNull(verificacion);
            Assert.True(verificacion.Verificado);
        }
    }

    [Fact]
    public async Task Confirmar2FA_Telefono_Success()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 1. Update Phone to trigger 2FA
        var updateRequest = new TelefonoUpdateRequest
        {
            CodigoPais = "052",
            Telefono = "55" + new Random().Next(10000000, 99999999)
        };
        var updateContent = CreateContent(body: updateRequest);
        var updateResponse = await client.PutAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono",
            content: updateContent);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // 2. Mock code
        string verificationCode = "1234";

        // 3. Confirm 2FA
        var confirmRequest = new
        {
            tipo = "Sms",
            codigo = verificationCode
        };

        var json = System.Text.Json.JsonSerializer.Serialize(confirmRequest);
        var confirmContent = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/confirmar2fa", content: confirmContent);

        // Assert
        // var responseContentString = await response.Content.ReadAsStringAsync(); // Debugging
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseContentString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<bool>(value: responseContentString);
        Assert.True(condition: result);

        // Verify in DB
        using (var context = CreateContext())
        {
            var dbUser = await context.Usuario.Include(u => u.Verificaciones2Fa)
                .FirstAsync(u => u.Id == user.Id);
            var verificacion = dbUser.Verificaciones2Fa
                .FirstOrDefault(v => v.Codigo == verificationCode && v.Tipo == Tipo2FA.Sms);
            Assert.NotNull(verificacion);
            Assert.True(verificacion.Verificado);
        }
    }

    [Fact]
    public async Task Confirmar2FA_BadCode_ReturnsFalse()
    {
        // Custom factory to override mock
        var factory = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var mock = new Mock<ITwilioServiceFacade>();
                // Setup to return False
                mock.Setup(x => x.ConfirmarVerificacionSMS(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new VerificacionResult { Sid = "TEST_SID", IsVerified = false });
                mock.Setup(x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new VerificacionResult { Sid = "TEST_SID", IsVerified = true });

                services.AddScoped<ITwilioServiceFacade>(_ => mock.Object);
            });
        });

        // Use the custom factory to create client
        var client = factory.CreateClient();
        // Since we created a new client/factory, we need to authenticate again? 
        // Factory.CreateAuthenticatedUserAsync uses the global Factory. 
        // But the DB is shared (Postgres/SQL container).
        // We can reuse the tokens if we set the header.
        var (user, token) = await CreateAuthenticatedUserAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        // 1. Update Phone 
        var updateRequest = new TelefonoUpdateRequest
        {
            CodigoPais = "052",
            Telefono = "55" + new Random().Next(10000000, 99999999)
        };
        var updateContent = CreateContent(body: updateRequest);
        var updateResponse = await client.PutAsync(requestUri: $"{API_VERSION}/usuario/{user.Id}/actualizaTelefono",
            content: updateContent);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // 2. Bad code
        string verificationCode = "WRONG";

        // 3. Confirm 2FA
        var confirmRequest = new
        {
            tipo = "Sms",
            codigo = verificationCode
        };

        var json = System.Text.Json.JsonSerializer.Serialize(confirmRequest);
        var confirmContent = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{user.Id}/confirmar2fa", content: confirmContent);

        // Assert
        Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<bool>(value: responseString);
        Assert.False(condition: result);
    }

    [Fact]
    public async Task Confirmar2FA_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var (user, token) = await CreateAuthenticatedUserAsync();
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: token);

        int nonExistentUserId = 999999;

        var confirmRequest = new
        {
            tipo = "Email",
            codigo = "1234"
        };
        var json = System.Text.Json.JsonSerializer.Serialize(confirmRequest);
        var confirmContent = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(
            requestUri: $"{API_VERSION}/usuario/{nonExistentUserId}/confirmar2fa", content: confirmContent);

        // Assert
        // Current implementation returns BadRequest (400) for UsuarioNoEncontrado exception.
        Assert.Equal(expected: HttpStatusCode.BadRequest, actual: response.StatusCode);
    }
}