using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace essentialMix.Web.Mvc;

public abstract class ResponseResultBase : IActionResult
{
	protected ResponseResultBase()
	{
	}

	/// <inheritdoc />
	public abstract Task ExecuteResultAsync(ActionContext context);
}