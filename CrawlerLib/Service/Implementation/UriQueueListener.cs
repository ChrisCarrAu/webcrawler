using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Interface;
using System;

namespace Crawler.Lib.Service.Implementation
{
    public class UriQueueListener : IUriQueueListener
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
            //
        }
    }
}
