using System;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace essentialMix.Web.TagHelpers;

[HtmlTargetElement(Attributes = "menu-route")]
public class ActiveMenuHelper : AnchorTagHelper
{
	/// <inheritdoc />
	public ActiveMenuHelper([NotNull] IHtmlGenerator generator)
		: base(generator)
	{
	}

	public override void Process(TagHelperContext context, [NotNull] TagHelperOutput output)
	{
		(string area, string controller, string action, string page) = GetRouteData();

		if (page != null && Page != null)
		{
			Page = FixPage(Page);
			if (page.EndsWith(Page, StringComparison.OrdinalIgnoreCase)) Page = ViewContext.ActionDescriptor.DisplayName;
		}

		(string carea, string ccontroller, string caction, string cpage) = FixData(true, Area, Controller, Action, Page);

		bool areaSame = AreEqual(carea, area);
		bool actionSame = AreEqual(caction, action);
		bool pageSame = AreEqual(cpage, page);
		bool controllerSame = AreEqual(ccontroller, controller);
		bool result = areaSame && controllerSame && actionSame && pageSame;

		string classes;

		if (output.Attributes.TryGetAttribute("class", out TagHelperAttribute attribute))
		{
			classes = attribute.Value?.ToString() ?? string.Empty;
			output.Attributes.Remove(attribute);
		}
		else
		{
			classes = string.Empty;
		}

		if (result)
		{
			if (!classes.Contains("active")) classes = string.Join(' ', classes, "active");
		}
		else
		{
			if (classes.Contains("active"))
			{
				classes = classes.Replace("active", string.Empty)
								.Replace("  ", " ")
								.Trim();
			}
		}

		output.Attributes.Add("class", classes);
	}

	private (string, string, string, string) GetRouteData()
	{
		RouteValueDictionary routeData = ViewContext.RouteData.Values;
		RouteValueDictionary dataTokens = ViewContext.RouteData.DataTokens;
		string area = (string)(routeData["area"] ?? dataTokens["area"]);
		string controller = (string)(routeData["controller"] ?? dataTokens["controller"]);
		string action = (string)(routeData["action"] ?? dataTokens["action"]);
		string page = (string)(routeData["page"] ?? dataTokens["page"]);
		return FixData(false, area, controller, action, page);
	}

	private (string, string, string, string) FixData(bool getDefaults, string area, string controller, string action, string page)
	{
		string defaultPage = null;

		if (getDefaults)
		{
			RouteValueDictionary routeData = ViewContext.RouteData.Values;
			RouteValueDictionary dataTokens = ViewContext.RouteData.DataTokens;
			if (string.IsNullOrEmpty(area)) area = (string)(routeData["area"] ?? dataTokens["area"]);
			if (string.IsNullOrEmpty(controller)) controller = (string)(routeData["controller"] ?? dataTokens["controller"]);
			defaultPage = (string)(routeData["page"] ?? dataTokens["page"]);
		}

		page = FixPage(page.ToNullIfEmpty() ?? defaultPage);

		if (getDefaults)
		{
			RouteValueDictionary routeData = ViewContext.RouteData.Values;
			RouteValueDictionary dataTokens = ViewContext.RouteData.DataTokens;
			if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(page)) action = (string)(routeData["action"] ?? dataTokens["action"]);
		}

		if (string.IsNullOrEmpty(page) && string.IsNullOrEmpty(action)) page = "Index";
		return (area, controller, action, page);
	}

	protected static string FixPage(string page)
	{
		return string.IsNullOrEmpty(page) || (page[0] != '.' && page[0] == '/' && page[0] == '~')
					? page
					: page.Trim('.', '/', '~');
	}

	protected static bool AreEqual(string a, string b)
	{
		if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return true;
		return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
	}
}