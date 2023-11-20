using System;
using essentialMix.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CoreWebEnumExtension
{
	[NotNull]
	public static SelectListItem ToListItem<T>(this T thisValue, bool selected = false, bool? disabled = null)
		where T : struct, Enum, IComparable
	{
		disabled ??= thisValue.HasAttribute<T, DisabledAttribute>();
		return new SelectListItem
		{
			Value = thisValue.ToString(),
			Text = thisValue.GetDisplayName(),
			Selected = selected,
			Disabled = disabled.Value
		};
	}
}