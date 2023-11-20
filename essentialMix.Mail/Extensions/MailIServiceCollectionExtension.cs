using essentialMix.Mail;
using essentialMix.Mail.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using MSIEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class MailIServiceCollectionExtension
{
	[NotNull]
	public static IServiceCollection AddEmailClient([NotNull] this IServiceCollection thisValue, [NotNull] IConfiguration configuration)
	{
		SmtpConfiguration smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
		SendGridConfiguration sendGridConfiguration = configuration.GetSection(nameof(SendGridConfiguration)).Get<SendGridConfiguration>();

		if (sendGridConfiguration != null && !string.IsNullOrWhiteSpace(sendGridConfiguration.ApiKey))
		{
			thisValue.AddSingleton<ISendGridClient>(_ => new SendGridClient(sendGridConfiguration.ApiKey));
			thisValue.AddSingleton(sendGridConfiguration);
			thisValue.AddTransient<MSIEmailSender, SendGridEmailSender>();
			thisValue.AddTransient<IEmailSender, SendGridEmailSender>();
		}
		else if (smtpConfiguration != null && !string.IsNullOrWhiteSpace(smtpConfiguration.Host))
		{
			thisValue.AddSingleton(smtpConfiguration);
			thisValue.AddTransient<MSIEmailSender, SmtpEmailSender>();
			thisValue.AddTransient<IEmailSender, SmtpEmailSender>();
		}
		else
		{
			thisValue.AddSingleton<MSIEmailSender, LogEmailSender>();
			thisValue.AddSingleton<IEmailSender, LogEmailSender>();
		}

		return thisValue;
	}
}