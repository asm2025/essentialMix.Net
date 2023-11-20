using System;
using System.Collections.Generic;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MSIFileProvider = Microsoft.Extensions.FileProviders.IFileProvider;

namespace essentialMix.Web.VirtualPath;

[Serializable]
public class VirtualPathSettings
{
	public IList<PathContent> PathContents { get; set; }
}

public static class VirtualPathSettingsExtension
{
	[NotNull]
	public static IServiceCollection AddVirtualPathSettings([NotNull] this IServiceCollection thisValue, [NotNull] Action<VirtualPathSettings> configure) { return AddVirtualPathSettings(thisValue, null, configure); }
	[NotNull]
	public static IServiceCollection AddVirtualPathSettings([NotNull] this IServiceCollection thisValue, string rootPath, [NotNull] Action<VirtualPathSettings> configure)
	{
		if (rootPath != null) rootPath = PathHelper.Trim(rootPath);

		VirtualPathSettings virtualPathSettings = new VirtualPathSettings();
		configure(virtualPathSettings);
		thisValue.AddSingleton(virtualPathSettings);
		if (virtualPathSettings.PathContents == null || virtualPathSettings.PathContents.Count == 0) return thisValue;

		foreach (PathContent item in virtualPathSettings.PathContents)
			thisValue.AddSingleton<IFileProvider>(FileProvider.From(item, rootPath));

		return thisValue;
	}

	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue) { return UseVirtualPathSettings(thisValue, null, null, null); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, string rootPath) { return UseVirtualPathSettings(thisValue, rootPath, null, null); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, [NotNull] Action<VirtualPathSettings> configure) { return UseVirtualPathSettings(thisValue, null, configure, null); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, string rootPath, Action<VirtualPathSettings> configure) { return UseVirtualPathSettings(thisValue, rootPath, configure, null); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, MSIFileProvider fileProvider) { return UseVirtualPathSettings(thisValue, null, null, fileProvider); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, string rootPath, MSIFileProvider fileProvider) { return UseVirtualPathSettings(thisValue, rootPath, null, fileProvider); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, [NotNull] Action<VirtualPathSettings> configure, MSIFileProvider fileProvider) { return UseVirtualPathSettings(thisValue, null, configure, fileProvider); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathSettings([NotNull] this IApplicationBuilder thisValue, string rootPath, Action<VirtualPathSettings> configure, MSIFileProvider fileProvider)
	{
		if (rootPath != null) rootPath = PathHelper.Trim(rootPath);

		VirtualPathSettings virtualPathSettings = thisValue.ApplicationServices.GetService<VirtualPathSettings>() ?? new VirtualPathSettings();
		configure?.Invoke(virtualPathSettings);

		if (virtualPathSettings.PathContents != null)
		{
			foreach (PathContent item in virtualPathSettings.PathContents)
			{
				thisValue.UseStaticFiles(new StaticFileOptions
				{
					FileProvider = FileProvider.From(item, rootPath),
					RequestPath = item.RequestPath
				});
			}
		}

		if (fileProvider != null) thisValue.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
		return thisValue;
	}

	[NotNull]
	public static IApplicationBuilder UseVirtualPathEndpoints([NotNull] this IApplicationBuilder thisValue) { return UseVirtualPathEndpoints(thisValue, null); }
	[NotNull]
	public static IApplicationBuilder UseVirtualPathEndpoints([NotNull] this IApplicationBuilder thisValue, string rootPath)
	{
		VirtualPathSettings virtualPathSettings = thisValue.ApplicationServices.GetService<VirtualPathSettings>();
		if (virtualPathSettings?.PathContents == null || virtualPathSettings.PathContents.Count == 0) return thisValue;
		if (rootPath != null) rootPath = PathHelper.Trim(rootPath);

		foreach (PathContent item in virtualPathSettings.PathContents)
		{
			thisValue.MapWhen(context => context.Request.Path.StartsWithSegments(item.RequestPath, StringComparison.OrdinalIgnoreCase), config => config.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = FileProvider.From(item, rootPath)
			}));
		}

		return thisValue;
	}
}