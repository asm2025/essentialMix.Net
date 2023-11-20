using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CookieBuilderExtension
{
	[NotNull]
	public static CookieBuilder UseDefaultCookieOptions([NotNull] this CookieBuilder thisValue)
	{
		thisValue.HttpOnly = true;
		thisValue.SameSite = SameSiteMode.Lax;
		thisValue.SecurePolicy = CookieSecurePolicy.SameAsRequest;
		return thisValue;
	}
}