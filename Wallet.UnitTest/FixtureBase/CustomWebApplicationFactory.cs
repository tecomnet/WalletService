using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Wallet.Funcionalidad;
using Moq;
using Wallet.Funcionalidad.ServiceClient;
using Wallet.Funcionalidad.Remoting.REST.TwilioManagement;

namespace Wallet.UnitTest.FixtureBase
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private Action<IServiceCollection>? _configureTestServices;

        public void ConfigureWebApplicationFactory(Action<IServiceCollection> configureTestServices)
        {
            _configureTestServices = configureTestServices;
        }

        public HttpClient CreateAuthenticatedClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(scheme: "TestScheme", parameter: "Token");
            return client;
        }

        public bool UseTestAuth { get; set; } = true;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable(variable: "LogTableName", value: "ServiceLog");
            Environment.SetEnvironmentVariable(variable: "API-Key", value: "14bb0ffb-7503-4fa6-9969-b721635929db");

            builder.ConfigureAppConfiguration(configureDelegate: (context, config) =>
            {
                config.AddInMemoryCollection(initialData: new Dictionary<string, string?>
                {
                    { "twilio-service", "http://localhost:4001" },
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "WalletService" },
                    { "Jwt:Audience", "WalletServiceUser" }
                });
            });

            base.ConfigureWebHost(builder: builder);

            builder.ConfigureTestServices(servicesConfiguration: services =>
            {
                _configureTestServices?.Invoke(obj: services);

                if (UseTestAuth)
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            authenticationScheme: "TestScheme", configureOptions: options => { });

                    services.AddAuthorization(configure: options =>
                    {
                        var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(authenticationSchemes: "TestScheme");
                        defaultAuthorizationPolicyBuilder =
                            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                        options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                    });
                }

                services.AddEmTestServices(configuration: services.BuildServiceProvider().GetService<IConfiguration>() ??
                                                          new ConfigurationBuilder().Build());

                // Mock TwilioServiceFacade
                services.AddScoped<ITwilioServiceFacade>(implementationFactory: sp =>
                {
                    var mock = new Mock<ITwilioServiceFacade>();
                    mock.Setup(expression: x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(value: new VerificacionResult { Sid = "TEST_SID", IsVerified = true });
                    mock.Setup(expression: x => x.VerificacionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(value: new VerificacionResult { Sid = "TEST_SID", IsVerified = true });
                    return mock.Object;
                });
            });

            builder.UseEnvironment(environment: "Development");
        }
    }
}
