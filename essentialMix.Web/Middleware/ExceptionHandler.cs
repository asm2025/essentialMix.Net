using System;
using System.Net;
using System.Threading.Tasks;
using essentialMix.Exceptions.Web;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace essentialMix.Web.Middleware;

public class ExceptionHandler : MiddlewareBase<ExceptionHandlerOptions>
{
	/// <inheritdoc />
	public ExceptionHandler(RequestDelegate next, [NotNull] IOptions<ExceptionHandlerOptions> options, ILogger<ExceptionHandler> logger)
		: base(next, options, logger)
	{
		Options.ExceptionHandler ??= Next;
	}

	/// <inheritdoc />
	public override async Task Invoke(HttpContext context)
	{
		if (Next == null) return;

		try
		{
			await Next(context);
		}
		catch (Exception e)
		{
			if (await OnError(context, e)) return;
			// Re-throw the original if we couldn't handle it
			throw;
		}
	}

	protected virtual async Task<bool> OnError([NotNull] HttpContext context, [NotNull] Exception exception)
	{
		if (context.Response.HasStarted) return false;

		string errorMessage = exception.CollectMessages();
		Logger.LogError(errorMessage);

		PathString originalPath = context.Request.Path;
		if (Options.ExceptionHandlingPath.HasValue) context.Request.Path = Options.ExceptionHandlingPath;

		try
		{
			ExceptionHandlerFeature errorHandlerFeature = new ExceptionHandlerFeature
			{
				Error = exception
			};

			context.Features.Set<IExceptionHandlerFeature>(errorHandlerFeature);
			context.Response.Headers.Clear();

			context.Response.StatusCode = exception switch
			{
				HttpException httpException => httpException.StatusCode,
				HttpListenerException httpListenerException => httpListenerException.ErrorCode,
				_ => (int)HttpStatusCode.InternalServerError
			};

			if (Options.ExceptionHandler != null)
				await Options.ExceptionHandler.Invoke(context);
			else
			{
				IHttpResponseFeature responseFeature = context.Features.Get<IHttpResponseFeature>();
				if (responseFeature != null) responseFeature.ReasonPhrase = errorMessage;
			}

			return true;
		}
		catch (Exception ex)
		{
			errorMessage = ex.CollectMessages();
			Logger.LogError(errorMessage);
			return false;
		}
		finally
		{
			context.Request.Path = originalPath;
		}
	}
}

public static class ExceptionHandlerExtension
{
	[NotNull]
	public static IApplicationBuilder UseExceptionHandler([NotNull] this IApplicationBuilder thisValue)
	{
		return thisValue.UseMiddleware<ExceptionHandler>();
	}
}