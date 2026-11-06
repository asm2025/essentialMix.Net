using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using essentialMix.Extensions;
using essentialMix.Web.Formatters.Csv.Internal;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;

namespace essentialMix.Web.Formatters.Csv;

public class CsvInputFormatter : TextInputFormatter, IInputFormatterExceptionPolicy
{
	/// <inheritdoc />
	public CsvInputFormatter([NotNull] CsvReaderOptions options)
	{
		Options = options;

		SupportedEncodings.Add(UTF8EncodingWithoutBOM);
		SupportedEncodings.Add(UTF16EncodingLittleEndian);
		if (options.Encoding != null && !SupportedEncodings.Contains(options.Encoding)) SupportedEncodings.Add(options.Encoding);

		SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationCsv);
		SupportedMediaTypes.Add(MediaTypeHeaderValues.TextCsv);
		SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationAnyCsvSyntax);
	}

	[NotNull]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public CsvReaderOptions Options { get; }

	/// <inheritdoc />
	public virtual InputFormatterExceptionPolicy ExceptionPolicy => GetType() == typeof(CsvInputFormatter) ? InputFormatterExceptionPolicy.MalformedInputExceptions : InputFormatterExceptionPolicy.AllExceptions;

	/// <inheritdoc />
	[ItemNotNull]
	public override async Task<InputFormatterResult> ReadRequestBodyAsync([NotNull] InputFormatterContext context, [NotNull] Encoding encoding)
	{
		if (context == null) throw new ArgumentNullException(nameof(context));
		if (encoding == null) throw new ArgumentNullException(nameof(encoding));

		HttpRequest request = context.HttpContext.Request;

		if (!request.Body.CanSeek)
		{
			request.EnableBuffering();
			await request.Body.DrainAsync(context.HttpContext.RequestAborted);
			if (context.HttpContext.RequestAborted.IsCancellationRequested) return await InputFormatterResult.NoValueAsync();
			request.Body.Seek(0L, SeekOrigin.Begin);
		}

		using (TextReader reader = context.ReaderFactory(request.Body, encoding))
		{
			using (CsvReader csvReader = CsvReader.Create(reader, Options, true))
			{
				Type modelType = context.ModelType;

				try
				{
					if (modelType.Implements<IEnumerable>() && modelType != typeof(string))
					{
						modelType = modelType.ResolveGenericType();
						object models = CsvReflection.ReadRecords(csvReader, modelType);
						return models != null || context.TreatEmptyInputAsDefaultValue
									? await InputFormatterResult.SuccessAsync(models)
									: await InputFormatterResult.NoValueAsync();
					}

					object model = await csvReader.ReadAsync()
										? CsvReflection.ReadRecord(csvReader, modelType)
										: null;
					return model != null || context.TreatEmptyInputAsDefaultValue
								? await InputFormatterResult.SuccessAsync(model)
								: await InputFormatterResult.NoValueAsync();
				}
				catch (Exception e)
				{
					ExceptionDispatchInfo.Capture(e).Throw();
					return await InputFormatterResult.FailureAsync();
				}
			}
		}
	}
}