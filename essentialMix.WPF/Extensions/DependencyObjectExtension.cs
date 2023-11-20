using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class DependencyObjectExtension
{
	[NotNull]
	[ItemNotNull]
	public static IEnumerable<T> Find<T>([NotNull] this DependencyObject thisValue)
		where T : DependencyObject
	{
		// Go through children in a breadth-first search
		Queue<DependencyObject> queue = new Queue<DependencyObject>();
		queue.Enqueue(thisValue);

		while (queue.Count > 0)
		{
			DependencyObject parent = queue.Dequeue();
			if (parent is T child) yield return child;

			int count = VisualTreeHelper.GetChildrenCount(parent);
			if (count == 0) continue;

			for (int i = 0; i < count; i++)
				queue.Enqueue(VisualTreeHelper.GetChild(parent, i));
		}
	}

	[NotNull]
	[ItemNotNull]
	public static IEnumerable<T> Find<T>([NotNull] this DependencyObject thisValue, [NotNull] Predicate<T> predicate)
		where T : DependencyObject
	{
		// Go through children in a breadth-first search
		Queue<DependencyObject> queue = new Queue<DependencyObject>();
		queue.Enqueue(thisValue);

		while (queue.Count > 0)
		{
			DependencyObject parent = queue.Dequeue();
			if (parent is T child && predicate(child)) yield return child;

			int count = VisualTreeHelper.GetChildrenCount(parent);
			if (count == 0) continue;

			for (int i = 0; i < count; i++)
				queue.Enqueue(VisualTreeHelper.GetChild(thisValue, i));
		}
	}

	public static bool HasBinding([NotNull] this DependencyObject thisValue, [NotNull] DependencyProperty property)
	{
		return BindingOperations.GetBinding(thisValue, property) != null;
	}
}