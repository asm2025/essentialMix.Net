using AutoMapper;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IMappingExpressionExtension
{
	[NotNull]
	public static IMappingExpression<TSource, TDestination> IgnoreAll<TSource, TDestination>([NotNull] this IMappingExpression<TSource, TDestination> thisValue)
	{
		thisValue.ForAllMembers(e => e.Ignore());
		return thisValue;
	}
}