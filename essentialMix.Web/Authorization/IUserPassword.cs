using System;
using essentialMix.Data.Model;
using JetBrains.Annotations;

namespace essentialMix.Web.Authorization;

public interface IUserPassword<TKey> : IEntity<TKey>
	where TKey : IComparable<TKey>, IEquatable<TKey>
{
	string Email { get; set; }
	DateTime? PasswordCreated { get; set; }
	DateTime? PasswordLastUpdated { get; set; }
}

public static class IUserPasswordExtension
{
	public static bool CanChangePassword<TKey>([NotNull] this IUserPassword<TKey> thisValue, TimeSpan passwordChangeTime)
		where TKey : IComparable<TKey>, IEquatable<TKey>
	{
		return passwordChangeTime <= TimeSpan.Zero || thisValue.PasswordLastUpdated is null || thisValue.PasswordLastUpdated.Value == thisValue.PasswordCreated || thisValue.PasswordLastUpdated.Value.Add(passwordChangeTime) <= DateTime.UtcNow;
	}

	public static bool IsPasswordExpired<TKey>([NotNull] this IUserPassword<TKey> thisValue, TimeSpan passwordExpirationTime)
		where TKey : IComparable<TKey>, IEquatable<TKey>
	{
		if (thisValue.PasswordLastUpdated is null) return true;
		if (passwordExpirationTime <= TimeSpan.Zero || thisValue.PasswordLastUpdated.Value == thisValue.PasswordCreated) return false;
		DateTime expirationDate = thisValue.PasswordLastUpdated.Value.Add(passwordExpirationTime);
		return expirationDate < DateTime.UtcNow;
	}
}