using System.Collections.Generic;
using System.Linq;
using essentialMix.Data.Entity.Collections;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IEnumerableExtension
{
	[NotNull]
	public static IQueryable<T> AsAsyncQueryable<T>([NotNull] this IEnumerable<T> thisValue)
	{
		return new AsyncQueryable<T>(thisValue);
	}
}