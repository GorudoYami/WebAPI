using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFramework;
using WebAPI.Data;
using WebAPI.Services;

namespace WebAPI {
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
				}));

			services.AddControllers();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters() {
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"])),
					ValidIssuer = Configuration["JwtSettings:Issuer"],
					ValidAudience = Configuration["JwtSettings:Audience"],
					ClockSkew = TimeSpan.Zero,
					RequireExpirationTime = true,
					ValidateIssuer = true,
					ValidateIssuerSigningKey = true,	
					ValidateAudience = true,
					ValidateLifetime = true,
				});
			services.AddLogging();

			services.AddDbContext<Database>(options =>
				options.UseMySQL(Configuration.GetConnectionString("Default")));

			services.AddScoped<AccountService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseCors("AllowEverything");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
