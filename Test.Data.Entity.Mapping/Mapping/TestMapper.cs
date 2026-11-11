using System;
using System.Collections.Generic;
using System.Linq;
using essentialMix.Data.Entity.Mapster;
using JetBrains.Annotations;
using Mapster;
using MapsterMapper;

namespace Test.Data.Entity.Mapping.Mapping;

/// <summary>
/// Builds the mapper the way a consuming application would. Tests go through this helper only, so swapping the
/// mapping library changes this file and the register, not the tests.
/// </summary>
public static class TestMapper
{
	[NotNull]
	public static TypeAdapterConfig CreateConfiguration() { return MappingConfiguration.Create(typeof(SampleRegister).Assembly); }

	[NotNull]
	public static IMapper Create() { return new Mapper(CreateConfiguration()); }

	public static void Compile() { CreateConfiguration().Compile(); }

	[NotNull]
	public static IEnumerable<(Type Source, Type Destination)> RegisteredPairs()
	{
		return CreateConfiguration().RuleMap
									.Keys
									.Select(e => (e.Source, e.Destination));
	}
}
