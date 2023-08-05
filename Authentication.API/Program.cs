using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Authentication.Data;
using WebAPI.Services.Interfaces;
using WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Authentication.Common.Data.Repositories;
using Authentication.Data.Repositories;

namespace Authentication.API;

public static class Program {
	public static void Main(string[] args) {
		InitializeNLog();

		var builder = WebApplication.CreateBuilder(args);
		var config = builder.Configuration;

		builder.Logging.ClearProviders();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
		builder.Host.UseNLog();

		builder.Services.AddControllers();
		builder.Services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen()
			.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.RequestProperties)
			.AddDbContext<DatabaseContext>(options => options.UseMySql(config.GetConnectionString("Default"), new MySqlServerVersion("")))
			.AddRepositories()
			.AddServices()
			.AddCors(options => options.AddPolicy("AllowEverything", builder => {
				builder.AllowAnyOrigin();
				builder.AllowAnyHeader();
				builder.AllowAnyMethod();
			}))
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer();

		var app = builder.Build();

		app.UseHttpsRedirection();
		app.UseHttpLogging();
		app.MapControllers();
		app.UseRouting();
		app.UseAuthorization();
		app.UseAuthentication();

		if (app.Environment.IsDevelopment()) {
			app.UseCors("AllowEverything");
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		NLog.LogManager.Shutdown();
	}

	private static void InitializeNLog() {
		NLog.LogManager.ThrowExceptions = true;
		NLog.LogManager.LoadConfiguration("nlog.config")
			.Setup()
			.LoadConfigurationFromAppSettings();
	}

	private static IServiceCollection AddRepositories(this IServiceCollection services) => services
		.AddScoped<IAccountRepository, AccountRepository>()
		.AddScoped<IRoleRepository, RoleRepository>()
		.AddScoped<IPermissionRepository, PermissionRepository>();

	private static IServiceCollection AddServices(this IServiceCollection services) => services
		.AddScoped<IJwtService, JwtService>();

	private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration) => services
		.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, configuration.GetSection("JwtService"));
}
