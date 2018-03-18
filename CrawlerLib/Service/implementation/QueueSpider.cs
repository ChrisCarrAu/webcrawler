using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Crawler.Lib.Service.implementation
{
    /// <summary>
    /// Given a queue of Uris, crawls the queue, retrieving anchor entries from the page and enqueueing them for subsequent processing
    /// </summary>
    public class QueueSpider : IQueueSpider
    {
        private readonly ILogger<QueueSpider> _logger;
        private readonly IUriQueue            _uriQueue;
        private readonly IProcessedSet        _processedSet;
        private readonly IServiceProvider     _serviceProvider;

        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        private int _activeCrawlers;

        public QueueSpider
        (
            ILogger<QueueSpider> logger, 
            IServiceProvider     serviceProvider, 
            IUriQueue            uriQueue, 
            IProcessedSet        processedSet
        )
        {
            _logger          = logger;
            _serviceProvider = serviceProvider;
            _uriQueue        = uriQueue;
            _processedSet    = processedSet;
        }

        public async Task Crawl(int maxDegreeOfParallelism = 1)
        {
            _logger.LogInformation($"Beginning Web Crawl");
            _logger.LogInformation($"Maximum Degree Of Parallelism = {maxDegreeOfParallelism}");

            var crawl = new ActionBlock<Anchor>(
                async anchor =>
                {
                    var crawler = _serviceProvider.GetRequiredService<IUriParser>();
                    //var crawler = ActivatorUtilities.CreateInstance<Implementation.UriParser>(_serviceProvider);
                    var count = Interlocked.Increment(ref _activeCrawlers);
                    _logger.LogDebug($"  -- INCREMEMT, Crawl Count = {count}");
                    crawler.Subscribe(this);
                    await crawler.Crawl(anchor);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism } );

            _logger.LogInformation($"Starting");

            while (!_uriQueue.IsEmpty || _activeCrawlers != 0)
            {
                while (_uriQueue.TryDequeue(out var anchor))
                {
                    if (true)   // TODO: Ask caller if we should crawl this one?
                    {
                        await crawl.SendAsync<Anchor>(anchor);
                    }
                }
                _manualResetEvent.WaitOne(500);
            }

            _logger.LogInformation($"Exiting - active crawlers = {_activeCrawlers}");
        }

        public void OnNext(Anchor anchor)
        {
            if (_processedSet.Processed(anchor.Uri.ToString()))
                return;

            if (anchor.Jumps.Count() > 2)
                return;

            _processedSet.Add(anchor.Uri.ToString());
            _uriQueue.Enqueue(anchor);

            // Wake up processing queue
            _manualResetEvent.Set();
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"{error.ToString()}");
        }

        public void OnCompleted()
        {
            var count = Interlocked.Decrement(ref _activeCrawlers);
            _logger.LogDebug($"  -- DECREMEMT, Crawl Count = {count}");
        }
    }
}
