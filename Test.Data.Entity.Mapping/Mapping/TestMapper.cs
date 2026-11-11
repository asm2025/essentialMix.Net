using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;

namespace Test.Data.Entity.Mapping.Mapping;

/// <summary>
/// Builds the mapper the way a consuming application would. Tests go through this helper only, so swapping the
/// mapping library changes this file and the profile, not the tests.
/// </summary>
public static class TestMapper
{
	[NotNull]
	public static MapperConfiguration CreateConfiguration()
	{
		return new MapperConfiguration(cfg => cfg.AddProfile<SampleProfile>(), NullLoggerFactory.Instance);
	}

	[NotNull]
	public static IMapper Create() { return CreateConfiguration().CreateMapper(); }

	public static void AssertConfigurationIsValid() { CreateConfiguration().AssertConfigurationIsValid(); }

	[NotNull]
	public static IEnumerable<(Type Source, Type Destination)> RegisteredPairs()
	{
		return CreateConfiguration().Internal()
									.GetAllTypeMaps()
									.Select(e => (e.SourceType, e.DestinationType));
	}
}
