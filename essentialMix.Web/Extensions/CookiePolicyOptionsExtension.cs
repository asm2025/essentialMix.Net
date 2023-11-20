using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CookiePolicyOptionsExtension
{
	[NotNull]
	public static CookiePolicyOptions UseDefaultCookiePolicy([NotNull] this CookiePolicyOptions thisValue)
	{
		thisValue.HttpOnly = HttpOnlyPolicy.Always;
		thisValue.MinimumSameSitePolicy = SameSiteMode.Unspecified;
		thisValue.Secure = CookieSecurePolicy.SameAsRequest;
		return thisValue;
	}

	[NotNull]
	public static CookiePolicyOptions UseDefaultCookiePolicy([NotNull] this CookiePolicyOptions thisValue, Action<CookiePolicyOptions> configure)
	{
		UseDefaultCookiePolicy(thisValue);
		if (configure == null) return thisValue;
		configure(thisValue);
		return thisValue;
	}
}
