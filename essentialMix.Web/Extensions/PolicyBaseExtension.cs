using System.Collections.Generic;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.Http;
using Polly;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class PolicyBaseExtension
{
	[NotNull]
	public static DelegatingHandler CreateHandler([NotNull] this IAsyncPolicy<HttpResponseMessage> thisValue)
	{
		return new PolicyHttpMessageHandler(thisValue)
		{
			InnerHandler = new HttpClientHandler()
		};
	}

	[NotNull]
	public static DelegatingHandler CreateHandler([NotNull] this IAsyncPolicy<HttpResponseMessage> thisValue, [NotNull] IAsyncPolicy<HttpResponseMessage> policy)
	{
		return new PolicyHttpMessageHandler(thisValue)
		{
			InnerHandler = new PolicyHttpMessageHandler(policy)
			{
				InnerHandler = new HttpClientHandler()
			}
		};
	}

	[NotNull]
	public static DelegatingHandler CreateHandler([NotNull] this IAsyncPolicy<HttpResponseMessage> thisValue, [NotNull] IAsyncPolicy<HttpResponseMessage> policy1, [NotNull] IAsyncPolicy<HttpResponseMessage> policy2)
	{
		return new PolicyHttpMessageHandler(thisValue)
		{
			InnerHandler = new PolicyHttpMessageHandler(policy1)
			{
				InnerHandler = new PolicyHttpMessageHandler(policy2)
				{
					InnerHandler = new HttpClientHandler()
				}
			}
		};
	}

	[NotNull]
	public static DelegatingHandler CreateHandler([NotNull] this IAsyncPolicy<HttpResponseMessage> thisValue, [NotNull] IAsyncPolicy<HttpResponseMessage> policy1, [NotNull] IAsyncPolicy<HttpResponseMessage> policy2, [NotNull] IAsyncPolicy<HttpResponseMessage> policy3)
	{
		return new PolicyHttpMessageHandler(thisValue)
		{
			InnerHandler = new PolicyHttpMessageHandler(policy1)
			{
				InnerHandler = new PolicyHttpMessageHandler(policy2)
				{
					InnerHandler = new PolicyHttpMessageHandler(policy3)
					{
						InnerHandler = new HttpClientHandler()
					}
				}
			}
		};
	}

	[NotNull]
	public static DelegatingHandler CreateHandler([NotNull] this IAsyncPolicy<HttpResponseMessage> thisValue, [NotNull] params IAsyncPolicy<HttpResponseMessage>[] policies)
	{
		DelegatingHandler nextHandler = new PolicyHttpMessageHandler(thisValue);
		DelegatingHandler previousHandler = nextHandler;

		foreach (IAsyncPolicy<HttpResponseMessage> policy in policies)
		{
			nextHandler = new PolicyHttpMessageHandler(policy);
			previousHandler.InnerHandler = nextHandler;
			previousHandler = nextHandler;
		}

		nextHandler.InnerHandler ??= new HttpClientHandler();
		return previousHandler;
	}

	public static DelegatingHandler CreateHandler([NotNull] this IEnumerable<IAsyncPolicy<HttpResponseMessage>> thisValue)
	{
		DelegatingHandler firstHandler = null;
		DelegatingHandler nextHandler = null;
		DelegatingHandler previousHandler = null;

		foreach (IAsyncPolicy<HttpResponseMessage> policy in thisValue)
		{
			if (firstHandler == null)
			{
				firstHandler = new PolicyHttpMessageHandler(policy);
				previousHandler = firstHandler;
				continue;
			}

			nextHandler = new PolicyHttpMessageHandler(policy);
			previousHandler.InnerHandler = nextHandler;
			previousHandler = nextHandler;
		}

		if (nextHandler != null) nextHandler.InnerHandler ??= new HttpClientHandler();
		return firstHandler;
	}
}