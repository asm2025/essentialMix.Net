using essentialMix.Cryptography;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace essentialMix.Web.Security;

public class PersonalDataProtector : IPersonalDataProtector
{
	private readonly IEncrypt _cipher;

	public PersonalDataProtector([NotNull] IEncrypt cipher)
	{
		_cipher = cipher;
	}

	/// <inheritdoc />
	public string Protect(string data) { return _cipher.Encrypt(data); }

	/// <inheritdoc />
	public string Unprotect(string data) { return _cipher.Decrypt(data); }
}