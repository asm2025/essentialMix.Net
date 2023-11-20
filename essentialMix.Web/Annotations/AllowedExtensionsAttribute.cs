using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using essentialMix.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace essentialMix.Web.Annotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class AllowedExtensionsAttribute : ValidationAttribute
{
	private readonly IReadOnlySet<string> _allowedExtensions;

	public AllowedExtensionsAttribute([NotNull] string extensions)
		: this(extensions.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
	{
	}

	public AllowedExtensionsAttribute([NotNull] IEnumerable<string> extensions)
	{
		IEnumerable<string> filtered = extensions.Select(e => e.Prefix('.'));
		_allowedExtensions = new HashSet<string>(filtered, StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		if (value is not IFormFile file) return ValidationResult.Success;
		string ext = Path.GetExtension(file.FileName);
		if (string.IsNullOrEmpty(ext) || _allowedExtensions.Contains(ext)) return ValidationResult.Success;
		return new ValidationResult($"File extension '{ext}' is not allowed.");
	}
}