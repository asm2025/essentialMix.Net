using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace essentialMix.Web.TagHelpers;

[HtmlTargetElement(Attributes = "policy")]
public class PolicyTagHelper : TagHelper
{
	private readonly IAuthorizationService _authService;
	private readonly ClaimsPrincipal _principal;

	public PolicyTagHelper([NotNull] IAuthorizationService authService, [NotNull] IHttpContextAccessor httpContextAccessor)
	{
		_authService = authService;
		_principal = httpContextAccessor.HttpContext?.User;
	}

	public string Policy { get; set; }

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		if (_principal != null && (await _authService.AuthorizeAsync(_principal, Policy)).Succeeded) return;
		output.SuppressOutput();
	}
}