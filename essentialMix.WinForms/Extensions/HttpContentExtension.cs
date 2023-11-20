using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using essentialMix.Helpers;
using essentialMix.Web;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class HttpContentExtension
{
	public static T ReadAs<T>([NotNull] this HttpContent thisValue, JsonSerializerSettings settings = null)
	{
		AssertContentType(thisValue, true);
		Stream stream = null;
		TextReader reader = null;
		JsonReader jsonReader = null;

		try
		{
			stream = thisValue.ReadAsStream();
			reader = new StreamReader(stream);
			jsonReader = new JsonTextReader(reader);
			JsonSerializer serializer = JsonSerializer.Create(settings);
			return serializer.Deserialize<T>(jsonReader);
		}
		finally
		{
			ObjectHelper.Dispose(ref jsonReader);
			ObjectHelper.Dispose(ref reader);
			ObjectHelper.Dispose(ref stream);
		}
	}

	public static T ReadAs<T>([NotNull] this HttpContent thisValue, T defaultValue, JsonSerializerSettings settings = null)
	{
		if (!AssertContentType(thisValue, false)) return defaultValue;
		Stream stream = null;
		TextReader reader = null;
		JsonReader jsonReader = null;

		try
		{
			stream = thisValue.ReadAsStream();
			reader = new StreamReader(stream);
			jsonReader = new JsonTextReader(reader);
			JsonSerializer serializer = JsonSerializer.Create(settings);
			return serializer.Deserialize<T>(jsonReader);
		}
		catch
		{
			return defaultValue;
		}
		finally
		{
			ObjectHelper.Dispose(ref jsonReader);
			ObjectHelper.Dispose(ref reader);
			ObjectHelper.Dispose(ref stream);
		}
	}

	[NotNull]
	public static Task<T> ReadAsAsync<T>([NotNull] this HttpContent thisValue, CancellationToken token = default(CancellationToken)) { return ReadAsAsync<T>(thisValue, null, token); }
	public static async Task<T> ReadAsAsync<T>([NotNull] this HttpContent thisValue, JsonSerializerSettings settings, CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		AssertContentType(thisValue, true);
		Stream stream = null;
		TextReader reader = null;
		JsonReader jsonReader = null;

		try
		{
			stream = await thisValue.ReadAsStreamAsync(token);
			token.ThrowIfCancellationRequested();
			reader = new StreamReader(stream);
			jsonReader = new JsonTextReader(reader);
			JsonSerializer serializer = JsonSerializer.Create(settings);
			return serializer.Deserialize<T>(jsonReader);
		}
		finally
		{
			ObjectHelper.Dispose(ref jsonReader);
			ObjectHelper.Dispose(ref reader);
			ObjectHelper.Dispose(ref stream);
		}
	}

	[NotNull]
	public static Task<T> ReadAsAsync<T>([NotNull] this HttpContent thisValue, T defaultValue, CancellationToken token = default(CancellationToken)) { return ReadAsAsync(thisValue, defaultValue, null, token); }
	public static async Task<T> ReadAsAsync<T>([NotNull] this HttpContent thisValue, T defaultValue, JsonSerializerSettings settings, CancellationToken token = default(CancellationToken))
	{
		token.ThrowIfCancellationRequested();
		if (!AssertContentType(thisValue, false)) return defaultValue;
		Stream stream = null;
		TextReader reader = null;
		JsonReader jsonReader = null;

		try
		{
			stream = await thisValue.ReadAsStreamAsync(token);
			token.ThrowIfCancellationRequested();
			reader = new StreamReader(stream);
			jsonReader = new JsonTextReader(reader);
			JsonSerializer serializer = JsonSerializer.Create(settings);
			return serializer.Deserialize<T>(jsonReader);
		}
		finally
		{
			ObjectHelper.Dispose(ref jsonReader);
			ObjectHelper.Dispose(ref reader);
			ObjectHelper.Dispose(ref stream);
		}
	}

	private static bool AssertContentType([NotNull] HttpContent content, bool throwOnBadValue)
	{
		if (content.Headers.ContentType != null && !content.Headers.ContentType.MediaType.IsSame(MediaTypeNames.Application.Json)) return true;
		if (!throwOnBadValue) return false;
		throw new NotSupportedException("Unexpected content type.");
	}
}