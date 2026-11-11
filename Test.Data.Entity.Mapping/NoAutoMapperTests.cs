using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using essentialMix.Data.Entity.Mapster;
using NUnit.Framework;

namespace Test.Data.Entity.Mapping;

/// <summary>
/// Guards against the commercially licensed mapping library coming back into the product projects.
/// </summary>
[TestFixture]
public class NoAutoMapperTests
{
	private const string LIBRARY_NAME = "AutoMapper";

	[Test]
	public void ProductAssembliesDoNotReferenceIt()
	{
		Assembly[] assemblies =
		[
			typeof(MappingConfiguration).Assembly,
			typeof(essentialMix.Data.Entity.DbContext).Assembly
		];
		string[] offending = assemblies.SelectMany(a => a.GetReferencedAssemblies().Select(r => $"{a.GetName().Name} -> {r.Name}"))
										.Where(e => e.Split(" -> ")[1].StartsWith(LIBRARY_NAME, StringComparison.OrdinalIgnoreCase))
										.ToArray();
		Assert.That(offending, Is.Empty);
	}

	[Test]
	public void ProductSourcesDoNotMentionIt()
	{
		string root = FindRepositoryRoot();
		string[] offending = Directory.EnumerateDirectories(root, "essentialMix.*")
									.SelectMany(EnumerateSources)
									.Where(file => File.ReadAllText(file).Contains(LIBRARY_NAME, StringComparison.OrdinalIgnoreCase))
									.Select(file => Path.GetRelativePath(root, file))
									.ToArray();
		Assert.That(offending, Is.Empty);
	}

	private static IEnumerable<string> EnumerateSources(string directory)
	{
		return Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
						.Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
						.Where(file =>
						{
							string[] parts = Path.GetRelativePath(directory, file).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
							return !parts.Any(p => p.Equals("bin", StringComparison.OrdinalIgnoreCase) || p.Equals("obj", StringComparison.OrdinalIgnoreCase));
						});
	}

	private static string FindRepositoryRoot([CallerFilePath] string callerFile = null)
	{
		DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(callerFile)!);

		while (directory != null && !File.Exists(Path.Combine(directory.FullName, "essentialMix.sln")))
			directory = directory.Parent;

		Assert.That(directory, Is.Not.Null, "Repository root not found.");
		return directory!.FullName;
	}
}
