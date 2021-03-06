﻿using Crawler.Lib.Repository.Implementation;
using Crawler.Lib.Repository.Interface;
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

            Task.WaitAll(crawler.Crawl());
            NLog.LogManager.Shutdown();

            Console.WriteLine("Press Enter to close.");
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
            services.AddSingleton<IQueueSpider, QueueSpider>();

            services.AddTransient<IUriParser, Lib.Service.Implementation.UriParser>();
            services.AddTransient<IWebClient, WebClient>();

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // Configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }

    }
}
