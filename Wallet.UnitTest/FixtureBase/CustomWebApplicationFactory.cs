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
                new AuthenticationHeaderValue(scheme: "TestScheme", "Token");
            return client;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("LogTableName", "ServiceLog");
            Environment.SetEnvironmentVariable("API-Key", "14bb0ffb-7503-4fa6-9969-b721635929db");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "twilio-service", "http://localhost:4001" }
                });
            });

            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                _configureTestServices?.Invoke(services);

                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });

                services.AddAuthorization(options =>
                {
                    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder("TestScheme");
                    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                });
                services.AddEmTestServices(services.BuildServiceProvider().GetService<IConfiguration>() ??
                                           new ConfigurationBuilder().Build());

                // Mock TwilioServiceFacade
                services.AddScoped<ITwilioServiceFacade>(sp =>
                {
                    var mock = new Mock<ITwilioServiceFacade>();
                    mock.Setup(x => x.VerificacionSMS(It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new VerificacionResult { Sid = "TEST_SID", IsVerified = true });
                    mock.Setup(x => x.VerificacionEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(new VerificacionResult { Sid = "TEST_SID", IsVerified = true });
                    return mock.Object;
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
