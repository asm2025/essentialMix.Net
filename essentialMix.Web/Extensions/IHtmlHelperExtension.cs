using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IHtmlHelperExtension
{
	private const string ScriptsKey = "DelayedScripts";

	[NotNull]
	public static IDisposable BeginScripts([NotNull] this IHtmlHelper helper)
	{
		return new ScriptBlock(helper.ViewContext);
	}

	[NotNull]
	public static HtmlString PageScripts([NotNull] this IHtmlHelper helper)
	{
		return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext)));
	}

	[NotNull]
	private static List<string> GetPageScriptsList([NotNull] HttpContext httpContext)
	{
		List<string> pageScripts = (List<string>)httpContext.Items[ScriptsKey];
		if (pageScripts != null) return pageScripts;
		pageScripts = new List<string>();
		httpContext.Items[ScriptsKey] = pageScripts;
		return pageScripts;
	}

	private class ScriptBlock : IDisposable
	{
		private readonly TextWriter _originalWriter;
		private readonly StringWriter _scriptsWriter;
		private readonly ViewContext _viewContext;

		public ScriptBlock(ViewContext viewContext)
		{
			_viewContext = viewContext;
			_originalWriter = _viewContext.Writer;
			_viewContext.Writer = _scriptsWriter = new StringWriter();
		}

		public void Dispose()
		{
			_viewContext.Writer = _originalWriter;
			List<string> pageScripts = GetPageScriptsList(_viewContext.HttpContext);
			pageScripts.Add(_scriptsWriter.ToString());
			_scriptsWriter.Dispose();
		}
	}
}