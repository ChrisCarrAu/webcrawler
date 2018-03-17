using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using System;

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

            _crawlFarm.Run(8);
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
            if (null == anchor.Exception)
            {
                Console.WriteLine($"{anchor.JumpCount} : {anchor.Uri}");
            }
            else
            {
                Console.WriteLine($"{anchor.JumpCount} : ({anchor.Exception.Status.ToString()}) {anchor.Uri}");
            }
        }
    }
}
