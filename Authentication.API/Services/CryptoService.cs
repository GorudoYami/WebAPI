using Authentication.API.Options;
using Authentication.Common.Data.Repositories;
using Authentication.Common.Services;
using Authentication.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace Authentication.API.Services;

public class CryptoService : ICryptoService {
	private readonly ILogger<ICryptoService> _logger;
	private readonly CryptoServiceOptions _options;

	public CryptoService(ILogger<ICryptoService> logger, CryptoServiceOptions options) {
		_logger = logger;
		_options = options;
	}

	public string GetPasswordHashBase64(string password, string salt) {
		byte[] saltData = Convert.FromBase64String(salt);
		return GetPasswordHashBase64(password, saltData);
	}

	public string GetPasswordHashBase64(string password, out string salt) {
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		byte[] saltData = new byte[_options.SaltBitLength / 8];
		rng.GetNonZeroBytes(saltData);
		salt = Convert.ToBase64String(saltData);

		return GetPasswordHashBase64(password, saltData);
	}

	private string GetPasswordHashBase64(string password, byte[] salt) {
		return Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: salt,
			prf: _options.Algorithm,
			iterationCount: _options.IterationCount,
			numBytesRequested: _options.KeyBitLength / 8));
	}
}
