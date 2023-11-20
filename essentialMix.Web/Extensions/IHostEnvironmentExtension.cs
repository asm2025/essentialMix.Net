using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IHostEnvironmentExtension
{
	public static bool IsDemo([NotNull] this IHostEnvironment hostEnvironment)
	{
		return hostEnvironment.IsEnvironment("Demo");
	}
}