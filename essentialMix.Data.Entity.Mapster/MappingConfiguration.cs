using System.Reflection;
using JetBrains.Annotations;
using Mapster;

namespace essentialMix.Data.Entity.Mapster;

/// <summary>
/// Creates a <see cref="TypeAdapterConfig" /> whose global settings reproduce the behavior of the previous mapping
/// library, so hosts and tests share one configuration.
/// </summary>
public static class MappingConfiguration
{
	[NotNull]
	public static TypeAdapterConfig Create([NotNull] params Assembly[] registerAssemblies)
	{
		TypeAdapterConfig config = new TypeAdapterConfig
		{
			// An unregistered source/destination pair throws instead of being mapped by convention.
			RequireExplicitMapping = true,
			// A destination member with no source fails Compile(), like a validated configuration did.
			RequireDestinationMemberSource = true
		};
		config.Default
				// Member names match case-insensitively.
				.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
				// Members of the same type are assigned, not deep-copied.
				.ShallowCopyForSameType(true)
				// A null collection member becomes an empty collection.
				.AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);
		if (registerAssemblies.Length > 0) config.Scan(registerAssemblies);
		return config;
	}
}
