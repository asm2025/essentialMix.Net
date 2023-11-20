using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace essentialMix.Web.Authorization;

public class PasswordValidator<TUser, TKey> : IPasswordValidator<TUser>
	where TUser : IdentityUser<TKey>
	where TKey : IEquatable<TKey>
{
	private const string PasswordCannotBeEmpty = "PasswordCannotBeEmpty";
	private const string PasswordCannotBeTheSame = "PasswordCannotBeTheSame";

	private readonly IPasswordHasher<TUser> _passwordHasher;

	public PasswordValidator([NotNull] IPasswordHasher<TUser> passwordHasher)
	{
		_passwordHasher = passwordHasher;
	}

	/// <inheritdoc />
	[NotNull]
	public Task<IdentityResult> ValidateAsync([NotNull] UserManager<TUser> manager, [NotNull] TUser user, string password)
	{
		if (string.IsNullOrWhiteSpace(password))
		{
			return Task.FromResult(IdentityResult.Failed(new IdentityError
			{
				Code = PasswordCannotBeEmpty,
				Description = "Password cannot be empty."
			}));
		}

		if (!string.IsNullOrEmpty(user.PasswordHash) &&
				_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded)
		{
			return Task.FromResult(IdentityResult.Failed(new IdentityError
			{
				Code = PasswordCannotBeTheSame,
				Description = "Password cannot be the same."
			}));
		}

		return Task.FromResult(IdentityResult.Success);
	}
}