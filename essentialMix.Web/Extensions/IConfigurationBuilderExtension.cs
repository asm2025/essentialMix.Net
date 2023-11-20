using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Json;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IConfigurationBuilderExtension
{
	/// <summary>
	/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostingenvironment?view=aspnetcore-2.2
	/// https://stackoverflow.com/questions/55601958/ihostingenvironment-is-obsolete
	/// </summary>
	[NotNull]
	public static IConfigurationBuilder Setup([NotNull] this IConfigurationBuilder thisValue, [NotNull] IWebHostEnvironment environment)
	{
		return thisValue.Setup(environment.ContentRootPath);
	}

	[NotNull]
	public static IConfigurationBuilder AddConfigurationFiles([NotNull] this IConfigurationBuilder thisValue, [NotNull] IWebHostEnvironment environment)
	{
		return thisValue.AddConfigurationFiles(environment.ContentRootPath, environment.EnvironmentName);
	}

	[NotNull]
	public static IConfigurationBuilder AddConfigurationFile([NotNull] this IConfigurationBuilder thisValue, [NotNull] string fileName, bool optional, [NotNull] IWebHostEnvironment environment)
	{
		return thisValue.AddConfigurationFile(environment.ContentRootPath, fileName, optional, environment.EnvironmentName);
	}

	[NotNull]
	public static IConfigurationBuilder AddConfigurationFile([NotNull] this IConfigurationBuilder thisValue, string path, [NotNull] string fileName, bool optional, [NotNull] string currentEnvironmentName, [NotNull] string environmentName)
	{
		return !string.Equals(currentEnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase)
					? thisValue
					: thisValue.AddConfigurationFile(path, fileName, optional, environmentName);
	}

	[NotNull]
	public static IConfigurationBuilder MakeJsonFilesMandatory([NotNull] this IConfigurationBuilder thisValue, string prefix)
	{
		prefix = prefix.ToNullIfEmpty();
		if (string.IsNullOrEmpty(prefix)) return thisValue;

		IEnumerable<JsonConfigurationSource> sources = thisValue.Sources
																.OfType<JsonConfigurationSource>()
																.Where(e => e.Path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

		foreach (JsonConfigurationSource source in sources)
		{
			source.Optional = false;
		}

		return thisValue;
	}

	[NotNull]
	public static IConfigurationBuilder ClearCommandLineProviders([NotNull] this IConfigurationBuilder thisValue)
	{
		IList<IConfigurationSource> sources = thisValue.Sources;
		if (sources.IsReadOnly) return thisValue;

		foreach (IConfigurationSource source in sources.Reverse())
		{
			switch (source)
			{
				case CommandLineConfigurationSource:
					sources.Remove(source);
					break;
				case ChainedConfigurationSource chainedConfigurationSource when ((IConfigurationRoot)chainedConfigurationSource.Configuration).Providers is IList<IConfigurationProvider>
				{
					IsReadOnly: false
				} sourcesRoot:
					foreach (CommandLineConfigurationProvider provider in sourcesRoot.OfType<CommandLineConfigurationProvider>().Reverse())
					{
						sourcesRoot.Remove(provider);
					}
					break;
			}
		}

		return thisValue;
	}
}