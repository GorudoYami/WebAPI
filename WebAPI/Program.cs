using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace WebAPI {
	public class Program {
		public static void Main(string[] args) {
			NLog.LogManager.ThrowExceptions = true;
			NLog.LogManager.LoadConfiguration("nlog.config")
				.Setup()
				.LoadConfigurationFromAppSettings();

			CreateHostBuilder(args).Build().Run();
			NLog.LogManager.Shutdown();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
				.ConfigureLogging(logging => {
					logging.ClearProviders();
					logging.SetMinimumLevel(LogLevel.Trace);
				})
				.UseNLog();
	}
}
