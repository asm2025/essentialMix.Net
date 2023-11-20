using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CookieOptionsExtension
{
	[NotNull]
	public static CookieOptions UseDefaultCookieOptions([NotNull] this CookieOptions thisValue, bool secure)
	{
		thisValue.HttpOnly = true;
		thisValue.SameSite = SameSiteMode.Lax;
		thisValue.Secure = secure;
		return thisValue;
	}
}