using System;
using essentialMix.Extensions;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace essentialMix.Web.Helpers;

public static class IConfigurationBuilderHelper
{
	[NotNull]
	public static IConfigurationBuilder CreateConfiguration()
	{
		IHostEnvironment env = new HostingEnvironment
		{
			EnvironmentName = EnvironmentHelper.GetEnvironmentName(),
			ApplicationName = AppDomain.CurrentDomain.FriendlyName,
			ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
			ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
		};

		return new ConfigurationBuilder()
			.Setup(env);
	}
}