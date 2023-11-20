using System.Net.Http;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class DelegatingHandlerExtension
{
	[NotNull]
	public static DelegatingHandler Combine([NotNull] this DelegatingHandler thisValue, [NotNull] DelegatingHandler handler)
	{
		DelegatingHandler lastHandler = GetLastParent(thisValue);
		lastHandler.InnerHandler = EnsureInnerHandler(handler);
		return thisValue;
	}

	[NotNull]
	public static DelegatingHandler Combine([NotNull] this DelegatingHandler thisValue, [NotNull] DelegatingHandler handler1, [NotNull] DelegatingHandler handler2)
	{
		DelegatingHandler lastHandler = GetLastParent(thisValue);
		lastHandler.InnerHandler = handler1;
		lastHandler = GetLastParent(handler1);
		lastHandler.InnerHandler = EnsureInnerHandler(handler2);
		return thisValue;
	}

	[NotNull]
	public static DelegatingHandler Combine([NotNull] this DelegatingHandler thisValue, [NotNull] DelegatingHandler handler1, [NotNull] DelegatingHandler handler2, [NotNull] DelegatingHandler handler3)
	{
		DelegatingHandler lastHandler = GetLastParent(thisValue);
		lastHandler.InnerHandler = handler1;
		lastHandler = GetLastParent(handler1);
		lastHandler.InnerHandler = handler2;
		lastHandler = GetLastParent(handler2);
		lastHandler.InnerHandler = EnsureInnerHandler(handler3);
		return thisValue;
	}

	[NotNull]
	public static DelegatingHandler Combine([NotNull] this DelegatingHandler thisValue, [NotNull] params DelegatingHandler[] handlers)
	{
		DelegatingHandler previousHandler = thisValue;

		foreach (DelegatingHandler handler in handlers)
		{
			DelegatingHandler parent = GetLastParent(previousHandler);
			parent.InnerHandler = handler;
			previousHandler = handler;
		}

		EnsureInnerHandler(previousHandler);
		return thisValue;
	}

	[NotNull]
	private static DelegatingHandler GetLastParent([NotNull] DelegatingHandler handler)
	{
		DelegatingHandler lastHandler = handler;

		while (lastHandler.InnerHandler is DelegatingHandler innerHandler)
			lastHandler = innerHandler;

		return lastHandler;
	}

	[NotNull]
	private static DelegatingHandler EnsureInnerHandler([NotNull] DelegatingHandler handler)
	{
		DelegatingHandler lastHandler = GetLastParent(handler);
		lastHandler.InnerHandler ??= new HttpClientHandler();
		return handler;
	}
}