using Authentication.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Services.Interfaces;

namespace Authentication.API.Services;

public class JwtService : IJwtService {
	private readonly SymmetricSecurityKey _tokenKey;
	private readonly string _tokenIssuer;
	private readonly string _tokenAudience;
	private readonly int _tokenLifetime;
	private readonly JwtSecurityTokenHandler _jwtHandler;
	private readonly ILogger<IJwtService> _logger;

	public JwtService(ILogger<IJwtService> logger, IOptions<JwtBearerOptions> options, IConfiguration configuration) {
		_logger = logger;
		_jwtHandler = new JwtSecurityTokenHandler();
		JwtBearerOptions jwtBearerOptions = options.Value;
		_tokenIssuer = jwtBearerOptions.TokenValidationParameters.ValidIssuer;
		_tokenAudience = jwtBearerOptions.Audience;
		_tokenLifetime = configuration[""]
	}


	public string CreateEncodedToken(int accountId) {
		List<Claim> claims = new()
		{
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Sub, accountId.ToString()),
		};

		return _jwtHandler.CreateEncodedJwt(
			issuer: _tokenIssuer,
			audience: _tokenAudience,
			subject: new ClaimsIdentity(claims),
			notBefore: DateTime.UtcNow,
			issuedAt: DateTime.UtcNow,
			expires: DateTime.UtcNow.AddMinutes(_tokenLifetime),
			signingCredentials: new SigningCredentials(_tokenKey, SecurityAlgorithms.HmacSha512)
		);
	}

	public string RefreshToken(string token) {

	}

	public string GetSubject(string token) {
		return _jwtHandler.ReadJwtToken(token).Subject;
	}
}