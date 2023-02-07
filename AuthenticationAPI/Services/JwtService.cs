using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Data;
using WebAPI.Data.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public class JwtService : IJwtService {
	public SymmetricSecurityKey TokenKey { get; private set; }
	public string TokenIssuer { get; private set; }
	public string TokenAudience { get; private set; }
	public int TokenLifetime { get; private set; }

	private JwtSecurityTokenHandler JwtHandler { get; set; }

	private readonly ILogger<IJwtService> Logger;
	private readonly DatabaseContext DatabaseContext;

	public JwtService(ILogger<IJwtService> logger, DatabaseContext databaseContext) {
		Logger = logger;
		DatabaseContext = databaseContext;

		TokenKey = GetTokenKey();
		TokenLifetime = GetTokenLifetime();
		TokenIssuer = GetTokenIssuer();
		TokenAudience = GetTokenAudience();
		JwtHandler = new JwtSecurityTokenHandler();
	}

	private int GetTokenLifetime() {
		int tokenLifetime = 5;
		try {
			GlobalVariable gv = DatabaseContext.GlobalVariables.Single(gv => gv.Name == "TokenLifetime");
			tokenLifetime = int.Parse(gv.Value);
		}
		catch (Exception ex) {
			Logger.LogError(ex, "Token lifetime defaulted to 5 minutes!");
		}

		return tokenLifetime;
	}

	private SymmetricSecurityKey GetTokenKey() {
		SymmetricSecurityKey tokenKey = null;
		try {
			GlobalVariable gv = DatabaseContext.GlobalVariables.Single(gv => gv.Name == "TokenKey");
			tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(gv.Value));
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "Token key missing! Account service may not work!");
		}

		return tokenKey;
	}

	private string GetTokenIssuer() {
		string tokenIssuer = string.Empty;
		try {
			GlobalVariable gv = DatabaseContext.GlobalVariables.Single(gv => gv.Name == "TokenIssuer");
			tokenIssuer = gv.Value;
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "Token issuer missing! Account service may not work!");
		}

		return tokenIssuer;
	}

	private string GetTokenAudience() {
		string tokenIssuer = string.Empty;
		try {
			GlobalVariable gv = DatabaseContext.GlobalVariables.Single(gv => gv.Name == "TokenAudience");
			tokenIssuer = gv.Value;
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "Token audience missing! Account service may not work!");
		}

		return tokenIssuer;
	}

	public string CreateEncodedJwt(int accountID) {
		List<Claim> claims = new()
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Sub, accountID.ToString()),
		};

		return JwtHandler.CreateEncodedJwt(
			issuer: TokenIssuer,
			audience: TokenAudience,
			subject: new ClaimsIdentity(claims),
			notBefore: DateTime.UtcNow,
			issuedAt: DateTime.UtcNow,
			expires: DateTime.UtcNow.AddMinutes(TokenLifetime),
			signingCredentials: new SigningCredentials(TokenKey, SecurityAlgorithms.HmacSha512)
		);
	}

	public string GetPasswordHash(string password, string saltString) {
		byte[] salt = Convert.FromBase64String(saltString);

		return Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA512,
			iterationCount: 10000,
			numBytesRequested: 512 / 8));
	}

	public (string hash, string salt) GetPasswordHash(string password) {
		using RandomNumberGenerator rng = RandomNumberGenerator.Create();
		byte[] saltBytes = new byte[128 / 8];
		rng.GetNonZeroBytes(saltBytes);

		string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: saltBytes,
			prf: KeyDerivationPrf.HMACSHA512,
			iterationCount: 10000,
			numBytesRequested: 512 / 8));

		string salt = Convert.ToBase64String(saltBytes);
		return (hash, salt);
	}

	public string GetSubject(string token) {
		return JwtHandler.ReadJwtToken(token).Subject;
	}
}