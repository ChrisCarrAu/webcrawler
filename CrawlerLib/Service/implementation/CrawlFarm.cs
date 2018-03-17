using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Implementation;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crawler.Lib.Service.implementation
{
    public class CrawlFarm : ICrawlFarm
    {
        private readonly ILogger<CrawlFarm> _logger;
        private readonly IUriQueue _uriQueue;
        private readonly IProcessedSet _processedSet;
        private readonly IServiceProvider _serviceProvider;

        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public CrawlFarm(ILogger<CrawlFarm> logger, IServiceProvider serviceProvider, IUriQueue uriQueue, IProcessedSet processedSet)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _uriQueue = uriQueue;
            _processedSet = processedSet;
        }

        public void Run(int maxDegreeOfParallelism = 1)
        {
            _logger.LogDebug($"Beginning Web Crawl");
            _logger.LogDebug($"Maximum Degree Of Parallelism = {maxDegreeOfParallelism}");
            var crawl = new ActionBlock<Anchor>(
                anchor =>
                {
                    var crawler = ActivatorUtilities.CreateInstance<WebCrawler>(_serviceProvider);
                    crawler.Subscribe(this);
                    crawler.Crawl(anchor);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism } );

            while (true)
            {
                while (_uriQueue.TryDequeue(out var anchor))
                {
                    crawl.Post(anchor);
                }
                Thread.Sleep(500);
                _manualResetEvent.WaitOne();
            }
        }

        public void OnNext(Anchor anchor)
        {
            if (_processedSet.Processed(anchor.Uri.ToString()))
                return;

            if (anchor.JumpCount > 5)
                return;

            _processedSet.Add(anchor.Uri.ToString());
            _uriQueue.Enqueue(anchor);

            // Wake up processing queue
            _manualResetEvent.Set();
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnCompleted()
        {
            //
        }
    }
}
