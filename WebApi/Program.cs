using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace WebApi
{
    public class Program
    {
        private const string DetailedErrorsSettingsKey = "HostSettings:detailedErrors";
        private const string ShutdownTimeoutSettingsKey = "HostSettings:UseShutdownTimeoutMinutes";
        private const string LoggingSettingsKey = "Logging";

        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = configBuilder.Build();
            

            BuildWebHost(args, configuration).Run();
        }

        public static IWebHost BuildWebHost(string[] args, IConfiguration configuration) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting("detailedErrors", configuration.GetSection(DetailedErrorsSettingsKey).Value)
                .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection(LoggingSettingsKey));
                        logging.AddConsole();
                        logging.AddDebug();
                    })
                .UseShutdownTimeout(TimeSpan.FromMinutes(int.Parse(configuration.GetSection(ShutdownTimeoutSettingsKey).Value)))
                .UseStartup<Startup>()
                .CaptureStartupErrors(true)
                .Build();
    }
}
