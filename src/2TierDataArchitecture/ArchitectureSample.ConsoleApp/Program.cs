using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace ArchitectureSample.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Logger DI
                IServiceProvider servicesProvider = BuildDependencyInjection();
                Runner runner = servicesProvider.GetRequiredService<Runner>();

                // Main Logic
                runner.Startup(args).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                Environment.ExitCode = 1;
            }
            finally
            {
                // Exit code
                Environment.Exit(Environment.ExitCode);
            }
        }

        /// <summary>
        /// https://github.com/NLog/NLog.Extensions.Logging/wiki/Getting-started-with-.NET-Core-2---Console-application
        /// </summary>
        /// <returns></returns>
        private static IServiceProvider BuildDependencyInjection()
        {
            ServiceCollection services = new ServiceCollection();

            //Runner is the custom class
            services.AddTransient<Runner>();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }
    }
}
