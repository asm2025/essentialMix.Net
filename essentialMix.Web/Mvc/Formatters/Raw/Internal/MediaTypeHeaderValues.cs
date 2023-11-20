using Microsoft.Net.Http.Headers;

namespace essentialMix.Web.Mvc.Formatters.Raw.Internal;

internal static class MediaTypeHeaderValues
{
	public static readonly MediaTypeHeaderValue TextPlain = MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Plain).CopyAsReadOnly();
	public static readonly MediaTypeHeaderValue OctetStream = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.OctetStream).CopyAsReadOnly();
}