using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(EduVerse.Startup))]

namespace EduVerse
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable cookie authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Account/Register")
            });

            // Configure Google authentication
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "1029790467333-q3ts5diolji6l0oipkd2j09cjt4000si.apps.googleusercontent.com",
                ClientSecret = "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",
                CallbackPath = new PathString("/signin-google"),

                // ✅ Tell OWIN where to sign the user in after external login
                SignInAsAuthenticationType = "ApplicationCookie",


                ////For cookie handling
                //Provider = new GoogleOAuth2AuthenticationProvider()
                //{
                //    OnAuthenticated = context =>
                //    {
                //        // Example of creating a local identity for the user after successful authentication
                //        var identity = new ClaimsIdentity(context.Identity.Claims, "ApplicationCookie");
                //        context.OwinContext.Authentication.SignIn(identity);
                //        return Task.CompletedTask;
                //    }
                //}
            });
        }
    }
}
