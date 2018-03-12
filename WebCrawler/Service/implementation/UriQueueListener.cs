using System;
using System.Threading.Tasks;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler.Service.Implementation
{
    internal class UriQueueListener : IUriQueueListener
    {
        private readonly IUriQueue _uriQueue;
        private readonly IProcessedSet _processedSet;

        public UriQueueListener(IUriQueue uriQueue, IProcessedSet processedSet)
        {
            _uriQueue = uriQueue;
            _processedSet = processedSet;
        }

        public void OnCompleted()
        {
            //
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnNext(Anchor value)
        {
            if (_uriQueue.TryDequeue(out Anchor anchor))
            {
                var t = new Task(() =>
                    {
                        var crawler = new WebCrawler(_uriQueue, _processedSet); // new Service.Implementation.WebCrawler(_queue);
                        crawler.Crawl(anchor);
                    }
                );
                t.Start();
            }
        }
    }
}
