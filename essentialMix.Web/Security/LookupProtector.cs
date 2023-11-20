using essentialMix.Cryptography;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace essentialMix.Web.Security
{
	public sealed class LookupProtector : ILookupProtector
	{
		private readonly IEncrypt _cipher;

		public LookupProtector([NotNull] IEncrypt cipher)
		{
			_cipher = cipher;
		}

		/// <inheritdoc />
		public string Protect(string keyId, string data) { return _cipher.Encrypt(data); }

		/// <inheritdoc />
		public string Unprotect(string keyId, string data) { return _cipher.Decrypt(data); }
	}
}
