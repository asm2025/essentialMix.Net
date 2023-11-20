using System;
using Microsoft.AspNetCore.Authorization;

namespace essentialMix.Web.Authorization;

public class PasswordExpirationRequirement : IAuthorizationRequirement
{
	public PasswordExpirationRequirement(TimeSpan expirationTime)
		: this(expirationTime, null)
	{
	}

	public PasswordExpirationRequirement(TimeSpan expirationTime, string changePasswordUrl)
	{
		ExpirationTime = expirationTime;
		ChangePasswordUrl = changePasswordUrl;
	}

	public TimeSpan ExpirationTime { get; set; }
	public string ChangePasswordUrl { get; set; }
}