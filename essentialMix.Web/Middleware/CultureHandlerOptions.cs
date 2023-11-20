using essentialMix.Extensions;

namespace essentialMix.Web.Middleware;

public class CultureHandlerOptions
{
	private string _parameterName = RequestParameterNames.Culture;

	public CultureHandlerOptions()
	{
	}

	public string ParameterName
	{
		get => _parameterName;
		set => _parameterName = value.ToNullIfEmpty() ?? RequestParameterNames.Culture;
	}
}