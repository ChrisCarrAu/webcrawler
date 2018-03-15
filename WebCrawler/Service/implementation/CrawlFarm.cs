using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler.Service.implementation
{
    internal class CrawlFarm : ICrawlFarm
    {
        private readonly IUriQueue _uriQueue;
        private readonly IProcessedSet _processedSet;

        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public CrawlFarm(IUriQueue uriQueue, IProcessedSet processedSet)
        {
            _uriQueue = uriQueue;
            _processedSet = processedSet;
        }

        public void Run()
        {
            var crawl = new ActionBlock<Anchor>(
                anchor =>
                {
                    var crawler = new Implementation.WebCrawler();
                    crawler.Subscribe(this);
                    crawler.Crawl(anchor);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 } );

            Task.Run(() =>
            {
                while (true)
                {
                    while (_uriQueue.TryDequeue(out var anchor))
                    {
                        crawl.Post(anchor);
                    }
                    Thread.Sleep(500);
                    _manualResetEvent.WaitOne();
                }
            });
        }

        public void OnNext(Anchor anchor)
        {
            if (_processedSet.Processed(anchor))
                return;

            if (anchor.JumpCount > 2)
                return;

            _processedSet.Add(anchor);
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
