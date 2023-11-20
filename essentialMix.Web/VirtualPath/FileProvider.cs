using System.IO;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace essentialMix.Web.VirtualPath;

public class FileProvider : PhysicalFileProvider, IFileProvider
{
	/// <inheritdoc />
	public FileProvider(string root)
		: base(root)
	{
	}

	/// <inheritdoc />
	public FileProvider(string root, ExclusionFilters filters)
		: base(root, filters)
	{
	}

	/// <inheritdoc />
	public FileProvider(string root, string alias)
		: base(root)
	{
		Alias = alias;
	}

	/// <inheritdoc />
	public FileProvider(string root, string alias, ExclusionFilters filters)
		: base(root, filters)
	{
		Alias = alias;
	}

	public string Alias { get; }

	[NotNull]
	public static FileProvider From([NotNull] PathContent pathContent) { return From(pathContent, null); }
	[NotNull]
	public static FileProvider From([NotNull] PathContent pathContent, string rootPath)
	{
		string path = string.IsNullOrEmpty(rootPath) || PathHelper.IsPathRooted(pathContent.PhysicalPath)
						? pathContent.PhysicalPath
						: Path.GetFullPath(pathContent.PhysicalPath, rootPath);
		return new FileProvider(path, pathContent.Alias);
	}
}