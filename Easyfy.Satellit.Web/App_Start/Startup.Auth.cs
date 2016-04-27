using System;
using System.Configuration;
using System.Threading.Tasks;
using Easyfy.Satellit.Model.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Easyfy.Satellit.Web.Models;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;

namespace Easyfy.Satellit.Web
{
  public partial class Startup
  {
    internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

    const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
    //const string ignoreClaimPrefix = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";

    // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
    public void ConfigureAuth(IAppBuilder app)
    {
      // Configure the db context, user manager and signin manager to use a single instance per request

      DataProtectionProvider = app.GetDataProtectionProvider();

      app.CreatePerOwinContext(() => ServiceLocator.Current.GetInstance<IDocumentSession>());
      app.CreatePerOwinContext(() => ServiceLocator.Current.GetInstance<ApplicationUserManager>());
      //app.CreatePerOwinContext(() => ServiceLocator.Current.GetInstance<ApplicationSignInManager>());

      //// Enable the application to use a cookie to store information for the signed in user
      app.UseCookieAuthentication(new CookieAuthenticationOptions
      {
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/loggain/"),
        CookieName = ConfigurationManager.AppSettings["RavenDb.DefaultDatabase"] ?? "Easyfy.Auth",
        Provider = new CookieAuthenticationProvider
        {
          // Enables the application to validate the security stamp when the user logs in.
          // This is a security feature which is used when you change a password or add an external login to your account.  
          OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
              validateInterval: TimeSpan.FromMinutes(30),
              regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
        }

      });

      // Use a cookie to temporarily store information about a user logging in with a third party login provider
      app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

      // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
      app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

      // Enables the application to remember the second login verification factor such as phone or email.
      // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
      // This is similar to the RememberMe option when you log in.
      app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

      // Uncomment the following lines to enable logging in with third party login providers

      if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("Microsoft.ClientId")))
      {
        app.UseMicrosoftAccountAuthentication(
          clientId: ConfigurationManager.AppSettings.Get("Microsoft.ClientId"),
          clientSecret: ConfigurationManager.AppSettings.Get("Microsoft.ClientSecret"));
      }

      if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("Google.ClientId")))
      {
        var googleOption = new GoogleOAuth2AuthenticationOptions()
        {
          ClientId = ConfigurationManager.AppSettings.Get("Google.ClientId"),
          ClientSecret = ConfigurationManager.AppSettings.Get("Google.ClientSecret"),
        };

        googleOption.Scope.Add("email");

        app.UseGoogleAuthentication(googleOption);
      }

      if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("Twitter.ConsumerKey")))
      {
        app.UseTwitterAuthentication(
          consumerKey: ConfigurationManager.AppSettings["Twitter.ConsumerKey"],
          consumerSecret: ConfigurationManager.AppSettings["Twitter.ConsumerSecret"]);
      }

      if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("Facebook.AppId")))
      {
        var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
        {
          AppId = ConfigurationManager.AppSettings.Get("Facebook.AppId"),
          AppSecret = ConfigurationManager.AppSettings.Get("Facebook.AppSecret"),
          Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
          {
            OnAuthenticated = (context) =>
            {
              context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));

              foreach (var x in context.User)
              {
                var claimType = string.Format("urn:facebook:{0}", x.Key);
                string claimValue = x.Value.ToString();
                if (!context.Identity.HasClaim(claimType, claimValue))
                  context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, XmlSchemaString, "Facebook"));

              }
              return Task.FromResult(0);
            }
          }

        };
        facebookOptions.Scope.Add("email");

        app.UseFacebookAuthentication(facebookOptions);
      }
    }
  }
}