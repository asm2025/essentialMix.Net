using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class IdentityResultExtension
{
	[NotNull]
	public static IdentityResult AddErrors([NotNull] this IdentityResult thisValue, [NotNull] ModelStateDictionary modelState)
	{
		foreach (IdentityError error in thisValue.Errors)
		{
			modelState.AddModelError(string.Empty, error.Description);
		}

		return thisValue;
	}
}