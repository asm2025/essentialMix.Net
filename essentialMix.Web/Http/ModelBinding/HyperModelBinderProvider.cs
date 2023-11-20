using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using essentialMix.Json.Abstraction;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace essentialMix.Web.Http.ModelBinding;

public static class HyperModelBinderProvider
{
	public static ISet<Type> ExcludedTypes { get; } = new HashSet<Type>
	{
		typeof(object)
	};

	public static ConcurrentDictionary<Type, HyperModelBinder> Types { get; } = new ConcurrentDictionary<Type, HyperModelBinder>();
}

public class HyperModelBinderProvider<T> : IModelBinderProvider
{
	public HyperModelBinderProvider([NotNull] IJsonSerializer serializer)
	{
		Type type = typeof(T);

		while (type != null && !HyperModelBinderProvider.ExcludedTypes.Contains(type))
		{
			HyperModelBinderProvider.Types.TryAdd(type, new HyperModelBinder(type, serializer));
			type = type.BaseType;
		}
	}

	public IModelBinder GetBinder(ModelBinderProviderContext context)
	{
		HyperModelBinderProvider.Types.TryGetValue(context.Metadata.ModelType, out HyperModelBinder binder);
		return binder;
	}
}