using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Authentication.Data;

namespace Authentication.Options;

[Obsolete("Moved options to appsettings.json")]
public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions> {
	private readonly IServiceProvider ServiceProvider;

	public ConfigureJwtBearerOptions(IServiceProvider serviceProvider) {
		ServiceProvider = serviceProvider;
	}

	public void Configure(string name, JwtBearerOptions options) {
		Configure(options);
	}

	public async void Configure(JwtBearerOptions options) {
		using IServiceScope scope = ServiceProvider.CreateScope();
		using DatabaseContext dbContext = scope.ServiceProvider.GetService<DatabaseContext>();
		try {
			string tokenKeyStr = (await dbContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenKey")).Value;
			string issuer = (await dbContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenIssuer")).Value;
			string audience = (await dbContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenAudience")).Value;
			string authority = (await dbContext.GlobalVariables.SingleAsync(gv => gv.Name == "TokenAuthority")).Value;

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

			options.Audience = audience;
			options.Authority = authority;

			options.SaveToken = true;
		}
		catch (Exception) {
			throw;
		}
	}
}
