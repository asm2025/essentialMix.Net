using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace essentialMix.Web.Security;

public class LookupProtectorKeyRing : ILookupProtectorKeyRing
{
	private const string KEY = "key";

	public LookupProtectorKeyRing()
	{
	}

	[NotNull]
	public string this[string keyId] => KEY;

	[NotNull]
	public string CurrentKeyId => KEY;

	[NotNull]
	public IEnumerable<string> GetAllKeyIds() { return new[] { KEY }; }
}