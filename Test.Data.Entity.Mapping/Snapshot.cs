using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using essentialMix.Patterns.Pagination;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Test.Data.Entity.Mapping;

/// <summary>
/// Golden-master snapshots. Set the environment variable RECORD_SNAPSHOTS=1 to write them. The snapshots were
/// recorded once under the previous mapping library and are the spec: never re-record them to make a test pass.
/// </summary>
public static class Snapshot
{
	private static readonly JsonSerializerOptions __options = new JsonSerializerOptions
	{
		WriteIndented = true
	};

	public static void Match([NotNull] string name, object value, [CallerFilePath] string callerFile = null)
	{
		string directory = Path.Combine(Path.GetDirectoryName(callerFile)!, "Snapshots");
		string path = Path.Combine(directory, name + ".json");
		string actual = Serialize(value);

		if (Environment.GetEnvironmentVariable("RECORD_SNAPSHOTS") == "1")
		{
			Directory.CreateDirectory(directory);
			File.WriteAllText(path, actual);
			return;
		}

		Assert.That(File.Exists(path), Is.True, $"Missing snapshot '{path}'.");
		string expected = File.ReadAllText(path).ReplaceLineEndings("\n");
		Assert.That(actual, Is.EqualTo(expected), $"Snapshot '{name}' changed.");
	}

	[NotNull]
	public static string Serialize(object value)
	{
		object shaped = value switch
		{
			IPaginated<object> paginated => new
			{
				Result = paginated.Result.ToList(),
				Pagination = paginated.Pagination == null
								? null
								: new
								{
									paginated.Pagination.Page,
									paginated.Pagination.PageSize,
									paginated.Pagination.Count
								}
			},
			_ => value
		};
		return JsonSerializer.Serialize(shaped, __options).ReplaceLineEndings("\n");
	}
}
