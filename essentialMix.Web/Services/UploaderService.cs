using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using essentialMix.Web.Helpers;
using essentialMix.Exceptions.Web;
using essentialMix.Extensions;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace essentialMix.Web.Services;

public class UploaderService : IUploaderService
{
	public UploaderService()
	{
	}

	/// <inheritdoc />
	public async Task<string> UploadAsync(IFormFile file, string path, string fileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		if (file.Length == 0) return null;
		if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
		path = PathHelper.AddDirectorySeparator(path);
		if (string.IsNullOrEmpty(path)) throw new ArgumentException($"{nameof(path)} cannot be empty.", nameof(path));

		fileName = EnsureFileDoesNotExists(path, fileName, overwrite, rename, token);
		if (!DirectoryHelper.Ensure(path)) throw new Exception($"Could not write to path '{path}'.");

		Stream source = null;
		Stream target = null;

		try
		{
			source = file.OpenReadStream();
			target = File.Create(Path.Combine(path, fileName));
			await source.CopyToAsync(target, token);
			await target.FlushAsync(token);
		}
		finally
		{
			ObjectHelper.Dispose(ref source);
			ObjectHelper.Dispose(ref target);
		}

		token.ThrowIfCancellationRequested();
		return fileName;
	}

	/// <inheritdoc />
	public async Task<string> UploadAsync(HttpRequest request, string path, string fileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType)) throw new ArgumentException("Request is not a multipart type.");
		if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
		path = PathHelper.AddDirectorySeparator(path);
		if (string.IsNullOrEmpty(path)) throw new ArgumentException($"{nameof(path)} cannot be empty.", nameof(path));

		string boundary = MediaTypeHeaderValue.Parse(request.ContentType).GetBoundary();
		if (string.IsNullOrEmpty(boundary)) throw new InvalidDataException("Could not get request boundary.");

		MultipartReader reader = new MultipartReader(boundary, request.Body);
		MultipartSection section;
		string file = null;

		do
		{
			section = await reader.ReadNextSectionAsync(token);
			if (section == null
				|| !ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition)
				|| !contentDisposition.HasFileContentDisposition()) continue;
			StringSegment fileNameSegment = contentDisposition.FileNameStar.Trim();
			if (StringSegment.IsNullOrEmpty(fileNameSegment)) fileNameSegment = contentDisposition.FileName.Trim();
			if (StringSegment.IsNullOrEmpty(fileNameSegment)) continue;
			file = fileNameSegment.Value;
		}
		while (section != null && string.IsNullOrEmpty(file) && !token.IsCancellationRequested);

		token.ThrowIfCancellationRequested();
		if (section == null) return null;
		fileName = EnsureFileDoesNotExists(path, fileName, overwrite, rename, token);
		if (!DirectoryHelper.Ensure(path)) throw new Exception($"Could not write to path '{path}'.");

		Stream target = null;

		try
		{
			target = File.Create(Path.Combine(path, fileName));
			await section.Body.CopyToAsync(target, token);
			await target.FlushAsync(token);
		}
		finally
		{
			ObjectHelper.Dispose(ref target);
		}

		token.ThrowIfCancellationRequested();
		return fileName;
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<string> UploadAsync(IEnumerable<IFormFile> files, string path, Func<IFormFile, string> getFileName, bool overwrite = false, bool rename = true, [EnumeratorCancellation] CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		path = PathHelper.AddDirectorySeparator(path);
		if (string.IsNullOrEmpty(path)) throw new ArgumentException($"{nameof(path)} cannot be empty.", nameof(path));

		bool pathEnsured = false;

		foreach (IFormFile file in files.TakeWhile(e => !token.IsCancellationRequested && e.Length > 0))
		{
			string fileName = EnsureFileDoesNotExists(path, getFileName(file), overwrite, rename, token);
			if (string.IsNullOrEmpty(fileName)) continue;

			if (!pathEnsured)
			{
				if (!DirectoryHelper.Ensure(path)) throw new Exception($"Could not write to path '{path}'.");
				pathEnsured = true;
			}

			Stream source = null;
			Stream target = null;

			try
			{
				source = file.OpenReadStream();
				target = File.Create(Path.Combine(path, fileName));
				await source.CopyToAsync(target, token);
				await target.FlushAsync(token);
				yield return fileName;
			}
			finally
			{
				ObjectHelper.Dispose(ref source);
				ObjectHelper.Dispose(ref target);
			}
		}
	}

	/// <inheritdoc />
	public async IAsyncEnumerable<string> UploadAsync(HttpRequest request, string path, Func<string, string> getFileName, bool overwrite = false, bool rename = true, [EnumeratorCancellation] CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType)) throw new ArgumentException("Request is not a multipart type.");
		path = PathHelper.AddDirectorySeparator(path);
		if (string.IsNullOrEmpty(path)) throw new ArgumentException($"{nameof(path)} cannot be empty.", nameof(path));

		string boundary = MediaTypeHeaderValue.Parse(request.ContentType).GetBoundary();
		if (string.IsNullOrEmpty(boundary)) throw new InvalidDataException("Could not get request boundary.");

		MultipartReader reader = new MultipartReader(boundary, request.Body);
		MultipartSection section;
		bool pathEnsured = false;

		do
		{
			section = await reader.ReadNextSectionAsync(token);
			if (section == null
				|| !ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition)
				|| !contentDisposition.HasFileContentDisposition()) continue;
			StringSegment fileNameSegment = contentDisposition.FileNameStar.Trim();
			if (StringSegment.IsNullOrEmpty(fileNameSegment)) fileNameSegment = contentDisposition.FileName.Trim();
			if (StringSegment.IsNullOrEmpty(fileNameSegment)) continue;
			string file = fileNameSegment.Value;
			if (string.IsNullOrEmpty(file)) continue;
			string fileName = EnsureFileDoesNotExists(path, getFileName(file), overwrite, rename, token);
			if (string.IsNullOrEmpty(fileName)) continue;

			if (!pathEnsured)
			{
				if (!DirectoryHelper.Ensure(path)) throw new Exception($"Could not write to path '{path}'.");
				pathEnsured = true;
			}

			Stream target = null;

			try
			{
				target = File.Create(Path.Combine(path, fileName));
				await section.Body.CopyToAsync(target, token);
				await target.FlushAsync(token);
				yield return fileName;
			}
			finally
			{
				ObjectHelper.Dispose(ref target);
			}
		}
		while (section != null && !token.IsCancellationRequested);
	}

	private static string EnsureFileDoesNotExists([NotNull] string path, string fileName, bool overwrite, bool rename, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		if (string.IsNullOrEmpty(fileName)) return null;
		if (fileName.IndexOf(Path.DirectorySeparatorChar) >= 0) fileName = Path.GetFileName(fileName);
		fileName = WebUtility.UrlDecode(fileName);

		string baseName = Path.GetFileNameWithoutExtension(fileName);
		string extension = Path.GetExtension(fileName);
		string destinationPath = Path.Combine(path, fileName);

		if (Directory.Exists(destinationPath))
		{
			if (!rename) throw new HttpException("A directory with the same name already exists.");

			int n = 0;

			do
			{
				fileName = $"{baseName} ({++n}){extension}";
				destinationPath = Path.Combine(path, fileName);
				// File.Exists tests both file and directory
			}
			while (!token.IsCancellationRequested && File.Exists(destinationPath));
		}

		if (File.Exists(destinationPath))
		{
			if (overwrite) File.Delete(destinationPath);
			if (!rename) throw new HttpException("A file with the same name already exists.");

			int n = 0;

			do
			{
				fileName = $"{baseName} ({++n}){extension}";
				destinationPath = Path.Combine(path, fileName);
				// File.Exists tests both file and directory
			}
			while (!token.IsCancellationRequested && File.Exists(destinationPath));
		}

		token.ThrowIfCancellationRequested();
		return fileName;
	}
}