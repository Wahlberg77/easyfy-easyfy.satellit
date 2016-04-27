using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Easyfy.Satellit.Contracts
{
  public interface IMailService
  {
    void SendEmail(MailMessage message, string[] tags = null);
    void SendEmailWithTemplate(MailMessage mailMessage, Dictionary<string, string> mergeVariables, string templateName, string[] tags = null);
    Task SendEmailWithTemplateAsync(MailMessage mailMessage, Dictionary<string, string> mergeVariables, string templateName, string[] tags = null);

  }
}
