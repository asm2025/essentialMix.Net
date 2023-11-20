using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class ISessionExtension
{
	public static void ToJson<T>([NotNull] this ISession session, [NotNull] string key, T value)
	{
		session.SetString(key, JsonConvert.SerializeObject(value));
	}

	public static T FromJson<T>([NotNull] this ISession session, [NotNull] string key, T defaultValue = default(T))
	{
		string s = session.GetString(key);
		return string.IsNullOrEmpty(s)
					? defaultValue
					: JsonConvert.DeserializeObject<T>(s);
	}
}