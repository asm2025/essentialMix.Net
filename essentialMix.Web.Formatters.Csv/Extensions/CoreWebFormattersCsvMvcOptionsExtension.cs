using System;
using System.Linq;
using CsvHelper;
using essentialMix.Web.Formatters.Csv;
using essentialMix.Web.Formatters.Csv.Internal;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class CoreWebFormattersCsvMvcOptionsExtension
{
	[NotNull]
	public static MvcOptions ConfigureCsvFormatter([NotNull] this MvcOptions thisValue, [NotNull] Func<CsvReaderOptions, CsvReaderOptions> configureReader, [NotNull] Func<CsvWriterOptions, CsvWriterOptions> configureWriter)
	{
		thisValue.RespectBrowserAcceptHeader = true;

		CsvReaderOptions readerOptions = configureReader(CsvReaderOptions.Default);
		CsvWriterOptions writerOptions = configureWriter(CsvWriterOptions.Default);

		CsvInputFormatter input = thisValue.InputFormatters.OfType<CsvInputFormatter>()
											.FirstOrDefault();

		if (input == null)
		{
			input = new CsvInputFormatter(readerOptions);
			thisValue.InputFormatters.Add(input);
		}

		CsvOutputFormatter output = thisValue.OutputFormatters.OfType<CsvOutputFormatter>()
											.FirstOrDefault();

		if (output == null)
		{
			output = new CsvOutputFormatter(writerOptions);
			thisValue.OutputFormatters.Add(output);
		}

		thisValue.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValues.ApplicationCsv);
		return thisValue;
	}
}