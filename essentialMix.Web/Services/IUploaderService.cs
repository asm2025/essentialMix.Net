using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace essentialMix.Web.Services;

public interface IUploaderService
{
	[NotNull]
	Task<string> UploadAsync([NotNull] IFormFile file, [NotNull] string path, [NotNull] string fileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken));
	[NotNull]
	Task<string> UploadAsync([NotNull] HttpRequest request, [NotNull] string path, [NotNull] string fileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken));
	[NotNull]
	[ItemNotNull]
	IAsyncEnumerable<string> UploadAsync([NotNull] IEnumerable<IFormFile> files, [NotNull] string path, [NotNull] Func<IFormFile, string> getFileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken));
	[NotNull]
	[ItemNotNull]
	IAsyncEnumerable<string> UploadAsync([NotNull] HttpRequest request, [NotNull] string path, [NotNull] Func<string, string> getFileName, bool overwrite = false, bool rename = true, CancellationToken token = default(CancellationToken));
}