using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace essentialMix.Web.Annotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MaxFileSizeAttribute : ValidationAttribute
{
	public MaxFileSizeAttribute(long size)
	{
		Size = size;
	}

	public long Size { get; }

	/// <inheritdoc />
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		if (value is not IFormFile file || file.Length <= Size) return ValidationResult.Success;
		return new ValidationResult($"Maximum allowed file size is {Size} bytes.");
	}
}