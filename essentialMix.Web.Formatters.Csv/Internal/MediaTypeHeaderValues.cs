using Microsoft.Net.Http.Headers;

namespace essentialMix.Web.Formatters.Csv.Internal;

internal static class MediaTypeHeaderValues
{
	public static readonly MediaTypeHeaderValue ApplicationCsv = MediaTypeHeaderValue.Parse("application/csv").CopyAsReadOnly();
	public static readonly MediaTypeHeaderValue TextCsv = MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Csv).CopyAsReadOnly();
	public static readonly MediaTypeHeaderValue ApplicationAnyCsvSyntax = MediaTypeHeaderValue.Parse("application/*+csv").CopyAsReadOnly();
}