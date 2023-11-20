using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Net.Http.Headers;

namespace essentialMix.Web.Helpers;

// https://github.com/aspnet/AspNetCore.Docs/blob/master/aspnetcore/mvc/models/file-uploads/sample/FileUploadSample/MultipartRequestHelper.cs
public static class MultipartRequestHelper
{
	public static bool IsMultipartContentType(string contentType)
	{
		return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
	{
		// Content-Disposition: form-data; name="key";
		return contentDisposition != null
				&& contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase)
				&& string.IsNullOrEmpty(contentDisposition.FileName.Value)
				&& string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
	}

	public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
	{
		// Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
		return contentDisposition != null
				&& contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase)
				&& (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
					|| !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
	}

	[NotNull]
	public static string GetBoundary([NotNull] MediaTypeHeaderValue contentType, int lengthLimit)
	{
		// Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
		// The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
		string boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

		if (string.IsNullOrWhiteSpace(boundary))
			throw new InvalidDataException("Missing content-type boundary.");

		if (boundary.Length > lengthLimit)
			throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");

		return boundary;
	}
}