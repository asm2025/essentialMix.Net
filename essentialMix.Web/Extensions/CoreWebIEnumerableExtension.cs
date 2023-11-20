using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CoreWebIEnumerableExtension
{
	[NotNull]
	public static IReadOnlyCollection<SelectListItem> ToSelectListItems<T>([NotNull] this IEnumerable<T> thisValue)
	{
		return ToSelectListItems(thisValue, null);
	}
	
	[NotNull]
	public static IReadOnlyCollection<SelectListItem> ToSelectListItems<T>([NotNull] this IEnumerable<T> thisValue, string placeholder)
	{
		return ToSelectListItems<T, T, T>(thisValue, null, null, placeholder);
	}
	
	[NotNull]
	public static IReadOnlyCollection<SelectListItem> ToSelectListItems<TSource, TText, TValue>([NotNull] this IEnumerable<TSource> thisValue, Func<TSource, TText> text, Func<TSource, TValue> value)
	{
		return ToSelectListItems(thisValue, text, value, null);
	}
	
	[NotNull]
	public static IReadOnlyCollection<SelectListItem> ToSelectListItems<TSource, TText, TValue>([NotNull] this IEnumerable<TSource> thisValue, Func<TSource, TText> text, Func<TSource, TValue> value, string placeholder)
	{
		Func<TSource, string> getText = text == null
											? s => Convert.ToString(s)
											: s => Convert.ToString(text(s));
		Func<TSource, string> getValue = value == null
											? s => Convert.ToString(s)
											: s => Convert.ToString(value(s));
		if (string.IsNullOrEmpty(placeholder))
		{
			return thisValue.Select(e => new SelectListItem
			{
				Text = getText(e),
				Value = getValue(e)
			})
							.ToArray();
		}

		List<SelectListItem> newCollection = new List<SelectListItem>
		{
			new SelectListItem(placeholder, string.Empty)
		};
		newCollection.AddRange(thisValue.Select(e => new SelectListItem
		{
			Text = getText(e),
			Value = getValue(e)
		}));
		return newCollection;
	}

	[NotNull]
	public static SelectList ToSelectList<T>([NotNull] this IEnumerable<T> thisValue)
	{
		return ToSelectList(thisValue, default(T), null);
	}
	
	[NotNull]
	public static SelectList ToSelectList<T>([NotNull] this IEnumerable<T> thisValue, T selectedValue)
	{
		return ToSelectList(thisValue, selectedValue, null);
	}
	
	[NotNull]
	public static SelectList ToSelectList<T>([NotNull] this IEnumerable<T> thisValue, T selectedValue, string placeholder)
	{
		IReadOnlyCollection<SelectListItem> items = ToSelectListItems<T, T, T>(thisValue, null, null, placeholder);
		return new SelectList(items, nameof(SelectListItem.Value), nameof(SelectListItem.Text), selectedValue);
	}

	[NotNull]
	public static SelectList ToSelectList<TSource, TText, TValue>([NotNull] this IEnumerable<TSource> thisValue, Func<TSource, TText> text, Func<TSource, TValue> value)
	{
		return ToSelectList(thisValue, text, value, default(TValue), null);
	}
	
	[NotNull]
	public static SelectList ToSelectList<TSource, TText, TValue>([NotNull] this IEnumerable<TSource> thisValue, Func<TSource, TText> text, Func<TSource, TValue> value, TValue selectedValue)
	{
		return ToSelectList(thisValue, text, value, selectedValue, null);
	}
	
	[NotNull]
	public static SelectList ToSelectList<TSource, TText, TValue>([NotNull] this IEnumerable<TSource> thisValue, Func<TSource, TText> text, Func<TSource, TValue> value, TValue selectedValue, string placeholder)
	{
		IReadOnlyCollection<SelectListItem> items = ToSelectListItems(thisValue, text, value, placeholder);
		return new SelectList(items, nameof(SelectListItem.Value), nameof(SelectListItem.Text), selectedValue);
	}
}