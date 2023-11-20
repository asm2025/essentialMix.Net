using System;
using System.Windows.Forms;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class ControlExtension
{
	public static void InvokeIf([NotNull] this Control thisValue, [NotNull] Action action)
	{
		if (!thisValue.InvokeRequired)
		{
			action();
			return;
		}

		thisValue.Invoke(action);
	}

	public static T InvokeIf<T>([NotNull] this Control thisValue, [NotNull] Func<T> func)
	{
		return !thisValue.InvokeRequired
					? func()
					: thisValue.Invoke(func);
	}

	public static object InvokeIf([NotNull] this Control thisValue, [NotNull] Delegate @delegate)
	{
		return !thisValue.InvokeRequired
					? @delegate.DynamicInvoke()
					: thisValue.Invoke(@delegate);
	}

	public static object InvokeIf([NotNull] this Control thisValue, [NotNull] Delegate @delegate, [NotNull] params object[] args)
	{
		return !thisValue.InvokeRequired
					? @delegate.DynamicInvoke(args)
					: thisValue.Invoke(@delegate, args);
	}
}