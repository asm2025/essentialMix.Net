using System.Net.Mail;
using System.Threading.Tasks;
using essentialMix.Mail.Configuration;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Patterns.Object;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace essentialMix.Mail;

public class SmtpEmailSender : Disposable, IEmailSender
{
	private readonly ILogger<SmtpEmailSender> _logger;
	private readonly string _defaultEmail;
	private readonly string _defaultName;

	private SmtpClient _client;

	public SmtpEmailSender([NotNull] IConfiguration configuration, [NotNull] SmtpConfiguration mailConfiguration, [NotNull] ILogger<SmtpEmailSender> logger)
	{
		_logger = logger;
		_defaultEmail = mailConfiguration.From.ToNullIfEmpty() ?? mailConfiguration.Login;
		_defaultName = configuration.GetValue<string>("SmtpConfiguration:FromName");
		_client = new SmtpClient
		{
			Host = mailConfiguration.Host,
			Port = mailConfiguration.Port,
			DeliveryMethod = SmtpDeliveryMethod.Network,
			EnableSsl = mailConfiguration.UseSSL
		};

		if (!string.IsNullOrEmpty(mailConfiguration.Password))
			_client.Credentials = new System.Net.NetworkCredential(mailConfiguration.Login, mailConfiguration.Password);
		else
			_client.UseDefaultCredentials = true;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (disposing) ObjectHelper.Dispose(ref _client);
		base.Dispose(disposing);
	}

	[NotNull]
	public Task SendEmailAsync([NotNull] string email, [NotNull] string subject, [NotNull] string htmlMessage)
	{
		MailMessage mail = new MailMessage(_defaultEmail, email)
		{
			Subject = subject,
			IsBodyHtml = true,
			Body = htmlMessage
		};
		_client.Send(mail);
		_logger.LogDebug(@$"Sent email:
to: {email},
subject: {subject}");
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Task SendEmailAsync(BasicEmail email)
	{
		if (string.IsNullOrWhiteSpace(email.From))
		{
			email.From = _defaultEmail;
			email.FromName = _defaultName;
		}

		MailMessage mail = new MailMessage(new MailAddress(email.From, email.FromName), new MailAddress(email.To, email.ToName))
		{
			Subject = email.Subject,
			IsBodyHtml = email.IsBodyHtml,
			Body = email.Body
		};
		_client.Send(mail);
		_logger.LogDebug(@$"Sent email: 
from: {string.Join(", ", email.FromName, email.From)},
to: {string.Join(", ", email.ToName, email.To)},
subject: {email.Subject}");
		return Task.CompletedTask;
	}
}