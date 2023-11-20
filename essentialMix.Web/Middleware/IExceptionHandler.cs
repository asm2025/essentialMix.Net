using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace essentialMix.Web.Middleware;

public interface IExceptionHandler
{
	bool OnError([NotNull] HttpContext context, [NotNull] Exception exception);
}