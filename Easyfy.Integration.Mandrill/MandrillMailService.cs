using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Easyfy.Satellit.Contracts;
using MailChimp;
using MailChimp.Types;

namespace Easyfy.Integration.Mandrill
{
  public class MandrillMailService : IMailService
  {
    private readonly MandrillApi _mandrillApi;

    public MandrillMailService()
    {
      _mandrillApi = new MandrillApi(ConfigurationManager.AppSettings["Mandrill.ApiKey"]);
    }
    public void SendEmail(MailMessage mailMessage, string[] tags = null)
    {
      try
      {
        var message = new MailChimp.Types.Mandrill.Messages.Message
        {
          To = mailMessage.To.Select(o => new MailChimp.Types.Mandrill.Messages.Recipient(o.Address, o.DisplayName)).ToArray(),
          FromEmail = mailMessage.From.Address,
          FromName = String.IsNullOrEmpty(mailMessage.From.DisplayName) ? mailMessage.From.DisplayName : "DefaultName",
          Subject = mailMessage.Subject,
          Html = mailMessage.IsBodyHtml ? mailMessage.Body : null,
          Text = mailMessage.IsBodyHtml ? null : mailMessage.Body,
        };

        if (mailMessage.ReplyToList.Any())
          message.Headers = new MCDict<MailChimp.Types.Mandrill.Messages.Header> {
						{"Reply-To", mailMessage.ReplyToList.First().Address}
					};

        if (mailMessage.Bcc.Any()) message.BccAddress = mailMessage.Bcc.First().Address;
        if (tags != null) message.Tags = tags;

        MVList<MailChimp.Types.Mandrill.Messages.SendResult> result = _mandrillApi.Send(message);
      }
      catch (Exception)
      {
        //TODO: Log
      }

    }

    public void SendEmailWithTemplate(MailMessage mailMessage, Dictionary<string, string> mergeVariables, string templateName, string[] tags = null)
    {
      var message = new MailChimp.Types.Mandrill.Messages.Message
      {
        To = mailMessage.To.Select(o => new MailChimp.Types.Mandrill.Messages.Recipient(o.Address, o.DisplayName)).ToArray(),
        FromEmail = mailMessage.From.Address,
        FromName = String.IsNullOrEmpty(mailMessage.From.DisplayName) ? mailMessage.From.DisplayName: "DefaultName",
        Subject = mailMessage.Subject,
        Html = null,
        Text = null
      };

      if (mailMessage.ReplyToList.Any())
        message.Headers = new MCDict<MailChimp.Types.Mandrill.Messages.Header> {
					{"Reply-To", mailMessage.ReplyToList.First().Address}
				};

      var k = new MailChimp.Types.Mandrill.NameContentList<string>();
      foreach (var mergeVariable in mergeVariables)
      {
        k.Add(mergeVariable.Key, mergeVariable.Value);
      }

      message.GlobalMergeVars = new Opt<MailChimp.Types.Mandrill.NameContentList<string>>(k);

      _mandrillApi.SendTemplate(templateName, k, message);
    }

    public async Task SendEmailWithTemplateAsync(MailMessage mailMessage, Dictionary<string, string> mergeVariables, string templateName,
      string[] tags = null)
    {
      var message = new MailChimp.Types.Mandrill.Messages.Message
      {
        To = mailMessage.To.Select(o => new MailChimp.Types.Mandrill.Messages.Recipient(o.Address, o.DisplayName)).ToArray(),
        FromEmail = mailMessage.From.Address,
        FromName = String.IsNullOrEmpty(mailMessage.From.DisplayName) ? mailMessage.From.DisplayName:"DefaultName",
        Subject = mailMessage.Subject,
        Html = null,
        Text = null
      };

      if (mailMessage.ReplyToList.Any())
        message.Headers = new MCDict<MailChimp.Types.Mandrill.Messages.Header> {
					{"Reply-To", mailMessage.ReplyToList.First().Address}
				};

      var k = new MailChimp.Types.Mandrill.NameContentList<string>();
      foreach (var mergeVariable in mergeVariables)
      {
        k.Add(mergeVariable.Key, mergeVariable.Value);
      }

      message.GlobalMergeVars = new Opt<MailChimp.Types.Mandrill.NameContentList<string>>(k);

      await Task.Run(() => _mandrillApi.SendTemplate(templateName, k, message));

    }
  }
}
