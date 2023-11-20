using System;
using System.Threading.Tasks;
using essentialMix.Web.Helpers;
using essentialMix.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace essentialMix.Web.Authorization;

public class PasswordExpirationHandler<TUser, TKey> : AuthorizationHandler<PasswordExpirationRequirement>
	where TUser : class, IUserPassword<TKey>
	where TKey : IComparable<TKey>, IEquatable<TKey>
{
	private readonly IServiceScopeFactory _scopeFactory;

	public PasswordExpirationHandler([NotNull] IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
	}

	/// <inheritdoc />
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, [NotNull] PasswordExpirationRequirement requirement)
	{
		IServiceScope scope = null;

		try
		{
			scope = _scopeFactory.CreateScope();
			IServiceProvider services = scope.ServiceProvider;
			UserManager<TUser> userManager = services.GetRequiredService<UserManager<TUser>>();
			TUser user = await userManager.GetUserAsync(context.User);

			if (user == null)
			{
				context.Fail();
				return;
			}

			if (user.IsPasswordExpired(requirement.ExpirationTime))
			{
				if (!string.IsNullOrEmpty(requirement.ChangePasswordUrl))
				{
					switch (context.Resource)
					{
						case DefaultHttpContext httpContext:
						{
							string code = ChallengeCodeHelper.ForValue(user.Email);
							string returnUrl = httpContext.Request.GetEncodedUrl();
							httpContext.Response.Redirect($"{requirement.ChangePasswordUrl}?code={code}&returnUrl={returnUrl}");
							context.Succeed(requirement);
							return;
						}
						case AuthorizationFilterContext filterContext:
						{
							string code = ChallengeCodeHelper.ForValue(user.Email);
							string returnUrl = filterContext.HttpContext.Request.GetEncodedUrl();
							filterContext.Result = new RedirectResult($"{requirement.ChangePasswordUrl}?code={code}&returnUrl={returnUrl}");
							context.Succeed(requirement);
							return;
						}
					}
				}

				context.Fail();
				return;
			}

			context.Succeed(requirement);
		}
		finally
		{
			ObjectHelper.Dispose(ref scope);
		}
	}
}