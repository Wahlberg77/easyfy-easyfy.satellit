using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Easyfy.Satellit.Contracts;
using Easyfy.Satellit.Model.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Practices.ServiceLocation;
using NLog;
using Raven.Client;
using RavenDB.AspNet.Identity;

namespace Easyfy.Satellit.Web
{
  public class EmailService : IIdentityMessageService
  {
    private Logger _logger;

    public EmailService()
    {
      _logger = ServiceLocator.Current.GetInstance<Logger>();
    }
    public async Task SendAsync(IdentityMessage message)
    {
      // Plug in your email service here to send an email.
      await ConfigMandrillAsync(message);
    }

    private async Task ConfigMandrillAsync(IdentityMessage message)
    {
      // message.Body contains url for the customers action.
      // message.Destination contains customers email
      // message.Subject contains template name

      var templateName = message.Subject;
      var sendToEmail = message.Destination;
      var customerEmailLink = message.Body;

      var mailService = ServiceLocator.Current.GetInstance<IMailService>();

      var mailMessage = new MailMessage(ConfigurationManager.AppSettings["Mandrill.MailFromEmail"], sendToEmail);

      var mergeVariables = new Dictionary<string, string>();
      if (templateName == "template-two-factor-email-code")
      {
        mergeVariables.Add("SECURITYCODE", customerEmailLink);
      }
      else
      {
        mergeVariables.Add("CUSTOMEREMAILLINK", customerEmailLink);
      }

      try
      {
        await mailService.SendEmailWithTemplateAsync(mailMessage, mergeVariables, templateName);
        _logger.Info("MAILINFO: Mail with template: '{0}' sent to: '{1}'", templateName, sendToEmail);
      }
      catch (Exception)
      {
        _logger.Error("MAILERROR: Mail with template: '{0}' sent to: '{1}'", templateName, sendToEmail);

      }

    }
  }

  public class SmsService : IIdentityMessageService
  {
    public Task SendAsync(IdentityMessage message)
    {
      // Plug in your SMS service here to send a text message.
      return Task.FromResult(0);
    }
  }

  // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
  public class ApplicationUserManager : UserManager<User>
  {

    public ApplicationUserManager(IUserStore<User> store)
      : base(store)
    {

      var session = HttpContext.Current.GetOwinContext().Get<IDocumentSession>();

      this.Store = new UserStore<User>(() => session ?? ServiceLocator.Current.GetInstance<IDocumentSession>());
      // Configure validation logic for usernames
      this.UserValidator = new UserValidator<User>(this)
      {
        AllowOnlyAlphanumericUserNames = false,
        RequireUniqueEmail = true
      };

      // Configure validation logic for passwords
      this.PasswordValidator = new PasswordValidator
      {
        RequiredLength = 6,
        RequireNonLetterOrDigit = false,
        RequireDigit = false,
        RequireLowercase = false,
        RequireUppercase = false,
      };

      // Configure user lockout defaults
      this.UserLockoutEnabledByDefault = true;
      this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      this.MaxFailedAccessAttemptsBeforeLockout = 5;

      // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
      // You can write your own provider and plug it in here.
      this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
      {
        MessageFormat = "Your security code is {0}"
      });
      this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
      {
        Subject = "template-two-factor-email-code",
        BodyFormat = "{0}"
      });
      this.EmailService = new EmailService();
      this.SmsService = new SmsService();

      var dataProtectionProvider = Startup.DataProtectionProvider;
      if (dataProtectionProvider != null)
      {
        IDataProtector dataProtector = dataProtectionProvider.Create("ASP.NET Identity");

        this.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtector);
      }
    }

    //public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
    //{
    //  var manager = new ApplicationUserManager(new UserStore<User>(context.Get<IDocumentSession>()));
    //  // Configure validation logic for usernames
    //  manager.UserValidator = new UserValidator<User>(manager)
    //  {
    //    AllowOnlyAlphanumericUserNames = false,
    //    RequireUniqueEmail = true
    //  };

    //  // Configure validation logic for passwords
    //  manager.PasswordValidator = new PasswordValidator
    //  {
    //    RequiredLength = 6,
    //    RequireNonLetterOrDigit = true,
    //    RequireDigit = true,
    //    RequireLowercase = true,
    //    RequireUppercase = true,
    //  };

    //  // Configure user lockout defaults
    //  manager.UserLockoutEnabledByDefault = true;
    //  manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //  manager.MaxFailedAccessAttemptsBeforeLockout = 5;

    //  // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
    //  // You can write your own provider and plug it in here.
    //  manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User>
    //  {
    //    MessageFormat = "Your security code is {0}"
    //  });
    //  manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User>
    //  {
    //    Subject = "Security Code",
    //    BodyFormat = "Your security code is {0}"
    //  });
    //  manager.EmailService = new EmailService();
    //  manager.SmsService = new SmsService();
    //  var dataProtectionProvider = Startup.DataProtectionProvider;
    //  if (dataProtectionProvider != null)
    //  {
    //    IDataProtector dataProtector = dataProtectionProvider.Create("ASP.NET Identity");

    //    manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtector);
    //  }
    //  return manager;
    //}
  }

  // Configure the application sign-in manager which is used in this application.
  public class ApplicationSignInManager : SignInManager<User, string>
  {
    public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
      : base(userManager, authenticationManager)
    {
    }

    public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
    {
      return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
    }

    public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
    {
      return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
    }
  }
}
