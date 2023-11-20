using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CoreWebHttpContextExtension
{
	[NotNull]
	public static Task WriteModelAsync<T>([NotNull] this HttpContext context, T model)
	{
		ObjectResult result = new ObjectResult(model)
		{
			DeclaredType = typeof(T)
		};

		return context.ExecuteResultAsync(result);
	}

	[NotNull]
	public static Task ExecuteResultAsync<TResult>([NotNull] this HttpContext context, [NotNull] TResult result)
		where TResult : IActionResult
	{
		IActionResultExecutor<TResult> executor = context.RequestServices.GetRequiredService<IActionResultExecutor<TResult>>();
		RouteData routeData = context.GetRouteData();
		ActionContext actionContext = new ActionContext(context, routeData, new ActionDescriptor());
		return executor.ExecuteAsync(actionContext, result);
	}

	public static (IOutputFormatter SelectedFormatter, OutputFormatterWriteContext FormatterContext) SelectFormatter<T>([NotNull] this HttpContext context, [NotNull] T model)
	{
		OutputFormatterSelector selector = context.RequestServices.GetRequiredService<OutputFormatterSelector>();
		IHttpResponseStreamWriterFactory writerFactory = context.RequestServices.GetRequiredService<IHttpResponseStreamWriterFactory>();
		OutputFormatterWriteContext formatterContext = new OutputFormatterWriteContext(context, writerFactory.CreateWriter, typeof(T), model);
		IOutputFormatter selectedFormatter = selector.SelectFormatter(formatterContext, Array.Empty<IOutputFormatter>(), new MediaTypeCollection());
		return (selectedFormatter, formatterContext);
	}

	public static void CheckSameSite(this HttpContext thisValue, [NotNull] CookieOptions options)
	{
		if (thisValue == null || options.SameSite != SameSiteMode.None) return;
		string userAgent = thisValue.Request.Headers["User-Agent"].ToString();
		if (thisValue.Request.IsHttps && !DisallowsSameSiteNone(userAgent)) return;
		options.SameSite = SameSiteMode.Unspecified;
	}

	private static bool DisallowsSameSiteNone(string userAgent)
	{
		return !string.IsNullOrWhiteSpace(userAgent) &&
				(userAgent.Contains("CPU iPhone OS 12") ||
				userAgent.Contains("iPad; CPU OS 12") ||
				userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/") && userAgent.Contains("Safari") ||
				userAgent.Contains("Chrome/5") ||
				userAgent.Contains("Chrome/6"));
	}
}