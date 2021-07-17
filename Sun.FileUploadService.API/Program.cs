using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sun.FileUploadService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger logger = null;
            try
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                loggerFactory.AddFile("Logs/StartUp-API-{Date}.txt");
                logger = loggerFactory.CreateLogger<Program>();
                logger.LogInformation("Web API Upload about to Start.");
                System.IO.Directory.CreateDirectory(@"Resources\Images");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Main Method");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logBuilder =>
                {
                    logBuilder.ClearProviders(); // removes all providers from LoggerFactory
                    logBuilder.AddConsole();
                    //logBuilder.AddFile("Logs/Upload-API-{Date}.txt");
                    logBuilder.AddTraceSource("Information"); // Add Trace listener provider
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
