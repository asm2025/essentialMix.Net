using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace essentialMix.Web.Middleware;

public class ExceptionMiddleware : MiddlewareBase
{
	private readonly IExceptionHandler _handler;

	public ExceptionMiddleware([NotNull] RequestDelegate next, [NotNull] IExceptionHandler handler, ILogger<ExceptionMiddleware> logger)
		: base(next, logger)
	{
		_handler = handler;
	}

	public override async Task Invoke([NotNull] HttpContext context)
	{
		if (Next == null) return;

		try
		{
			await Next(context);
		}
		catch (Exception ex)
		{
			if (!_handler.OnError(context, ex)) throw;
			context.Response.Redirect(context.Request.GetDisplayUrl());
		}
	}
}

public static class ExceptionMiddlewareExtension
{
	[NotNull]
	public static IApplicationBuilder UseExceptionMiddleware([NotNull] this IApplicationBuilder thisValue)
	{
		return thisValue.UseMiddleware<ExceptionMiddleware>();
	}
}