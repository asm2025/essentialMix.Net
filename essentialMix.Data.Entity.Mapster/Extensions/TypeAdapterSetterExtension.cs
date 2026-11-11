using JetBrains.Annotations;
using Mapster;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class TypeAdapterSetterExtension
{
	/// <summary>
	/// Ignores every destination member. Members mapped explicitly after this call are still mapped.
	/// </summary>
	[NotNull]
	public static TypeAdapterSetter<TSource, TDestination> IgnoreAll<TSource, TDestination>([NotNull] this TypeAdapterSetter<TSource, TDestination> thisValue)
	{
		thisValue.IgnoreMember((_, side) => side == MemberSide.Destination);
		return thisValue;
	}
}
