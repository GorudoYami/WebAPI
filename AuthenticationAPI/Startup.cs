using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAPI.Configuration;
using WebAPI.Data;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI;

public class Startup {
	public IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration) {
		Configuration = configuration;
	}

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services) {
		services.AddCors(options =>
			// Change later
			options.AddPolicy("AllowEverything", builder => {
				builder.AllowAnyOrigin();
				builder.AllowAnyMethod();
				builder.AllowAnyHeader();
			})
		);

		services.AddControllers();
		services.AddLogging();

		services.AddDbContext<DatabaseContext>(options =>
			options.UseMySql(Configuration.GetConnectionString("Default"), new MySqlServerVersion("10.6.5")));

		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IJwtService, JwtService>();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer();

		services.ConfigureOptions<ConfigureJwtBearerOptions>();
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext db) {
		if (env.IsDevelopment()) {
			app.UseDeveloperExceptionPage();
		}
		else {
			app.UseHsts();
		}

		db.Database.EnsureCreated();
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseCors("AllowEverything");
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}
