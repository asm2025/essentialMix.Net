using System;
using System.Text;
using essentialMix.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace essentialMix.Web.Helpers;

public static class ChallengeCodeHelper
{
	private static readonly TimeSpan CodeTimeout = TimeSpan.FromMinutes(5);

	public static string ForValue(string userName)
	{
		userName = userName.ToNullIfEmpty();
		if (string.IsNullOrEmpty(userName)) return null;
		long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes($"{timestamp:D10}{userName}"));
	}

	public static string FromValueChallenge(string code)
	{
		code = code.ToNullIfEmpty();
		if (string.IsNullOrEmpty(code)) return null;

		string decode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		if (string.IsNullOrEmpty(decode) || decode.Length < 11) return null;

		string timestamp = decode.Left(10);
		DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp));
		return DateTimeOffset.UtcNow - date > CodeTimeout
					? null
					: decode.Right(decode.Length - 10);
	}
}