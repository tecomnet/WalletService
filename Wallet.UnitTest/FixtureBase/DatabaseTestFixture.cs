using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Funcionalidad;
using Wallet.DOM.ApplicationDbContext;
using Wallet.RestAPI;
using Wallet.DOM.Modelos;
using Wallet.DOM.Helper;
using Respawn;

namespace Wallet.UnitTest.FixtureBase
{
    [Collection(name: "FunctionalCollection")]
    public class DatabaseTestFixture : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly CustomWebApplicationFactory<Program> Factory;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // Static fields to ensure singleton behavior across test instances
        private static bool _isInitialized;
        private static Respawner? _respawner;
        private static readonly object _lock = new();

        public DatabaseTestFixture()
        {
            // Build configuration with User Secrets
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<DatabaseTestFixture>()
                .AddEnvironmentVariables()
                .AddInMemoryCollection(initialData: new Dictionary<string, string?>
                {
                    { "Jwt:Key", "SuperSecretKeyForTestingPurposesOnly123!" },
                    { "Jwt:Issuer", "WalletService" },
                    { "Jwt:Audience", "WalletServiceUser" }
                });
            _configuration = builder.Build();

            _connectionString = DbConnectionHelper.GetConnectionString(configuration: _configuration);

            SetEnvironmentalVariables();
            var factory = new CustomWebApplicationFactory<Program>();
            factory.ConfigureWebApplicationFactory(configureTestServices: services =>
            {
                services.AddSingleton(implementationInstance: _configuration);
            });
            Factory = factory;

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            lock (_lock)
            {
                if (!_isInitialized)
                {
                    using var context = CreateContext();
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();

                    // Initialize Respawner
                    _respawner = Respawner.CreateAsync(_connectionString, new RespawnerOptions
                    {
                        TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
                        WithReseed = true
                    }).GetAwaiter().GetResult();

                    _isInitialized = true;
                }
            }

            // Reset database for the current test
            if (_respawner != null)
            {
                _respawner.ResetAsync(_connectionString).GetAwaiter().GetResult();
            }
        }

        protected async Task SetupDataAsync(Func<ServiceDbContext, Task> setupDataAction)
        {
            await using var context = CreateContext();
            await setupDataAction(arg: context);
        }

        protected internal ServiceDbContext CreateContext()
            => new(
                options: new DbContextOptionsBuilder<ServiceDbContext>()
                    .UseSqlServer(connectionString: _connectionString,
                        sqlServerOptionsAction: optionsBuilder =>
                            optionsBuilder.UseQuerySplittingBehavior(
                                querySplittingBehavior: QuerySplittingBehavior.SplitQuery))
                    .Options);

        public async Task<(Usuario User, string Token)> CreateAuthenticatedUserAsync()
        {
            await using var context = CreateContext();
            var user = new Usuario(
                codigoPais: "+52",
                telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
                correoElectronico: $"user{Guid.NewGuid()}@test.com",
                contrasena: "Password123!",
                estatus: "Activo",
                creationUser: Guid.NewGuid(),
                testCase: "IntegrationTest");

            await context.Usuario.AddAsync(entity: user);
            await context.SaveChangesAsync();

            using var scope = Factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider
                .GetRequiredService<Wallet.Funcionalidad.Services.TokenService.ITokenService>();

            var claims = new List<System.Security.Claims.Claim>
            {
                new(type: System.Security.Claims.ClaimTypes.Name, value: user.CorreoElectronico ?? user.Telefono),
                new(type: System.Security.Claims.ClaimTypes.NameIdentifier, value: user.Id.ToString())
            };

            var token = tokenService.GenerateAccessToken(claims: claims);

            return (user, token);
        }

        public async Task<List<Usuario>> CreateUsersAsync(int count)
        {
            await using var context = CreateContext();
            var users = new List<Usuario>();
            for (var i = 0; i < count; i++)
            {
                users.Add(item: new Usuario(
                    codigoPais: "+52",
                    telefono: $"9{new Random().Next(minValue: 100000000, maxValue: 999999999)}",
                    correoElectronico: $"user{Guid.NewGuid()}@test.com",
                    contrasena: "Password123!",
                    estatus: "Activo",
                    creationUser: Guid.NewGuid(),
                    testCase: "IntegrationTest"));
            }

            await context.Usuario.AddRangeAsync(entities: users);
            await context.SaveChangesAsync();
            return users;
        }

        private void SetEnvironmentalVariables()
        {
            Environment.SetEnvironmentVariable(
                variable: "dbConnectionString",
                value: _connectionString);
        }
    }
}