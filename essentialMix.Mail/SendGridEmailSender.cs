using System.Linq;
using System.Threading.Tasks;
using essentialMix.Mail.Configuration;
using essentialMix.Exceptions.Web;
using essentialMix.Patterns.Object;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace essentialMix.Mail;

public class SendGridEmailSender : Disposable, IEmailSender
{
	private readonly ISendGridClient _client;
	private readonly SendGridConfiguration _mailConfiguration;
	private readonly ILogger<SendGridEmailSender> _logger;
	private readonly string _defaultEmail;
	private readonly string _defaultName;

	public SendGridEmailSender([NotNull] ISendGridClient client, [NotNull] SendGridConfiguration mailConfiguration, [NotNull] ILogger<SendGridEmailSender> logger)
	{
		_client = client;
		_mailConfiguration = mailConfiguration;
		_logger = logger;
		_defaultEmail = mailConfiguration.SourceEmail;
		_defaultName = mailConfiguration.SourceName;
	}

	[NotNull]
	public Task SendEmailAsync([NotNull] string email, [NotNull] string subject, [NotNull] string htmlMessage)
	{
		SendGridMessage message = MailHelper.CreateSingleEmail(new EmailAddress(_defaultEmail, _defaultName),
																new EmailAddress(email),
																subject,
																null,
																htmlMessage);
		return SendEmailAsync(message);
	}

	/// <inheritdoc />
	public Task SendEmailAsync(BasicEmail email)
	{
		if (string.IsNullOrWhiteSpace(email.From))
		{
			email.From = _defaultEmail;
			email.FromName = _defaultName;
		}

		string body = email.IsBodyHtml
						? null
						: email.Body;
		string htmlBody = email.IsBodyHtml
							? email.Body
							: null;
		SendGridMessage message = MailHelper.CreateSingleEmail(new EmailAddress(email.From, email.FromName),
																new EmailAddress(email.To, email.ToName),
																email.Subject,
																body,
																htmlBody);
		return SendEmailAsync(message);
	}

	private async Task SendEmailAsync([NotNull] SendGridMessage message)
	{
		// More information about click tracking: https://sendgrid.com/docs/ui/account-and-settings/tracking/
		message.SetClickTracking(_mailConfiguration.EnableClickTracking, _mailConfiguration.EnableClickTracking);

		Response response = await _client.SendEmailAsync(message);

		if (response.IsSuccessStatusCode)
		{
			EmailAddress to = message.Personalizations.FirstOrDefault()?.Tos?.FirstOrDefault();
			_logger.LogDebug(@$"Sent email:
from: {message.From},
to: {to},
subject: {message.Subject}");
			return;
		}

		string errorMessage = await response.Body.ReadAsStringAsync();
		throw new HttpException(response.StatusCode, errorMessage);
	}
}