using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using CTROLibrary.Model;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using CTROLibrary.Repository;
using Microsoft.AspNet.Identity.EntityFramework;
using CTROLibrary.DBbase;
using System.Security.Claims;
using CTROReporting.ViewModels;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace CTROReporting
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public void ConfigureOAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/ApplicationUser/Login")
                //ExpireTimeSpan = TimeSpan.FromMinutes(30)
                //Provider = new CookieAuthenticationProvider
                //{
                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.  
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                //        validateInterval: TimeSpan.FromMinutes(30),
                //        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //}
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();


            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            ApplicationUser client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            using (AuthRepository _repo = new AuthRepository())
            {
                client = _repo.FindClient(context.ClientId);
            }

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (!client.Activated)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            using (AuthRepository _repo = new AuthRepository())
            {
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("sub", context.UserName));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    {
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(identity);

        }
    }

    public class AuthRepository : IDisposable
    {
        private EntitiesInitial _ctx;

        private UserManager<ApplicationUser> _userManager;

        public AuthRepository()
        {
            _ctx = new EntitiesInitial();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);
            return result;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public ApplicationUser FindClient(string clientId)
        {
            var client = _ctx.Users.Find(clientId);
            return client;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}