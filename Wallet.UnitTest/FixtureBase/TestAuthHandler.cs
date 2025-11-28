using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wallet.UnitTest.FixtureBase
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options: options, logger: logger, encoder: encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(key: "Authorization"))
            {
                return AuthenticateResult.Fail(failureMessage: "Missing Authorization Header");
            }

            var claims = new[]
            {
                new Claim(type: ClaimTypes.Authentication, value: "TestUser"),
                new Claim(type: "Guid", value: new Guid().ToString()),
                new Claim(type: "Channel", value: "Template"),
                new Claim(type: "UserName", value: "UserNameTest"),
            };
            var identity = new ClaimsIdentity(claims: claims, authenticationType: "Test");
            var principal = new ClaimsPrincipal(identity: identity);
            var ticket = new AuthenticationTicket(principal: principal, authenticationScheme: "TestScheme");

            var result = AuthenticateResult.Success(ticket: ticket);

            return await Task.FromResult(result: result);
        }
    }
}
