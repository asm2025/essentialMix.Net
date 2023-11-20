using System.Threading.Tasks;
using essentialMix.Logging.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace essentialMix.Web.Middleware;

public abstract class MiddlewareBase
{
	protected MiddlewareBase(RequestDelegate next, ILogger logger)
	{
		Next = next;
		Logger = logger ?? LogHelper.Empty;
	}

	protected RequestDelegate Next { get; }

	[NotNull]
	protected ILogger Logger { get; }

	public abstract Task Invoke(HttpContext context);
}

public abstract class MiddlewareBase<TOptions> : MiddlewareBase
	where TOptions : class, new()
{
	/// <inheritdoc />
	protected MiddlewareBase(RequestDelegate next, [NotNull] IOptions<TOptions> options, ILogger logger)
		: base(next, logger)
	{
		Options = options.Value;
	}

	protected TOptions Options { get; }
}