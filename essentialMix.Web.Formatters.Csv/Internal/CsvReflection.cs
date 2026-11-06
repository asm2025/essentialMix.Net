using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CsvHelper;
using JetBrains.Annotations;

namespace essentialMix.Web.Formatters.Csv.Internal;

/// <summary>
/// CsvHelper 34 reads and writes through generic methods over <see cref="CsvRecordAttribute"/> types.
/// Formatters only know the model type at run time, so these helpers close the generic methods over it.
/// </summary>
internal static class CsvReflection
{
	private static readonly ConcurrentDictionary<(MethodInfo, Type), MethodInfo> __closed = new ConcurrentDictionary<(MethodInfo, Type), MethodInfo>();

	private static readonly MethodInfo __getRecord = typeof(CsvReader).GetMethod(nameof(CsvReader.GetRecord), 1, Type.EmptyTypes);
	private static readonly MethodInfo __getRecords = typeof(CsvReader).GetMethod(nameof(CsvReader.GetRecords), 1, Type.EmptyTypes);
	private static readonly MethodInfo __writeRecord = typeof(CsvWriter).GetMethods().First(m => m.Name == nameof(CsvWriter.WriteRecord) && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);
	private static readonly MethodInfo __writeRecords = typeof(CsvWriter).GetMethods().First(m => m.Name == nameof(CsvWriter.WriteRecords) && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);
	private static readonly MethodInfo __toList = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

	private static MethodInfo Close(MethodInfo definition, Type type)
	{
		return __closed.GetOrAdd((definition, type), key => key.Item1.MakeGenericMethod(key.Item2));
	}

	private static object Invoke(MethodInfo method, object target, params object[] args)
	{
		try
		{
			return method.Invoke(target, args);
		}
		catch (TargetInvocationException e) when (e.InnerException != null)
		{
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
			throw;
		}
	}

	/// <summary>Reads the current record as <paramref name="recordType"/>.</summary>
	public static object ReadRecord([NotNull] CsvReader reader, [NotNull] Type recordType)
	{
		return Invoke(Close(__getRecord, recordType), reader);
	}

	/// <summary>Reads all remaining records as a <c>List&lt;recordType&gt;</c>, so the reader can be disposed afterwards.</summary>
	public static object ReadRecords([NotNull] CsvReader reader, [NotNull] Type recordType)
	{
		object records = Invoke(Close(__getRecords, recordType), reader);
		return Invoke(Close(__toList, recordType), null, records);
	}

	public static void WriteRecords([NotNull] CsvWriter writer, [NotNull] object value)
	{
		if (value is IEnumerable enumerable and not string)
		{
			Type elementType = enumerable.GetType()
										.GetInterfaces()
										.Concat(new[] { enumerable.GetType() })
										.Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
										.Select(t => t.GetGenericArguments()[0])
										.FirstOrDefault() ?? typeof(object);
			Invoke(Close(__writeRecords, elementType), writer, enumerable);
			return;
		}

		Invoke(Close(__writeRecord, value.GetType()), writer, value);
	}
}
