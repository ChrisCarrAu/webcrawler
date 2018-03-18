using Crawler.Lib.Model;
using Crawler.Lib.Repository.Interface;
using Crawler.Lib.Service.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        public async Task Crawl()
        {
            _uriQueue.Subscribe(this);
            //_uriQueue.Enqueue(new Anchor { Uri = new Uri("http://www.thegravenimage.com/controltechnology") });
            _uriQueue.Enqueue(new Anchor { Uri = new Uri("http://www.appthem.com") });

            await _crawlFarm.Run(3);
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
                Console.WriteLine($"{anchor.Jumps.Count()} : {anchor.Uri}");
            }
            else
            {
                _logger.LogError($"{webException.Status.ToString()}\n{anchor.Uri}");
                Console.WriteLine($"{anchor.Jumps.Count()} : ({webException.Status.ToString()}) {anchor.Uri}");
            }
        }
    }
}
