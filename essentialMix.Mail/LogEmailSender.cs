using System.Threading.Tasks;
using essentialMix.Patterns.Object;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace essentialMix.Mail;

public class LogEmailSender : Disposable, IEmailSender
{
	private readonly ILogger<LogEmailSender> _logger;

	public LogEmailSender(ILogger<LogEmailSender> logger)
	{
		_logger = logger;
	}

	[NotNull]
	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		_logger.LogInformation(@$"Sent email:
to: {email},
subject: {subject}");
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Task SendEmailAsync(BasicEmail email)
	{
		_logger.LogInformation(@$"Sent email: 
from: {string.Join(", ", email.FromName, email.From)},
to: {string.Join(", ", email.ToName, email.To)},
subject: {email.Subject}");
		return Task.CompletedTask;
	}
}