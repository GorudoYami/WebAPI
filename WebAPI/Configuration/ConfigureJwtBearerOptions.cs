using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Data;

namespace WebAPI.Configuration;

public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions> {
	private readonly DatabaseContext DatabaseContext;
	private readonly ILogger<ConfigureJwtBearerOptions> Logger;

	public ConfigureJwtBearerOptions(ILogger<ConfigureJwtBearerOptions> logger, DatabaseContext databaseContext) {
		DatabaseContext = databaseContext;
		Logger = logger;
	}

	public async void Configure(string name, JwtBearerOptions options) {
		try {
			string tokenKeyStr = (await DatabaseContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenKey")).Value;
			string issuer = (await DatabaseContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenIssuer")).Value;
			string audience = (await DatabaseContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenAudience")).Value;

			SymmetricSecurityKey tokenKey = new(Encoding.UTF8.GetBytes(tokenKeyStr));

			options.TokenValidationParameters = new() {
				IssuerSigningKey = tokenKey,
				ValidIssuer = issuer,
				ValidAudience = audience,
				ClockSkew = TimeSpan.Zero,
				RequireExpirationTime = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				ValidateAudience = true,
				ValidateLifetime = true
			};
		}
		catch (Exception ex) {
			Logger.LogCritical(ex, "JWT Configuration failed! Account service may not work!");
		}
	}

	public void Configure(JwtBearerOptions options) => throw new NotImplementedException();
}
