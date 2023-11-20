using System;
using essentialMix.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CoreWebHttpResponseExtension
{
	public static void AllowOrigin([NotNull] this HttpResponse thisValue, StringValues value)
	{
		thisValue.Headers.Append(HeaderNames.AccessControlAllowOrigin, value);
	}

	public static void AddError([NotNull] this HttpResponse thisValue, [NotNull] Exception exception)
	{
		thisValue.Headers.Append(HeaderNames.ApplicationError, exception.CollectMessages());
		thisValue.Headers.Append(HeaderNames.AccessControlExposeHeaders, HeaderNames.ApplicationError);
	}

	public static void AddHeader([NotNull] this HttpResponse thisValue, [NotNull] string name, StringValues value)
	{
		thisValue.Headers.Append(name, value);
		thisValue.Headers.Append(HeaderNames.AccessControlExposeHeaders, name);
	}
}