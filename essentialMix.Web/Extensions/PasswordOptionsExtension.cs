using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class PasswordOptionsExtension
{
	[NotNull]
	public static ICollection<string> BuildTips([NotNull] this PasswordOptions thisValue)
	{
		ICollection<string> requirements = new Collection<string>();
		if (thisValue.RequiredLength > 0) requirements.Add($"Must be at least {thisValue.RequiredLength} characters.");
		if (thisValue.RequireUppercase) requirements.Add("Must contain an upper case character.");
		if (thisValue.RequireLowercase) requirements.Add("Must contain a lower case character.");
		if (thisValue.RequireDigit) requirements.Add("Must contain a digit.");
		if (thisValue.RequireNonAlphanumeric) requirements.Add("Must contain a non-alphanumeric character.");
		if (thisValue.RequiredUniqueChars > 0) requirements.Add($"Must contain at least {thisValue.RequiredUniqueChars} unique characters.");
		return requirements;
	}
}