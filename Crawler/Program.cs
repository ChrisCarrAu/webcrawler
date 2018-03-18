using Crawler.Lib.Model;
using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.implementation;
using Crawler.Lib.Service.Implementation;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var servicesProvider = BuildDi();
            var crawler = servicesProvider.GetRequiredService<Crawler>();

            Task.Factory.StartNew(async () =>
            {
                await crawler.Crawl();
            }).ContinueWith(task =>
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            });

            Console.WriteLine("The Console is waiting on a keypress to close.");
            Console.ReadLine();
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            //Runner is the custom class
            services.AddTransient<Crawler>();

            // Logging
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            // Crawler.Lib
            services.AddSingleton<IProcessedSet, ProcessedSet>();
            services.AddSingleton<IUriQueue, ObservableUriQueue>();
            //services.AddSingleton<IUriQueueListener, UriQueueListener>();
            services.AddTransient<IUriParser, Lib.Service.Implementation.UriParser>();
            services.AddSingleton<IQueueSpider, QueueSpider>();

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }

    }
}
