using DripChipWebApplication.Server.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DripChipWebApplication.Server.Services
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BasicAuthorisation : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly IAccountService _accountService;



        public BasicAuthorisation(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IAccountService accountService)
        : base(options, logger, encoder, clock)
        {
            _accountService= accountService;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Name, "guest"),
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");
                var email = credentials[0];
                var password = credentials[1];

                var user = _accountService.Authenticate(email, password);

                if (user == null)
                {
                    return AuthenticateResult.Fail("Неверные авторизационные данные");
                }
                else
                {
                   
                }
                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);

            }
            catch
            {
                return AuthenticateResult.Fail("Ошибка заголовка");
            }
        }
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthorisation>();
        }
    }
}
