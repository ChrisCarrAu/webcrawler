using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Crawler
{
    public class Crawler : IObserver<Anchor>
    {
        private readonly ILogger<Crawler> _logger;
        private readonly ICrawlFarm _crawlFarm;
        private readonly IUriQueue _uriQueue;

        public Crawler(ILogger<Crawler> logger, ICrawlFarm crawlFarm, IUriQueue uriQueue)
        {
            _logger = logger;
            _crawlFarm = crawlFarm;
            _uriQueue = uriQueue;
        }

        public void Crawl()
        {
            _uriQueue.Subscribe(this);
            //_uriQueue.Enqueue(new Anchor { Uri = new Uri("http://www.thegravenimage.com/controltechnology") });
            _uriQueue.Enqueue(new Anchor { Uri = new Uri("http://www.appthem.com") });

            _crawlFarm.Run(3);
        }

        public void OnCompleted()
        {
            //
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnNext(Anchor anchor)
        {
            var webException = anchor.Exception as WebException;
            if (null == webException)
            {
                Console.WriteLine($"{anchor.JumpCount} : {anchor.Uri}");
            }
            else
            {
                _logger.LogError($"{webException.Status.ToString()}\n{anchor.Uri}");
                Console.WriteLine($"{anchor.JumpCount} : ({webException.Status.ToString()}) {anchor.Uri}");
            }
        }
    }
}
